namespace VFS.WCFService
{
    using System.ServiceProcess;

    /// <summary>
    /// Служба для работы с виртуальным файловым сервером через WCF.
    /// </summary>
    public partial class VFSService : ServiceBase
    {
        private readonly WCFHost wcfHost = new WCFHost();

        /// <summary>
        /// Создать экземпляр службы.
        /// </summary>
        public VFSService()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Запуск службы.
        /// </summary>
        /// <param name="args">Параметры команды "net start" ?</param>
        protected override void OnStart(string[] args)
        {
            this.wcfHost.Start();
        }

        /// <summary>
        /// Остановка службы.
        /// </summary>
        protected override void OnStop()
        {
            this.wcfHost.Stop();
        }
    }
}