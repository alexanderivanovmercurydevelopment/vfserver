using SimpleWCFInterfaces;
using SimpleWCFServer.ServiceImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWCFServer
{
    class Program
    {
        private static ServiceHost m_svcHost = null;

        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName();
            string strAdrTCP = "net.tcp://" + hostName + ":9002/SimpleService";

            Uri[] adrbase = { new Uri(strAdrTCP) };
            m_svcHost = new ServiceHost(typeof(SimpleService), adrbase);

            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            m_svcHost.Description.Behaviors.Add(mBehave);

            NetTcpBinding tcpb = new NetTcpBinding();
            m_svcHost.AddServiceEndpoint(typeof(ISimpleService), tcpb, strAdrTCP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            m_svcHost.Open();

            Console.WriteLine("Сервер запущен");
            Console.ReadLine();

            m_svcHost.Close();
        }
    }
}
