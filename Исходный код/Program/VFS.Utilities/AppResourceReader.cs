namespace VFS.Utilities
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Класс чтения ресурсов сборки.
    /// </summary>
    public static class AppResourceReader
    {
        /// <summary>
        /// Прочитать ресурс сборки в строку.
        /// </summary>
        /// <param name="assembly">Сборка.</param>
        /// <param name="resourcePath">Полный путь к ресурсу.</param>
        /// <returns>Содержимое ресурса в виде строки.</returns>
        public static string GetResource(
            Assembly assembly,
            string resourcePath)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return null;
        }
    }
}