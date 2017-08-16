namespace VFS.Server
{
    using VFS.Utilities;

    /// <summary>
    /// Конфигурация виртуального файлового сервера.
    /// </summary>
    internal class VFSConfig
    {
        /// <summary>
        /// Создать конфигурацию виртуального файлового сервера.
        /// </summary>
        /// <param name="defauldDirPath">Путь к папке, с которой
        /// начинают работу все только что подключенные пользователи.</param>
        internal VFSConfig(string defauldDirPath)
        {
            defauldDirPath.ValidateCorrectPath();
            this.DefaultDirPath = defauldDirPath;
        }

        /// <summary>
        /// Путь к директории, с которой начинают 
        /// работу все новые пользователи
        /// </summary>
        internal string DefaultDirPath { get; private set; }
    }
}
