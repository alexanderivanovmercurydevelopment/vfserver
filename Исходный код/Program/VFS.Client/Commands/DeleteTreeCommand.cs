namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда удаления директории и всех её поддиректорий.
    /// </summary>
    internal class DeleteTreeCommand : VFSClientCommand
    {
        internal DeleteTreeCommand(IVFSSingleUserService vfsService)
            : base(vfsService)
        {
        }

        protected override int MinParametersCount => 1;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.RemoveDirectory(parameters[0], true);
        }
    }
}