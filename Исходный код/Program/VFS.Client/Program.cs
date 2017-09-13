namespace VFS.Client
{
    using System;
    using System.Text;

    using VFS.Client.Commands;
    using VFS.Client.WCF;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Консольный клиент виртуального файлового сервера.
    /// </summary>
    public static class Program
    {
        private static ClientApplication application;

        private static void Main()
        {
            try
            {
                Program.SetupConsoleEncoding();
                Program.WriteProgramDescription();

                if (Program.Connect())
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

        private static void SetupConsoleEncoding()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
        }

        private static void WriteProgramDescription()
        {
            Console.WriteLine("ВИРТУАЛЬНЫЙ ФАЙЛОВЫЙ СЕРВЕР");
            Console.WriteLine("Тестовое задание MERCURY DEVELOPMENT");
            Console.WriteLine();
        }

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
            Console.WriteLine("Пример: connect Alexandr");

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

                    if (parameters.Length == 1)
                    {
                        Program.Connect(parameters[0]);
                        return true;
                    }
                }

                Console.WriteLine("Команда connect введена неверно. Пример: connect Alexandr.");
            }
        }

        /// <param name="userName">Имя подключаемого пользователя.</param>
        private static void Connect(string userName)
        {
            Program.application = new ClientApplication(new WCFBasedServiceProvider());
            Console.WriteLine(Program.application.Connect(userName));
        }

        /// <summary>
        /// Выполнение команд виртуального файлового сервера.
        /// </summary>
        private static void PerformCommands()
        {
            while (true)
            {
                string command = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(command)
                    && ConsoleCommandParser.GetCommandName(command).ToLowerInvariant() == "quit")
                {
                    return;
                }

                StandardOperationResult result =
                    Program.application.TryPerformCommand(command);

                Console.WriteLine(result.Succeed ? result.ResultMessage : result.ErrorMessage);

                Console.WriteLine();
                Console.WriteLine("Введите следующую команду. (quit - выход)");
            }
        }

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