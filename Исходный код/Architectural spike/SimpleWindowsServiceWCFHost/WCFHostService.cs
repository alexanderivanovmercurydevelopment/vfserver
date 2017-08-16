using SimpleWCFInterfaces;
using SimpleWindowsServiceWCFHost.WCFServiceRealization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWindowsServiceWCFHost
{
    public partial class WCFHostService : ServiceBase
    {
        private ServiceHost m_svcHost = null;

        public WCFHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string hostName = Dns.GetHostName();

            string strAdrTCP = "net.tcp://" + hostName + ":9002/SimpleService";

            // отдельный порт для mex, чтобы разрешить установку свойства NetTCPBinding.MaxConnections
            string strAdrMex = "net.tcp://" + hostName + ":9003/mex"; // standard - "mex"

            Uri[] adrbase = { new Uri(strAdrTCP) };
            m_svcHost = new ServiceHost(typeof(SimpleService), adrbase);
            
            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            m_svcHost.Description.Behaviors.Add(mBehave);
            
            NetTcpBinding tcpb = new NetTcpBinding();
            tcpb.CloseTimeout = TimeSpan.FromMinutes(3);
            tcpb.OpenTimeout = TimeSpan.FromMinutes(3);
            tcpb.SendTimeout = TimeSpan.FromMinutes(3);
            tcpb.ReceiveTimeout = TimeSpan.FromHours(10);
            tcpb.MaxConnections = 1000;

            m_svcHost.AddServiceEndpoint(
                typeof(ISimpleService), 
                tcpb, 
                strAdrTCP);

            m_svcHost.AddServiceEndpoint(
                typeof(IMetadataExchange), 
                MetadataExchangeBindings.CreateMexTcpBinding(),
                strAdrMex);
            
            m_svcHost.Open();
        }

        protected override void OnStop()
        {
            if (m_svcHost != null)
            {
                m_svcHost.Close();
                m_svcHost = null;
            }
        }
    }
}
