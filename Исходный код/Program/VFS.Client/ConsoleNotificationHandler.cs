namespace VFS.Client
{
    using System;

    using VFS.Interfaces.Service;

    internal class ConsoleNotificationHandler : IVFSNotificationHandler
    {
        public void HandleNotification(string notification)
        {
            Console.WriteLine(notification);
        }
    }
}