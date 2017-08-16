namespace VFS.Tests.WCF
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using VFS.WCFService;

    /// <summary>
    /// Тестирование создания WCF-хостинга.
    /// </summary>
    [TestClass]
    public class WCFHostCreation
    {
        /// <summary>
        /// Запуск и остановка WCF-хостинга.
        /// </summary>
        [TestMethod]
        public void CreateWCFHost()
        {
            string configPath =
                Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
                + Path.DirectorySeparatorChar
                + "config.txt";

            File.AppendAllText(configPath, "PortNumber = 8000");

            WCFHost host = new WCFHost();
            host.Start();
            host.Stop();

            File.Delete(configPath);
        }
    }
}
