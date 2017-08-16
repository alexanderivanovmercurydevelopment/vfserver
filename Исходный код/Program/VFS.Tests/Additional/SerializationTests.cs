namespace VFS.Tests.Additional
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Utilities;

    /// <summary>
    /// Тесты сериализации в xml.
    /// </summary>
    [TestClass]
    public class SerializationTests
    {
        /// <summary>
        /// Сериализация структуры диска проходит без исключений
        /// (сам результат - не проверяется).
        /// </summary>
        [TestMethod]
        public void SerializationWithoutExceptions()
        {
            var drive = new DriveStructureInfo();
            drive.Directories = new List<VFSDirectoryInfo>();
            drive.Files = new List<VFSFileInfo>();
            drive.Directories.Add(new VFSDirectoryInfo() { Name = "dir" });
            drive.Files.Add(new VFSFileInfo() { Name = "file" });

            string str = XMLUtilities.SerializeToXml<DriveStructureInfo>(drive);
        }
    }
}
