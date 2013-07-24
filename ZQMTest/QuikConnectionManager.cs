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
        private static string _requestConnectionString = "tcp://10.1.1.108:5562";
        private static string _subscribeConnectionString = "tcp://10.1.1.108:5563";
        private static TimeSpan _connectionTimeout = new TimeSpan(0, 0, 10);
        public static void Disconect() { _isConnected = false; }
        public static void Connect()
        {
            using (var context = ZmqContext.Create())
            {
                using (ZmqSocket req = context.CreateSocket(SocketType.REQ), subscriber = context.CreateSocket(SocketType.SUB))
                {
                    //Console.WriteLine("Aplication started");
                    //Console.WriteLine("Bind Request socket ")

                    req.Connect(_requestConnectionString);
                    subscriber.Connect(_subscribeConnectionString);
                    subscriber.SubscribeAll();
                    req.Send(Encoding.UTF8.GetBytes("Hello"));
                    var syncmsg = ZeroMQ.SendReceiveExtensions.Receive(req, Encoding.UTF8, _connectionTimeout);
                       //Receive(Encoding.UTF8,_connectionTimeout);
                    if (syncmsg != null)
                        _isConnected = true;
                    else
                    {
                        NotifyOnError("Can`t connect to data server");
                    }
                        

                    while(_isConnected)
                    {
                        ZmqMessage msg=new ZmqMessage();
                        try
                        {
                            msg = subscriber.ReceiveMessage();
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
                                case "NEWACCOUNT":
                                    NotifyOnNewAccount(JsonConvert.DeserializeObject<Account>(Encoding.UTF8.GetString(msg[1])));
                                    break;
                                case "INSTRUMENT":
                                    NotifyOnInstrument(JsonConvert.DeserializeObject<DynamicInstrument>(Encoding.UTF8.GetString(msg[1])));
                                    break;
                                case "POSITION":
                                    NotifyOnPosition(JsonConvert.DeserializeObject<Position>(Encoding.UTF8.GetString(msg[1])));
                                    break;
                                case "NEWPOSITION":
                                    NotifyOnNewPosition(JsonConvert.DeserializeObject<Position>(Encoding.UTF8.GetString(msg[1])));
                                    break;
                                case "ACCOUNT":
                                    NotifyOnAccount(JsonConvert.DeserializeObject<Account>(Encoding.UTF8.GetString(msg[1])));
                                    break;
                                default: throw new Exception("Unknown message title");
                            }
                        }
                        catch (SystemException e)
                        {
                            Console.Write(e.Message);
                        }
                        
                        
                        
                        //Object obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(msg[1]));
                        //Console.WriteLine("Title={0} Data={1}", msgTitle, result.ToString());
                    }
                    req.Disconnect(_requestConnectionString);
                    subscriber.Disconnect(_subscribeConnectionString);
                    req.Close();
                    subscriber.Close();
                }
                context.Terminate();
                context.Dispose();
            }
        }
 
        #region Events
        public static event Action<StaticInstrument> OnNewInstrument;
        public static event Action<DynamicInstrument> OnInstrument;
        public static event Action<Account> OnNewAccount;
        public static event Action<Account> OnAccount;
        public static event Action<Position> OnPosition;
        public static event Action<Position> OnNewPosition;
        public static event Action<string> OnConnected;
        public static event Action<string> OnError;
        private static void NotifyOnError(string msg)
        {
            if (OnError != null) OnError(msg);
        }
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
        private static void NotifyOnNewAccount(Account a)
        {
            if (OnNewAccount != null)
            {
                OnNewAccount(a);
            }
        }
        private static void NotifyOnPosition(Position p)
        {
            if (OnPosition != null) OnPosition(p);
        }
        private static void NotifyOnNewPosition(Position p)
        {
            if (OnNewPosition != null) OnNewPosition(p);
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
        public string BaseContractClass { get; set; }
    }

    [JsonObject]
    public class DynamicInstrument
    {
        public double LastPrice { get; set; }
        public double SettlePrice { get; set; }
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
        public int BuyQty { get; set; }
        public int SellQty { get; set; }
        public double VarMargin { get; set; }
    }
}
