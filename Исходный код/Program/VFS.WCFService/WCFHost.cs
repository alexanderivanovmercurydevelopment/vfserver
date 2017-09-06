namespace VFS.WCFService
{
    using System;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using VFS.Interfaces.Service;

    /// <summary>
    /// Хостинг WCF.
    /// </summary>
    public class WCFHost
    {
        /// <summary>
        /// Экземпляр хостинга.
        /// </summary>
        private ServiceHost serviceHost;

        /// <summary>
        /// Начать работу.
        /// </summary>
        public void Start()
        {
            if (this.serviceHost != null)
            {
                throw new InvalidOperationException(
                    "Хостинг WCF уже запущен.");
            }

            Uri[] adrbase = {new Uri(WCFHost.GetTcpServiceUri())};
            this.serviceHost = new ServiceHost(typeof(VFSSingleUserServiceWCF), adrbase);

            this.SetMetadataBehavior();
            this.AddTcpServiceEndpoint();
            this.AddMexEndpoint();

            this.serviceHost.Open();
        }

        /// <summary>
        /// Завершить работу.
        /// </summary>
        public void Stop()
        {
            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
                this.serviceHost = null;
            }
        }

        /// <summary>
        /// Получить URI сервиса.
        /// </summary>
        /// <returns>URI сервиса.</returns>
        private static string GetTcpServiceUri()
        {
            return "net.tcp://" + Dns.GetHostName() + ":" + WCFHost.GetPortNumber() + "/VFSSingleUserServiceWCF";
        }

        /// <summary>
        /// Получить URI для mex (с другим портом, чтобы позволить
        /// указывать MaxConnections для TCP-биндинга).
        /// </summary>
        /// <returns>URI для mex.</returns>
        private static string GetUriForMex()
        {
            return "net.tcp://" + Dns.GetHostName() + ":" + (WCFHost.GetPortNumber() + 1) + "/mex";
        }

        /// <summary>
        /// Прочитать номер порта из конфигурационного файла.
        /// </summary>
        /// <returns>Номер порта.</returns>
        private static int GetPortNumber()
        {
            string serviceFolderPath =
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

            string configText = File.ReadAllText(
                serviceFolderPath
                + Path.DirectorySeparatorChar
                + "config.txt");

            string stringPortNumber = configText.Split('=')[1].Trim();
            int portNumber = int.Parse(stringPortNumber);
            return portNumber;
        }

        /// <summary>
        /// Установить поведение метаданных.
        /// </summary>
        private void SetMetadataBehavior()
        {
            var mBehave = new ServiceMetadataBehavior();
            this.serviceHost.Description.Behaviors.Add(mBehave);
        }

        /// <summary>
        /// Добавить конечную точку доступа к сервису.
        /// </summary>
        private void AddTcpServiceEndpoint()
        {
            var tcpb = new NetTcpBinding
            {
                CloseTimeout = TimeSpan.FromMinutes(3),
                OpenTimeout = TimeSpan.FromMinutes(3),
                SendTimeout = TimeSpan.FromMinutes(3),
                ReceiveTimeout = TimeSpan.FromHours(10),
                MaxConnections = 1000
            };

            this.serviceHost.AddServiceEndpoint(
                typeof(IVFSSingleUserService),
                tcpb,
                WCFHost.GetTcpServiceUri());
        }

        /// <summary>
        /// Добавить конечную точку доступа к метаданным.
        /// </summary>
        private void AddMexEndpoint()
        {
            this.serviceHost.AddServiceEndpoint(
                typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                WCFHost.GetUriForMex());
        }
    }
}