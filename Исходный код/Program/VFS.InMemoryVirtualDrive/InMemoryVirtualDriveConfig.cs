namespace VFS.InMemoryVirtualDrive
{
    using System;
    using System.Xml.Serialization;

    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Конфигурация виртуального диска, хранящего данные в памяти.
    /// </summary>
    [XmlRoot("InMemoryVirtualDrive")]
    public class InMemoryVirtualDriveConfig
    {
        /// <summary>
        /// Максимальная длина имени файла.
        /// </summary>
        [XmlElement("MaxFileNameLength")]
        public int MaxFileNameLength { get; set; }

        /// <summary>
        /// Максимальная длина имени папки.
        /// </summary>
        [XmlElement("MaxDirectoryNameLength")]
        public int MaxDirectoryNameLength { get; set; }

        /// <summary>
        /// Поведение интеграции с другими реализациями
        /// виртуальных дисков.
        /// </summary>
        [XmlIgnore]
        public IIntegrationBehaviour IntegrationBehaviour { get; set; }

        /// <summary>
        /// Выдать исключение, если поведение интеграции
        /// с другими реализациями виртуальных дисков не задано.
        /// </summary>
        internal void ThrowIfIntegrationNotSet()
        {
            if (this.IntegrationBehaviour == null)
            {
                throw new InvalidOperationException(
                    "Данный виртуальный диск не поддерживает взаимодействие "
                    + "с виртуальными дисками, работающими по другой системе");
            }
        }
    }
}