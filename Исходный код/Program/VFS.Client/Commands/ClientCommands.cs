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
        md,

        /// <summary>
        /// Установить текущую рабочую директорию.
        /// </summary>
        cd,

        /// <summary>
        /// Удалить директорию.
        /// </summary>
        rd,

        /// <summary>
        /// Удалить директорию и все её поддиректории.
        /// </summary>
        deltree,

        /// <summary>
        /// Создать файл.
        /// </summary>
        mf,

        /// <summary>
        /// Удалить файл.
        /// </summary>
        del,

        /// <summary>
        /// Запретить удаление файла.
        /// </summary>
        @lock,

        /// <summary>
        /// Отменить запрет удаления файла.
        /// </summary>
        unlock,

        /// <summary>
        /// Копировать файл или директорию.
        /// </summary>
        copy,

        /// <summary>
        /// Переместить файл или директорию.
        /// </summary>
        move,

        /// <summary>
        /// Получить информацию о дереве каталогов.
        /// </summary>
        print
    }
}
