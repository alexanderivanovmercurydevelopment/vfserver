namespace VFS.Server
{
    using System;

    /// <summary>
    /// Аргументы событий, связанных с пользователями
    /// виртуального файлового сервера.
    /// </summary>
    internal class VFSUserEventArgs : EventArgs
    {
        /// <summary>
        /// Создать аргументы события, связанного с пользователем
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="user">Пользователь.</param>
        internal VFSUserEventArgs(VFSUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(
                    nameof(user),
                    "Необходимо передать экземпляр класса пользователя.");
            }

            this.User = user;
        }

        /// <summary>
        /// Пользователь виртуального файлового сервера.
        /// </summary>
        internal VFSUser User { get; }
    }
}
