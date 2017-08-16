namespace VFS.Interfaces
{
    using System;

    /// <summary>
    /// Аргументы события уведомления.
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Создать аргументы события уведомления.
        /// </summary>
        /// <param name="notification">Сообщение уведомоления.</param>
        /// <param name="userName">Имя пользователя.</param>
        public NotificationEventArgs(
            string notification,
            string userName)
        {
            if (string.IsNullOrWhiteSpace(notification))
            {
                throw new ArgumentNullException(
                    "notification",
                    "Необходимо передать не пустое сообщение.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(
                    "userName",
                    "Необходимо указать имя пользователя.");
            }

            this.Notification = notification;
            this.UserName = userName;
        }

        /// <summary>
        /// Уведомление.
        /// </summary>
        public string Notification { get; private set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserName { get; private set; }
    }
}
