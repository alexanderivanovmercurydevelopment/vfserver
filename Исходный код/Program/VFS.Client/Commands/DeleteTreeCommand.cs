namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда удаления директории и всех её поддиректорий.
    /// </summary>
    internal class DeleteTreeCommand : VFSClientCommand
    {
        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="vfsService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        internal DeleteTreeCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        /// <summary>
        /// Необходимое количество параметров команды.
        /// </summary>
        protected override int MinParametersCount => 1;

        /// <summary>
        /// Выполнить команду удаления директории и всех её поддиректорий.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.RemoveDirectory(parameters[0], true);
        }
    }
}
