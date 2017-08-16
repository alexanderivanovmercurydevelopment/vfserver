namespace VFS.WCFService
{
    using System;
    using System.ServiceModel;

    using VFS.Interfaces;
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.Service;
    using VFS.Server;
    using VFS.Utilities;

    /// <summary>
    /// Сервис для работы одного пользователя с виртуальным
    /// файловым сервером.
    /// </summary>
    internal class VFSSingleUserServiceWCF : IVFSSingleUserService
    {
        /// <summary>
        /// Экземпляр виртуального файлового сервера.
        /// </summary>
        private static readonly IVirtualFileServer server
            = new SyncronizedVirtualFileServer(
                new VirtualFileServer(),
                100);

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        private string userName;

        /// <summary>
        /// Действие, выполняемое при завершении любой операции сервера.
        /// </summary>
        private Action<string> notifyAction;

        /// <summary>
        /// Начать сеанс работы с виртуальным файловым сервером.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult Connect(
            string userName)
        {
            if (!string.IsNullOrWhiteSpace(this.userName))
            {
                return new StandardOperationResult(
                    null,
                    "Пользователь уже подключен как \"" + this.userName + "\".");
            }

            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.ConnectUser(userName);
                
                int usersCount = 
                    VFSSingleUserServiceWCF.server.GetUsersCount();

                this.userName = userName;
                return new StandardOperationResult(
                    "Подключение к серверу произведено. Количество пользователей - " + usersCount,
                    null);
            });
        }

        /// <summary>
        /// Завершить сеанс работы с виртуальным файловым сервером.
        /// </summary>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult Quit()
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.DisconnectUser(this.userName);
                this.userName = null;

                VFSSingleUserServiceWCF.server.OperationPerformed
                    -= this.ServerOnOperationPerformed;

                return new StandardOperationResult(
                    "Пользователь успешно отключен.",
                    null);
            });
        }

        /// <summary>
        /// Подписаться на уведомления о действиях других пользователей.
        /// </summary>
        public void SubscribeForNotifications()
        {
            if (this.notifyAction != null)
            {
                throw new InvalidOperationException(
                    "Пользователь " + this.userName + " уже подписан на оповещения.");
            }

            IVFSNotificationHandler handler =
                OperationContext.Current
                .GetCallbackChannel<IVFSNotificationHandler>();

            this.notifyAction = handler.HandleNotification;
            VFSSingleUserServiceWCF.server.OperationPerformed  += 
                this.ServerOnOperationPerformed;
        }

        /// <summary>
        /// Создать директорию.
        /// </summary>
        /// <param name="path">Путь к создаваемой папке.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult MakeDirectory(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.CreateDirectory(
                    this.userName, 
                    path);

                return new StandardOperationResult(
                    "Директория " + path + " успешно создана.",
                    null);
            });
        }

        /// <summary>
        /// Установить текущую директорию.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult SetCurrentDirectory(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server
                    .ChangeUsersCurrentDirectory(this.userName, path);

                return new StandardOperationResult(
                    "Текущая директория успешно изменена.",
                    null);
            });
        }

        /// <summary>
        /// Удалить директорию.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <param name="recursive">Вместе со всеми входящими директориями.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult RemoveDirectory(
            string path, 
            bool recursive)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.RemoveDirectory(
                    this.userName,
                    path,
                    recursive);

                return new StandardOperationResult(
                    "Директория " + path + " успешно удалена.",
                    null);
            });
        }

        /// <summary>
        /// Создать файл.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult MakeFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.CreateFile(
                    this.userName,
                    path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно создан.",
                    null);
            });
        }

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult DeleteFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.RemoveFile(
                    this.userName,
                    path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно удален.",
                    null);
            });
        }

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult LockFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.LockFile(this.userName, path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно заблокирован.",
                    null);
            });
        }

        /// <summary>
        /// Разрешить удаление файла.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult UnlockFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.UnlockFile(this.userName, path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно разблокирован.",
                    null);
            });
        }

        /// <summary>
        /// Копировать файл или директорию.
        /// </summary>
        /// <param name="sourcePath">Путь к копируемому файлу или директории.</param>
        /// <param name="destinationPath">Путь к целевой директории.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult Copy(
            string sourcePath, 
            string destinationPath)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.Copy(
                    this.userName, 
                    sourcePath,
                    destinationPath);

                return new StandardOperationResult(
                    "Скопировано успешно.",
                    null);
            });
        }

        /// <summary>
        /// Переместить файл или директорию.
        /// </summary>
        /// <param name="sourcePath">Путь к перемещаемому файлу или директории.</param>
        /// <param name="destinationPath">Путь к целевой директории.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult Move(
            string sourcePath, 
            string destinationPath)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.Move(
                    this.userName,
                    sourcePath,
                    destinationPath);

                return new StandardOperationResult(
                    "Перемещено успешно.",
                    null);
            });
        }

        /// <summary>
        /// Получить структуру папок и файлов определенного диска.
        /// </summary>
        /// <param name="drive">Буква диска.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        public StandardOperationResult GetDriveStructure(
            string drive)
        {
            return this.SafeExecute(() =>
            {
                DriveStructureInfo driveStructure =
                    VFSSingleUserServiceWCF.server.GetDriveStructure(drive);

                return new StandardOperationResult(
                    XMLUtilities.SerializeToXml<DriveStructureInfo>(driveStructure),
                    null);
            });
        }

        /// <summary>
        /// Выполнить операцию, и вернуть нормальное сообщение
        /// об ошибке в случае сбоя операции.
        /// </summary>
        /// <param name="operation">Операция.</param>
        /// <returns>Стандартный результат операции.</returns>
        private StandardOperationResult SafeExecute(
            Func<StandardOperationResult> operation)
        {
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                return new StandardOperationResult(
                    null,
                    ex.Message);
            }
        }

        /// <summary>
        /// Обработчик события завершения операции сервера.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        private void ServerOnOperationPerformed(
            object sender,
            NotificationEventArgs e)
        {
            if (this.notifyAction != null)
            {
                try
                {
                    if (e.UserName != this.userName)
                    {
                        this.notifyAction(e.Notification);
                    }
                }
                catch
                {
                    // при сбое на стороне клиента
                    // больше не оповещаем этого клиента
                    this.notifyAction = null;
                }
            }
        }

        /// <summary>
        /// (Не тестировалось) Попытка корректного завершения 
        /// работы с сервером при сбое клиента.
        /// </summary>
        ~VFSSingleUserServiceWCF()
        {
            if (this.userName != null)
            {
                try
                {
                    VFSSingleUserServiceWCF.server.OperationPerformed 
                        -= this.ServerOnOperationPerformed;

                    VFSSingleUserServiceWCF.server
                        .DisconnectUser(this.userName);
                }
                catch
                { }
            }
        }
    }
}
