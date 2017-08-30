namespace VFS.Interfaces.Service
{
    using System.ServiceModel;

    /// <summary>
    /// Интерфейс доступа ОДНОГО клиента к операциям сервера.
    /// </summary>
    [ServiceContract (CallbackContract=typeof(IVFSNotificationHandler))]
    public interface IVFSSingleUserService
    {
        /// <summary>
        /// Начать сеанс работы с виртуальным файловым сервером.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult Connect(string userName);

        /// <summary>
        /// Завершить сеанс работы с виртуальным файловым сервером.
        /// </summary>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult Quit();

        /// <summary>
        /// Подписаться на уведомления о действиях других пользователей.
        /// </summary>
        [OperationContract]
        void SubscribeForNotifications();

        /// <summary>
        /// Создать директорию.
        /// </summary>
        /// <param name="path">Путь к создаваемой папке.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult MakeDirectory(string path);

        /// <summary>
        /// Установить текущую директорию.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult SetCurrentDirectory(string path);

        /// <summary>
        /// Удалить директорию.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <param name="recursive">Вместе со всеми входящими директориями.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult RemoveDirectory(string path, bool recursive);

        /// <summary>
        /// Создать файл.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult MakeFile(string path);

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult DeleteFile(string path);

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult LockFile(string path);

        /// <summary>
        /// Разрешить удаление файла.
        /// </summary>
        /// <param name="path">Путь к файлу (включая имя файла).</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult UnlockFile(string path);

        /// <summary>
        /// Копировать файл или директорию.
        /// </summary>
        /// <param name="sourcePath">Путь к копируемому файлу или директории.</param>
        /// <param name="destinationPath">Путь к целевой директории.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult Copy(
            string sourcePath,
            string destinationPath);

        /// <summary>
        /// Переместить файл или директорию.
        /// </summary>
        /// <param name="sourcePath">Путь к перемещаемому файлу или директории.</param>
        /// <param name="destinationPath">Путь к целевой директории.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult Move(
            string sourcePath,
            string destinationPath);

        /// <summary>
        /// Получить структуру папок и файлов определенного диска.
        /// </summary>
        /// <param name="drive">Буква диска.</param>
        /// <returns>Стандартный результат выполнения операции.</returns>
        [OperationContract]
        StandardOperationResult GetDriveStructure(string drive);
    }
}
