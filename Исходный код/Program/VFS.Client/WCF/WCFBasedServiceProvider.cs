namespace VFS.Client.WCF
{
    using System.ServiceModel;

    using VFS.Interfaces.Service;

    /// <summary>
    /// Поставщик доступа пользователя к интерфейсу виртуального файлового сервера.
    /// </summary>
    internal class WCFBasedServiceProvider : IVFSSingleUserServiceProvider
    {
        /// <summary>
        /// Фабрика-поставщик экземпляров сервиса.
        /// </summary>
        private DuplexChannelFactory<IVFSSingleUserService> channelFactory;

        /// <inheritdoc />
        public IVFSSingleUserService CreateVFSService(
            IVFSNotificationHandler notificationHandler)
        {
            var instanceContext = new InstanceContext(notificationHandler);
            var duplexChannelFactory = new DuplexChannelFactory<IVFSSingleUserService>(
                instanceContext, "NetTCPBinding_IVFSSingleUserServiceClient");

            this.channelFactory = duplexChannelFactory;

            var instance = duplexChannelFactory.CreateChannel();

            instance.SubscribeForNotifications();
            return instance;
        }

        public void Dispose()
        {
            this.channelFactory?.Close();
        }
    }
}