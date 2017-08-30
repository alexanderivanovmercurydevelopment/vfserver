namespace VFS.Client
{
    using System;

    using VFS.Client.Commands;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Класс клиентского приложения виртуального
    /// файлового сервера.
    /// </summary>
    public class ClientApplication
    {
        /// <summary>
        /// Поставщик доступа к интерфейсу работы 
        /// с виртуальным файловым сервером.
        /// </summary>
        private readonly IVFSSingleUserServiceProvider provider;

        /// <summary>
        /// Интерфейс доступа пользователя к виртуальному 
        /// файловому серверу.
        /// </summary>
        private IVFSSingleUserService server;

        /// <summary>
        /// Подключение к серверу произведено.
        /// </summary>
        private bool connected;

        /// <summary>
        /// Клиентское приложение для работы с 
        /// виртуальным файловым сервером.
        /// </summary>
        /// <param name="provider">Поставщик доступа к интерфейсу
        /// работы с виртуальным файловым сервером.</param>
        public ClientApplication(
            IVFSSingleUserServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            this.provider = provider;
        }

        /// <summary>
        /// Подключиться к серверу.
        /// </summary>
        /// <param name="hostName">Имя сервера.</param>
        /// <param name="port">Порт.</param>
        /// <param name="userName">Имя пользователя.</param>
        /// <returns>Сообщение о результате подключения.</returns>
        public string Connect(
            string hostName,
            int? port,
            string userName)
        {
            this.server = this.provider.CreateVFSService(
                hostName,
                port,
                new ConsoleNotificationHandler());

            StandardOperationResult result = 
                this.server.Connect(userName);

            if (result.Succeed)
            {
                this.connected = true;
                return result.ResultMessage;
            }
            else
            {
                throw new InvalidOperationException(
                    "Ошибка подключения: " + result.ErrorMessage);
            }
        }

        /// <summary>
        /// Выполнить команду виртуального файлового сервера.
        /// </summary>
        /// <param name="commandString">Команда (например: "md C:\test").</param>
        /// <returns>Результат выполнения команды (в т.ч. может быть
        /// информация об ошибке сервера или подсказка).</returns>
        public StandardOperationResult TryPerformCommand(string commandString)
        {
            if (!this.connected)
            {
                throw new InvalidOperationException(
                    "Подключение к серверу не произведено");
            }
            else if (string.IsNullOrWhiteSpace(commandString))
            {
                return new StandardOperationResult(
                    null, 
                    "Введена пустая команда.");
            }

            VFSClientCommand command =
                CommandCreator.CreateCommand(
                    ConsoleCommandParser.GetCommandName(commandString),
                    this.server);

            if (command == null)
            {
                string commandName = ConsoleCommandParser.GetCommandName(commandString);
                string message = $"Команды {commandName} не существует.";

                return new StandardOperationResult(null, message);
            }

            return command.Execute(
                ConsoleCommandParser.GetCommandParams(commandString));
        }

        /// <summary>
        /// Отключиться от сервера.
        /// </summary>
        public void Disconnect()
        {
            if (this.connected)
            {
                this.server.Quit();
                this.provider.Dispose();
                this.connected = false;
            }
        }        
    }
}
