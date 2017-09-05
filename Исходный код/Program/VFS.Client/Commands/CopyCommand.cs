namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class CopyCommand : VFSClientCommand
    {
        internal CopyCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 2;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.Copy(parameters[0], parameters[1]);
        }
    }
}
