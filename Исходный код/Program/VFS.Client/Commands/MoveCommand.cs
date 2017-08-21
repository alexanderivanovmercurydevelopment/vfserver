namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда перемещения файла или директории.
    /// </summary>
    internal class MoveCommand : VFSClientCommand
    {
        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="vfsService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        internal MoveCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        /// <summary>
        /// Необходимое количество параметров команды.
        /// </summary>
        protected override int MinParametersCount => 2;

        /// <summary>
        /// Выполнить команду перемещения файла или директории.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.Move(parameters[0], parameters[1]);
        }
    }
}
