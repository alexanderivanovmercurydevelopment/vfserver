namespace VFS.Tests.WCF
{
    using System.ServiceModel;

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
            var host = new WCFHost();

            try
            {
                host.Start();
            }
            // ReSharper disable once RedundantCatchClause Объяснение причины исключения.
            catch (AddressAlreadyInUseException)
            {
                // см. VFS.Tests.App.config - в нём должны быть указаны свободные порты, 
                // по-возможности, отличные от используемых в реальном приложении.
                // Данное исключение может вызываться, если порты заняты, или если порты
                // совпадают с теми, которые указаны в App.config службы Windows, и 
                // эта служба запущена.
                throw;
            }

            host.Stop();
        }
    }
}