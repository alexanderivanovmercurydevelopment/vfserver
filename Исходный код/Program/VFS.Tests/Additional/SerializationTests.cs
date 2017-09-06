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
            var drive = new DriveStructureInfo
            {
                Directories = new List<VFSDirectoryInfo>(),
                Files = new List<VFSFileInfo>()
            };

            drive.Directories.Add(new VFSDirectoryInfo {Name = "dir"});
            drive.Files.Add(new VFSFileInfo {Name = "file"});

            XmlUtilities.SerializeToXml(drive);
        }
    }
}