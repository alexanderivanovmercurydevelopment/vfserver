namespace VFS.Interfaces.VirtualDrive
{
    using System;

    /// <summary>
    /// Виртуальный диск.
    /// </summary>
    public interface IVirtualDrive : IVirtualDirectory, IDisposable
    {
        /// <summary>
        /// Найти директорию по полному пути.
        /// </summary>
        /// <param name="fullPath">Полный путь к директории.</param>
        /// <returns>Найденная директория, или null.</returns>
        IVirtualDirectory FindDirectory(string fullPath);
    }
}