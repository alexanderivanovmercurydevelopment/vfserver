namespace VFS.WCFService
{
    using System.ComponentModel;
    using System.ServiceProcess;

    /// <summary>
    /// Инсталлятор службы.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Создать инсталлятор службы.
        /// </summary>
        public ProjectInstaller()
        {
            serviceProcessInstaller1 = new ServiceProcessInstaller();
            serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            serviceInstaller1 = new ServiceInstaller();
            serviceInstaller1.ServiceName = "VirtualFileServer";
            serviceInstaller1.DisplayName = "Virtual File Server";
            serviceInstaller1.Description = "Виртуальный файловый сервер (тестовое задание MERCURY DEVELOPMENT)";
            serviceInstaller1.StartType = ServiceStartMode.Automatic;
            Installers.Add(serviceProcessInstaller1);
            Installers.Add(serviceInstaller1);
        }
    }
}
