using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;

namespace ZQMTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = ZmqContext.Create())
            {
                using (var subscriber = context.CreateSocket(SocketType.SUB))
                {
                    subscriber.Connect("tcp://localhost:5563");
                    while (true)
                    {
                        var msg = subscriber.Receive(Encoding.UTF8);
                        Console.WriteLine(msg);
                    }
                }
            }
        }
    }
}
