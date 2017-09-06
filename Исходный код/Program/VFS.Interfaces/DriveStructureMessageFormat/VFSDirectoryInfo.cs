namespace VFS.Interfaces.DriveStructureMessageFormat
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Информация о папке и её содержимом.
    /// </summary>
    public class VFSDirectoryInfo : IVFSDirectoryInfo
    {
        /// <summary>
        /// Имя папки.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Информация о дочерних папках.
        /// </summary>
        [XmlArray("Directories")]
        [XmlArrayItem("Directory")]
        public List<VFSDirectoryInfo> Directories { get; set; }

        /// <summary>
        /// Информация о файлах в папке.
        /// </summary>
        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<VFSFileInfo> Files { get; set; }
    }
}