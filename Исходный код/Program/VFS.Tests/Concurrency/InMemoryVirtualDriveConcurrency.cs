namespace VFS.Tests.Concurrency
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.InMemoryVirtualDrive;

    /// <summary>
    /// Тестирование параллельной работы с виртуальным
    /// диском, хранящим данные в памяти.
    /// </summary>
    [TestClass]
    public class InMemoryVirtualDriveConcurrency
    {
        private const int UsersCount = 150;

        /// <summary>
        /// Количество создаваемых/удаляемых каждым пользователем директорий.
        /// </summary>
        private const int DirectoriesCount = 50;

        private List<Exception> creationErrors;

        /// <summary>
        /// Список ошибок удаления директорий.
        /// </summary>
        private List<Exception> deletionErrors;

        private InMemoryVirtualDrive drive;

        /// <summary>
        /// Тестирование параллельного добавления директорий.
        /// </summary>
        [TestMethod]
        public void AddAndRemoveDirectoriesParallel()
        {
            this.Initialize();

            this.CreateDirectoriesInParallel();
            int expectedCreationErrorsCount =
                InMemoryVirtualDriveConcurrency.DirectoriesCount *
                (InMemoryVirtualDriveConcurrency.UsersCount - 1);
            Assert.AreEqual(
                this.creationErrors.Count,
                expectedCreationErrorsCount);
            Assert.AreEqual(
                this.drive.ChildDirectories.Count(),
                InMemoryVirtualDriveConcurrency.DirectoriesCount);
            Assert.IsTrue(this.creationErrors.All(err => err.Message.StartsWith("Папка")));

            this.RemoveDirectoriesInParallel();
            int expectedDeletionErrorsCount =
                expectedCreationErrorsCount;
            Assert.AreEqual(
                this.deletionErrors.Count,
                expectedDeletionErrorsCount);
            Assert.AreEqual(this.drive.ChildDirectories.Count(), 0);
            Assert.IsTrue(this.deletionErrors.All(err => err.Message.StartsWith("В папке")));

            this.Dispose();
        }

        /// <summary>
        /// Создать и инициализировать стандартный экземпляр
        /// виртуального файлового диска, работающего с памятью.
        /// </summary>
        /// <returns>Готовый к работе виртуальный файловый диск.</returns>
        private static InMemoryVirtualDrive GetNewVirtualDrive()
        {
            return new InMemoryVirtualDrive();
        }

        private void Initialize()
        {
            this.drive = InMemoryVirtualDriveConcurrency.GetNewVirtualDrive();
            this.creationErrors = new List<Exception>();
            this.deletionErrors = new List<Exception>();
        }

        private void Dispose()
        {
            this.drive?.Dispose();

            this.drive = null;
            this.creationErrors = null;
            this.deletionErrors = null;
        }

        private void CreateDirectoriesInParallel()
        {
            var tasks = new List<Task>();

            for (var taskNumber = 1;
                taskNumber <= InMemoryVirtualDriveConcurrency.UsersCount;
                taskNumber++)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    for (var dirName = 1;
                        dirName <= InMemoryVirtualDriveConcurrency.DirectoriesCount;
                        dirName++)
                        try
                        {
                            this.drive.CreateDirectory(dirName.ToString());
                        }
                        catch (Exception ex)
                        {
                            Assert.IsTrue(ex.Message.StartsWith("Папка"));
                            this.creationErrors.Add(ex);
                        }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void RemoveDirectoriesInParallel()
        {
            var tasks = new List<Task>();

            for (var taskNumber = 1;
                taskNumber <= InMemoryVirtualDriveConcurrency.UsersCount;
                taskNumber++)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    for (var dirName = 1;
                        dirName <= InMemoryVirtualDriveConcurrency.DirectoriesCount;
                        dirName++)
                        try
                        {
                            this.drive.RemoveDirectory(dirName.ToString(), false);
                        }
                        catch (Exception ex)
                        {
                            Assert.IsTrue(ex.Message.StartsWith("В папке"));
                            this.deletionErrors.Add(ex);
                        }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}