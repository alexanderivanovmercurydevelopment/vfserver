namespace VFS.Client.Commands
{
    using System;

    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Interfaces.Service;

    /// <summary>
    /// Команда, выполняемая клиентом виртуального
    /// файлового сервера.
    /// </summary>
    public abstract class VFSClientCommand
    {
        /// <summary>
        /// Команда выполняется для этого экземпляра.
        /// </summary>
        protected readonly IVFSSingleUserService VFSService;

        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="VFSService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        protected VFSClientCommand(
            IVFSSingleUserService VFSService)
        {
            if (VFSService == null)
            {
                throw new ArgumentNullException(
                    "VFSService",
                    "Команде нужно передать интерфейс доступа к операциям файлового сервера.");
            }

            this.VFSService = VFSService;
        }

        /// <summary>
        /// Минимальное количество параметров, необходимое для выполнения команды.
        /// </summary>
        protected abstract int MinParametersCount { get; }

        /// <summary>
        /// Выполнить команду.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        public StandardOperationResult Execute(
            params string[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(
                    "parameters",
                    "Не переданы параметры команды.");
            }

            if (parameters.Length < this.MinParametersCount)
            {
                return new StandardOperationResult(
                    null,
                    "Минимальное необходимое количество параметров команды: " + this.MinParametersCount);
            }

            return this.ExecuteImpl(parameters);
        }

        /// <summary>
        /// Выполнить команду.
        /// </summary>
        /// <param name="parameters">Параметры команды.</param>
        /// <returns>Сообщение пользователю о результате выполнения
        /// команды или об ошибке.</returns>
        protected abstract StandardOperationResult ExecuteImpl(
            params string[] parameters);
    }
}
