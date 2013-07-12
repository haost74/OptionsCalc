using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using Newtonsoft.Json;

namespace ZQMTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                using (ZmqSocket req = context.CreateSocket(SocketType.REQ), subscriber = context.CreateSocket(SocketType.SUB))
                {
                    Console.WriteLine("Aplication started");
                    Console.WriteLine("Bind Request socket ");
                    req.Connect("tcp://127.0.0.1:5562");
                    subscriber.Connect("tcp://127.0.0.1:5563");
                    subscriber.SubscribeAll();
                    req.Send(Encoding.UTF8.GetBytes("Hello"));
                    var syncmsg=req.Receive(Encoding.UTF8);
                    Console.WriteLine(syncmsg.ToString());
                    /*
                    var title="";
                    do
                    {
                        var msg=req.ReceiveMessage();
                        title=Encoding.UTF8.GetString(msg[0]);
                        if (title!="SYNCEND")
                        {
                            Object obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(msg[1]));
                            Console.WriteLine("Title={0} Data={1}", title, obj.ToString());
                        }
                        
                    }
                    while(title!="SYNCEND");
                    */
                    while(true)
                    {
                        var msg = subscriber.ReceiveMessage();
                        var msgTitle = Encoding.UTF8.GetString(msg[0]);
                        Object obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(msg[1]));
                        Console.WriteLine("Title={0} Data={1}", msgTitle, obj.ToString());
                    }
                }
                
            }
        }
    }
}
