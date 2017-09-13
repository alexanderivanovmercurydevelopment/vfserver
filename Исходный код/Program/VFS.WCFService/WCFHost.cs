namespace VFS.WCFService
{
    using System;
    using System.ServiceModel;

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

            this.serviceHost = new ServiceHost(typeof(VFSSingleUserServiceWCF));
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
    }
}