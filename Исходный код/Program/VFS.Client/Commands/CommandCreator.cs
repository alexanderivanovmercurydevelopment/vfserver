namespace VFS.Client.Commands
{
    using System;

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
        /// <param name="vfsService">Интерфейс доступа
        /// к виртуальному файловому серверу.</param>
        /// <returns>Созданная команда, или null, если команды
        /// с таким именем не существует.</returns>
        public static VFSClientCommand CreateCommand(
            string commandName,
            IVFSSingleUserService vfsService)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new ArgumentNullException(
                    nameof(commandName),
                    "Имя команды не должно быть пустым.");
            }

            // команды - регистронезависимы.
            string commandNameLower = commandName.ToLowerInvariant();

            if (commandNameLower == ClientCommands.Md.ToString().ToLowerInvariant())
            {
                return new MakeDirectoryCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Cd.ToString().ToLowerInvariant())
            {
                return new SetCurrentWorkingDirectoryCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Rd.ToString().ToLowerInvariant())
            {
                return new RemoveDirectoryCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Deltree.ToString().ToLowerInvariant())
            {
                return new DeleteTreeCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Mf.ToString().ToLowerInvariant())
            {
                return new MakeFileCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Del.ToString().ToLowerInvariant())
            {
                return new DeleteFileCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Lock.ToString().ToLowerInvariant())
            {
                return new LockFileCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Unlock.ToString().ToLowerInvariant())
            {
                return new UnlockFileCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Copy.ToString().ToLowerInvariant())
            {
                return new CopyCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Move.ToString().ToLowerInvariant())
            {
                return new MoveCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Print.ToString().ToLowerInvariant())
            {
                return new GetTreeCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Upload.ToString().ToLowerInvariant())
            {
                return new UploadCommand(vfsService);
            }
            if (commandNameLower == ClientCommands.Download.ToString().ToLowerInvariant())
            {
                return new DownloadCommand(vfsService);
            }

            return null;
        }
    }
}