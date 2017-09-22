namespace VFS.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Interfaces;

    /// <summary>
    /// Тестирование создания одной и той же папки из разных потоков.
    /// </summary>
    [TestClass]
    public class VFSParallelDirectoryCreationTest
    {
        private const int UsersCount = 10;

        private readonly List<Exception> exceptionsOccured
            = new List<Exception>();

        private readonly object lockObject = new object();

        /// <summary>
        /// Имитация сервера (для создания директорий).
        /// </summary>
        private IVirtualFileServer server;

        /// <summary>
        /// Имитация сервера (для проверки созданных директорий).
        /// </summary>
        private VirtualFileServerSlowTestDouble serverTestDouble;

        /// <summary>
        /// Создание одной и той же папки из разных потоков
        /// без синхронизации доступа.
        /// </summary>
        [TestMethod]
        public void CreateDirectoriesWithSameNameWithoutSyncroniser()
        {
            this.serverTestDouble = new VirtualFileServerSlowTestDouble();
            this.server = this.serverTestDouble;

            this.CreateDirectoriesParallel();

            Assert.AreNotEqual(
                VFSParallelDirectoryCreationTest.UsersCount - 1,
                this.exceptionsOccured.Count);

            Assert.AreNotEqual(
                1,
                this.serverTestDouble.CreatedDirectoriesNames.Count);
        }

        /// <summary>
        /// Имитация создания всеми пользователями папки с одним именем.
        /// </summary>
        private void CreateDirectoriesParallel()
        {
            this.exceptionsOccured.Clear();

            var tasks = new List<Task>();

            for (var userNumber = 1;
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
                            this.exceptionsOccured.Add(ex);
                        }
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}