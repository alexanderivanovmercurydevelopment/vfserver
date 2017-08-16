using SimpleWCFInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWindowsServiceWCFHost.WCFServiceRealization
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    class SimpleService : ISimpleService
    {
        private static DateTime? checkStaticVar = null;

        private readonly DateTime checkInstanceVar = DateTime.Now;

        private static List<Action<string>> notifyActions = 
            new List<Action<string>>();

        private static VirtualFileServerTestDouble virtualFileServer 
            = new VirtualFileServerTestDouble();

        public void SubscribeEvent()
        {
            IWCFEventHandler handler =
                OperationContext.Current.GetCallbackChannel<IWCFEventHandler>();

            SimpleService.notifyActions.Add(handler.HandleEvent);
        }

        public string SimpleOperation(string input)
        {
            try
            {
                //if (!File.Exists("C:\\log.txt"))
                //{
                //    File.Create("C:\\log.txt");
                //}

                //File.AppendAllText("C:\\log.txt", "Пользователь ввел " + input + "\n");

                if (SimpleService.checkStaticVar == null)
                {
                    SimpleService.checkStaticVar = DateTime.Now;
                }

                string result = "Вызов метода через службу-хостинг WCF!!!\n"
                    + string.Format("Вы ввели - {0}\n", input)
                    + string.Format("Статическая переменная - {0}\n", SimpleService.checkStaticVar.Value.ToLongTimeString())
                    + string.Format("Переменная экземпляра - {0}\n", this.checkInstanceVar.ToLongTimeString());

                SimpleService.virtualFileServer.LongOperation();

                string dateStr = DateTime.Now.ToLongTimeString();

                foreach (Action<string> act in SimpleService.notifyActions.ToArray())
                {
                    try
                    {
                        act("Пользователь вызвал операцию, " + DateTime.Now);
                    }
                    catch
                    {
                        SimpleService.notifyActions.Remove(act);
                    }
                }

                //File.AppendAllText("C:\\log.txt", "Пользователь, который ввел " + input + ", получает результат\n");
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
