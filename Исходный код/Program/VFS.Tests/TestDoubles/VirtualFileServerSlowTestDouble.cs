namespace VFS.Tests.TestDoubles
{
    using System;
    using System.Collections.Generic;

    using VFS.Interfaces;
    using VFS.Interfaces.DriveStructureMessageFormat;

    internal class VirtualFileServerSlowTestDouble : IVirtualFileServer
    {
        private readonly List<string> createdFolderNames
            = new List<string>();

        public List<string> CreatedDirectoriesNames => this.createdFolderNames;

        /// <summary>
        /// Метод сам по себе - не потокобезопасный. 
        /// Использование данного сервера без обертки-"синхронизатора" должно
        /// привести к некорректным результатам при одновременном создании
        /// нескольких папок с одинаковыми именами.
        /// </summary>
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

            this.OperationPerformed?.Invoke(
                this, 
                new NotificationEventArgs("Операция выполнена успешно.", "noUser"));
        }
    }
}
