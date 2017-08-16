namespace VFS.Interfaces.VirtualDrive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Виртуальный диск.
    /// </summary>
    public interface IVirtualDrive : IVirtualDirectory, IDisposable
    {
        /// <summary>
        /// Инициализировать виртуальный диск для начала работы.
        /// </summary>
        /// <param name="xmlConfig">Конфигурация виртуального диска.</param>
        /// <param name="driveName">Имя диска (например, "C:").</param>
        /// <remarks>У каждой реализации виртуального диска предполагается
        /// свой формат конфигурационных данных, схему которого можно
        /// получить, вызвав <see cref="GetXmlConfigSchema"/>.</remarks>
        void Initialize(string xmlConfig, string driveName);

        /// <summary>
        /// Получить схему xml-конфигурации виртуального диска.
        /// </summary>
        /// <returns>Схема xml-конфигурации виртуального диска.</returns>
        /// <remarks>Схема - .xsd документ, содержащий правила формирования
        /// xml-документа.</remarks>
        string GetXmlConfigSchema();

        /// <summary>
        /// Назначить поведение интеграции с другими
        /// реализациями виртуальных дисков.
        /// </summary>
        /// <param name="integrationBehaviour">Поведение интеграции
        /// с другими реализациями виртуальных дисков.</param>
        void SetIntegrationBehaviour(
            IIntegrationBehaviour integrationBehaviour);

        /// <summary>
        /// Найти директорию по полному пути.
        /// </summary>
        /// <param name="fullPath">Полный путь к директории.</param>
        /// <returns>Найденная директория, или null.</returns>
        IVirtualDirectory FindDirectory(string fullPath);
    }
}
