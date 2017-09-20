namespace VFS.Tests.Performance
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Interfaces;
    using VFS.Tests.TestDoubles;

    /// <summary>
    /// Тестирование производительности виртуального файлового сервера.
    /// </summary>
    [TestClass]
    public class VirtualFileServerPerformanceTests
    {
        private readonly PerformanceCharacteristics config
            = new PerformanceCharacteristics
            {
                // характеристики заменены на меньшие для более быстрого прогона всех тестов
                UsersCount = 1,
                ServerOperationTimeInMillisec = 200,
                IntervalBetweenUserRequestsInMillisec = 10,
                MaxParallelQueries = 100,
                RequestsPerUser = 1

                // УСПЕШНО ТЕСТИРОВАЛСЯ С ЭТИМИ ХАРАКТЕРИСТИКАМИ:
                //UsersCount = 100,
                //ServerOperationTimeInMillisec = 200,
                //IntervalBetweenUserRequestsInMillisec = 10,
                //MaxParallelQueries = 100,
                //RequestsPerUser = 10
            };

        private readonly List<Exception> exceptionsOccurred = new List<Exception>();

        /// <summary>
        /// Имитация сервера.
        /// </summary>
        private readonly IVirtualFileServer server;

        private readonly object syncObject = new object();

        private int notificationsCount;

        private int successRequestsCount;

        /// <summary>
        /// Подготовка экземпляра класса тестирования
        /// производительности виртуального файлового сервера.
        /// </summary>
        public VirtualFileServerPerformanceTests()
        {
            this.server = new VirtualFileServerSlowTestDouble();
        }

        /// <summary>
        /// Тестирование производительности виртуального файлового сервера.
        /// </summary>
        [TestMethod]
        public void VirtualFileServerPerformanceTest()
        {
            this.exceptionsOccurred.Clear();

            for (var i = 1;
                i <= this.config.UsersCount;
                i++)
                this.server.OperationPerformed += (sender, args) =>
                {
                    lock (this.syncObject)
                    {
                        this.notificationsCount++;
                    }
                };

            var tasks = new List<Task>();

            for (var userNumber = 1;
                userNumber <= this.config.UsersCount;
                userNumber++)
            {
                Task task = Task.Factory.StartNew(this.SingleUserOperations);

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(0, this.exceptionsOccurred.Count);
            Assert.AreEqual(
                this.config.UsersCount * this.config.RequestsPerUser,
                this.successRequestsCount);

            Assert.AreEqual(
                this.config.UsersCount * this.config.RequestsPerUser * this.config.UsersCount,
                this.notificationsCount);
        }

        /// <summary>
        /// Набор операций отдельного пользователя.
        /// </summary>
        private void SingleUserOperations()
        {
            try
            {
                for (var i = 1;
                    i <= this.config.RequestsPerUser;
                    i++)
                {
                    this.server.ChangeUsersCurrentDirectory(null, null);

                    Thread.Sleep(this.config.IntervalBetweenUserRequestsInMillisec);

                    lock (this.syncObject)
                    {
                        this.successRequestsCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                lock (this.syncObject)
                {
                    this.exceptionsOccurred.Add(ex);
                }
            }
        }
    }
}