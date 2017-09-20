namespace VFS.Client
{
    using System;

    using VFS.Client.Commands;
    using VFS.Interfaces.Service;

    internal class ClientApplication
    {
        /// <summary>
        /// Поставщик доступа к интерфейсу работы
        /// с виртуальным файловым сервером.
        /// </summary>
        private readonly IVFSSingleUserServiceProvider provider;

        private bool connected;

        /// <summary>
        /// Интерфейс доступа пользователя к виртуальному
        /// файловому серверу.
        /// </summary>
        private IVFSSingleUserService server;

        public ClientApplication(
            IVFSSingleUserServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            this.provider = provider;
        }

        public string Connect(
            string userName)
        {
            this.server = this.provider.CreateVFSService(
                new ConsoleNotificationHandler());

            StandardOperationResult result =
                this.server.Connect(userName);

            if (result.Succeed)
            {
                this.connected = true;
                return result.ResultMessage;
            }

            throw new InvalidOperationException(
                "Ошибка подключения: " + result.ErrorMessage);
        }

        public StandardOperationResult GetCurrentWorkingDirectory()
        {
            return this.server.GetCurrentWorkingDirectoryPath();
        }

        public StandardOperationResult TryPerformCommand(string commandString)
        {
            if (!this.connected)
            {
                throw new InvalidOperationException(
                    "Подключение к серверу не произведено");
            }
            if (string.IsNullOrWhiteSpace(commandString))
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