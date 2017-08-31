namespace VFS.Tests.Concurrency
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.InMemoryVirtualDrive;
    using VFS.Utilities;

    /// <summary>
    /// Тестирование параллельной работы с виртуальным
    /// диском, хранящим данные в памяти.
    /// </summary>
    [TestClass]
    public class InMemoryVirtualDriveConcurrency
    {
        /// <summary>
        /// Количество пользователей.
        /// </summary>
        private const int UsersCount = 150;

        /// <summary>
        /// Количество создаваемых/удаляемых каждым пользователем директорий.
        /// </summary>
        private const int DirectoriesCount = 50;

        /// <summary>
        /// Виртуальный диск.
        /// </summary>
        InMemoryVirtualDrive drive;

        /// <summary>
        /// Список ошибок создания директорий.
        /// </summary>
        List<Exception> createErrors;

        /// <summary>
        /// Список ошибок удаления директорий.
        /// </summary>
        List<Exception> deleteErrors;

        /// <summary>
        /// Тестирование параллельного добавления директорий.
        /// </summary>
        [TestMethod]
        public void AddAndRemoveDirectoriesParallel()
        {
            this.Initialize();

            this.CreateDirectories();
            int expectedCreationErrorsCount =
                InMemoryVirtualDriveConcurrency.DirectoriesCount *
                (InMemoryVirtualDriveConcurrency.UsersCount - 1);
            Assert.AreEqual(
                this.createErrors.Count, 
                expectedCreationErrorsCount);
            Assert.AreEqual(
                this.drive.Directories.Count(), 
                InMemoryVirtualDriveConcurrency.DirectoriesCount);
            Assert.IsTrue(this.createErrors.All(err => err.Message.StartsWith("Папка")));

            this.RemoveDirectories();
            int expectedDeletionErrorsCount =
                expectedCreationErrorsCount;
            Assert.AreEqual(
                this.deleteErrors.Count, 
                expectedDeletionErrorsCount);
            Assert.AreEqual(this.drive.Directories.Count(), 0);
            Assert.IsTrue(this.deleteErrors.All(err => err.Message.StartsWith("В папке")));

            this.Dispose();
        }

        /// <summary>
        /// Создать и инициализировать стандартный экземпляр
        /// виртуального файлового диска, работающего с памятью.
        /// </summary>
        /// <returns>Готовый к работе виртуальный файловый диск.</returns>
        private static InMemoryVirtualDrive GetNewVirtualDrive()
        {
            InMemoryVirtualDrive result = new InMemoryVirtualDrive();

            string xmlConfig = AppResourceReader.GetResource(
                typeof(InMemoryVirtualDrive).Assembly,
                "VFS.InMemoryVirtualDrive.ConfigExample.xml");

            result.Initialize(xmlConfig, "C:");

            return result;
        }

        /// <summary>
        /// Начать работу.
        /// </summary>
        private void Initialize()
        {
            this.drive = InMemoryVirtualDriveConcurrency.GetNewVirtualDrive();
            this.createErrors = new List<Exception>();
            this.deleteErrors = new List<Exception>();
        }

        /// <summary>
        /// Завершить работу.
        /// </summary>
        private void Dispose()
        {
            this.drive?.Dispose();

            this.drive = null;
            this.createErrors = null;
            this.deleteErrors = null;
        }

        /// <summary>
        /// Одновременное создание директорий.
        /// </summary>
        private void CreateDirectories()
        {
            List<Task> tasks = new List<Task>();

            for (int taskNumber = 1; 
                taskNumber <= InMemoryVirtualDriveConcurrency.UsersCount; 
                taskNumber++)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    for (int dirName = 1; 
                        dirName <= InMemoryVirtualDriveConcurrency.DirectoriesCount; 
                        dirName++)
                    {
                        try
                        {
                            this.drive.CreateDirectory(dirName.ToString());
                        }
                        catch (Exception ex)
                        {
                            Assert.IsTrue(ex.Message.StartsWith("Папка"));
                            this.createErrors.Add(ex);
                        }
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Одновременное удаление директорий.
        /// </summary>
        private void RemoveDirectories()
        {
            List<Task> tasks = new List<Task>();

            for (int taskNumber = 1; 
                taskNumber <= InMemoryVirtualDriveConcurrency.UsersCount; 
                taskNumber++)
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    for (int dirName = 1;
                        dirName <= InMemoryVirtualDriveConcurrency.DirectoriesCount;
                        dirName++)
                    {
                        try
                        {
                            this.drive.RemoveDirectory(dirName.ToString(), false);
                        }
                        catch (Exception ex)
                        {
                            Assert.IsTrue(ex.Message.StartsWith("В папке"));
                            this.deleteErrors.Add(ex);
                        }
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
