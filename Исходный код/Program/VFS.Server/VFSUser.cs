namespace VFS.Server
{
    using System;

    using VFS.Utilities;

    /// <summary>
    /// Пользователь виртуального файлового сервера.
    /// </summary>
    internal class VFSUser
    {
        /// <summary>
        /// Путь к текущей папке пользователя.
        /// </summary>
        private string currentDirectoryPath;

        /// <summary>
        /// Создать нового пользователя виртуального 
        /// файлового сервера.
        /// </summary>
        /// <param name="name">Имя пользователя.</param>
        /// <param name="currentDirectoryPath">Путь к текущей рабочей
        /// папке пользователя.</param>
        internal VFSUser(string name, string currentDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(
                    nameof(name),
                    "Имя пользователя не должно быть пустым.");
            }

            currentDirectoryPath.ValidateCorrectPath();

            this.Name = name;
            this.currentDirectoryPath = currentDirectoryPath;
        }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Путь к текущей папке пользователя.
        /// </summary>
        internal string CurrentDirectoryPath 
        {
            get
            {
                return this.currentDirectoryPath;
            }
            set
            {
                value.ValidateCorrectPath();
                this.currentDirectoryPath = value;
            }
        }
    }
}
