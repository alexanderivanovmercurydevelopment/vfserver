namespace VFS.Tests.ServerSide
{
    using System;
    using System.IO;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.InMemoryVirtualDrive;
    using VFS.Interfaces.VirtualDrive;
    using VFS.Utilities;

    
    /// <summary>
    /// Класс тестирования виртуального диска, работающего с памятью.
    /// </summary>
    [TestClass]
    public class InMemoryVirtualDriveTests
    {
        /// <summary>
        /// Инициализация виртуального диска некорректным именем.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void InvalidDriveNameTest()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            string config = this.GetDriveStandardConfig();
            drive.Initialize(config, "C");// без двоеточия, нестандартное имя диска.
        }

        /// <summary>
        /// Инициализация виртуального диска некорректным именем.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void InvalidConfigTest()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<InMemoryVirtualDrive>
  <MaxFileNameLength>100</MaxFileNameLength>
  <MaxDirectoryNameLength>100</MaxDirectoryNameLength>
  <INVALIDELEMENT>100</INVALIDELEMENT>
</InMemoryVirtualDrive>", "C:");
        }

        /// <summary>
        /// Инициализация виртуального диска со стандартной конфигурацией.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(ArgumentNullException))]
        public void InitializeWithoutConfigTest()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize(null, "D:");
        }

        /// <summary>
        /// Инициализация виртуального диска со стандартной конфигурацией.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitializeWithEmptyConfigTest()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize("   ", "D:");
        }

        /// <summary>
        /// Нормальная инициализация.
        /// </summary>
        [TestMethod]
        public void NormalInitialization()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            string config = this.GetDriveStandardConfig();
            drive.Initialize(config, "C:");
        }

        /// <summary>
        /// Имя диска совпадает с назначенным при инициализации.
        /// </summary>
        [TestMethod]
        public void DriveNameCheck()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            string config = this.GetDriveStandardConfig();
            drive.Initialize(config, "C:");
            Assert.AreEqual("C:", drive.Name);
        }

        /// <summary>
        /// Схема конфигурации существует.
        /// </summary>
        [TestMethod]
        public void ConfigSchemaNotEmpty()
        {
            IVirtualDrive drive = new InMemoryVirtualDrive();
            string schema = drive.GetXmlConfigSchema();
            Assert.IsFalse(string.IsNullOrWhiteSpace(schema));
        }

        /// <summary>
        /// Тест поиска директорий по полному пути.
        /// </summary>
        [TestMethod]
        public void FindDirectoryTest()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize(this.GetDriveStandardConfig(), "C:");
            IVirtualDirectory rootDir = drive.CreateDirectory("dir1");
            IVirtualDirectory dir2 = rootDir.CreateDirectory("DIR2");
            dir2.CreateDirectory("dir3");

            Assert.IsNotNull(drive.FindDirectory("C:\\dir1\\dir2\\dir3"));
            Assert.IsNotNull(drive.FindDirectory("c:\\dIr1\\DiR2\\DIR3"));
            Assert.IsNull(drive.FindDirectory("c:\\ dir1 \\dir2 \\ dir3"));
            Assert.IsNull(drive.FindDirectory("C:\\dir1\\dir3"));
            Assert.IsNull(drive.FindDirectory("C:\\dir33"));
        }

        /// <summary>
        /// Тест поиска директорий по некорректному полному пути.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FindDirectoryWithoutDriveNamePathTest()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize(this.GetDriveStandardConfig(), "C:");
            IVirtualDirectory rootDir = drive.CreateDirectory("dir1");
            IVirtualDirectory dir2 = rootDir.CreateDirectory("DIR2");
            dir2.CreateDirectory("dir3");
            drive.FindDirectory("ffff");
        }

        /// <summary>
        /// Некорректный путь к файлу.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(ArgumentException))]
        public void FindDirectoryWithInvalidPath()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize(this.GetDriveStandardConfig(), "C:");
            IVirtualDirectory rootDir = drive.CreateDirectory("dir1");
            IVirtualDirectory dir2 = rootDir.CreateDirectory("DIR2");
            dir2.CreateDirectory("dir3");
            drive.FindDirectory("c:\\d:ir::");
        }

        /// <summary>
        /// Поиск папки не на том диске.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void FindDirInAnotherDrive()
        {
            InMemoryVirtualDrive drive = new InMemoryVirtualDrive();
            drive.Initialize(this.GetDriveStandardConfig(), "C:");
            IVirtualDirectory rootDir = drive.CreateDirectory("dir1");
            Assert.IsNull(drive.FindDirectory("D:\\dir1\\DIR2\\dir3"));
        }

        /// <summary>
        /// Получить строку xml из примера структуры файлов и папок.
        /// </summary>
        /// <returns>XML-структура папок и файлов.</returns>
        private string GetDriveStandardConfig()
        {
            return AppResourceReader.GetResource(
                typeof(InMemoryVirtualDrive).Assembly,
                "VFS.InMemoryVirtualDrive.ConfigExample.xml");
        }
    }
}
