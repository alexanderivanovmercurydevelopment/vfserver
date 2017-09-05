namespace VFS.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Класс управления пользователями.
    /// </summary>
    internal class VFSLockingPolicy
    {
        /// <summary>
        /// Подключенные пользователи.
        /// </summary>
        private readonly VFSConnectedUsers connectedUsers;

        /// <summary>
        /// Список заблокированных файлов.
        /// </summary>
        private readonly List<FileLockInfo> locks = new List<FileLockInfo>();

        /// <summary>
        /// Создать политику блокировки файлов.
        /// </summary>
        /// <param name="connectedUsers">Подключенные пользователи.</param>
        internal VFSLockingPolicy(
            VFSConnectedUsers connectedUsers)
        {
            if (connectedUsers == null)
            {
                throw new ArgumentNullException(
                    nameof(connectedUsers),
                    "Необходимо передать коллекцию подключенных пользователей.");
            }

            this.connectedUsers = connectedUsers;
            this.connectedUsers.UserUnregistered +=
                this.ConnectedUsersOnUserUnregistered;
        }

        /// <summary>
        /// Вызвать исключение, если нельзя удалить файл или папку.
        /// </summary>
        /// <param name="fullSource">Полный путь к файлу или папке.</param>
        /// <param name="virtualFileSystem">Виртуальная файловая система.</param>
        internal void ThrowIfCantRemove(
            string fullSource, 
            VirtualFileSystem virtualFileSystem)
        {
            IVirtualFile file = virtualFileSystem.FindFile(fullSource);
            if (file != null)
            {
                this.ThrowIfCantRemoveFile(file);
            }
            else
            {
                IVirtualDirectory directory =
                    virtualFileSystem.FindDirectory(fullSource);

                this.ThrowIfCantRemoveDirectory(directory);
            }
        }

        /// <summary>
        /// Сгенерировать исключение, если пользователь не может
        /// удалить директорию.
        /// </summary>
        /// <param name="directory">Директория.</param>
        internal void ThrowIfCantRemoveDirectory(
            IVirtualDirectory directory)
        {
            foreach (IVirtualFile file in directory.Files)
            {
                if (this.IsFileLocked(file))
                {
                    throw new InvalidOperationException(
                        "Невозможно удалить директорию " + directory.Name + ". "
                        + " Файл " + file.Name + " заблокирован для удаления");
                }
            }

            foreach (IVirtualDirectory dir in directory.ChildDirectories)
            {
                this.ThrowIfCantRemoveDirectory(dir);
            }
        }

        /// <summary>
        /// Вызвать исключение, если пользователь не может удалить файл.
        /// </summary>
        /// <param name="file">Файл.</param>
        internal void ThrowIfCantRemoveFile(IVirtualFile file)
        {
            if (this.IsFileLocked(file))
            {
                throw new InvalidOperationException(
                    "Файл " + file.Name + " заблокирован для удаления.");
            }
        }

        /// <summary>
        /// Заблокировать файл для удаления.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="file">Файл.</param>
        internal void LockFile(string userName, IVirtualFile file)
        {
            VFSUser user = this.connectedUsers.GetConnectedUser(userName);

            if (this.locks.Any(l => l.BlockedBy == user && l.File == file))
            {
                throw new InvalidOperationException(
                    "Файл " + file.Name + " уже заблокирован текущим пользователем (" + userName + ").\n"
                    + "Невозможно повторно заблокировать один и тот же файл.");
            }

            this.locks.Add(new FileLockInfo(user, file));
        }

        /// <summary>
        /// Разблокировать удаление файла.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="file">Файл.</param>
        internal void UnlockFile(string userName, IVirtualFile file)
        {
            VFSUser user = this.connectedUsers.GetConnectedUser(userName);

            FileLockInfo @lock = this.locks.FirstOrDefault(
                l => l.BlockedBy == user && l.File == file);

            if (@lock == null)
            {
                throw new InvalidOperationException(
                    "Пользователь " + userName + " не блокировал файл " + file.Name);
            }

            this.locks.Remove(@lock);
        }

        /// <summary>
        /// Сформировать информацию о директории, включающую 
        /// информацию о блокировках.
        /// </summary>
        /// <param name="directory">Директория.</param>
        /// <returns>Информация о структуре директории.</returns>
        internal DriveStructureInfo GetDirStructureWithLockingInfo(
            IVirtualDirectory directory)
        {
            DriveStructureInfo root = new DriveStructureInfo();
            this.AppendDirAndFileInfos(directory, root);
            return root;
        }

        /// <summary>
        /// Признак того, что файл заблокирован для удаления.
        /// </summary>
        /// <param name="file">Файл.</param>
        /// <returns>True - файл заблокирован, false - нет.</returns>
        private bool IsFileLocked(IVirtualFile file)
        {
            return this.locks.Any(l => l.File == file);
        }

        /// <summary>
        /// Добавить информацию о директориях.
        /// </summary>
        /// <param name="from">Реальная директория.</param>
        /// <param name="to">Информация о директории.</param>
        private void AppendDirAndFileInfos(
            IVirtualDirectory from, IVFSDirectoryInfo to)
        {
            to.Name = from.Name;
            to.Files = new List<VFSFileInfo>(from.Files.Select(f =>
                new VFSFileInfo() 
                { 
                    Name = f.Name,
                    LockingUsers = this.locks
                        .Where(l => l.File == f)
                        .Select(l => l.BlockedBy.Name)
                        .ToList()
                }));

            to.Directories = new List<VFSDirectoryInfo>();

            foreach (IVirtualDirectory childDir in from.ChildDirectories)
            {
                VFSDirectoryInfo childDirInfo = new VFSDirectoryInfo();
                to.Directories.Add(childDirInfo);
                this.AppendDirAndFileInfos(childDir, childDirInfo);
            }
        }

        /// <summary>
        /// Обработка события отключения пользователя.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="args">Аргументы события.</param>
        private void ConnectedUsersOnUserUnregistered(
            object sender, 
            VFSUserEventArgs args)
        {
            // снятие блокировки с файлов, заблокированных этим пользователем
            List<FileLockInfo> obsoleteLocks = this.locks.Where(
                l => l.BlockedBy == args.User)
                .ToList();

            obsoleteLocks.ForEach(l => this.locks.Remove(l));
        }
    }
}
