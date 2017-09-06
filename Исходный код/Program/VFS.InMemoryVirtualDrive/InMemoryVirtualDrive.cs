namespace VFS.InMemoryVirtualDrive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using VFS.Interfaces.VirtualDrive;
    using VFS.Utilities;

    /// <summary>
    /// Виртуальный диск, использующий оперативную память
    /// в качестве хранилища данных.
    /// </summary>
    public class InMemoryVirtualDrive : IVirtualDrive
    {
        private InMemoryVirtualDriveConfig config;

        private InMemoryVirtualDirectory root;

        /// <summary>
        /// Инизиализировать виртуальный диск для начала работы.
        /// </summary>
        /// <param name="xmlConfig">Xml-конфигурация.</param>
        /// <param name="driveName">Имя диска (например, "C:").</param>
        public void Initialize(string xmlConfig, string driveName)
        {
            if (!Regex.IsMatch(driveName, "\\w:"))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(driveName),
                    "Имя диска должно содержать букву диска и символ \":\".");
            }

            if (string.IsNullOrWhiteSpace(xmlConfig))
            {
                throw new ArgumentNullException(
                    nameof(xmlConfig),
                    "Для инициализации интеграции необходимо передать XML-конфигурацию настройки.");
            }

            if (!XmlUtilities.ValidateXml(xmlConfig, this.GetXmlConfigSchema()))
            {
                throw new InvalidOperationException(
                    "Переданная xml-конфигурация не соответствует описанной в xsd-схеме.");
            }

            this.Name = driveName;

            this.config = XmlUtilities
                .DeserializeFromXml<InMemoryVirtualDriveConfig>(
                    xmlConfig);

            this.root = new InMemoryVirtualDirectory(
                "root",
                this.config);
        }

        /// <summary>
        /// Получить схему конфигурации виртуального диска.
        /// </summary>
        /// <returns>Схема конфигурации виртуального диска.</returns>
        public string GetXmlConfigSchema()
        {
            return AppResourceReader.GetResource(
                typeof(InMemoryVirtualDrive).Assembly,
                "VFS.InMemoryVirtualDrive.ConfigSchema.xsd");
        }

        /// <summary>
        /// Имя диска.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Получить список директорий в корне диска.
        /// </summary>
        public IEnumerable<IVirtualDirectory> ChildDirectories
        {
            get
            {
                this.ThrowIfNotInitialized();
                return this.root.ChildDirectories;
            }
        }

        /// <summary>
        /// Получить список файлов в корне диска.
        /// </summary>
        public IEnumerable<IVirtualFile> Files
        {
            get
            {
                this.ThrowIfNotInitialized();
                return this.root.Files;
            }
        }

        /// <summary>
        /// Создать файл в корне диска.
        /// </summary>
        /// <param name="name">Имя файла.</param>
        /// <returns>Созданный файл.</returns>
        public IVirtualFile CreateFile(string name)
        {
            this.ThrowIfNotInitialized();
            return this.root.CreateFile(name);
        }

        /// <summary>
        /// Создать новую папку в корне диска.
        /// </summary>
        /// <param name="name">Имя папки.</param>
        /// <returns>Созданная папка.</returns>
        public IVirtualDirectory CreateDirectory(string name)
        {
            this.ThrowIfNotInitialized();
            return this.root.CreateDirectory(name);
        }

        /// <summary>
        /// Удалить файл из корня диска.
        /// </summary>
        /// <param name="name">Имя файла.</param>
        public void RemoveFile(string name)
        {
            this.ThrowIfNotInitialized();
            this.root.RemoveFile(name);
        }

        /// <summary>
        /// Удалить директорию из корня диска.
        /// </summary>
        /// <param name="name">Имя директории.</param>
        /// <param name="recursive">Вместе с дочерними подпапками.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр <paramref name="recursive" />=false, а
        /// директория содержит подпапки.
        /// </exception>
        public void RemoveDirectory(string name, bool recursive)
        {
            this.ThrowIfNotInitialized();
            this.root.RemoveDirectory(name, recursive);
        }

        /// <summary>
        /// Переместить папку из корня диска в другую папку.
        /// </summary>
        /// <param name="childDirName">Имя папки из корня диска.</param>
        /// <param name="destination">Папка назначения.</param>
        public void MoveDirectoryTo(
            string childDirName,
            IVirtualDirectory destination)
        {
            this.ThrowIfNotInitialized();
            this.root.MoveDirectoryTo(childDirName, destination);
        }

        /// <summary>
        /// Переместить файл из корня диска в другую папку.
        /// </summary>
        /// <param name="childFileName">Имя файла в корне диска.</param>
        /// <param name="destination">Папка назначения.</param>
        public void MoveFileTo(
            string childFileName,
            IVirtualDirectory destination)
        {
            this.ThrowIfNotInitialized();
            this.root.MoveFileTo(childFileName, destination);
        }

        /// <summary>
        /// Создать новую папку (копию переданной) в корне диска.
        /// </summary>
        /// <param name="copiedDirectory">Копируемая папка.</param>
        /// <returns>Созданная копия.</returns>
        public IVirtualDirectory CopyDirectoryFrom(
            IVirtualDirectory copiedDirectory)
        {
            this.ThrowIfNotInitialized();
            return this.root.CopyDirectoryFrom(copiedDirectory);
        }

        /// <summary>
        /// Создать в корне диска новый файл - копию переданного.
        /// </summary>
        /// <param name="copiedFile">Копируемый файл.</param>
        /// <returns>Созданная копия.</returns>
        public IVirtualFile CopyFileFrom(
            IVirtualFile copiedFile)
        {
            this.ThrowIfNotInitialized();
            return this.root.CopyFileFrom(copiedFile);
        }

        /// <summary>
        /// Поиск директории по полному пути.
        /// </summary>
        /// <param name="fullPath">Полный путь к директории.</param>
        /// <returns>Найденная директория, или null.</returns>
        public IVirtualDirectory FindDirectory(string fullPath)
        {
            this.ThrowIfNotInitialized();
            fullPath.ValidateCorrectPath();

            if (!fullPath.ContainsDriveName())
            {
                throw new InvalidOperationException(
                    "Для поиска директории требуется полный путь, включая имя диска.");
            }

            if (!fullPath.ToLowerInvariant().StartsWith(this.Name.ToLowerInvariant()))
            {
                throw new InvalidOperationException(
                    fullPath + " находится не на диске " + this.Name);
            }

            string[] lowerDirNames = fullPath.ToLowerInvariant().Replace(
                    this.Name.ToLowerInvariant() + Path.DirectorySeparatorChar,
                    string.Empty)
                .Split(
                    new[] {Path.DirectorySeparatorChar},
                    StringSplitOptions.RemoveEmptyEntries);

            if (lowerDirNames.Length == 0
                && fullPath.ToLowerInvariant().Contains(this.Name.ToLowerInvariant()))
            {
                return this;
            }

            IVirtualDirectory foundDir = this.root;
            foreach (string lowerDirName in lowerDirNames)
            {
                foundDir = foundDir.ChildDirectories.ToList()
                    .FirstOrDefault(d =>
                        d.Name.ToLowerInvariant() == lowerDirName);

                if (foundDir == null)
                {
                    return null;
                }
            }

            return foundDir;
        }

        /// <summary>
        /// Завершить работу с диском.
        /// </summary>
        public void Dispose()
        {
            this.config = null;
            this.root = null;
        }

        internal InMemoryVirtualDirectory GetRoot()
        {
            return this.root;
        }

        private void ThrowIfNotInitialized()
        {
            if (this.config == null || this.root == null)
            {
                throw new InvalidOperationException(
                    "Виртуальный диск не инициализирован.");
            }
        }
    }
}