namespace VFS.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Расширяющие методы для работы со строкой пути к файлу.
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Проверить корректность пути к файлу/папке.
        /// </summary>
        /// <param name="filePath">Путь к файлу/папке.</param>
        /// <exception cref="ArgumentException">
        /// Путь к папке или файлу некорректен.
        /// </exception>
        public static void ValidateCorrectPath(
            this string filePath)
        {
            if (!filePath.IsCorrectPath())
            {
                throw new ArgumentException(
                    "filePath",
                    "Строка " + filePath + " не является корректным именем файла/папки.");
            }
        }

        /// <summary>
        /// Путь содержит название диска.
        /// </summary>
        /// <param name="path">Путь.</param>
        /// <returns>True - содержит название диска, false - нет.</returns>
        public static bool ContainsDriveName(this string path)
        {
            path.ValidateCorrectPath();
            return path.Contains(":");
        }

        /// <summary>
        /// Получить имя диска (например, "c:") в нижнем регистре.
        /// </summary>
        /// <param name="path">Путь, содержащий в т.ч. имя диска.</param>
        /// <returns>Имя диска (с двоеточием, без дополнительных разделителей).</returns>
        public static string GetLowerDriveName(this string path)
        {
            path.ValidateCorrectPath();
            
            string root = Path.GetPathRoot(path)
                .Trim()
                .Replace(Path.DirectorySeparatorChar.ToString(), string.Empty);           

            if (!root.Contains(":") || root.Length > 2)
            {
                throw new ArgumentException(
                    "path",
                    "Путь не содержит имя диска.");
            }

            return root.ToLowerInvariant();
        }

        /// <summary>
        /// Получить путь без последней папки или файла 
        /// (и без последнего разделителя).
        /// </summary>
        /// <param name="path">Путь.</param>
        /// <returns>Путь без последней папки или файла.</returns>
        /// <exception cref="ArgumentException">
        /// Путь некорректен.
        /// </exception>
        public static string GetPathWithoutLastItem(
            this string path)
        {
            path.ValidateCorrectPath();

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Remove(path.Length - 1);
            }

            if (!path.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                throw new ArgumentException(
                    "Путь содержит только имя файла/папки.");
            }

            return Path.GetDirectoryName(path) 
                ?? Path.GetFileName(path);
        }

        /// <summary>
        /// Получить имя директории или файла в пути (т.е.
        /// название последнего элемента).
        /// </summary>
        /// <param name="path">Корректный путь.</param>
        /// <returns>Название последнего элемента.</returns>
        public static string GetDirectoryOrFileName(
            this string path)
        {
            path.ValidateCorrectPath();

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.Remove(path.Length - 1);
            }

            int lastSeparatorIndex = path.LastIndexOf(Path.DirectorySeparatorChar);
            return path.Substring(lastSeparatorIndex + 1);
        }

        /// <summary>
        /// Признак корректности пути к файлу/папке.
        /// </summary>
        /// <param name="path">Путь к файлу/папке.</param>
        /// <returns>True - путь корректен, false - нет.</returns>
        public static bool IsCorrectPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            List<string> dirOrDriveNames = path.Split(
                new char[] { Path.DirectorySeparatorChar },
                StringSplitOptions.None)
                .ToList();

            if (String.IsNullOrWhiteSpace(dirOrDriveNames.Last()))
            {
                dirOrDriveNames.RemoveAt(dirOrDriveNames.Count - 1);
            }

            if (dirOrDriveNames.Any(s => string.IsNullOrWhiteSpace(s)))
            {
                return false;
            }

            List<string> checkedStrings = new List<string>(dirOrDriveNames);

            try
            {
                if (Path.IsPathRooted(path))
                {
                    checkedStrings.Remove(checkedStrings.First());
                }
            }
            catch
            {
                return false;
            }

            string regexString =
                "["
                + Regex.Escape(string.Join(string.Empty, Path.GetInvalidPathChars()))
                + Regex.Escape(string.Join(string.Empty, Path.GetInvalidFileNameChars()))
                + "]";

            if (checkedStrings.Any(name => Regex.IsMatch(name, regexString)))
            {
                return false;
            }

            return true;
        }
    }
}
