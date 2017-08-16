using SimpleWCFInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWCFClient
{
    class Program
    {
        private static ChannelFactory<ISimpleService> factory;

        private static DuplexChannelFactory<ISimpleService> duplexFactory;

        static void Main(string[] args)
        {
            try
            {               
                Program.Connect();

                string yesNo = "y";

                while (yesNo != "n")
                {
                    Console.WriteLine("Вводите любые данные. Для выхода - exit.");
                    Program.ProcessRequests();

                    do
                    {
                        Console.WriteLine("Запросить следующий экземпляр сервисного объекта? y/n");
                        yesNo = Console.ReadLine();
                    }
                    while (yesNo != "y" && yesNo != "n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }

            try
            {
                Program.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка отключения от WCF.");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }

        private static void Connect()
        {
            string serverName = File.ReadAllText("config.txt");
            string hostName = serverName.Split('=')[1].Trim();
                       
            Uri tcpUri = new Uri("net.tcp://" + hostName + ":9002/SimpleService");
            EndpointAddress address = new EndpointAddress(tcpUri);
            Binding binding = new NetTcpBinding();

            binding.CloseTimeout = TimeSpan.FromMinutes(3);
            binding.OpenTimeout = TimeSpan.FromMinutes(3);
            binding.SendTimeout = TimeSpan.FromMinutes(3);
            binding.ReceiveTimeout = TimeSpan.FromHours(10);

            //Program.factory = new ChannelFactory<ISimpleService>(binding, address);

            IWCFEventHandler eventHandler = new WCFNotificationHandler();
            InstanceContext instanceContext = new InstanceContext(eventHandler);
            Program.duplexFactory = new DuplexChannelFactory<ISimpleService>(instanceContext, binding, address);
        }

        private static void ProcessRequests()
        {
            //ISimpleService service = factory.CreateChannel();
            
            ISimpleService proxy = Program.duplexFactory.CreateChannel();

            proxy.SubscribeEvent();

            string input = Console.ReadLine();

            while (input != "exit")
            {
                Console.WriteLine(proxy.SimpleOperation(input));
                input = Console.ReadLine();
            }
        }

        private static void Disconnect()
        {
            if (Program.factory != null)
            {
                factory.Close();
            }
        }
    }
}
