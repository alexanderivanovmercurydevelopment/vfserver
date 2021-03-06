﻿namespace VFS.Tests.Common.Performance
{
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Тестирование производительности <see cref="StringBuilder" />
    /// в сравнении со <see cref="string" /> при поэтапном создании
    /// очень длинных строк.
    /// </summary>
    [TestClass]
    public class StringBuilderPerformanceTest
    {
        /// <summary>
        /// Тест построения длинной строки с помощью
        /// <see cref="StringBuilder" /> и с помощью
        /// "<see cref="string" /> +=".
        /// </summary>
        [TestMethod]
        public void StringBuilderVsStringPerformanceTest()
        {
            TimeSpan stringBuilderTime = this.GetStringBuilderTime();
            TimeSpan standardStringTime = this.GetStringTime();

            Assert.IsTrue(
                standardStringTime - stringBuilderTime > TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Получить время создания большой строки с помощью
        /// <see cref="StringBuilder" />.
        /// </summary>
        private TimeSpan GetStringBuilderTime()
        {
            DateTime startTime = DateTime.Now;

            var builder = new StringBuilder();

            for (var i = 0; i <= 10000; i++)
                builder.Append("fsdfsdfsdfdsf");

            // ReSharper disable once UnusedVariable
            // для тестирования производительности StringBuilder.ToString() метода.
            string result = builder.ToString();

            TimeSpan resultTime = DateTime.Now - startTime;
            return resultTime;
        }

        /// <summary>
        /// Получить время создания большой строки с помощью
        /// "<see cref="String" /> +=".
        /// </summary>
        private TimeSpan GetStringTime()
        {
            DateTime startTime = DateTime.Now;

            // ReSharper disable once NotAccessedVariable
            // для тестирования производительности простой конкатенации.
            string result = string.Empty;

            for (var i = 0; i <= 10000; i++)
                result += "fsdfsdfsdfdsf";

            TimeSpan resultTime = DateTime.Now - startTime;
            return resultTime;
        }
    }
}