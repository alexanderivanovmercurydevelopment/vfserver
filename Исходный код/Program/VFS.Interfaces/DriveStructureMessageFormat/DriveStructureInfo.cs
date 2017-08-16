namespace VFS.Interfaces.DriveStructureMessageFormat
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Информация о структуре файлов и папок на диске.
    /// </summary>
    [XmlRoot("DriveStructure")]
    public class DriveStructureInfo : IVFSDirectoryInfo
    {
        /// <summary>
        /// Имя диска.
        /// </summary>
        [XmlAttribute("driveName")]
        public string Name { get; set; }

        /// <summary>
        /// Информация о папках в корне диска.
        /// </summary>
        [XmlArray("Directories")]
        [XmlArrayItem("Directory")]
        public List<VFSDirectoryInfo> Directories { get; set; }

        /// <summary>
        /// Информация о файлах в корне диска.
        /// </summary>
        [XmlArray("Files")]
        [XmlArrayItem("File")]
        public List<VFSFileInfo> Files { get; set; }
    }
}
