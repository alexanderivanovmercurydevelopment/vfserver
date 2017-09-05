namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class DeleteFileCommand : VFSClientCommand
    {
        internal DeleteFileCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.DeleteFile(parameters[0]);
        }
    }
}
