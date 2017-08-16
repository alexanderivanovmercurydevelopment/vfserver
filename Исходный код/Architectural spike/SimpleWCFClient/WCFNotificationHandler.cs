using SimpleWCFInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWCFClient
{
    internal class WCFNotificationHandler : IWCFEventHandler
    {
        public void HandleEvent(string message)
        {
            Console.WriteLine("Пришло сообщение от сервера: " + message);
        }
    }
}
