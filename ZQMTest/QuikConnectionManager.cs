using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using Newtonsoft.Json;

namespace QuikConnectionManager
{
    public class ConnectionManager
    {
        
        static volatile bool _isConnected;
        public static void Disconect() { _isConnected = false; }
        public static void Connect()
        {
            using (var context = ZmqContext.Create())
            {
                using (ZmqSocket req = context.CreateSocket(SocketType.REQ), subscriber = context.CreateSocket(SocketType.SUB))
                {
                    //Console.WriteLine("Aplication started");
                    //Console.WriteLine("Bind Request socket ");
                    var ts = new System.TimeSpan(0, 0, 30);
                    try
                    {
                        req.Connect("tcp://127.0.0.1:5562");
                        subscriber.Connect("tcp://127.0.0.1:5563");
                        subscriber.SubscribeAll();
                        req.Send(Encoding.UTF8.GetBytes("Hello"));
                        var syncmsg = req.Receive(Encoding.UTF8);
                        _isConnected = true;
                        
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    while(_isConnected)
                    {
                        ZmqMessage msg=new ZmqMessage();
                        try
                        {
                            msg = subscriber.ReceiveMessage();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        string msgTitle = Encoding.UTF8.GetString(msg[0]);
                        //object result;
                        switch (msgTitle)
                        {
                            case "COMMON":
                                NotifyOnInitialDataLoaded(Encoding.UTF8.GetString(msg[1]));
                                break;
                            case "NEWINSTRUMENT": 
                                NotifyOnNewInstrument(JsonConvert.DeserializeObject<StaticInstrument>(Encoding.UTF8.GetString(msg[1])));
                                break;
                            case "INSTRUMENT":
                                NotifyOnInstrument(JsonConvert.DeserializeObject<DynamicInstrument>(Encoding.UTF8.GetString(msg[1])));
                                break;
                            case "POSITION":
                                NotifyOnPosition(JsonConvert.DeserializeObject<Position>(Encoding.UTF8.GetString(msg[1])));
                                break;
                            case "ACCOUNT":
                                NotifyOnAccount(JsonConvert.DeserializeObject<Account>(Encoding.UTF8.GetString(msg[1])));
                                break;
                            default: throw new Exception("Unknown message title");   
                        }
                        
                        //Object obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(msg[1]));
                        //Console.WriteLine("Title={0} Data={1}", msgTitle, result.ToString());
                    }
                }
                
            }
        }
 
        #region Events
        public static event Action<StaticInstrument> OnNewInstrument;
        public static event Action<DynamicInstrument> OnInstrument;
        public static event Action<Account> OnAccount;
        public static event Action<Position> OnPosition;
        public static event Action<string> OnConnected;
        private static void NotifyOnNewInstrument(StaticInstrument arg)
        {
            if (OnNewInstrument != null) OnNewInstrument(arg);
        }
        private static void NotifyOnInstrument(DynamicInstrument i)
        {
            if (OnInstrument != null)
            {
                OnInstrument(i);
            }
        }
        private static void NotifyOnAccount(Account a)
        {
            if (OnAccount != null)
            {
                OnAccount(a);
            }
        }
        private static void NotifyOnPosition(Position p)
        {
            if (OnPosition != null) OnPosition(p);
        }
        private static void NotifyOnInitialDataLoaded(string arg)
        {
            if (OnConnected != null) OnConnected(arg);
        }
        #endregion

    }
    [JsonObject]
    public class StaticInstrument
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Class { get; set; }
        public string FullName { get; set; }
        public string InstrumentType { get; set; }
        public string OptionType { get; set; }
        public double Strike { get; set; }
        public string BaseContract { get; set; }
        public int DaysToMate { get; set; }
        public string MaturityDate { get; set; }
        
    }

    [JsonObject]
    public class DynamicInstrument
    {
        public double LastPrice { get; set; }
        public double Volatility { get; set; }
        public double TheorPrice { get; set; }
        public int Id { get; set; }
        public double Bid { get; set; }
        public int BidVol { get; set; }
        public double Ask { get; set; }
        public int AskVol { get; set; }
    }

    [JsonObject]
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [JsonObject]
    public class Position
    {
        public string AccountName { get; set; }
        public string SecurityCode { get; set; }
        public int TotalNet { get; set; }
    }
}
