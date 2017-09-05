namespace VFS.Server
{
    using System;

    /// <summary>
    /// Аргументы событий, связанных с пользователями
    /// виртуального файлового сервера.
    /// </summary>
    internal class VFSUserEventArgs : EventArgs
    {
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

        internal VFSUser User { get; }
    }
}
