namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    internal class DownloadCommand : VFSClientCommand
    {
        public DownloadCommand(IVFSSingleUserService vfsService) : base(vfsService)
        {
        }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(params string[] parameters)
        {
            // в консоли пока достаточно такого варианта. Отзывчивость интерфейса не реализована.
            return this.vfsService.DownloadFileAsync(parameters[0])
                .GetAwaiter().GetResult();
        }
    }
}
