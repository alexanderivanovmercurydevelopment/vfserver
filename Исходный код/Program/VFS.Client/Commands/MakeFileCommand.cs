namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class MakeFileCommand : VFSClientCommand
    {
        internal MakeFileCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.MakeFile(parameters[0]);
        }
    }
}
