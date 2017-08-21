namespace VFS.Client.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Класс разбора строки консольной команды.
    /// </summary>
    public static class ConsoleCommandParser
    {
        /// <summary>
        /// Получить имя команды.
        /// </summary>
        /// <param name="command">Непустая строка команды.</param>
        /// <returns>Имя (название) команды.</returns>
        public static string GetCommandName(string command)
        {
            ConsoleCommandParser.ThrowIfEmpty(command);
            ConsoleCommandParser.ThrowIfMultiline(command);

            string[] parts = 
                ConsoleCommandParser.GetAllCommandParts(command);

            return parts[0];
        }

        /// <summary>
        /// Получить параметры команды.
        /// </summary>
        /// <param name="command">Строка команды.</param>
        /// <returns>Параметры команды (может содержать 0 параметров).</returns>
        public static string[] GetCommandParams(string command)
        {
            ConsoleCommandParser.ThrowIfEmpty(command);
            ConsoleCommandParser.ThrowIfMultiline(command);

            List<string> parts =
                ConsoleCommandParser.GetAllCommandParts(command)
                .ToList();

            parts.Remove(parts.First());

            return parts.ToArray();
        }

        /// <summary>
        /// Сгенерировать исключение, если команда пустая.
        /// </summary>
        /// <param name="command">Команда.</param>
        private static void ThrowIfEmpty(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentNullException(
                    nameof(command),
                    "Разбор пустой команды невозможен.");
            }
        }

        /// <summary>
        /// Сгенерировать исключение, если команда
        /// состоит более чем из одной строки.
        /// </summary>
        /// <param name="command">Команда.</param>
        private static void ThrowIfMultiline(string command)
        {
            string[] strings = command.Split(
                new[] { '\n' }, 
                StringSplitOptions.None);

            if (strings.Length > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(command),
                    "Команда должна состоять из одной строки.");
            }
        }

        /// <summary>
        /// Получить все отдельные части команды.
        /// </summary>
        /// <param name="command">Команда, например "md C:/MyDir".</param>
        /// <returns>Составные части команды, например "md", "C:/MyDir".</returns>
        private static string[] GetAllCommandParts(string command)
        {
            return command.Split(
                new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
