﻿namespace VFS.Client.WCF
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using VFS.Interfaces.Service;

    /// <summary>
    /// Поставщик доступа пользователя к интерфейсу виртуального файлового сервера.
    /// </summary>
    internal class WCFBasedServiceProvider : IVFSSingleUserServiceProvider
    {
        /// <summary>
        /// Фабрика-поставщик экземпляров сервиса.
        /// </summary>
        private DuplexChannelFactory<IVFSSingleUserService> duplexFactory;

        /// <summary>
        /// Создать интерфейс для доступа одного пользователя
        /// к виртуальному файловому серверу.
        /// </summary>
        /// <param name="serverName">Имя сервера.</param>
        /// <param name="port">Порт.</param>
        /// <param name="notificationHandler">Обработчик оповещений
        /// виртуального файлового сервера.</param>
        /// <returns>Интерфейс доступа пользователя к виртуальному 
        /// файловому серверу.</returns>
        public IVFSSingleUserService CreateVFSService(
            string serverName, 
            int? port, 
            IVFSNotificationHandler notificationHandler)
        {
            if (port == null)
            {
                throw new ArgumentNullException(
                    nameof(port),
                    "Необходимо указать порт.");
            }

            Uri tcpUri = new Uri("net.tcp://" + serverName + ":" + port + "/VFSSingleUserServiceWCF");

            EndpointAddress address = new EndpointAddress(tcpUri);

            Binding binding = new NetTcpBinding();

            binding.CloseTimeout = TimeSpan.FromMinutes(3);
            binding.OpenTimeout = TimeSpan.FromMinutes(3);
            binding.SendTimeout = TimeSpan.FromMinutes(3);
            binding.ReceiveTimeout = TimeSpan.FromHours(10);

            InstanceContext instanceContext = new InstanceContext(notificationHandler);
            this.duplexFactory = new DuplexChannelFactory<IVFSSingleUserService>(instanceContext, binding, address);
            IVFSSingleUserService service = this.duplexFactory.CreateChannel();
            service.SubscribeForNotifications();
            return service;
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            this.duplexFactory?.Close();
        }        
    }
}
