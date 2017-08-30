namespace VFS.InMemoryVirtualDrive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Виртуальная директория.
    /// </summary>
    internal class InMemoryVirtualDirectory : IVirtualDirectory
    {
        /// <summary>
        /// Имя директории.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Конфигурация виртуального диска.
        /// </summary>
        private readonly InMemoryVirtualDriveConfig config;

        /// <summary>
        /// Дочерние папки.
        /// </summary>
        private readonly List<InMemoryVirtualDirectory> childDirectories
            = new List<InMemoryVirtualDirectory>();

        /// <summary>
        /// Файлы.
        /// </summary>
        private readonly List<InMemoryVirtualFile> files
            = new List<InMemoryVirtualFile>();

        /// <summary>
        /// Блокируемый объект (для параллельных операций).
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// Создать виртуальную директорию.
        /// </summary>
        /// <param name="name">Имя директории.</param>
        /// <param name="config">Конфигурация текущего виртуального диска.</param>
        internal InMemoryVirtualDirectory(
            string name, 
            InMemoryVirtualDriveConfig config)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(
                    nameof(name),
                    "Имя папки не должно быть пустым.");
            }

            if (name.Length > config.MaxDirectoryNameLength)
            {
                throw new InvalidOperationException(
                    "Длина имени папки превышает допустимый предел - "
                    + config.MaxDirectoryNameLength + ".");
            }

            this.name = name;
            this.config = config;
        }

        /// <summary>
        /// Имя папки.
        /// </summary>
        public string Name => this.name;

        /// <summary>
        /// Дочерние папки.
        /// </summary>
        public IEnumerable<IVirtualDirectory> Directories
        {
            get 
            { 
                lock (this.lockObj)
                {
                    return this.childDirectories.ToList(); 
                }
            }
        }

        /// <summary>
        /// Файлы.
        /// </summary>
        public IEnumerable<IVirtualFile> Files
        {
            get 
            { 
                lock (this.lockObj)
                {
                    return this.files.ToList(); 
                }
            }
        }

        /// <summary>
        /// Создать новый файл.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Созданный файл.</returns>
        public IVirtualFile CreateFile(string fileName)
        {
            lock (this.lockObj)
            {
                if (this.files.Any((f) => f.Name == fileName))
                {
                    throw new InvalidOperationException(
                        "Файл с именем " + fileName + " уже существует в папке " + this.name);
                }

                InMemoryVirtualFile newFile = new InMemoryVirtualFile(
                    fileName, this.config);

                this.files.Add(newFile);

                return newFile;
            }
        }

        /// <summary>
        /// Создать новую папку.
        /// </summary>
        /// <param name="directoryName">Имя папки.</param>
        /// <returns>Созданная папка.</returns>
        public IVirtualDirectory CreateDirectory(string directoryName)
        {
            lock (this.lockObj)
            {
                if (this.childDirectories.Any(d => d.Name == directoryName))
                {
                    throw new InvalidOperationException(
                        "Папка " + directoryName + " уже существует.");
                }

                InMemoryVirtualDirectory newDirectory =
                    new InMemoryVirtualDirectory(
                        directoryName,
                        this.config);

                this.childDirectories.Add(newDirectory);

                return newDirectory;
            }
        }

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="fileName">Имя удаляемого файла.</param>
        public void RemoveFile(string fileName)
        {
            lock (this.lockObj)
            {
                InMemoryVirtualFile file = this.files.FirstOrDefault(
                    f => f.Name == fileName);

                if (file == null)
                {
                    throw new FileNotFoundException(
                        "Файла " + fileName + " не существует в папке " + this.name + ".");
                }

                this.files.Remove(file);
            }
        }

        /// <summary>
        /// Удалить дочернюю папку.
        /// </summary>
        /// <param name="directoryName">Имя удаляемой папки.</param>
        /// <param name="recursive">Если папка содержит подпапки, удалить 
        /// также и подпапки.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр <paramref name="recursive"/> равен false, 
        /// а папка содержит подпапки.
        /// </exception>
        public void RemoveDirectory(string directoryName, bool recursive)
        {
            lock (this.lockObj)
            {
                InMemoryVirtualDirectory directory =
                    this.childDirectories.FirstOrDefault(d => d.Name == directoryName);

                if (directory == null)
                {
                    throw new DirectoryNotFoundException(
                        "В папке " + this.name + " не существует папки " + directoryName);
                }

                bool containsDirectories = directory.childDirectories.Count > 0;

                if (containsDirectories && !recursive)
                {
                    throw new InvalidOperationException(
                        "Невозможно удалить папку " + directoryName + "."
                        + " Сначала нужно удалить дочерние папки.");
                }

                directory.ClearRecursive();
                this.childDirectories.Remove(directory);
            }
        }

        /// <summary>
        /// Переместить дочернюю папку в другую папку.
        /// </summary>
        /// <param name="childDirName">Имя дочерней папки.</param>
        /// <param name="destination">Папка назначения.</param>
        public void MoveDirectoryTo(
            string childDirName, 
            IVirtualDirectory destination)
        {
            lock (this.lockObj)
            {               
                InMemoryVirtualDirectory copiedDir = this.childDirectories
                    .FirstOrDefault(d => d.name == childDirName);

                if (copiedDir == null)
                {
                    throw new DirectoryNotFoundException(
                        "В папке " + this.name + " отсутствует папка " + childDirName + ".");
                }

                InMemoryVirtualDirectory inMemoryDestinationDir = 
                    InMemoryVirtualDirectory.TryCastToInMemoryVirtualDirectory(destination);

                if (inMemoryDestinationDir != null)
                {
                    if (inMemoryDestinationDir.childDirectories.Any(d => d.name == childDirName))
                    {
                        throw new InvalidOperationException(
                            "Директория " + childDirName + " уже существует.");
                    }                        

                    // папка не будут удалена, просто перемещена в другую папку.
                    inMemoryDestinationDir.childDirectories.Add(copiedDir);
                    this.childDirectories.Remove(copiedDir);
                }
                else
                {
                    this.config.ThrowIfIntegrationNotSet();
                    this.config.IntegrationBehaviour.MoveChildDirectoryTo(
                        this,
                        childDirName,
                        destination);
                }
            }
        }

        /// <summary>
        /// Переместить файл из папки в другую папку.
        /// </summary>
        /// <param name="childFileName">Имя дочернего файла.</param>
        /// <param name="destination">Папка назначения.</param>
        public void MoveFileTo(
            string childFileName, 
            IVirtualDirectory destination)
        {
            lock (this.lockObj)
            {
                InMemoryVirtualFile copiedFile = this.files
                .FirstOrDefault(f => f.Name == childFileName);

                if (copiedFile == null)
                {
                    throw new FileNotFoundException(
                        "В папке " + this.name + " отсутствует файл " + childFileName + ".");
                }

                InMemoryVirtualDirectory inMemoryDestinationDir =
                    InMemoryVirtualDirectory.TryCastToInMemoryVirtualDirectory(destination);

                if (inMemoryDestinationDir != null)
                {
                    if (inMemoryDestinationDir.files.Any(f => f.Name == childFileName))
                    {
                        throw new InvalidOperationException(
                            "Файл " + childFileName + " уже существует.");
                    }     

                    inMemoryDestinationDir.files.Add(copiedFile);
                    this.files.Remove(copiedFile);
                }
                else
                {
                    this.config.ThrowIfIntegrationNotSet();
                    this.config.IntegrationBehaviour.MoveChildFileTo(
                        this,
                        childFileName,
                        destination);
                }
            }            
        }

        /// <summary>
        /// Создать новую дочернюю папку - копию переданной.
        /// </summary>
        /// <param name="copiedDirectory">Копируемая папка.</param>
        /// <returns>Созданная копия.</returns>
        public IVirtualDirectory CopyDirectoryFrom(
            IVirtualDirectory copiedDirectory)
        {
            lock (this.lockObj)
            {
                if (this.childDirectories.Any(d => d.Name == copiedDirectory.Name))
                {
                    throw new InvalidOperationException(
                        "Директория " + copiedDirectory.Name + " уже существует.");
                }

                InMemoryVirtualDirectory inMemoryDirSource
                    = copiedDirectory as InMemoryVirtualDirectory;

                if (inMemoryDirSource == null)
                {
                    this.config.ThrowIfIntegrationNotSet();
                }

                InMemoryVirtualDirectory copy = inMemoryDirSource != null
                    ? inMemoryDirSource.CreateCopy()
                    : this.config.IntegrationBehaviour
                        .CreateDirCopy<InMemoryVirtualDirectory>(copiedDirectory);

                this.childDirectories.Add(copy);
                return copy;
            }
        }

        /// <summary>
        /// Создать в директории новый файл - копию переданного.
        /// </summary>
        /// <param name="copiedFile">Копируемый файл.</param>
        /// <returns>Созданная копия.</returns>
        public IVirtualFile CopyFileFrom(
            IVirtualFile copiedFile)
        {
            lock (this.lockObj)
            {
                if (this.files.Any(f => f.Name == copiedFile.Name))
                {
                    throw new InvalidOperationException(
                        "Файл " + copiedFile.Name + " уже существует.");
                }

                InMemoryVirtualFile inMemoryFileSource
                    = copiedFile as InMemoryVirtualFile;

                if (inMemoryFileSource == null)
                {
                    this.config.ThrowIfIntegrationNotSet();
                }

                InMemoryVirtualFile copy = inMemoryFileSource != null
                    ? new InMemoryVirtualFile(copiedFile.Name, this.config)
                    : this.config.IntegrationBehaviour
                        .CreateFileCopy<InMemoryVirtualFile>(copiedFile);

                this.files.Add(copy);
                return copy;
            }
        }

        /// <summary>
        /// Попробовать привести <see cref="IVirtualDirectory"/> к типу
        /// <see cref="InMemoryVirtualDirectory"/>.
        /// </summary>
        /// <param name="directory">Интерфейс директории.</param>
        /// <returns><see cref="InMemoryVirtualDirectory"/> или null.</returns>
        private static InMemoryVirtualDirectory TryCastToInMemoryVirtualDirectory(
            IVirtualDirectory directory)
        {
            InMemoryVirtualDirectory inMemoryDestinationDir
                = directory as InMemoryVirtualDirectory;

            if (inMemoryDestinationDir == null)
            {
                InMemoryVirtualDrive drive = directory as InMemoryVirtualDrive;

                if (drive != null)
                {
                    inMemoryDestinationDir = drive.GetRoot();
                }
            }

            return inMemoryDestinationDir;
        }

        /// <summary>
        /// Очистить директорию, т.е. удалить всё содержимое.
        /// </summary>
        private void ClearRecursive()
        {
            lock (this.lockObj)
            {
                foreach (InMemoryVirtualDirectory directory
                    in this.childDirectories.ToArray())
                {
                    directory.ClearRecursive();
                    this.childDirectories.Remove(directory);
                }

                foreach (InMemoryVirtualFile file
                    in this.files.ToArray())
                {
                    this.files.Remove(file);
                }
            }
        }

        /// <summary>
        /// Получить полную копию директории.
        /// </summary>
        /// <returns>Созданная полная копия .</returns>
        private InMemoryVirtualDirectory CreateCopy()
        {
            lock (this.lockObj)
            {
                InMemoryVirtualDirectory copy =
                    new InMemoryVirtualDirectory(this.name, this.config);

                foreach (InMemoryVirtualFile file
                    in this.files)
                {
                    copy.files.Add(
                        new InMemoryVirtualFile(file.Name, this.config));
                }

                foreach (InMemoryVirtualDirectory directory
                    in this.childDirectories)
                {
                    copy.childDirectories.Add(directory.CreateCopy());
                }

                return copy;
            }
        }
    }
}
