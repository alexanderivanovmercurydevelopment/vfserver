namespace VFS.Client.Commands
{
    using System;

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
        protected readonly IVFSSingleUserService vfsService;

        /// <summary>
        /// Создать экземпляр команды, выполняемой клиентом
        /// виртуального файлового сервера.
        /// </summary>
        /// <param name="vfsService">Фасад виртуального файлового
        /// сервера (над ним будут производится операции команды).</param>
        protected VFSClientCommand(
            IVFSSingleUserService vfsService)
        {
            if (vfsService == null)
            {
                throw new ArgumentNullException(
                    nameof(vfsService),
                    "Команде нужно передать интерфейс доступа к операциям файлового сервера.");
            }

            this.vfsService = vfsService;
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
                    nameof(parameters),
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