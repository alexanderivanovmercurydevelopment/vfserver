namespace VFS.Server
{
    using System;

    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Информация о блокировке файла.
    /// </summary>
    internal class LockInfo
    {
        /// <summary>
        /// Создать информацию о блокировке файла.
        /// </summary>
        /// <param name="user">Пользователь, заблокировавший файл.</param>
        /// <param name="file">Блокируемый файл.</param>
        internal LockInfo(VFSUser user, IVirtualFile file)
        {
            if (user == null)
            {
                throw new ArgumentNullException(
                    "user",
                    "Необходимо указать пользователя, который блокирует файл.");
            }

            if (file == null)
            {
                throw new ArgumentNullException(
                    "file",
                    "Необходимо указать блокируемый файл.");
            }

            this.User = user;
            this.File = file;
        }

        /// <summary>
        /// Пользователь, заблокировавший файл.
        /// </summary>
        internal VFSUser User { get; private set; }

        /// <summary>
        /// Заблокированный файл.
        /// </summary>
        internal IVirtualFile File { get; private set; }
    }
}
