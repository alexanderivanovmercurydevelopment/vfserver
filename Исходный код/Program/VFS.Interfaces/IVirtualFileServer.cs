namespace VFS.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using VFS.Interfaces.DriveStructureMessageFormat;

    /// <summary>
    /// Виртуальный файловый сервер.
    /// </summary>
    /// <remarks>ПУТЬ К ФАЙЛУ ИЛИ ДИРЕКТОРИИ - ВЕЗДЕ - ЛИБО ПОЛНЫЙ ПУТЬ,
    /// ЛИБО ПУТЬ ОТНОСИТЕЛЬНО ТЕКУЩЕЙ РАБОЧЕЙ ДИРЕКТОРИИ ПОЛЬЗОВАТЕЛЯ.</remarks>
    public interface IVirtualFileServer
    {
        /// <summary>
        /// Событие завершения выполнения какой-либо операции.
        /// </summary>
        event EventHandler<NotificationEventArgs> OperationPerformed;

        /// <summary>
        /// Подключить нового пользователя.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        void ConnectUser(string userName);

        /// <summary>
        /// Изменить текущую директорию пользователя.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="directoryPath">Путь к новой текущей директории.</param>
        void ChangeUsersCurrentDirectory(
            string userName,
            string directoryPath);

        /// <summary>
        /// Получить полный путь к текущей рабочей папке пользователя.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        string GetUsersCurrentWorkingDirectoryPath(
            string userName);

        /// <summary>
        /// Отключить пользователя.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        void DisconnectUser(string userName);

        /// <summary>
        /// Получить общее количество подключенных пользователей.
        /// </summary>
        /// <returns>Общее количество подключенных пользователей.</returns>
        int GetUsersCount();

        /// <summary>
        /// Получить информацию о структуре диска.
        /// </summary>
        /// <param name="driveName">Имя диска.</param>
        /// <returns>Информация о структуре диска.</returns>
        DriveStructureInfo GetDriveStructure(
            string driveName);

        /// <summary>
        /// Создать папку.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="directoryPath">Путь и имя создаваемой директории.</param>
        void CreateDirectory(
            string userName,
            string directoryPath);

        /// <summary>
        /// Удалить папку.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="directoryPath">Путь к удаляемой папке.</param>
        /// <param name="recursive">Удалить, даже если есть подпапки.</param>
        void RemoveDirectory(
            string userName,
            string directoryPath,
            bool recursive);

        /// <summary>
        /// Создать файл.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="filePath">Путь к файлу.</param>
        void CreateFile(
            string userName,
            string filePath);

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="filePath">Путь к удаляемому файлу.</param>
        void RemoveFile(
            string userName,
            string filePath);

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="filePath">Путь к удаляемому файлу.</param>
        void LockFile(
            string userName,
            string filePath);

        /// <summary>
        /// Снять запрет удаления файла.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="filePath">Путь к файлу.</param>
        void UnlockFile(
            string userName,
            string filePath);

        /// <summary>
        /// Копировать папку или файл в целевую папку.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="sourcePath">Путь к копируемой папке или файлу.</param>
        /// <param name="destinationPath">Путь к целевой папке.</param>
        void Copy(
            string userName,
            string sourcePath,
            string destinationPath);

        /// <summary>
        /// Переместить директорию или файл в целевую папку.
        /// </summary>
        /// <param name="userName">Имя пользователя.</param>
        /// <param name="sourcePath">Путь к перемещаемой папке или файлу.</param>
        /// <param name="destinationPath">Путь к целевой папке.</param>
        void Move(
            string userName,
            string sourcePath,
            string destinationPath);

        /// <summary>
        /// Загрузить данные в файл.
        /// </summary>
        Task UploadFileAsync(string userName, string filePath, string data);

        /// <summary>
        /// Получить данные из файла.
        /// </summary>
        Task<string> DownloadFileAsync(string userName, string filePath);
    }
}