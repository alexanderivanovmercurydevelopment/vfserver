namespace VFS.Interfaces.DriveStructureMessageFormat
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Информация о файле.
    /// </summary>
    public class VFSFileInfo
    {
        /// <summary>
        /// Имя папки.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Имена пользователей, блокирующих файл.
        /// </summary>
        [XmlArray("LockingUsers")]
        [XmlArrayItem("LockingUser")]
        public List<string> LockingUsers { get; set; }
    }
}
