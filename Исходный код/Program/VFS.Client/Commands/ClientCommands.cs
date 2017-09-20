namespace VFS.Client.Commands
{
    /// <summary>
    /// Перечисление возможных команд, вводимых пользователем.
    /// </summary>
    /// <remarks>Здесь команды записываются в нижнем регистре.</remarks>
    public enum ClientCommands
    {
        /// <summary>
        /// Создать директорию.
        /// </summary>
        Md,

        /// <summary>
        /// Установить текущую рабочую директорию.
        /// </summary>
        Cd,

        /// <summary>
        /// Удалить директорию.
        /// </summary>
        Rd,

        /// <summary>
        /// Удалить директорию и все её поддиректории.
        /// </summary>
        Deltree,

        /// <summary>
        /// Создать файл.
        /// </summary>
        Mf,

        /// <summary>
        /// Удалить файл.
        /// </summary>
        Del,

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        Lock,

        /// <summary>
        /// Отменить запрет удаления файла.
        /// </summary>
        Unlock,

        /// <summary>
        /// Копировать файл или директорию.
        /// </summary>
        Copy,

        /// <summary>
        /// Переместить файл или директорию.
        /// </summary>
        Move,

        /// <summary>
        /// Получить информацию о дереве каталогов.
        /// </summary>
        Print,

        /// <summary>
        /// Загрузить данные в файл.
        /// </summary>
        Upload,

        /// <summary>
        /// Загрузить данные из файла.
        /// </summary>
        Download
    }
}