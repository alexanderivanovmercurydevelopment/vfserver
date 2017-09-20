namespace VFS.Interfaces.VirtualDrive
{
    using System.Threading.Tasks;

    /// <summary>
    /// Виртуальный файл.
    /// </summary>
    public interface IVirtualFile
    {
        /// <summary>
        /// Имя файла.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Получить данные из файла.
        /// </summary>
        Task<string> GetDataAsync();

        /// <summary>
        /// Записать данные в файл.
        /// </summary>
        /// <param name="data">Данные.</param>
        /// <returns></returns>
        Task WriteDataAsync(string data);
    }
}