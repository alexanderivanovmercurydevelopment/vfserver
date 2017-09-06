namespace VFS.WCFService
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.ServiceProcess;

    /// <summary>
    /// Инсталлятор службы.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        /// <summary>
        /// Создать инсталлятор службы.
        /// </summary>
        public ProjectInstaller()
        {
            this.serviceProcessInstaller1 = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            this.serviceInstaller1 = new ServiceInstaller
            {
                ServiceName = "VirtualFileServer",
                DisplayName = "Virtual File Server",
                Description = "Виртуальный файловый сервер (тестовое задание MERCURY DEVELOPMENT)",
                StartType = ServiceStartMode.Automatic
            };

            this.Installers.Add(this.serviceProcessInstaller1);
            this.Installers.Add(this.serviceInstaller1);
        }
    }
}