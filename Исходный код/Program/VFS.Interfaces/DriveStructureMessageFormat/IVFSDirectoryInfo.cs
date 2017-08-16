namespace VFS.Interfaces.DriveStructureMessageFormat
{
    using System.Collections.Generic;

    /// <summary>
    /// Информация о папке и её содержимом.
    /// </summary>
    public interface IVFSDirectoryInfo
    {
        /// <summary>
        /// Имя папки.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Информация о дочерних папках.
        /// </summary>
        List<VFSDirectoryInfo> Directories { get; set; }

        /// <summary>
        /// Информация о файлах в папке.
        /// </summary>
        List<VFSFileInfo> Files { get; set; }
    }
}
