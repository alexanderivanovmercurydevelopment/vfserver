namespace VFS.Tests.Additional
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Проверка, что не возникает замыкание.
    /// </summary>
    [TestClass]
    public class ClosureTests
    {
        /// <summary>
        /// Список чисел.
        /// </summary>
        private readonly List<int> numbers = 
            new List<int>();

        /// <summary>
        /// Проверка, что не возникает замыкания.
        /// </summary>
        [TestMethod]
        public void ClosureTest()
        {
            List<Task> tasks = new List<Task>();

            int startNumber = 1;
            int tasksNumber = 20;

            for (int i = startNumber; i <= tasksNumber; i++)
            {
                int iTemp = i;
                tasks.Add(
                    Task.Factory.StartNew(() => this.ParallelOperation(iTemp)));
            }

            Task.WaitAll(tasks.ToArray());

            for (int i = startNumber; i <= tasksNumber; i++)
            {
                Assert.IsTrue(this.numbers.Contains(i));
            }
        }

        /// <summary>
        /// Переместить папку или файл в другую папку.
        /// </summary>
        /// <param name="userName">Имя подключенного пользователя.</param>
        /// <param name="sourcePath">Путь к перемещаемой папке или файлу.</param>
        /// <param name="destinationPath">Путь к целевой папке.</param>
        public int ParallelOperation(
            int number)
        {            
            int num = this.PerformFunction(() => { return this.GetSameNumber(number); });
            
            lock(this.numbers)
            {
                Assert.IsFalse(this.numbers.Contains(num));
                this.numbers.Add(num);
            }

            return num;
        }

        /// <summary>
        /// Функция выполнения другой функции.
        /// </summary>
        /// <param name="function">Другая функция. Для проверки замыкания
        /// должна передаваться как лямбда-выражение.</param>
        /// <returns>Результат выполнения переданной функции.</returns>
        private int PerformFunction(Func<int> function)
        {
            Thread.Sleep(100);
            return function();
        }

        /// <summary>
        /// Функция, возвращяющая то же число.
        /// </summary>
        /// <param name="number">Число.</param>
        /// <returns>То же число.</returns>
        private int GetSameNumber(int number)
        {
            return number;
        }
    }
}
