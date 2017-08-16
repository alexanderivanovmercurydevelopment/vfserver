﻿namespace VFS.Client
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Utilities;

    /// <summary>
    /// Представление информации о структуре диска в удобном для консоли виде.
    /// </summary>
    public class ConsoleDriveStructureInfo
    {
        /// <summary>
        /// Информация о структуре диска.
        /// </summary>
        private readonly DriveStructureInfo driveStructure;

        /// <summary>
        /// Создать экземпляр консольного представления
        /// информации о структуре диска.
        /// </summary>
        /// <param name="xmlDriveInfo">
        /// Информация о структуре диска в виде xml.
        /// </param>
        public ConsoleDriveStructureInfo(string xmlDriveInfo)
        {
            DriveStructureInfo driveStrInfo =
                XMLUtilities.DeserializeFromXml<DriveStructureInfo>(
                    xmlDriveInfo);

            this.driveStructure = driveStrInfo;
        }

        /// <summary>
        /// Получить информацию о структуре диска,
        /// в пригодном для отображения в консоли виде.
        /// </summary>
        public string GetConsoleFriendlyString()
        {
            this.SortRecursively(this.driveStructure);

            StringBuilder result = new StringBuilder();
            result.AppendLine(this.driveStructure.Name);
            this.GetPresentationStringRecursively(
                this.driveStructure,
                string.Empty,
                result);
            return result.ToString();
        }

        /// <summary>
        /// Произвести сортировку.
        /// </summary>
        /// <param name="directory">Директория.</param>
        private void SortRecursively(IVFSDirectoryInfo directory)
        {
            directory.Directories.Sort(
                new Comparison<VFSDirectoryInfo>((x, y) => 
                {
                    return string.Compare(
                        x.Name, 
                        y.Name, 
                        StringComparison.OrdinalIgnoreCase);
                }));

            directory.Files.Sort(
                new Comparison<VFSFileInfo>((x, y) =>
                {
                    return string.Compare(
                        x.Name, 
                        y.Name, 
                        StringComparison.OrdinalIgnoreCase);
                }));

            foreach (IVFSDirectoryInfo childDir 
                in directory.Directories)
            {
                this.SortRecursively(childDir);
            }
        }

        /// <summary>
        /// Получить строку представления информации о папке и 
        /// её содержимом.
        /// </summary>
        /// <param name="directory">Папка.</param>
        /// <param name="beginningLeftString">Символы, которые 
        /// должны выводится слева перед именем папки/файла.</param>
        /// <param name="currentResult">Текущая строка представления
        /// информации о структуре папок и файлов.</param>
        private void GetPresentationStringRecursively(
            IVFSDirectoryInfo directory,
            string beginningLeftString,
            StringBuilder currentResult)
        {
            int directoriesCount = directory.Directories.Count;
            for (int i = 0; i < directoriesCount; i++)
            {
                IVFSDirectoryInfo childDir = 
                    directory.Directories[i];

                currentResult.AppendLine(
                    beginningLeftString + "|_" + childDir.Name);

                string additionalLeftString = 
                    ((i == directoriesCount - 1) && (directory.Files.Count == 0))
                    ? "  "
                    : "| ";

                this.GetPresentationStringRecursively(
                    childDir,
                    beginningLeftString + additionalLeftString,
                    currentResult);
            }

            foreach (VFSFileInfo childFile
                in directory.Files)
            {
                string str = beginningLeftString + "|_" + childFile.Name;

                if (childFile.LockingUsers.Count > 0)
                {
                    str += "[LOCKED by ";

                    int lockingUsersCount = childFile.LockingUsers.Count;
                    for (int i = 0; i < lockingUsersCount; i++)
                    {
                        str += childFile.LockingUsers[i];
                        
                        if (i != lockingUsersCount - 1)
                        {
                            str += ",";
                        }
                    }

                    str += "]";
                }

                currentResult.AppendLine(str);
            }
        }
    }
}
