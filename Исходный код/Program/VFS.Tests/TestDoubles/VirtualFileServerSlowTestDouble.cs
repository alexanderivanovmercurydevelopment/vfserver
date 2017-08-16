namespace VFS.Tests.TestDoubles
{
    using System;
    using System.Collections.Generic;

    using VFS.Interfaces;
    using VFS.Interfaces.DriveStructureMessageFormat;

    /// <summary>
    /// Имитация виртуального файлового сервера, выполняющего 
    /// длительные операции и корректно создающего новые папки.
    /// </summary>
    public class VirtualFileServerSlowTestDouble : IVirtualFileServer
    {
        /// <summary>
        /// Среднее время выполнения каждой операции.
        /// </summary>
        private readonly int operationTimeInMilliseconds;

        /// <summary>
        /// Имена созданных папок.
        /// </summary>
        private readonly List<string> createdFolderNames
            = new List<string>();

        /// <summary>
        /// Создать имитацию виртуального файлового сервера, выполняющего
        /// длительные операции и успешно создающего новые папки.
        /// </summary>
        /// <param name="milliseconds">Примерное время выполнения
        /// каждой операции.</param>
        public VirtualFileServerSlowTestDouble(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                throw new ArgumentException(
                    "milliseconds",
                    "Продолжительность операций не может быть " + milliseconds + " миллисекунд.");
            }

            this.operationTimeInMilliseconds = milliseconds;
        }

        /// <summary>
        /// Список имен созданных директорий.
        /// </summary>
        public List<string> CreatedDirectories { get { return this.createdFolderNames; } }

        /// <summary>
        /// Метод сам по себе - не потокобезопасный. 
        /// Использование данного сервера без обертки-"синхронизатора" должно
        /// привести к некорректным результатам при одновременном создании
        /// нескольких папок с одинаковыми именами.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="directoryPath"></param>
        public void CreateDirectory(string userName, string directoryPath)
        {
            if (this.createdFolderNames.Contains(directoryPath))
            {
                throw new InvalidOperationException(
                    "Папка уже существует.");
            }

            this.PerformSlowOperation();

            this.createdFolderNames.Add(directoryPath);
        }
                
        public event EventHandler<NotificationEventArgs> OperationPerformed;

        public void ConnectUser(string userName)
        {
            this.PerformSlowOperation();
        }

        public void ChangeUsersCurrentDirectory(string userName, string directoryPath)
        {
            this.PerformSlowOperation();
        }

        public void DisconnectUser(string userName)
        {
            this.PerformSlowOperation();
        }

        public int GetUsersCount()
        {
            this.PerformSlowOperation();
            return 0;
        }

        public DriveStructureInfo GetDriveStructure(string driveName)
        {
            this.PerformSlowOperation();
            return null;
        }

        public void RemoveDirectory(string userName, string directoryPath, bool recursive)
        {
            this.PerformSlowOperation();
        }

        public void CreateFile(string userName, string filePath)
        {
            this.PerformSlowOperation();
        }

        public void RemoveFile(string userName, string filePath)
        {
            this.PerformSlowOperation();
        }

        public void LockFile(string userName, string filePath)
        {
            this.PerformSlowOperation();
        }

        public void UnlockFile(string userName, string filePath)
        {
            this.PerformSlowOperation();
        }

        public void Copy(string userName, string sourcePath, string destinationPath)
        {
            this.PerformSlowOperation();
        }

        public void Move(string userName, string sourcePath, string destinationPath)
        {
            this.PerformSlowOperation();
        }

        private void PerformSlowOperation()
        {
            System.Threading.Thread.Sleep(500);

            if (this.OperationPerformed != null)
            {
                this.OperationPerformed(this, new NotificationEventArgs("Операция выполнена успешно.", "noUser"));
            }
        }
    }
}
