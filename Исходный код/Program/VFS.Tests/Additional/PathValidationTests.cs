namespace VFS.Tests.Additional
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Utilities;

    /// <summary>
    /// Тестирование валидации путей к файлам/папкам.
    /// </summary>
    [TestClass]
    public class PathValidationTests
    {
        /// <summary>
        /// Тестирование корректных путей.
        /// </summary>
        [TestMethod]
        public void CorrectPathValidation()
        {
            "c:\\Users\\Dir1".ValidateCorrectPath();
            "B:\\123\\456".ValidateCorrectPath();
            "C:".ValidateCorrectPath();
            "d:".ValidateCorrectPath();
            "fff.txt".ValidateCorrectPath();
        }

        /// <summary>
        /// Path возвращает имена дисков с двоеточием.
        /// </summary>
        [TestMethod]
        public void CorrectDriveNamesWithColon()
        {
            Assert.AreEqual("c:", "c:\\1\\2".GetLowerDriveName());
            Assert.AreEqual("d:", "D:".GetLowerDriveName());
            Assert.AreEqual("d:", "d:\\".GetLowerDriveName());
        }

        /// <summary>
        /// Имя файла или папки не является 
        /// </summary>
        [TestMethod]
        public void PathContainsDriveName()
        {
            Assert.IsFalse("abc.txt".ContainsDriveName());
            Assert.IsFalse("abc".ContainsDriveName());
            Assert.IsTrue("c:".ContainsDriveName());
            Assert.IsTrue("C:\\f".ContainsDriveName());
        }

        /// <summary>
        /// Тест получения пути без последнего файла или папки.
        /// </summary>
        [TestMethod]
        public void GetPathWithoutLastItem()
        {
            Assert.AreEqual("c:\\1\\2", "c:\\1\\2\\3".GetPathWithoutLastItem());
            Assert.AreEqual("c:\\1\\2", "c:\\1\\2\\3\\".GetPathWithoutLastItem());
            Assert.AreEqual("c:\\1\\2", "c:\\1\\2\\3.txt".GetPathWithoutLastItem());
            Assert.AreEqual("c:\\1.txt\\2.txt", "c:\\1.txt\\2.txt\\3.txt".GetPathWithoutLastItem());
        }

        /// <summary>
        /// Тест получения пути без последнего файла или папки.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(ArgumentException))]
        public void GetPathWithoutLastItemEmpty()
        {
            "c:".GetPathWithoutLastItem();
        }

        /// <summary>
        /// Получение имени файла или папки из корректного пути.
        /// </summary>
        [TestMethod]
        public void GetLastItemName()
        {
            Assert.AreEqual("3", "c:\\1\\2\\3".GetDirectoryOrFileName());
            Assert.AreEqual("3", "c:\\1\\2\\3\\".GetDirectoryOrFileName());
            Assert.AreEqual("3.txt", "c:\\1\\2\\3.txt".GetDirectoryOrFileName());
            Assert.AreEqual("c:", "c:\\".GetDirectoryOrFileName());
            Assert.AreEqual("c:", "c:".GetDirectoryOrFileName());
        }

        /// <summary>
        /// Тестирование некорректных путей.
        /// </summary>
        [TestMethod]
        public void IncorrectPathValidation()
        {
            Assert.IsFalse("c:\\\\".IsCorrectPath());
            Assert.IsFalse("c:\\\\Users".IsCorrectPath());
            Assert.IsFalse("dir1/dir2".IsCorrectPath());
            Assert.IsFalse(string.Empty.IsCorrectPath());
            Assert.IsFalse("CDE:\\Users".IsCorrectPath());
            Assert.IsFalse("   ".IsCorrectPath());
            string nullString = null;
            Assert.IsFalse(nullString.IsCorrectPath());
        }
    }
}
