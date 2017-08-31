// ReSharper disable UnusedVariable
// названия директорий и другие переменные могут использоваться
// просто для наглядности.
namespace VFS.Tests.ServerSide
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Interfaces;
    using VFS.Server;

    /// <summary>
    /// Тесты некоторых описанных в задании требований.
    /// </summary>
    [TestClass]
    public class RequirementsTests
    {
        /// <summary>
        /// Позитивное тестирование подключения.
        /// </summary>
        [TestMethod]
        public void ConnectionPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            Assert.AreEqual(1, server.GetUsersCount());
        }

        /// <summary>
        /// Негативное тестирование подключения.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void ConnectionNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.ConnectUser("User1");
        }

        /// <summary>
        /// Позитивное тестирование отключения.
        /// </summary>
        [TestMethod]
        public void DisconnectPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.DisconnectUser("User1");
            Assert.AreEqual(0, server.GetUsersCount());
        }

        /// <summary>
        /// Негативное тестирование отключения.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void DisconnectNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.DisconnectUser("User1");
            Assert.AreEqual(0, server.GetUsersCount());
        }

        /// <summary>
        /// Позитивное тестирование создания директории.
        /// </summary>
        [TestMethod]
        public void MakeDirPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "c:\\dir1");
            server.CreateDirectory("User1", "dir2");
            server.CreateDirectory("User1", "C:\\dir2\\dir3");
            
            var dirs = server.GetDriveStructure("C:").Directories;

            Assert.AreEqual(2, dirs.Count);

            var dir3 = dirs.First(d => d.Name == "dir2")
                .Directories.First();

            Assert.AreEqual("dir3", dir3.Name);
        }

        /// <summary>
        /// Негативное тестирование создания директории.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(DirectoryNotFoundException))]
        public void MakeDirNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "c:\\dir1\\dir2");
        }

        /// <summary>
        /// Позитивное тестирование смены текущей директории.
        /// </summary>
        [TestMethod]
        public void ChangeDirPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.ChangeUsersCurrentDirectory("User1", "c:");
            server.CreateDirectory("User1", "c:\\temp");
            server.ChangeUsersCurrentDirectory("User1", "temp");
            server.CreateDirectory("User1", "dir2");
            server.ChangeUsersCurrentDirectory("User1", "c:\\temp\\dir2");
            server.CreateDirectory("User1", "dir3");

            var drive = server.GetDriveStructure("C:");

            var thirdDir = drive
                .Directories.First(d => d.Name == "temp")
                .Directories.First(d => d.Name == "dir2")
                .Directories.First(d => d.Name == "dir3");
        }

        /// <summary>
        /// Негативное тестирование смены текущей директории.
        /// </summary>
        [TestMethod]
        public void ChangeDirNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.ConnectUser("User2");

            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "dir2");

            server.ChangeUsersCurrentDirectory("User1", "dir1");
            server.ChangeUsersCurrentDirectory("User2", "dir2");

            server.CreateDirectory("User1", "dir11");
            server.CreateDirectory("User2", "dir22");

            var drive = server.GetDriveStructure("c:");

            var dir11 = drive
                .Directories.First(d => d.Name == "dir1")
                .Directories.First(d => d.Name == "dir11");

            var dir22 = drive
                .Directories.First(d => d.Name == "dir2")
                .Directories.First(d => d.Name == "dir22");
        }

        /// <summary>
        /// Позитивное тестирование удаления директории.
        /// </summary>
        [TestMethod]
        public void RemoveDirPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateFile("User1", "c:\\dir1\\1.txt");
            server.RemoveDirectory("User1", "c:\\dir1", false);

            var drive = server.GetDriveStructure("c:");
            Assert.AreEqual(0, drive.Directories.Count);
        }

        /// <summary>
        /// Негативное тестирование удаления директории.
        /// </summary>
        [TestMethod]
        public void RemoveDirNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "c:\\dIr1\\dir2");
            server.ChangeUsersCurrentDirectory("User1", "dir1\\dir2");
            
            try
            {
                server.RemoveDirectory("User1", "c:\\DIR1\\dir2", false);
                throw new InvalidOperationException(
                    "Пользователь не должен был удалить текущую директорию.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidOperationException);
            }

            server.ChangeUsersCurrentDirectory("User1", "c:");
            
            try
            {
                server.RemoveDirectory("User1", "dir1", false);
                throw new InvalidOperationException(
                    "Пользователь не должен был удалить директорию, в которой есть поддиректории.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidOperationException);
            }
        }

        /// <summary>
        /// Позитивное тестирование удаления директории с подпапками.
        /// </summary>
        [TestMethod]
        public void DeltreePositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "c:\\dIr1\\dir2");
            server.RemoveDirectory("User1", "dir1", true);

            var drive = server.GetDriveStructure("C:");

            Assert.AreEqual(0, drive.Directories.Count);
        }

        /// <summary>
        /// Позитивное тестирование создания файла.
        /// </summary>
        [TestMethod]
        public void MakeFilePositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "test");
            server.CreateFile("User1", "c:\\test\\1.txt");
            server.CreateFile("User1", "test\\2.txt");
            server.CreateFile("User1", "3.txt");

            var drive = server.GetDriveStructure("c:");

            var file1 = drive
                .Directories.First(d => d.Name == "test")
                .Files.First(f => f.Name == "1.txt");

            var file2 = drive
                .Directories.First(d => d.Name == "test")
                .Files.First(f => f.Name == "2.txt");

            var file3 = drive
                .Files.First(f => f.Name == "3.txt");
        }

        /// <summary>
        /// Негативное тестирование создания файла.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void MakeFileNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.CreateFile("User1", "1.txt");
        }

        /// <summary>
        /// Позитивное тестирование удаления файла.
        /// </summary>
        [TestMethod]
        public void DeleteFilePositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.RemoveFile("User1", "1.txt");

            var drive = server.GetDriveStructure("c:");
            Assert.AreEqual(0, drive.Directories.Count);
        }

        /// <summary>
        /// Негативное тестирование удаления файла.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(FileNotFoundException))]
        public void DeleteFileNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.RemoveFile("User1", "1.txt");
        }

        /// <summary>
        /// Позитивное тестирование блокировки файла.
        /// </summary>
        [TestMethod]
        public void LockFilePositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.ConnectUser("User2");
            server.LockFile("User2", "1.txt");
            server.LockFile("User1", "1.txt");

            var drive = server.GetDriveStructure("c:");

            Assert.IsTrue(drive.Files.First().LockingUsers.Contains("User1"));
            Assert.IsTrue(drive.Files.First().LockingUsers.Contains("User2"));

            try
            {
                server.RemoveFile("User1", "1.txt");
                throw new InvalidOperationException(
                    "Заблокированный файл не должен был удалиться");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidOperationException);
            }
        }

        /// <summary>
        /// Удаление директории с заблокированными файлами.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void RemoveDirWithLockingFiles()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateFile("User1", "dir1\\1.txt");
            server.LockFile("User1", "dir1\\1.txt");
            server.RemoveDirectory("User1", "dir1", false);
        }

        /// <summary>
        /// Перемещение директории с заблокированнными файлами.
        /// </summary>
        [TestMethod]
        public void MoveDirWithLockingFiles()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "dir1\\dir2");
            server.CreateFile("User1", "dir1\\dir2\\1.txt");
            server.LockFile("User1", "dir1\\dir2\\1.txt");
            server.CreateDirectory("User1", "destination");

            try
            {
                server.Move("User1", "dir1", "destination");
                throw new InvalidOperationException(
                    "Директория с заблокированными файлами не должна была переместиться.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidOperationException);

                var drive = server.GetDriveStructure("C:");
                Assert.IsTrue(drive.Directories.Any(d => d.Name == "dir1"));
            }

            server.UnlockFile("User1", "dir1\\dir2\\1.txt");
            server.Move("User1", "dir1", "destination");
        }

        /// <summary>
        /// Перемещение заблокированнного файла.
        /// </summary>
        [TestMethod]
        public void MoveLockingFile()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.LockFile("User1", "1.txt");
            server.CreateDirectory("User1", "destination");

            try
            {
                server.Move("User1", "1.txt", "destination");
                throw new InvalidOperationException(
                    "Заблокированный файл не должен был переместиться.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidOperationException);
            }

            server.UnlockFile("User1", "1.txt");
            server.Move("User1", "1.txt", "destination");
        }

        /// <summary>
        /// Негативное тестирование блокировки файла.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(InvalidOperationException))]
        public void LockFileNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.LockFile("User1", "1.txt");
            server.LockFile("User1", "1.txt");
        }        

        /// <summary>
        /// Позитивное тестирование разблокировки файла.
        /// </summary>
        [TestMethod]
        public void UnlockFilePositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.ConnectUser("User2");
            server.LockFile("User2", "1.txt");
            server.LockFile("User1", "1.txt");

            server.UnlockFile("User1", "1.txt");
            server.UnlockFile("User2", "1.txt");
            server.RemoveFile("User1", "1.txt");

            var drive = server.GetDriveStructure("C:");
            Assert.AreEqual(0, drive.Files.Count);
        }

        /// <summary>
        /// Негативное тестирование разблокировки файла.
        /// </summary>
        [TestMethod]
        public void UnlockFileNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.ConnectUser("User2");
            server.LockFile("User2", "1.txt");
            server.LockFile("User1", "1.txt");
            server.UnlockFile("User1", "1.txt");

            var drive = server.GetDriveStructure("C:");

            Assert.IsTrue(drive.Files.First().LockingUsers.Contains("User2"));
            Assert.IsFalse(drive.Files.First().LockingUsers.Contains("User1"));
        }

        /// <summary>
        /// Блокировка файлов должна сниматься, когда 
        /// пользователь отключается от сервера.
        /// </summary>
        [TestMethod]
        public void UnlockFilesWhenUserDisconnects()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateFile("User1", "1.txt");
            server.LockFile("User1", "1.txt");
            server.DisconnectUser("User1");
            
            var drive = server.GetDriveStructure("C:");

            Assert.IsTrue(drive.Files.First(f => f.Name == "1.txt")
                .LockingUsers.Count == 0);
        }

        /// <summary>
        /// Позитивное тестирование копирования файла или папки.
        /// </summary>
        [TestMethod]
        public void CopyPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "source");
            server.CreateFile("User1", "1.txt");
            server.CreateFile("User1", "source\\2.txt");
            server.CreateDirectory("User1", "destination");

            server.Copy("User1", "1.txt", "destination");
            server.Copy("User1", "source", "destination");

            var drive = server.GetDriveStructure("C:");

            // всё скопировалось
            var newFile1 = drive.Directories.First(d => d.Name == "destination")
                .Files.First(f => f.Name == "1.txt");
            var newFile2 = drive.Directories.First(d => d.Name == "destination")
                .Directories.First(d => d.Name == "source")
                .Files.First(f => f.Name == "2.txt");

            // старые папки и файлы остались
            var oldFile1 = drive.Files.First(f => f.Name == "1.txt");
            var oldFile2 = drive.Directories.First(d => d.Name == "source")
                .Files.First(f => f.Name == "2.txt");
        }

        /// <summary>
        /// Destination не может указывать на имя файла.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(DirectoryNotFoundException))]
        public void CopyNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateFile("User1", "1.txt");
            server.Copy("User1", "dir1", "1.txt");
        }

        /// <summary>
        /// Позитивное тестирование перемещения файла или папки.
        /// </summary>
        [TestMethod]
        public void MovePositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "source");
            server.CreateFile("User1", "1.txt");
            server.CreateFile("User1", "source\\2.txt");
            server.CreateDirectory("User1", "destination");

            server.Move("User1", "1.txt", "destination");
            server.Move("User1", "source", "destination");

            var drive = server.GetDriveStructure("C:");

            // всё переместилось
            var newFile1 = drive.Directories.First(d => d.Name == "destination")
                .Files.First(f => f.Name == "1.txt");
            var newFile2 = drive.Directories.First(d => d.Name == "destination")
                .Directories.First(d => d.Name == "source")
                .Files.First(f => f.Name == "2.txt");

            // старые папки и файлы удалились
            Assert.AreEqual(null, drive.Files.FirstOrDefault(f => f.Name == "1.txt"));
            Assert.AreEqual(null, drive.Directories.FirstOrDefault(d => d.Name == "source"));
        }

        /// <summary>
        /// Негативное тестирование перемещения файла или папки.
        /// </summary>
        [TestMethod]
        [ExpectedException (typeof(DirectoryNotFoundException))]
        public void MoveNegative()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateFile("User1", "1.txt");
            server.Move("User1", "dir1", "1.txt");
        }

        /// <summary>
        /// Позитивное тестирование перемещения файла или папки.
        /// </summary>
        [TestMethod]
        public void DriveInfoPositive()
        {
            IVirtualFileServer server = this.CreateStandardServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "c:\\dir1\\dir2");
            server.CreateFile("User1", "1.txt");
            server.CreateFile("User1", "c:\\dir1\\dir2\\2.txt");

            var drive = server.GetDriveStructure("C:");

            var dir1 = drive.Directories.First(d => d.Name == "dir1");
            var dir2 = dir1.Directories.First(d => d.Name == "dir2");
            var file1 = drive.Files.First(f => f.Name == "1.txt");
            var file2 = dir2.Files.First(f => f.Name == "2.txt");
        }

        /// <summary>
        /// Создать экземпляр виртуального файлового сервера.
        /// </summary>
        /// <returns>Новый экземпляр виртуального файлового сервера.</returns>
        private IVirtualFileServer CreateStandardServer()
        {
            return new SyncronizedVirtualFileServer(
                new VirtualFileServer(),
                100);
        }
    }
}
