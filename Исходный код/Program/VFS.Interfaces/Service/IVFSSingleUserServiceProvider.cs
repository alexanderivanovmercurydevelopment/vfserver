namespace VFS.Interfaces.Service
{
    using System;

    /// <summary>
    /// Поставщик доступа к интерфейсу работы пользователя
    /// с виртуальным файловым сервером.
    /// </summary>
    public interface IVFSSingleUserServiceProvider : IDisposable
    {
        /// <summary>
        /// Получить интерфейс работы пользователя
        /// с виртуальным файловым сервером.
        /// </summary>
        /// <param name="serverName">Имя сервера.</param>
        /// <param name="port">Порт.</param>
        /// <param name="notificationHandler">Обработчик уведомлений сервера.</param>
        /// <returns>Интерфейс доступа пользователя к виртуальному
        /// файловому серверу.</returns>
        IVFSSingleUserService CreateVFSService(
            string serverName,
            int? port,
            IVFSNotificationHandler notificationHandler);
    }
}