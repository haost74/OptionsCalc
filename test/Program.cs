using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuikConnectionManager;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start app");
            ConnectionManager.OnConnected += new Action<string>(ConnectionManager_OnConnected);
            ConnectionManager.OnNewInstrument += new Action<StaticInstrument>(ConnectionManager_OnNewInstrument);
            ConnectionManager.OnPosition += new Action<Position>(ConnectionManager_OnPosition);
            ConnectionManager.OnAccount += new Action<Account>(ConnectionManager_OnAccount);
            ConnectionManager.OnInstrument += new Action<DynamicInstrument>(ConnectionManager_OnInstrument);
            try
            {
                ConnectionManager.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            

            Console.ReadKey();
            ConnectionManager.Disconect();
            Console.WriteLine("Close app");
        }

        static void ConnectionManager_OnInstrument(DynamicInstrument obj)
        {
            Console.WriteLine("Instrument update");
        }

        static void ConnectionManager_OnConnected(string obj)
        {
            Console.WriteLine(obj);
        }

        static void ConnectionManager_OnAccount(Account obj)
        {
            Console.WriteLine("New Account");
        }

        static void ConnectionManager_OnPosition(Position obj)
        {
            Console.WriteLine("New Position");
        }

        static void ConnectionManager_OnNewInstrument(StaticInstrument obj)
        {
            Console.WriteLine("New instrumetn");
        }
    }
}
