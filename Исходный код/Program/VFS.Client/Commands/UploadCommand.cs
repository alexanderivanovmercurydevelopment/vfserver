namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class UploadCommand : VFSClientCommand
    {
        public UploadCommand(IVFSSingleUserService vfsService)
            : base(vfsService)
        {
        }

        protected override int MinParametersCount => 2;

        protected override StandardOperationResult ExecuteImpl(params string[] parameters)
        {
            // в консоли пока достаточно такого варианта. Отзывчивость интерфейса не реализована.
            return this.vfsService.UploadFileAsync(parameters[0], parameters[1])
                .GetAwaiter().GetResult();
        }
    }
}