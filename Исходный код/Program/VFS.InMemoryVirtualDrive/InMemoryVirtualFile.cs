namespace VFS.InMemoryVirtualDrive
{
    using System;
    using System.Threading.Tasks;

    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Виртуальный файл, хранящий данные в памяти.
    /// </summary>
    internal class InMemoryVirtualFile : IVirtualFile
    {
        private string data = string.Empty;

        internal InMemoryVirtualFile(
            string name,
            InMemoryVirtualDriveConfig config)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(
                    nameof(name),
                    "Имя файла не должно быть пустым.");
            }

            if (name.Length > config.MaxFileNameLength)
            {
                throw new InvalidOperationException(
                    "Длина имени файла превышает допустимый предел - "
                    + config.MaxDirectoryNameLength + ".");
            }

            this.Name = name;
        }

        public string Name { get; }

        public async Task<string> GetDataAsync()
        {
            await Task.Delay(1000);
            return this.data;
        }

        public async Task WriteDataAsync(string newData)
        {
            await Task.Delay(1000);
            this.data = newData;
        }
    }
}