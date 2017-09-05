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
        private static readonly IVirtualFileServer server
            = new VFSSyncronizationDecorator(
                new VirtualFileServer(),
                100);

        private string connectedUserName;

        /// <summary>
        /// Действие, выполняемое при завершении любой операции сервера.
        /// </summary>
        private Action<string> notifyAction;

        public StandardOperationResult Connect(
            string userName)
        {
            if (!string.IsNullOrWhiteSpace(this.connectedUserName))
            {
                return new StandardOperationResult(
                    null,
                    "Пользователь уже подключен как \"" + this.connectedUserName + "\".");
            }

            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.ConnectUser(userName);
                
                int usersCount = 
                    VFSSingleUserServiceWCF.server.GetUsersCount();

                this.connectedUserName = userName;
                return new StandardOperationResult(
                    "Подключение к серверу произведено. Количество пользователей - " + usersCount,
                    null);
            });
        }

        public StandardOperationResult Quit()
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.DisconnectUser(this.connectedUserName);
                this.connectedUserName = null;

                VFSSingleUserServiceWCF.server.OperationPerformed
                    -= this.ServerOnOperationPerformed;

                return new StandardOperationResult(
                    "Пользователь успешно отключен.",
                    null);
            });
        }

        public void SubscribeForNotifications()
        {
            if (this.notifyAction != null)
            {
                throw new InvalidOperationException(
                    "Пользователь " + this.connectedUserName + " уже подписан на оповещения.");
            }

            IVFSNotificationHandler handler =
                OperationContext.Current
                .GetCallbackChannel<IVFSNotificationHandler>();

            this.notifyAction = handler.HandleNotification;
            VFSSingleUserServiceWCF.server.OperationPerformed  += 
                this.ServerOnOperationPerformed;
        }

        public StandardOperationResult MakeDirectory(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.CreateDirectory(
                    this.connectedUserName, 
                    path);

                return new StandardOperationResult(
                    "Директория " + path + " успешно создана.",
                    null);
            });
        }

        public StandardOperationResult SetCurrentWorkingDirectory(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server
                    .ChangeUsersCurrentDirectory(this.connectedUserName, path);

                return new StandardOperationResult(
                    "Текущая директория успешно изменена.",
                    null);
            });
        }

        public StandardOperationResult RemoveDirectory(
            string path, 
            bool recursive)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.RemoveDirectory(
                    this.connectedUserName,
                    path,
                    recursive);

                return new StandardOperationResult(
                    "Директория " + path + " успешно удалена.",
                    null);
            });
        }

        public StandardOperationResult MakeFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.CreateFile(
                    this.connectedUserName,
                    path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно создан.",
                    null);
            });
        }

        public StandardOperationResult DeleteFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.RemoveFile(
                    this.connectedUserName,
                    path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно удален.",
                    null);
            });
        }

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        public StandardOperationResult LockFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.LockFile(this.connectedUserName, path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно заблокирован.",
                    null);
            });
        }

        /// <summary>
        /// Разрешить удаление файла.
        /// </summary>
        public StandardOperationResult UnlockFile(
            string path)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.UnlockFile(this.connectedUserName, path);

                return new StandardOperationResult(
                    "Файл " + path + " успешно разблокирован.",
                    null);
            });
        }

        public StandardOperationResult Copy(
            string sourcePath, 
            string destinationPath)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.Copy(
                    this.connectedUserName, 
                    sourcePath,
                    destinationPath);

                return new StandardOperationResult(
                    "Скопировано успешно.",
                    null);
            });
        }

        public StandardOperationResult Move(
            string sourcePath, 
            string destinationPath)
        {
            return this.SafeExecute(() =>
            {
                VFSSingleUserServiceWCF.server.Move(
                    this.connectedUserName,
                    sourcePath,
                    destinationPath);

                return new StandardOperationResult(
                    "Перемещено успешно.",
                    null);
            });
        }

        public StandardOperationResult GetDriveStructure(
            string drive)
        {
            return this.SafeExecute(() =>
            {
                DriveStructureInfo driveStructure =
                    VFSSingleUserServiceWCF.server.GetDriveStructure(drive);

                return new StandardOperationResult(
                    XmlUtilities.SerializeToXml(driveStructure),
                    null);
            });
        }

        /// <summary>
        /// Выполнить операцию, и вернуть нормальное сообщение
        /// об ошибке в случае сбоя операции.
        /// </summary>
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

        private void ServerOnOperationPerformed(
            object sender,
            NotificationEventArgs e)
        {
            if (this.notifyAction != null)
            {
                try
                {
                    if (e.UserName != this.connectedUserName)
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
            if (this.connectedUserName != null)
            {
                try
                {
                    VFSSingleUserServiceWCF.server.OperationPerformed 
                        -= this.ServerOnOperationPerformed;

                    VFSSingleUserServiceWCF.server
                        .DisconnectUser(this.connectedUserName);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                // Prevent any error during finalization.
                catch
                { }
            }
        }
    }
}
