﻿namespace VFS.Tests.Additional
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Класс тестирования оператора typeof().
    /// </summary>
    [TestClass]
    public class TypeOfOperatorTests
    {
        /// <summary>
        /// Тест оператора typeof().
        /// </summary>
        [TestMethod]
        public void TypeOfOperatorTest()
        {
            IA a = new A();
            Assert.IsTrue(a.GetType() == typeof(A));
            Assert.IsTrue(a is A);

            // ReSharper disable once IsExpressionAlwaysTrue
            // Тест - для учебных целей, чтобы как раз показать, что 
            // это условие всегда верное.
            Assert.IsTrue(a is IA);
        }

        /// <summary>
        /// Интерфейс.
        /// </summary>
        private interface IA { }

        /// <summary>
        /// Реализация.
        /// </summary>
        private class A : IA { }
    }
}
