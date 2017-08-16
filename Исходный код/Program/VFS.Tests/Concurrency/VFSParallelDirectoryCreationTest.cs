namespace VFS.Tests.Concurrency
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Interfaces;
    using VFS.Server;
    using VFS.Tests.TestDoubles;

    /// <summary>
    /// Тестирование создания одной и той же папки из разных потоков.
    /// </summary>
    [TestClass]
    public class VFSParallelDirectoryCreationTest
    {
        /// <summary>
        /// Количество пользователей.
        /// </summary>
        private const int UsersCount = 10;

        /// <summary>
        /// Имитация сервера (для проверки созданных директорий).
        /// </summary>
        private VirtualFileServerSlowTestDouble serverTestDouble;
            
        /// <summary>
        /// Имитация сервера (для создания директорий).
        /// </summary>
        private IVirtualFileServer server;

        /// <summary>
        /// Список исключений.
        /// </summary>
        private readonly List<Exception> exceptions 
            = new List<Exception>();

        /// <summary>
        /// Объект синхронизации.
        /// </summary>
        private readonly object lockObject = new object();

        /// <summary>
        /// Создание одной и той же папки из разных потоков
        /// БЕЗ использования обертки-"синхронизатора".
        /// </summary>
        [TestMethod]
        public void CreateDirectoriesWithSameNameWithoutSyncroniser()
        {
            this.serverTestDouble = new VirtualFileServerSlowTestDouble(300);
            this.server = this.serverTestDouble;

            this.CreateDirectoriesParallel();

            Assert.AreNotEqual(
                VFSParallelDirectoryCreationTest.UsersCount - 1, 
                this.exceptions.Count);

            Assert.AreNotEqual(
                1,
                this.serverTestDouble.CreatedDirectories.Count);
        }

        /// <summary>
        /// Создание одной и той же папки из разных потоков
        /// с использованием обертки-"синхронизатора".
        /// </summary>
        [TestMethod]
        public void CreateDirectoriesWithSameNameWithSyncroniser()
        {
            this.serverTestDouble = new VirtualFileServerSlowTestDouble(200);
            this.server = new SyncronizedVirtualFileServer(
                this.serverTestDouble,
                100);

            this.CreateDirectoriesParallel();

            Assert.AreEqual(
                VFSParallelDirectoryCreationTest.UsersCount - 1,
                this.exceptions.Count);

            Assert.IsTrue(this.exceptions.All(ex => ex.Message == "Папка уже существует."));

            Assert.AreEqual(
                1,
                this.serverTestDouble.CreatedDirectories.Count);
        }

        /// <summary>
        /// Имитация создания всеми пользователями папки с одним именем.
        /// </summary>
        private void CreateDirectoriesParallel()
        {
            this.exceptions.Clear();

            List<Task> tasks = new List<Task>();

            for (int userNumber = 1;
                userNumber <= VFSParallelDirectoryCreationTest.UsersCount;
                userNumber++)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        this.server.CreateDirectory(null, "222");
                    }
                    catch (Exception ex)
                    {
                        lock (this.lockObject)
                        {
                            this.exceptions.Add(ex);
                        }
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
