namespace VFS.Tests.ServerSide
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.InMemoryVirtualDrive;
    using VFS.Interfaces.VirtualDrive;

    /// <summary>
    /// Класс тестирования виртуального диска, работающего с памятью.
    /// </summary>
    [TestClass]
    public class InMemoryVirtualDriveTests
    {
        /// <summary>
        /// Нормальная инициализация.
        /// </summary>
        [TestMethod]
        public void NormalInitialization()
        {
            // ReSharper disable once ObjectCreationAsStatement в целях тестирования создания объекта
            new InMemoryVirtualDrive();
        }

        /// <summary>
        /// Имя диска совпадает с назначенным при инициализации.
        /// </summary>
        [TestMethod]
        public void DriveNameCheck()
        {
            var drive = new InMemoryVirtualDrive();
            Assert.AreEqual("C:", drive.Name);
        }

        /// <summary>
        /// Тест поиска директорий по полному пути.
        /// </summary>
        [TestMethod]
        public void FindDirectoryTest()
        {
            var drive = new InMemoryVirtualDrive();
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
            var drive = new InMemoryVirtualDrive();
            IVirtualDirectory rootDir = drive.CreateDirectory("dir1");
            IVirtualDirectory dir2 = rootDir.CreateDirectory("DIR2");
            dir2.CreateDirectory("dir3");
            drive.FindDirectory("ffff");
        }

        /// <summary>
        /// Некорректный путь к файлу.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FindDirectoryWithInvalidPath()
        {
            var drive = new InMemoryVirtualDrive();
            IVirtualDirectory rootDir = drive.CreateDirectory("dir1");
            IVirtualDirectory dir2 = rootDir.CreateDirectory("DIR2");
            dir2.CreateDirectory("dir3");
            drive.FindDirectory("c:\\d:ir::");
        }

        /// <summary>
        /// Поиск папки не на том диске.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FindDirInAnotherDrive()
        {
            var drive = new InMemoryVirtualDrive();
            drive.CreateDirectory("dir1");
            Assert.IsNull(drive.FindDirectory("D:\\dir1\\DIR2\\dir3"));
        }
    }
}