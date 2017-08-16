namespace VFS.Client
{
    using System;

    using VFS.Client.Commands;
    using VFS.Client.WCF;
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Консольный клиент виртуального файлового сервера.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Клиентское приложение для работы с виртуальным
        /// файловым сервером.
        /// </summary>
        private static ClientApplication application;

        /// <summary>
        /// Выполнение консольного клиентского приложения.
        /// </summary>
        /// <param name="args">Параметры командной строки.</param>
        static void Main(string[] args)
        {
            try
            {
                Program.WriteProgramDescription();
                bool connected = Program.Connect();

                if (connected)
                {
                    Program.WriteCommandsDescription();
                    Program.PerformCommands();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("В программе произошла ошибка:");
                Console.WriteLine(ex);
            }
            finally
            {
                Program.SafeDisconnect();
                Program.Quit();
            }
        }

        /// <summary>
        /// Отобразить описание приложения.
        /// </summary>
        private static void WriteProgramDescription()
        {
            Console.WriteLine("ВИРТУАЛЬНЫЙ ФАЙЛОВЫЙ СЕРВЕР");
            Console.WriteLine("Тестовое задание MERCURY DEVELOPMENT");
            Console.WriteLine();
        }

        /// <summary>
        /// Вывести названия основных команд.
        /// </summary>
        private static void WriteCommandsDescription()
        {
            Console.WriteLine("MD [Drive:]Path - создание директории");
            Console.WriteLine("CD [Drive:]Path - смена текущей директории");
            Console.WriteLine("RD [Drive:]Path - удаление директории");
            Console.WriteLine("DELTREE [Drive:]Path - удаление директории и всех её поддиректорий");
            Console.WriteLine("MF [[Drive:]Path]FileName - создание файла");
            Console.WriteLine("DEL [[Drive:]Path]FileName - удаление файла");
            Console.WriteLine("LOCK [[Drive:]Path]FileName - запрет удаления (блокировка) файла");
            Console.WriteLine("UNLOCK [[Drive:]Path]FileName - снятие блокировки (запрета удаления) файла");
            Console.WriteLine("COPY [Drive:]source [Drive:]destination - копировать файл или папку");
            Console.WriteLine("MOVE [Drive:]source [Drive:]destination - переместить файл или папку");
            Console.WriteLine("PRINT Drive: - вывести на экран дерево каталогов диска");
            Console.WriteLine("quit - выход");
        }

        /// <summary>
        /// Подключиться к серверу c помощью команды пользователя.
        /// </summary>
        /// <returns>True - подключение произведено, false - отмена.</returns>
        private static bool Connect()
        {
            Console.WriteLine("Введите команду подключения и нажмите Enter.");
            Console.WriteLine("Пример: connect localhost:9000 Alexandr");

            while (true)
            {
                string command = Console.ReadLine();

                string commandName = string.IsNullOrWhiteSpace(command)
                    ? null
                    : ConsoleCommandParser.GetCommandName(command).ToLowerInvariant();

                if (commandName == "connect")
                {
                    string[] parameters =
                        ConsoleCommandParser.GetCommandParams(command);

                    if (parameters.Length == 2)
                    {
                        Program.Connect(parameters);
                        return true;
                    }
                }

                Console.WriteLine("Команда connect введена неверно. Пример: connect localhost:9000 Alexandr.");
            }
        }

        /// <summary>
        /// Подключиться к серверу.
        /// </summary>
        /// <param name="parameters">Параметры команды подключения.</param>
        private static void Connect(string[] parameters)
        {
            string[] hostNameAndPort =
                parameters[0].Split(
                    new[] { ':' },
                    StringSplitOptions.RemoveEmptyEntries);

            int port = 0;
            bool portIsTyped = hostNameAndPort.Length == 2
                && int.TryParse(hostNameAndPort[1], out port);

            Program.application = new ClientApplication(new WCFBasedServiceProvider());
            
            Console.WriteLine(Program.application.Connect(
                hostNameAndPort[0],
                portIsTyped ? new int?(port) : null,
                parameters[1]));
        }

        /// <summary>
        /// Выполнение команд виртуального файлового сервера.
        /// </summary>
        private static void PerformCommands()
        {
            while(true)
            {
                string command = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(command)
                    && ConsoleCommandParser.GetCommandName(command).ToLowerInvariant() == "quit")
                {
                    return;
                }

                StandardOperationResult result = 
                    Program.application.TryPerformCommand(command);

                if (result.Succeed)
                {
                    Console.WriteLine(result.ResultMessage);
                }
                else
                {
                    Console.WriteLine(result.ErrorMessage);
                }

                Console.WriteLine();
                Console.WriteLine("Введите следующую команду. (quit - выход)");
            }
        }

        /// <summary>
        /// Завершение работы с приложением.
        /// </summary>
        private static void SafeDisconnect()
        {
            if (Program.application != null)
            {
                try
                {
                    Program.application.Disconnect();
                    Console.WriteLine("Отключение произведено.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка завершения работы с сервером.");
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Выход из консоли.
        /// </summary>
        private static void Quit()
        {
            Console.WriteLine();
            Console.WriteLine("Работа с виртуальным файловым сервером завершена.");
            Console.WriteLine("Для выхода нажмите Enter.");
            Console.ReadLine();
        }
    }
}
