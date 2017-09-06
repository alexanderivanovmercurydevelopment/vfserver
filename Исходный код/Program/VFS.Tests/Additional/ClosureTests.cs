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
        private readonly List<int> numbers =
            new List<int>();

        /// <summary>
        /// Проверка, что не возникает замыкания.
        /// </summary>
        [TestMethod]
        public void ClosureTest()
        {
            var tasks = new List<Task>();

            var startNumber = 1;
            var tasksNumber = 20;

            for (int i = startNumber; i <= tasksNumber; i++)
            {
                int iTemp = i;
                tasks.Add(
                    Task.Factory.StartNew(() => this.ParallelOperation(iTemp)));
            }

            Task.WaitAll(tasks.ToArray());

            for (int i = startNumber; i <= tasksNumber; i++)
                lock (this.numbers)
                {
                    Assert.IsTrue(this.numbers.Contains(i));
                }
        }

        /// <summary>
        /// Переместить папку или файл в другую папку.
        /// </summary>
        private int ParallelOperation(
            int number)
        {
            int num = this.PerformFunction(() => this.GetSameNumber(number));

            lock (this.numbers)
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