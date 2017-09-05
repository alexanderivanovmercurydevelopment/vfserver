namespace VFS.Client.Commands
{
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда получения информации о структуре папок и файлов.
    /// </summary>
    internal class GetTreeCommand : VFSClientCommand
    {
        internal GetTreeCommand(IVFSSingleUserService vfsService)
            : base(vfsService) { }

        protected override int MinParametersCount => 0;

        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            string driveName = parameters.Length > 0
                ? parameters[0]
                : null;

            StandardOperationResult result =
                this.vfsService.GetDriveStructure(driveName);

            if (!result.Succeed)
            {
                return result;
            }

            try
            {
                ConsoleDriveStructureInfo consoleResult =
                    new ConsoleDriveStructureInfo(result.ResultMessage);

                return new StandardOperationResult(
                    consoleResult.GetConsoleFriendlyString(),
                    null);
            }
            catch
            {
                return result;
            }
        }
    }
}