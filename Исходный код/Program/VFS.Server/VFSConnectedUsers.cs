namespace VFS.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Активные (подключенные) пользователи
    /// виртуального файлового сервера.
    /// </summary>
    internal class VFSConnectedUsers
    {
        /// <summary>
        /// Подключенные пользователи.
        /// </summary>
        private readonly List<VFSUser> registeredUsers
            = new List<VFSUser>();

        /// <summary>
        /// Событие отключения пользователя.
        /// </summary>
        public event EventHandler<VFSUserEventArgs> UserUnregistered;

        /// <summary>
        /// Подключить нового пользователя.
        /// </summary>
        /// <param name="name">Имя нового пользователя.</param>
        /// <param name="currentDirPath">Путь к текущей директории.</param>
        internal void RegisterUser(string name, string currentDirPath)
        {
            if (this.registeredUsers.Any(u => u.Name == name))
            {
                throw new InvalidOperationException(
                    "Пользователь с именем " + name + " уже подключен.");
            }

            this.registeredUsers.Add(new VFSUser(name, currentDirPath));
        }

        /// <summary>
        /// Отключить пользователя.
        /// </summary>
        /// <param name="name">Имя подключенного пользователя.</param>
        internal void UnregisterUser(string name)
        {
            this.ThrowIfUserIsNotConnected(name);

            VFSUser user = this.registeredUsers.First(
                u => u.Name == name);

            this.registeredUsers.Remove(user);

            this.UserUnregistered?.Invoke(
                this,
                new VFSUserEventArgs(user));
        }

        /// <summary>
        /// Удостовериться, что пользователь подключен.
        /// </summary>
        /// <param name="name">Имя пользователя.</param>
        /// <exception cref="InvalidOperationException">
        /// Пользователь с таким именем еще не подключен.
        /// </exception>
        internal void ThrowIfUserIsNotConnected(string name)
        {
            VFSUser user = this.registeredUsers
                .FirstOrDefault(u => u.Name == name);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Пользователь " + name + " еще не подключен.");
            }
        }

        /// <summary>
        /// Получить подключенного пользователя по имени.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <returns>Подключенный пользователь.</returns>
        internal VFSUser GetConnectedUser(string userName)
        {
            this.ThrowIfUserIsNotConnected(userName);
            return this.registeredUsers.First(u => u.Name == userName);
        }

        /// <summary>
        /// Список подключенных пользователей.
        /// </summary>
        internal IEnumerable<VFSUser> RegisteredUsers 
            => this.registeredUsers.ToList();
    }
}
