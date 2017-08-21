namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда создания директории.
    /// </summary>
    internal class MakeDirectoryCommand : VFSClientCommand
    {
        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="vfsService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        internal MakeDirectoryCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        /// <summary>
        /// Необходимое количество параметров команды.
        /// </summary>
        protected override int MinParametersCount => 1;

        /// <summary>
        /// Выполнить команду создания директории.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.MakeDirectory(parameters[0]);
        }
    }
}
