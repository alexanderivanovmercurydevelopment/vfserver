namespace VFS.Interfaces.Service
{
    using System.ServiceModel;

    /// <summary>
    /// Обработчик уведомлений сервера.
    /// </summary>
    [ServiceContract]
    public interface IVFSNotificationHandler
    {
        /// <summary>
        /// Выполнить какие-либо действия при получении
        /// уведомления виртуального файлового сервера.
        /// </summary>
        /// <param name="notification">Уведомление.</param>
        [OperationContract(IsOneWay = true)]
        void HandleNotification(string notification);
    }
}