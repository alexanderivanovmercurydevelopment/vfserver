namespace VFS.Interfaces.VirtualDrive
{
    /// <summary>
    /// Виртуальный файл.
    /// </summary>
    public interface IVirtualFile
    {
        /// <summary>
        /// Имя файла.
        /// </summary>
        string Name { get; }
    }
}