namespace VFS.Client
{
    using System;

    using VFS.Interfaces.Service;

    /// <summary>
    /// Обработчик уведомлений виртуального файлового сервера.
    /// </summary>
    internal class ConsoleNotificationHandler : IVFSNotificationHandler
    {
        /// <summary>
        /// Выполнить какие-либо действия при получении
        /// уведомления виртуального файлового сервера.
        /// </summary>
        /// <param name="notification">Уведомление.</param>
        public void HandleNotification(string notification)
        {
            Console.WriteLine(notification);
        }
    }
}
