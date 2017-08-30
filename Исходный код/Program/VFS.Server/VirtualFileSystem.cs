namespace VFS.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using VFS.InMemoryVirtualDrive;
    using VFS.Interfaces.VirtualDrive;
    using VFS.Utilities;

    /// <summary>
    /// Единая виртуальная файловая система, работающая
    /// с разными реализациями виртуальных дисков.
    /// </summary>
    internal class VirtualFileSystem
    {
        /// <summary>
        /// Виртуальные диски.
        /// </summary>

        private readonly List<IVirtualDrive> drives
            = new List<IVirtualDrive>();

        /// <summary>
        /// Создать единую виртуальную файловую систему.
        /// </summary>
        internal VirtualFileSystem()
        {
            this.AddInMemoryVirtualDrive("C:");
        }

        /// <summary>
        /// Проверить существование директории.
        /// </summary>
        /// <param name="fullPath">Полный путь к директории.</param>
        internal void CheckDirectoryExisting(string fullPath)
        {
            this.GetExistingDirectory(fullPath);
        }

        /// <summary>
        /// Получить информацию о структуре виртуального диска.
        /// </summary>
        /// <param name="driveName">Имя виртуального диска.</param>
        /// <returns>Информация о структуре виртуального диска.</returns>
        internal IVirtualDirectory GetRootDirectory(string driveName)
        {
            return this.GetExistingDrive(driveName);
        }

        /// <summary>
        /// Создать новую директорию.
        /// </summary>
        /// <param name="directoryFullPath">Полный путь.</param>
        internal void CreateDirectory(string directoryFullPath)
        {
            this.ThrowIfContainsOnlyDriveName(directoryFullPath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                directoryFullPath.GetPathWithoutLastItem());

            directory.CreateDirectory(
                directoryFullPath.GetDirectoryOrFileName());
        }

        /// <summary>
        /// Удалить директорию.
        /// </summary>
        /// <param name="fullPath">Полный путь к директории.</param>
        /// <param name="recursive">Вместе с дочерними папками.</param>
        internal void RemoveDirectory(string fullPath, bool recursive)
        {
            this.ThrowIfContainsOnlyDriveName(fullPath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                fullPath.GetPathWithoutLastItem());

            directory.RemoveDirectory(
                fullPath.GetDirectoryOrFileName(),
                recursive);
        }

        /// <summary>
        /// Создать новый файл.
        /// </summary>
        /// <param name="fullFilePath">Полный путь к создаваемому файлу.</param>
        internal void CreateFile(string fullFilePath)
        {
            this.ThrowIfContainsOnlyDriveName(fullFilePath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                 fullFilePath.GetPathWithoutLastItem());

            directory.CreateFile(fullFilePath.GetDirectoryOrFileName());
        }

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="fullFilePath">Полный путь к файлу.</param>
        internal void RemoveFile(string fullFilePath)
        {
            this.ThrowIfContainsOnlyDriveName(fullFilePath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                 fullFilePath.GetPathWithoutLastItem());

            directory.RemoveFile(fullFilePath.GetDirectoryOrFileName());
        }

        /// <summary>
        /// Найти существующую директорию.
        /// </summary>
        /// <param name="fullDirectoryPath">Полный путь к существующей директории.</param>
        /// <returns>Найденная директория.</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// Директория не существует.
        /// </exception>
        internal IVirtualDirectory GetExistingDirectory(
            string fullDirectoryPath)
        {
            IVirtualDirectory directory = 
                this.FindDirectory(fullDirectoryPath);

            if (directory == null)
            {
                throw new DirectoryNotFoundException(
                    "Папки " + fullDirectoryPath + " не существует.");
            }

            return directory;
        }

        /// <summary>
        /// Найти существующий файл.
        /// </summary>
        /// <param name="fullFilePath">Полный путь к существующему файлу.</param>
        /// <returns>Найденный файл.</returns>
        /// <exception cref="FileNotFoundException">
        /// Файл не существует.
        /// </exception>
        internal IVirtualFile GetExistingFile(string fullFilePath)
        {
            IVirtualFile file = this.FindFile(fullFilePath);

            if (file == null)
            {
                throw new FileNotFoundException(
                    "Файл " + fullFilePath + " не найден.");
            }

            return file;
        }

        /// <summary>
        /// Копировать папку или файл в целевую папку.
        /// </summary>
        /// <param name="fullSource">Полный путь к папке или файлу.</param>
        /// <param name="fullDestination">Полный путь к целевой папке.</param>
        internal void Copy(string fullSource, string fullDestination)
        {
            IVirtualDirectory destinationDir = 
                this.GetExistingDirectory(fullDestination);

            IVirtualDirectory sourceDirectory = this.FindDirectory(fullSource);
            
            if (sourceDirectory != null)
            {
                destinationDir.CopyDirectoryFrom(sourceDirectory);
            }
            else
            {
                IVirtualFile sourceFile = this.GetExistingFile(fullSource);
                destinationDir.CopyFileFrom(sourceFile);
            }            
        }

        /// <summary>
        /// Переместить папку или файл в целевую папку.
        /// </summary>
        /// <param name="fullSource">Полный путь к папке или файлу.</param>
        /// <param name="fullDestination">Полный путь к целевой папке.</param>
        internal void Move(string fullSource, string fullDestination)
        {
            this.ThrowIfContainsOnlyDriveName(fullSource);
            IVirtualDirectory destinationDir =
                this.GetExistingDirectory(fullDestination);

            IVirtualDirectory sourceParentDirectory = 
                this.FindDirectory(fullSource.GetPathWithoutLastItem());

            IVirtualDirectory sourceDirectory = 
                this.FindDirectory(fullSource);

            if (sourceDirectory != null)
            {
                sourceParentDirectory.MoveDirectoryTo(
                    fullSource.GetDirectoryOrFileName(), 
                    destinationDir);
            }
            else
            {
                sourceParentDirectory.MoveFileTo(
                    fullSource.GetDirectoryOrFileName(), 
                    destinationDir);
            }        
        }

        /// <summary>
        /// Найти директорию по полному пути.
        /// </summary>
        /// <param name="fullDirectoryPath">Путь к директории.</param>
        /// <returns>Найденная директория, или null.</returns>
        internal IVirtualDirectory FindDirectory(string fullDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(fullDirectoryPath))
            {
                throw new ArgumentNullException(
                    nameof(fullDirectoryPath),
                    "Необходимо указать путь к директории.");
            }

            string lowerDriveName = fullDirectoryPath.GetLowerDriveName();
            IVirtualDrive drive = this.GetExistingDrive(lowerDriveName);


            if (lowerDriveName ==
                fullDirectoryPath.ToLowerInvariant().Replace(
                    Path.DirectorySeparatorChar.ToString(),
                    string.Empty))
            {
                return drive;
            }

            return drive.FindDirectory(fullDirectoryPath);
        }

        /// <summary>
        /// Найти файл по полному пути.
        /// </summary>
        /// <param name="fullFilePath">Полный путь к файлу.</param>
        /// <returns>Найденный файл, или null.</returns>
        internal IVirtualFile FindFile(string fullFilePath)
        {
            this.ThrowIfContainsOnlyDriveName(fullFilePath);
            IVirtualDirectory directory = this.FindDirectory(
                fullFilePath.GetPathWithoutLastItem());

            return directory?.Files.FirstOrDefault(f =>
                f.Name == fullFilePath.GetDirectoryOrFileName());
        }

        /// <summary>
        /// Добавить виртуальный диск, хранящий данные в памяти.
        /// </summary>
        /// <param name="driveName">Имя диска.</param>
        private void AddInMemoryVirtualDrive(string driveName)
        {
            IVirtualDrive inMemoryDrive
                = new InMemoryVirtualDrive();

            string xmlConfig = AppResourceReader.GetResource(
                typeof(InMemoryVirtualDrive).Assembly,
                "VFS.InMemoryVirtualDrive.ConfigExample.xml");

            inMemoryDrive.Initialize(xmlConfig, driveName);
            this.drives.Add(inMemoryDrive);
        }

        /// <summary>
        /// Получить корневую папку существующего диска.
        /// </summary>
        /// <param name="driveName">Имя диска.</param>
        /// <returns>Корневая папка диска.</returns>
        private IVirtualDrive GetExistingDrive(string driveName)
        {
            IVirtualDrive drive = this.drives.FirstOrDefault(d =>
                d.Name.ToLowerInvariant() == driveName.ToLowerInvariant());

            if (drive == null)
            {
                throw new InvalidOperationException(
                    "Диска " + driveName + " не существует.");
            }

            return drive;
        }

        /// <summary>
        /// Сгенерировать исключение, если полный путь содержит только имя диска.
        /// </summary>
        /// <param name="fullPath">Полный путь.</param>
        // ReSharper disable once UnusedParameter.Local Весь метод используется только с целью проверки.
        private void ThrowIfContainsOnlyDriveName(string fullPath)
        {
            if (!fullPath.Contains(Path.DirectorySeparatorChar)
                && fullPath.EndsWith(":"))
            {
                throw new InvalidOperationException(
                    "Путь содержит только имя диска.");
            }
        }
    }
}
