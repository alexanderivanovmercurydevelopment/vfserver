namespace VFS.Client.Commands
{
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда удаления директории.
    /// </summary>
    internal class RemoveDirectoryCommand : VFSClientCommand
    {
        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="VFSService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        internal RemoveDirectoryCommand(IVFSSingleUserService VFSService)
            : base(VFSService) { }

        /// <summary>
        /// Необходимое количество параметров команды.
        /// </summary>
        protected override int MinParametersCount { get { return 1; } }

        /// <summary>
        /// Выполнить команду удаления директории.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.VFSService.RemoveDirectory(parameters[0], false);
        }
    }
}
