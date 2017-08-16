namespace VFS.Interfaces.VirtualDrive
{
    /// <summary>
    /// Интерфейс взаимодействия различных виртуальных дисков.
    /// </summary>
    public interface IIntegrationBehaviour
    {
        /// <summary>
        /// Переместить дочернюю папку в другую папку.
        /// </summary>
        /// <param name="source">Исходная папка.</param>
        /// <param name="childDirName">Имя дочерней папки.</param>
        /// <param name="destination">Целевая папка.</param>
        void MoveChildDirectoryTo(
            IVirtualDirectory source,
            string childDirName,
            IVirtualDirectory destination);

        /// <summary>
        /// Переместить файл из папки в другую папку.
        /// </summary>
        /// <param name="source">Исходная папка.</param>
        /// <param name="childFileName">Имя дочернего файла.</param>
        /// <param name="destination">Целевая папка.</param>
        void MoveChildFileTo(
            IVirtualDirectory source,
            string childFileName,
            IVirtualDirectory destination);

        /// <summary>
        /// Создать копию директории.
        /// </summary>
        /// <typeparam name="TResult">Тип результата (копии).</typeparam>
        /// <param name="source">Копируемая директория.</param>
        /// <returns>Копия директории.</returns>
        TResult CreateDirCopy<TResult>(IVirtualDirectory source) 
            where TResult : IVirtualDirectory;

        /// <summary>
        /// Создать копию файла.
        /// </summary>
        /// <typeparam name="TResult">Тип результата (копии).</typeparam>
        /// <param name="source">Копируемый файл.</param>
        /// <returns>Копия файла.</returns>
        TResult CreateFileCopy<TResult>(IVirtualFile source)
            where TResult : IVirtualFile;
    }
}
