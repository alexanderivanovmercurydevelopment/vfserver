namespace VFS.Server.Tests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Server;

    /// <summary>
    /// Пользователь не должен удалять свою текущую директорию.
    /// </summary>
    [TestClass]
    public class CanRemoveCurrentDirectory
    {
        /// <summary>
        /// Удостовериться, что пользователь не может удалять
        /// текущую директорию.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserCantRemoveCurrentDir()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.ChangeUsersCurrentDirectory("User1", "dir1");
            server.RemoveDirectory("User1", "c:\\dir1", false);
        }

        /// <summary>
        /// Удостовериться, что пользователь не может удалять
        /// директорию, родительскую по отношению к текущей.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserCantRemoveParentOfCurrentDir()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "dir1\\dir2");
            server.ChangeUsersCurrentDirectory("User1", "dir1\\dir2");
            server.RemoveDirectory("User1", "c:\\dir1", false);
        }

        /// <summary>
        /// Удостовериться, что пользователь не может переместить
        /// текущую директорию.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserCantMoveCurrentDir()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.ChangeUsersCurrentDirectory("User1", "dir1");

            server.CreateDirectory("User1", "C:\\dir2");
            server.Move("User1", "c:\\dir1", "C:\\dir2");
        }

        /// <summary>
        /// Удостовериться, что пользователь не может переместить
        /// директорию, родительскую по отношению к текущей.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserCantMoveParentOfCurrentDir()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "dir1\\dir2");
            server.ChangeUsersCurrentDirectory("User1", "dir1\\dir2");

            server.CreateDirectory("User1", "C:\\dir3");
            server.Move("User1", "c:\\dir1", "C:\\dir3");
        }

        /// <summary>
        /// Если рабочая папка пользователя была удалена,
        /// пользователь должен получить исключение.
        /// </summary>
        [TestMethod]
        public void CurrentDirWasRemoved()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "dir1\\dir2");
            server.ChangeUsersCurrentDirectory("User1", "dir1\\dir2");

            server.ConnectUser("User2");
            server.RemoveDirectory("User2", "dir1", true);

            try
            {
                server.CreateDirectory("User1", "dir3");
                throw new InvalidOperationException(
                    "Пользователь не должен был создать директорию в уже удаленной папке.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is DirectoryNotFoundException);
                Assert.IsTrue(ex.Message.Contains("Для корректной работы нужно изменить рабочую директорию"));
            }
        }
    }
}