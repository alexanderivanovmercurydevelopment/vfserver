namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class RemoveDirectoryCommand : VFSClientCommand
    {
        internal RemoveDirectoryCommand(IVFSSingleUserService vfsService)
            : base(vfsService)
        {
        }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.RemoveDirectory(parameters[0], false);
        }
    }
}