namespace VFS.Utilities.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.Utilities;

    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void ServiceLocator_SharedInstanceReallyShared()
        {
            var sharedInstance = new ForServiceLocatorTests1();

            ServiceLocator.Instance.RegisterService(sharedInstance);

            object first = ServiceLocator.Instance.GetService<ForServiceLocatorTests1>();
            object second = ServiceLocator.Instance.GetService<ForServiceLocatorTests1>();
            
            Assert.AreEqual(first, second);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ServiceLocator_RegisteringNullSharedInstanceIsForbidden()
        {
            ServiceLocator.Instance.RegisterService<ForServiceLocatorTests2>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ServiceLocator_CantGetNotRegisteredInstance()
        {
            ServiceLocator.Instance.GetService<ForServiceLocatorTests3>();
        }

        /// <summary>
        /// Специальные классы, по одному на каждый тестовый метод, чтобы
        /// тестовые методы не влияли друг на друга и на уже зарегистрированные
        /// в ServiceLocator'е сервисы.
        /// </summary>
        private class ForServiceLocatorTests1 {}

        private class ForServiceLocatorTests2 { }

        private class ForServiceLocatorTests3 { }
    }
}
