namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда запрещения удаления файла.
    /// </summary>
    internal class LockFileCommand : VFSClientCommand
    {
        internal LockFileCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.LockFile(parameters[0]);
        }
    }
}
