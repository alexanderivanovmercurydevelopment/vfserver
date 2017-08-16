using SimpleWCFInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWCFPerformanceTest
{
    class Program
    {
        private static readonly List<string> notificationMessages
            = new List<string>();

        private static readonly List<string> resultMessages
            = new List<string>();

        private static readonly List<string> errors
            = new List<string>();

        private static readonly List<string> duplexFactoriesCloseErrors
            = new List<string>();

        private static string hostName;

        static void Main(string[] args)
        {
            string serverName = File.ReadAllText("config.txt");
            Program.hostName = serverName.Split('=')[1].Trim();

            List<Task> tasks = new List<Task>();
            List<DuplexChannelFactory<ISimpleService>> duplexFactories
                = new List<DuplexChannelFactory<ISimpleService>>();

            for (int i = 0; i <= 150; i++)
            {
                string iString = i.ToString();

                Task task = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Uri tcpUri = new Uri("net.tcp://" + Program.hostName + ":9002/SimpleService");
                            EndpointAddress address = new EndpointAddress(tcpUri);
                            NetTcpBinding binding = new NetTcpBinding();
                            binding.CloseTimeout = TimeSpan.FromMinutes(3);
                            binding.OpenTimeout = TimeSpan.FromMinutes(3);
                            binding.SendTimeout = TimeSpan.FromMinutes(3);
                            binding.ReceiveTimeout = TimeSpan.FromMinutes(3);

                            IWCFEventHandler eventHandler = new WCFNotificationHandler();
                            InstanceContext instanceContext = new InstanceContext(eventHandler);
                            DuplexChannelFactory<ISimpleService> duplexFactory =
                                new DuplexChannelFactory<ISimpleService>(instanceContext, binding, address);

                            duplexFactories.Add(duplexFactory);

                            ISimpleService proxy = duplexFactory.CreateChannel();
                            proxy.SubscribeEvent();
                            Program.resultMessages.Add(proxy.SimpleOperation(iString));
                        }
                        catch (Exception ex)
                        {
                            Program.errors.Add(ex.ToString());
                        }
                    });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            Thread.Sleep(10000); // ожидание завершения посылки всех оповещений

            foreach (var df in duplexFactories)
            {
                try
                {
                    df.Close();
                }
                catch (Exception ex)
                {
                    Program.duplexFactoriesCloseErrors.Add(ex.ToString());
                }
            }
        }

        internal static void AddNotificationMessage(string message)
        {
            Program.notificationMessages.Add(message);
        }
    }

    internal class WCFNotificationHandler : IWCFEventHandler
    {
        public void HandleEvent(string message)
        {
            Program.AddNotificationMessage(message);
        }
    }
}
