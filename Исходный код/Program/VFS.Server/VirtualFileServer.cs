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

    /// <summary>
    /// Виртуальный файловый сервер.
    /// </summary>
    public class VirtualFileServer : IVirtualFileServer
    {
        private const string CantRemoveCurrentDirectory = "Нельзя удалить или переместить текущую директорию, "
            + "или директорию, родительскую по отношению к текущей.";

        /// <summary>
        /// Единая виртуальная файловая система.
        /// </summary>
        private readonly VirtualFileSystem fileSystem;

        /// <summary>
        /// Подключенные пользователи.
        /// </summary>
        private readonly VFSConnectedUsers connectedUsers;

        /// <summary>
        /// Политика блокировки удаления файлов.
        /// </summary>
        private readonly VFSLockingPolicy lockingPolicy;

        /// <summary>
        /// Конфигурация виртуального файлового сервера.
        /// </summary>
        private readonly VFSConfig config;

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

        /// <summary>
        /// Подключить нового пользователя.
        /// </summary>
        /// <param name="userName">Имя еще не подключенного пользователя.</param>
        public void ConnectUser(
            string userName)
        {
            this.connectedUsers.RegisterUser(userName, this.config.DefaultDirPath);
        }

        /// <summary>
        /// Изменить текущую директорию пользователя.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="directoryPath">Путь к новой текущей директории.</param>
        public void ChangeUsersCurrentDirectory(
            string userName, 
            string directoryPath)
        {
            string fullDirPath = this.GetFullPath(userName, directoryPath);
            this.fileSystem.CheckDirectoryExisting(fullDirPath);
            this.connectedUsers.GetConnectedUser(userName)
                .CurrentDirectoryPath = fullDirPath;
        }

        /// <summary>
        /// Отключить пользователя.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        public void DisconnectUser(
            string userName)
        {
            this.connectedUsers.UnregisterUser(userName);
        }

        /// <summary>
        /// Получить общее количество подключенных пользователей.
        /// </summary>
        /// <returns>Общее количество подключенных пользователей.</returns>
        public int GetUsersCount()
        {
            return this.connectedUsers.RegisteredUsers.Count();
        }

        /// <summary>
        /// Получить информацию о структуре диска.
        /// </summary>
        /// <param name="driveName">Имя диска.</param>
        /// <returns>Информация о структуре диска.</returns>
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

        /// <summary>
        /// Создать папку.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="directoryPath">Путь и имя создаваемой директории.</param>
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

        /// <summary>
        /// Удалить папку.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="directoryPath">Путь к удаляемой папке.</param>
        /// <param name="recursive">Удалить, даже если есть подпапки.</param>
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

        /// <summary>
        /// Создать файл.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="filePath">Путь к файлу.</param>
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

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="filePath">Путь к удаляемому файлу.</param>
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

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="filePath">Путь к удаляемому файлу.</param>
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

        /// <summary>
        /// Снять запрет удаления файла.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="filePath">Путь к файлу.</param>
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

        /// <summary>
        /// Копировать папку или файл в целевую папку.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="sourcePath">Путь к копируемой папке или файлу.</param>
        /// <param name="destinationPath">Путь к целевой папке.</param>
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

        /// <summary>
        /// Переместить папку или файл в другую папку.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="sourcePath">Путь к перемещаемой папке или файлу.</param>
        /// <param name="destinationPath">Путь к целевой папке.</param>
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
            else
            {
                string currentDir = this.connectedUsers
                    .GetConnectedUser(userName).CurrentDirectoryPath;

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
        }

        /// <summary>
        /// Сгенерировать исключение, если пользователь
        /// пытается удалить свою текущую директорию, или директорию,
        /// родительскую по отношению к текущей.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="fullPath">Полный путь к директории, которую
        /// пользователь собирается удалить/переместить.</param>
        private bool IsCurrentOrParentDirForUser(
            string userName,
            string fullPath)
        {
            string currentDir = this.connectedUsers
                .GetConnectedUser(userName).CurrentDirectoryPath;

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
