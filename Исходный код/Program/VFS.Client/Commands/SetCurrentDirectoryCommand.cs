namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class SetCurrentWorkingDirectoryCommand : VFSClientCommand
    {
        internal SetCurrentWorkingDirectoryCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.SetCurrentWorkingDirectory(parameters[0]);
        }
    }
}
