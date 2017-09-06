namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда перемещения файла или директории.
    /// </summary>
    internal class MoveCommand : VFSClientCommand
    {
        internal MoveCommand(IVFSSingleUserService vfsService)
            : base(vfsService)
        {
        }

        protected override int MinParametersCount => 2;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            return this.vfsService.Move(parameters[0], parameters[1]);
        }
    }
}