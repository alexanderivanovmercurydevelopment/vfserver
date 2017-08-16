namespace VFS.Interfaces.VirtualDrive
{
    using System.Collections.Generic;

    /// <summary>
    /// Виртуальная папка.
    /// </summary>
    public interface IVirtualDirectory
    {
        /// <summary>
        /// Имя директории.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Корневые папки.
        /// </summary>
        IEnumerable<IVirtualDirectory> Directories { get; }

        /// <summary>
        /// Корневые файлы.
        /// </summary>
        IEnumerable<IVirtualFile> Files { get; }

        /// <summary>
        /// Создать новый файл.
        /// </summary>
        /// <param name="name">Имя файла.</param>
        /// <returns>Созданный файл.</returns>
        IVirtualFile CreateFile(string name);

        /// <summary>
        /// Создать новую папку.
        /// </summary>
        /// <param name="name">Имя папки.</param>
        /// <returns>Созданная папка.</returns>
        IVirtualDirectory CreateDirectory(string name);

        /// <summary>
        /// Удалить файл.
        /// </summary>
        /// <param name="name">Имя удаляемого файла.</param>
        void RemoveFile(string name);

        /// <summary>
        /// Удалить дочернюю папку.
        /// </summary>
        /// <param name="name">Имя удаляемой дочерней папки.</param>
        /// <param name="recursive">Удалить вместе с подпапками.</param>
        void RemoveDirectory(string name, bool recursive);

        /// <summary>
        /// Переместить дочернюю папку в другую папку.
        /// </summary>
        /// <param name="childDirName">Имя дочерней папки.</param>
        /// <param name="destination">Папка назначения.</param>
        void MoveDirectoryTo(
            string childDirName, 
            IVirtualDirectory destination);

        /// <summary>
        /// Переместить файл из папки в другую папку.
        /// </summary>
        /// <param name="childFileName">Имя дочернего файла.</param>
        /// <param name="destination">Папка назначения.</param>
        void MoveFileTo(
            string childFileName, 
            IVirtualDirectory destination);

        /// <summary>
        /// Создать новую дочернюю папку - копию переданной.
        /// </summary>
        /// <param name="copiedDirectory">Копируемая папка.</param>
        /// <returns>Созданная копия.</returns>
        IVirtualDirectory CopyDirectoryFrom(
            IVirtualDirectory copiedDirectory);

        /// <summary>
        /// Создать в директории новый файл - копию переданного.
        /// </summary>
        /// <param name="copiedFile">Копируемый файл.</param>
        /// <returns>Созданная копия.</returns>
        IVirtualFile CopyFileFrom(
            IVirtualFile copiedFile);
    }
}
