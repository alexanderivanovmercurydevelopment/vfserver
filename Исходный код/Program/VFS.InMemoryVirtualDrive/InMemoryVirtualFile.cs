namespace VFS.InMemoryVirtualDrive
{
    using System;

    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Виртуальный файл, хранящий данные в памяти.
    /// </summary>
    internal class InMemoryVirtualFile : IVirtualFile
    {
        /// <summary>
        /// Имя файла.
        /// </summary>
        private string name;

        /// <summary>
        /// Конфигурация виртуального диска.
        /// </summary>
        private InMemoryVirtualDriveConfig config;

        /// <summary>
        /// Создать файл, хранящий данные в памяти.
        /// </summary>
        /// <param name="name">Имя файла.</param>
        /// <param name="config">Конфигурация виртуального диска.</param>
        internal InMemoryVirtualFile(
            string name, 
            InMemoryVirtualDriveConfig config)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(
                    "name",
                    "Имя файла не должно быть пустым.");
            }

            if (name.Length > config.MaxFileNameLength)
            {
                throw new InvalidOperationException(
                    "Длина имени файла превышает допустимый предел - "
                    + config.MaxDirectoryNameLength + ".");
            }

            this.name = name;
            this.config = config;
        }

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
    }
}
