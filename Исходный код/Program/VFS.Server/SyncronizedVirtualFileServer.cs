namespace VFS.Server
{
    using System;

    using VFS.Interfaces;
    using VFS.Interfaces.DriveStructureMessageFormat;

    /// <summary>
    /// Обертка виртуального файлового сервера, позволяющая 
    /// работать с ним сразу многим пользователям.
    /// </summary>
    public class SyncronizedVirtualFileServer : IVirtualFileServer
    {
        /// <summary>
        /// Экземпляр виртуального файлового сервера.
        /// </summary>
        private readonly IVirtualFileServer vfServer;

        /// <summary>
        /// Максимальное количество одновременных запросов.
        /// </summary>
        private readonly int maxParallelQueries;

        /// <summary>
        /// Текущее количество выполняющихся запросов.
        /// </summary>
        private int currentQueriesCount = 0;

        /// <summary>
        /// Объект для синхронизации.
        /// </summary>
        private object syncObject = new object();

        /// <summary>
        /// Создать обертку-"синхронизатор" виртуального файлового сервера.
        /// </summary>
        /// <param name="virtualFileServer">Виртуальный файловый сервер.</param>
        /// <param name="maxParallelQueries">Максимальное количество параллельных запросов.</param>
        /// <remarks>
        /// Если количество одновременно выполняющихся запросов достигнет
        /// <paramref name="maxParallelQueries"/>, последующие запросы не будут выполняться.
        /// </remarks>
        public SyncronizedVirtualFileServer(
            IVirtualFileServer virtualFileServer,
            int maxParallelQueries)
        {
            if (virtualFileServer == null)
            {
                throw new ArgumentNullException(
                    "virtualFileServer",
                    "Необходимо передать экземпляр виртуального файлового сервера.");
            }

            if (maxParallelQueries <= 0)
            {
                throw new ArgumentException(
                    "maxParallelUsers",
                    "Количество пользователей должно быть > 0.");
            }

            this.vfServer = virtualFileServer;
            this.maxParallelQueries = maxParallelQueries;
        }

        /// <summary>
        /// Событие завершения выполнения какой-либо операции.
        /// </summary>
        public event EventHandler<NotificationEventArgs> OperationPerformed
        {
            add
            {
                this.vfServer.OperationPerformed += value;
            }

            remove
            {
                this.vfServer.OperationPerformed -= value;
            }
        }

        /// <summary>
        /// Подключить нового пользователя.
        /// </summary>
        /// <param name="userName">Имя еще не подключенного пользователя.</param>
        public void ConnectUser(
            string userName)
        {
            this.SyncPerformAction(() => 
            { 
                this.vfServer.ConnectUser(userName); 
            });
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
            this.SyncPerformAction(() => 
            { 
                this.vfServer.ChangeUsersCurrentDirectory(userName, directoryPath); 
            });           
        }

        /// <summary>
        /// Отключить пользователя.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        public void DisconnectUser(
            string userName)
        {
            this.SyncPerformAction(() =>
            {
                this.vfServer.DisconnectUser(userName);
            });                      
        }

        /// <summary>
        /// Получить общее количество подключенных пользователей.
        /// </summary>
        /// <returns>Общее количество подключенных пользователей.</returns>
        public int GetUsersCount()
        {
            return this.SyncPerformFunction<int>(() =>
            {
                return this.vfServer.GetUsersCount();
            });            
        }

        /// <summary>
        /// Получить информацию о структуре диска.
        /// </summary>
        /// <param name="driveName">Имя диска.</param>
        /// <returns>Информация о структуре диска.</returns>
        public DriveStructureInfo GetDriveStructure(
            string driveName)
        {
            return this.SyncPerformFunction<DriveStructureInfo>(() =>
            {
                return this.vfServer.GetDriveStructure(driveName);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.CreateDirectory(userName, directoryPath);
            });            
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.RemoveDirectory(userName, directoryPath, recursive);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.CreateFile(userName, filePath);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.RemoveFile(userName, filePath);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.LockFile(userName, filePath);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.UnlockFile(userName, filePath);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.Copy(userName, sourcePath, destinationPath);
            });
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
            this.SyncPerformAction(() =>
            {
                this.vfServer.Move(userName, sourcePath, destinationPath);
            });            
        }

        /// <summary>
        /// Выполнить функцию синхронно (т.е. после завершения выполнения
        /// других операций и функций).
        /// </summary>
        /// <typeparam name="T">Тип результата функции.</typeparam>
        /// <param name="function">Функция.</param>
        /// <returns>Результат функции.</returns>
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
        /// <param name="action">Операция.</param>
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
