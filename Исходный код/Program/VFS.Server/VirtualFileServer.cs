namespace VFS.Server
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using VFS.Interfaces;
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.VirtualDrive;
    using VFS.Utilities;

    /// <inheritdoc chref="IVirtualFileServer" />
    public class VirtualFileServer : IVirtualFileServer
    {
        private const string CantRemoveCurrentDirectory = "Нельзя удалить или переместить текущую директорию, "
                                                          + "или директорию, родительскую по отношению к текущей.";

        private readonly VFSConfig config;

        private readonly VFSConnectedUsers connectedUsers;

        private readonly VirtualFileSystem fileSystem;

        /// <summary>
        /// Политика блокировки удаления файлов.
        /// </summary>
        private readonly VFSLockingPolicy lockingPolicy;

        /// <summary>
        /// Создать виртуальный файловый сервер.
        /// </summary>
        public VirtualFileServer()
        {
            this.connectedUsers = new VFSConnectedUsers();
            this.lockingPolicy = new VFSLockingPolicy(
                this.connectedUsers);
            this.fileSystem = new VirtualFileSystem();
            this.config = new VFSConfig("C:");
        }

        /// <summary>
        /// Событие завершения выполнения какой-либо операции.
        /// </summary>
        public event EventHandler<NotificationEventArgs> OperationPerformed;

        /// <inheritdoc/>
        public void ConnectUser(
            string userName)
        {
            this.connectedUsers.RegisterUser(userName, this.config.DefaultDirPath);
        }

        /// <inheritdoc/>
        public void ChangeUsersCurrentDirectory(
            string userName,
            string directoryPath)
        {
            string fullDirPath = this.GetFullPath(userName, directoryPath);
            this.fileSystem.CheckDirectoryExisting(fullDirPath);
            this.connectedUsers.GetConnectedUser(userName)
                .CurrentWorkingDirectoryPath = fullDirPath;
        }

        /// <inheritdoc />
        public string GetUsersCurrentWorkingDirectoryPath(string userName)
        {
            return this.connectedUsers.GetConnectedUser(userName)
                .CurrentWorkingDirectoryPath;
        }

        /// <inheritdoc />
        public void DisconnectUser(
            string userName)
        {
            this.connectedUsers.UnregisterUser(userName);
        }

        /// <inheritdoc />
        public int GetUsersCount()
        {
            return this.connectedUsers.RegisteredUsers.Count();
        }

        /// <inheritdoc/>
        public DriveStructureInfo GetDriveStructure(
            string driveName)
        {
            if (string.IsNullOrWhiteSpace(driveName))
            {
                driveName = "C:"; // диск C: по-умолчанию присутствует в системе.
            }

            IVirtualDirectory rootDirectory
                = this.fileSystem.GetRootDirectory(driveName);
            return this.lockingPolicy.GetDirStructureWithLockingInfo(
                rootDirectory);
        }

        /// <inheritdoc/>
        public void CreateDirectory(
            string userName,
            string directoryPath)
        {
            string fullPath = this.GetFullPath(userName, directoryPath);
            this.fileSystem.CreateDirectory(fullPath);
            this.RaiseOperationPerformedAsync(
                "Пользователь " + userName + " создал папку " + fullPath,
                userName);
        }

        /// <inheritdoc/>
        public void RemoveDirectory(
            string userName,
            string directoryPath,
            bool recursive)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullPath = this.GetFullPath(userName, directoryPath);

            if (this.IsCurrentOrParentDirForUser(userName, fullPath))
            {
                throw new InvalidOperationException(VirtualFileServer.CantRemoveCurrentDirectory);
            }

            IVirtualDirectory directory = this.fileSystem.GetExistingDirectory(fullPath);
            this.lockingPolicy.ThrowIfCantRemoveDirectory(directory);
            this.fileSystem.RemoveDirectory(fullPath, recursive);
            this.RaiseOperationPerformedAsync(
                "Пользователь " + userName + " удалил папку " + fullPath,
                userName);
        }

        /// <inheritdoc/>
        public void CreateFile(
            string userName,
            string filePath)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullFilePath = this.GetFullPath(userName, filePath);
            this.fileSystem.CreateFile(fullFilePath);
            this.RaiseOperationPerformedAsync(
                "Пользователь " + userName + " создал файл " + fullFilePath,
                userName);
        }

        /// <inheritdoc/>
        public void RemoveFile(
            string userName,
            string filePath)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullFilePath = this.GetFullPath(userName, filePath);
            IVirtualFile file = this.fileSystem.GetExistingFile(fullFilePath);
            this.lockingPolicy.ThrowIfCantRemoveFile(file);
            this.fileSystem.RemoveFile(fullFilePath);
            this.RaiseOperationPerformedAsync(
                "Пользователь " + userName + " удалил файл " + fullFilePath,
                userName);
        }

        /// <inheritdoc/>
        public void LockFile(
            string userName,
            string filePath)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullPath = this.GetFullPath(userName, filePath);
            IVirtualFile file = this.fileSystem.GetExistingFile(fullPath);
            this.lockingPolicy.LockFile(userName, file);
            this.RaiseOperationPerformedAsync(
                "Файл " + fullPath + " заблокирован на удаление пользователем " + userName,
                userName);
        }

        /// <inheritdoc/>
        public void UnlockFile(
            string userName,
            string filePath)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullPath = this.GetFullPath(userName, filePath);
            IVirtualFile file = this.fileSystem.GetExistingFile(fullPath);
            this.lockingPolicy.UnlockFile(userName, file);
            this.RaiseOperationPerformedAsync(
                "Файл " + fullPath + " разблокирован для удаления пользователем " + userName,
                userName);
        }

        /// <inheritdoc/>
        public void Copy(
            string userName,
            string sourcePath,
            string destinationPath)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullSource = this.GetFullPath(userName, sourcePath);
            string fullDestination = this.GetFullPath(userName, destinationPath);
            this.fileSystem.Copy(fullSource, fullDestination);
            this.RaiseOperationPerformedAsync(
                "Пользователь " + userName + " скопировал " + fullSource + " в " + fullDestination,
                userName);
        }

        /// <inheritdoc/>
        public void Move(
            string userName,
            string sourcePath,
            string destinationPath)
        {
            this.connectedUsers.ThrowIfUserIsNotConnected(userName);
            string fullSource = this.GetFullPath(userName, sourcePath);

            if (this.IsCurrentOrParentDirForUser(userName, fullSource))
            {
                throw new InvalidOperationException(VirtualFileServer.CantRemoveCurrentDirectory);
            }

            string fullDestination = this.GetFullPath(userName, destinationPath);
            this.lockingPolicy.ThrowIfCantRemove(fullSource, this.fileSystem);
            this.fileSystem.Move(fullSource, fullDestination);
            this.RaiseOperationPerformedAsync(
                "Пользователь " + userName + " переместил " + fullSource + " в " + fullDestination,
                userName);
        }

        /// <summary>
        /// Получить полное имя файла или папки.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="path">Полный путь или путь относительно текущей
        /// рабочей директории пользователя.</param>
        /// <returns>Полный путь к файлу или папке.</returns>
        private string GetFullPath(string userName, string path)
        {
            path.ValidateCorrectPath();

            if (path.ContainsDriveName())
            {
                return path;
            }
            string currentDir = this.connectedUsers
                .GetConnectedUser(userName).CurrentWorkingDirectoryPath;

            if (this.fileSystem.FindDirectory(currentDir) == null)
            {
                throw new DirectoryNotFoundException(
                    "Рабочая директория пользователя " + userName + " удалена.\n"
                    + "Для корректной работы нужно изменить рабочую директорию.");
            }

            if (!currentDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                currentDir += Path.DirectorySeparatorChar;
            }

            return currentDir + path;
        }

        /// <summary>
        /// Является ли <paramref name="fullPath" /> текущей директорией,
        /// или директорией, родительской по отношению к текущей для
        /// пользователя <paramref name="userName" />.
        /// </summary>
        private bool IsCurrentOrParentDirForUser(
            string userName,
            string fullPath)
        {
            string currentDir = this.connectedUsers
                .GetConnectedUser(userName).CurrentWorkingDirectoryPath;

            return currentDir.ToLowerInvariant().Contains(fullPath.ToLowerInvariant());
        }

        /// <summary>
        /// Уведомить всех пользователей о завершении операции (асинхронно).
        /// </summary>
        /// <param name="notification">Краткое описание операции.</param>
        /// <param name="userName">Имя пользователя, выполнившего операцию.</param>
        private void RaiseOperationPerformedAsync(
            string notification, string userName)
        {
            if (this.OperationPerformed != null)
            {
                Task.Factory.StartNew(() =>
                {
                    this.OperationPerformed(
                        this,
                        new NotificationEventArgs(notification, userName));
                });
            }
        }
    }
}