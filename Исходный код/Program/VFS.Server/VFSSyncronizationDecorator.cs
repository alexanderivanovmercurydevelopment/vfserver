namespace VFS.Server
{
    using System;

    using VFS.Interfaces;
    using VFS.Interfaces.DriveStructureMessageFormat;

    /// <inheritdoc/>
    /// <summary>
    /// Обертка виртуального файлового сервера, позволяющая
    /// работать с ним сразу многим пользователям.
    /// </summary>
    public class VFSSyncronizationDecorator : IVirtualFileServer
    {
        private readonly int maxParallelQueries;

        private readonly object syncObject = new object();
        private readonly IVirtualFileServer vfServer;

        /// <summary>
        /// Текущее количество выполняющихся запросов.
        /// </summary>
        private int currentQueriesCount;

        /// <summary>
        /// Создать обертку-"синхронизатор" виртуального файлового сервера.
        /// </summary>
        /// <param name="virtualFileServer">Виртуальный файловый сервер.</param>
        /// <param name="maxParallelQueries">Максимальное количество параллельных запросов.</param>
        /// <remarks>
        /// Если количество одновременно выполняющихся запросов достигнет
        /// <paramref name="maxParallelQueries" />, последующие запросы не будут выполняться.
        /// </remarks>
        public VFSSyncronizationDecorator(
            IVirtualFileServer virtualFileServer,
            int maxParallelQueries)
        {
            if (virtualFileServer == null)
            {
                throw new ArgumentNullException(
                    nameof(virtualFileServer),
                    "Необходимо передать экземпляр виртуального файлового сервера.");
            }

            if (maxParallelQueries <= 0)
            {
                throw new ArgumentException(
                    "Количество пользователей должно быть > 0.",
                    nameof(maxParallelQueries));
            }

            this.vfServer = virtualFileServer;
            this.maxParallelQueries = maxParallelQueries;
        }

        /// <summary>
        /// Событие завершения выполнения какой-либо операции.
        /// </summary>
        public event EventHandler<NotificationEventArgs> OperationPerformed
        {
            add { this.vfServer.OperationPerformed += value; }

            remove { this.vfServer.OperationPerformed -= value; }
        }

        /// <inheritdoc/>
        public void ConnectUser(
            string userName)
        {
            this.SyncPerformAction(() => { this.vfServer.ConnectUser(userName); });
        }

        /// <inheritdoc/>
        public void ChangeUsersCurrentDirectory(
            string userName,
            string directoryPath)
        {
            this.SyncPerformAction(() => { this.vfServer.ChangeUsersCurrentDirectory(userName, directoryPath); });
        }

        /// <inheritdoc/>
        public string GetUsersCurrentWorkingDirectoryPath(string userName)
        {
            return this.SyncPerformFunction(() => this.vfServer.GetUsersCurrentWorkingDirectoryPath(userName));
        }

        /// <inheritdoc/>
        public void DisconnectUser(
            string userName)
        {
            this.SyncPerformAction(() => { this.vfServer.DisconnectUser(userName); });
        }

        /// <inheritdoc/>
        public int GetUsersCount()
        {
            return this.SyncPerformFunction(
                () => this.vfServer.GetUsersCount());
        }

        /// <inheritdoc/>
        public DriveStructureInfo GetDriveStructure(
            string driveName)
        {
            return this.SyncPerformFunction(
                () => this.vfServer.GetDriveStructure(driveName));
        }

        /// <inheritdoc/>
        public void CreateDirectory(
            string userName,
            string directoryPath)
        {
            this.SyncPerformAction(() => { this.vfServer.CreateDirectory(userName, directoryPath); });
        }

        /// <inheritdoc/>
        public void RemoveDirectory(
            string userName,
            string directoryPath,
            bool recursive)
        {
            this.SyncPerformAction(() => { this.vfServer.RemoveDirectory(userName, directoryPath, recursive); });
        }

        /// <inheritdoc/>
        public void CreateFile(
            string userName,
            string filePath)
        {
            this.SyncPerformAction(() => { this.vfServer.CreateFile(userName, filePath); });
        }

        /// <inheritdoc/>
        public void RemoveFile(
            string userName,
            string filePath)
        {
            this.SyncPerformAction(() => { this.vfServer.RemoveFile(userName, filePath); });
        }

        /// <inheritdoc/>
        public void LockFile(
            string userName,
            string filePath)
        {
            this.SyncPerformAction(() => { this.vfServer.LockFile(userName, filePath); });
        }

        /// <inheritdoc/>
        public void UnlockFile(
            string userName,
            string filePath)
        {
            this.SyncPerformAction(() => { this.vfServer.UnlockFile(userName, filePath); });
        }

        /// <inheritdoc/>
        public void Copy(
            string userName,
            string sourcePath,
            string destinationPath)
        {
            this.SyncPerformAction(() => { this.vfServer.Copy(userName, sourcePath, destinationPath); });
        }

        /// <inheritdoc/>
        public void Move(
            string userName,
            string sourcePath,
            string destinationPath)
        {
            this.SyncPerformAction(() => { this.vfServer.Move(userName, sourcePath, destinationPath); });
        }

        /// <summary>
        /// Выполнить функцию синхронно (т.е. после завершения выполнения
        /// других операций и функций).
        /// </summary>
        private T SyncPerformFunction<T>(Func<T> function)
        {
            lock (this.syncObject)
            {
                this.ThrowIfOverload();
                this.currentQueriesCount++;
                T result = function();
                this.currentQueriesCount--;
                return result;
            }
        }

        /// <summary>
        /// Выполнить операцию синхронно, т.е. после завершения
        /// выполнения других операций и функций.
        /// </summary>
        private void SyncPerformAction(Action action)
        {
            lock (this.syncObject)
            {
                this.ThrowIfOverload();
                this.currentQueriesCount++;
                action();
                this.currentQueriesCount--;
            }
        }

        /// <summary>
        /// Выдать исключение, если превышен предел одновременных
        /// запросов.
        /// </summary>
        private void ThrowIfOverload()
        {
            if (this.currentQueriesCount == this.maxParallelQueries)
            {
                throw new InvalidOperationException(
                    "Превышено максимальное одновременное количество запросов к серверу - "
                    + this.maxParallelQueries + ".");
            }
        }
    }
}