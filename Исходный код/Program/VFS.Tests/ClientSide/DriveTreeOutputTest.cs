namespace VFS.Tests.ClientSide
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Client;
    using VFS.Interfaces.DriveStructureMessageFormat;
    using VFS.Utilities;
    
    /// <summary>
    /// Тестирование вывода структуры папок и файлов.
    /// </summary>
    [TestClass]
    public class DriveTreeOutputTest
    {
        private const string ExpectedConsoleOutput = @"C:
|_DIR1
| |_DIR2
| |_DIR4
| | |_DIR3
| | |_1.txt
| |_DIR5
|   |_2.txt[LOCKED by Me]
|   |_3.txt[LOCKED by TestUser]
|_DIR7
|_dIR8
|_DIR9
";

        /// <summary>
        /// Тестирование вывода структуры папок и файлов.
        /// </summary>
        [TestMethod]
        public void DriveTreeConsoleOutputTest()
        {
            string xml =  this.GetDriveStructureExample();

            ConsoleDriveStructureInfo info = 
                new ConsoleDriveStructureInfo(xml);

            string result = info.GetConsoleFriendlyString();

            Assert.AreEqual(
                result, 
                DriveTreeOutputTest.ExpectedConsoleOutput);
        }

        /// <summary>
        /// Коллекции файлов и папок при парсинге xml
        /// не должны быть равны null (должны быть пустые коллекции).
        /// </summary>
        [TestMethod]
        public void DriveTreeCollectionLinksNotNullTest()
        {
            string xml = this.GetDriveStructureExample();

            DriveStructureInfo structureInfo =
                XMLUtilities.DeserializeFromXml<DriveStructureInfo>(
                    xml);

            this.CheckCollectionsNotNullRecursive(structureInfo);
        }

        /// <summary>
        /// Получить строку xml из примера структуры файлов и папок.
        /// </summary>
        /// <returns>XML-структура папок и файлов.</returns>
        private string GetDriveStructureExample()
        {
            return AppResourceReader.GetResource(
                typeof(DriveStructureInfo).Assembly,
                "VFS.Interfaces.DriveStructureMessageFormat.Example.xml");            
        }

        /// <summary>
        /// Проверить, что внутренние коллекции не равны нулю.
        /// </summary>
        /// <param name="dirStructureInfo">Информация о папке.</param>
        private void CheckCollectionsNotNullRecursive(
            IVFSDirectoryInfo dirStructureInfo)
        {
            Assert.IsNotNull(dirStructureInfo.Files);
            Assert.IsNotNull(dirStructureInfo.Directories);

            foreach (VFSDirectoryInfo childDir 
                in dirStructureInfo.Directories)
            {
                this.CheckCollectionsNotNullRecursive(childDir);
            }

            foreach (VFSFileInfo childFile in dirStructureInfo.Files)
            {
                Assert.IsNotNull(childFile.LockingUsers);
            }
        }
    }
}
