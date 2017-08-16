namespace VFS.Client.Commands
{
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда получения информации о структуре папок и файлов.
    /// </summary>
    internal class GetTreeCommand : VFSClientCommand
    {
        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="VFSService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        internal GetTreeCommand(IVFSSingleUserService VFSService)
            : base(VFSService) { }

        /// <summary>
        /// Необходимое количество параметров команды.
        /// </summary>
        protected override int MinParametersCount { get { return 0; } }

        /// <summary>
        /// Выполнить команду получения информации о структуре папок и файлов.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        protected override StandardOperationResult ExecuteImpl(
            params string[] parameters)
        {
            string driveName = parameters.Length > 0
                ? parameters[0]
                : null;

            StandardOperationResult result =
                this.VFSService.GetDriveStructure(driveName);

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