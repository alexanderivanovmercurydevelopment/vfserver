﻿namespace VFS.Server.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Server;

    /// <summary>
    /// Тестирование перемещения папки или файла на диск C:
    /// </summary>
    [TestClass]
    public class MoveToDriveC
    {
        /// <summary>
        /// Тестирование перемещения файла на диск С:.
        /// </summary>
        [TestMethod]
        public void MoveFileToDriveC()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateFile("User1", "dir1\\1.txt");
            server.Move("User1", "dir1\\1.txt", "c:");
        }

        /// <summary>
        /// Тестирование перемещения папки на диск C:.
        /// </summary>
        [TestMethod]
        public void MoveDirToDriveC()
        {
            var server = new VirtualFileServer();
            server.ConnectUser("User1");
            server.CreateDirectory("User1", "dir1");
            server.CreateDirectory("User1", "dir1\\dir2");
            server.Move("User1", "dir1\\dir2", "c:");
        }
    }
}