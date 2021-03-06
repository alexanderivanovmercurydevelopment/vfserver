﻿namespace VFS.Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    using VFS.Interfaces.VirtualDrive;
    using VFS.Utilities;

    /// <summary>
    /// Единая виртуальная файловая система, работающая
    /// с разными реализациями виртуальных дисков.
    /// </summary>
    internal class VirtualFileSystem
    {
        private readonly List<IVirtualDrive> virtualDrives
            = new List<IVirtualDrive>();

        internal VirtualFileSystem()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            IList<IVirtualDrive> drives = VirtualFileSystem.GetExternalConfiguredDrives()
                .ToList();

            VirtualFileSystem.CheckDriveNames(drives);

            this.virtualDrives.AddRange(drives);
        }

        private static IEnumerable<IVirtualDrive> GetExternalConfiguredDrives()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VFS.InMemoryVirtualDrive.dll");
            ComposablePartCatalog catalog = new AssemblyCatalog(Assembly.LoadFile(path));
            CompositionContainer container = new CompositionContainer(catalog);
            container.ComposeParts();
            IEnumerable<IVirtualDrive> drives = container.GetExportedValues<IVirtualDrive>();
            return drives;
        }

        private static void CheckDriveNames(IList<IVirtualDrive> drives)
        {
            List<IVirtualDrive> checkedDrives = new List<IVirtualDrive>();

            foreach (var drive in drives)
            {
                if (!Regex.IsMatch(drive.Name, "\\w:"))
                {
                    throw new InvalidOperationException(
                        $"Ошибка подключения виртуального диска. Диск {drive.Name} имеет недопустимое имя. " +
                        "Допустимы имена, содержащие один символ и двоеточие (пример - C:, D:, Z:).");
                }

                if (checkedDrives.Any(checkedDrive =>
                    string.Equals(checkedDrive.Name, drive.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        $"Невозможно подключить несколько виртуальных дисков с одним именем {drive.Name}.");
                }

                checkedDrives.Add(drive);
            }
        }

        internal void CheckDirectoryExisting(string fullPath)
        {
            this.GetExistingDirectory(fullPath);
        }

        internal IVirtualDirectory GetRootDirectory(string driveName)
        {
            return this.GetExistingDrive(driveName);
        }

        internal void CreateDirectory(string directoryFullPath)
        {
            this.ThrowIfContainsOnlyDriveName(directoryFullPath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                directoryFullPath.GetPathWithoutLastItem());

            directory.CreateDirectory(
                directoryFullPath.GetDirectoryOrFileName());
        }

        internal void RemoveDirectory(string fullPath, bool recursive)
        {
            this.ThrowIfContainsOnlyDriveName(fullPath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                fullPath.GetPathWithoutLastItem());

            directory.RemoveDirectory(
                fullPath.GetDirectoryOrFileName(),
                recursive);
        }

        internal void CreateFile(string fullFilePath)
        {
            this.ThrowIfContainsOnlyDriveName(fullFilePath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                fullFilePath.GetPathWithoutLastItem());

            directory.CreateFile(fullFilePath.GetDirectoryOrFileName());
        }

        internal void RemoveFile(string fullFilePath)
        {
            this.ThrowIfContainsOnlyDriveName(fullFilePath);
            IVirtualDirectory directory = this.GetExistingDirectory(
                fullFilePath.GetPathWithoutLastItem());

            directory.RemoveFile(fullFilePath.GetDirectoryOrFileName());
        }

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

        internal void Copy(string fullSourcePath, string fullDestinationPath)
        {
            IVirtualDirectory destinationDir =
                this.GetExistingDirectory(fullDestinationPath);

            IVirtualDirectory sourceDirectory = this.FindDirectory(fullSourcePath);

            if (sourceDirectory != null)
            {
                destinationDir.CopyDirectoryFrom(sourceDirectory);
            }
            else
            {
                IVirtualFile sourceFile = this.GetExistingFile(fullSourcePath);
                destinationDir.CopyFileFrom(sourceFile);
            }
        }

        internal void Move(string fullSourcePath, string fullDestinationPath)
        {
            this.ThrowIfContainsOnlyDriveName(fullSourcePath);
            IVirtualDirectory destinationDir =
                this.GetExistingDirectory(fullDestinationPath);

            IVirtualDirectory sourceParentDirectory =
                this.FindDirectory(fullSourcePath.GetPathWithoutLastItem());

            IVirtualDirectory sourceDirectory =
                this.FindDirectory(fullSourcePath);

            if (sourceDirectory != null)
            {
                sourceParentDirectory.MoveDirectoryTo(
                    fullSourcePath.GetDirectoryOrFileName(),
                    destinationDir);
            }
            else
            {
                sourceParentDirectory.MoveFileTo(
                    fullSourcePath.GetDirectoryOrFileName(),
                    destinationDir);
            }
        }

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

        internal IVirtualFile FindFile(string fullFilePath)
        {
            this.ThrowIfContainsOnlyDriveName(fullFilePath);
            IVirtualDirectory directory = this.FindDirectory(
                fullFilePath.GetPathWithoutLastItem());

            return directory?.Files.FirstOrDefault(f =>
                f.Name == fullFilePath.GetDirectoryOrFileName());
        }

        /// <summary>
        /// Получить корневую папку существующего диска.
        /// </summary>
        /// <param name="driveName">Имя диска.</param>
        /// <returns>Корневая папка диска.</returns>
        private IVirtualDrive GetExistingDrive(string driveName)
        {
            IVirtualDrive drive = this.virtualDrives.FirstOrDefault(d =>
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
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Весь метод используется только с целью проверки.
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