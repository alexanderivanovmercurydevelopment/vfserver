namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда снятия запрета на удаление файла.
    /// </summary>
    internal class UnlockFileCommand : VFSClientCommand
    {
        internal UnlockFileCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.UnlockFile(parameters[0]);
        }
    }
}
