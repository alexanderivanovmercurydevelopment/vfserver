namespace VFS.Client.Commands
{
    using System;

    using VFS.Client.Commands;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Класс создания экземпляров команд.
    /// </summary>
    public static class CommandCreator
    {
        /// <summary>
        /// Создать экземпляр команды.
        /// </summary>
        /// <param name="commandName">Имя команды.</param>
        /// <param name="VFSService">Интерфейс доступа
        /// к виртуальному файловому серверу.</param>
        /// <returns>Созданная команда, или null, если команды
        /// с таким именем не существует.</returns>
        public static VFSClientCommand CreateCommand(
            string commandName,
            IVFSSingleUserService VFSService)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new ArgumentNullException(
                    "commandName",
                    "Имя команды не должно быть пустым.");
            }

            // команды - регистронезависимы.
            string commandNameLower = commandName.ToLowerInvariant();

            Enum.GetName(typeof(ClientCommands), ClientCommands.md);

            if (commandNameLower == ClientCommands.md.ToString())
            {
                return new MakeDirectoryCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.cd.ToString())
            {
                return new SetCurrentDirectoryCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.rd.ToString())
            {
                return new RemoveDirectoryCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.deltree.ToString())
            {
                return new DeleteTreeCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.mf.ToString())
            {
                return new MakeFileCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.del.ToString())
            {
                return new DeleteFileCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.@lock.ToString())
            {
                return new LockFileCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.unlock.ToString())
            {
                return new UnlockFileCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.copy.ToString())
            {
                return new CopyCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.move.ToString())
            {
                return new MoveCommand(VFSService);
            }
            else if (commandNameLower == ClientCommands.print.ToString())
            {
                return new GetTreeCommand(VFSService);
            }

            return null;
        }
    }
}
