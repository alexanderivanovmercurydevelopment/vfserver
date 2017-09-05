namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class MakeDirectoryCommand : VFSClientCommand
    {
        internal MakeDirectoryCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.MakeDirectory(parameters[0]);
        }
    }
}
