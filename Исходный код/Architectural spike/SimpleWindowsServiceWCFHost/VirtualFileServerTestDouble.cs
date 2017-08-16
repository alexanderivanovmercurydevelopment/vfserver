using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWindowsServiceWCFHost
{
    internal class VirtualFileServerTestDouble
    {
        private int data = 0;

        internal string LongOperation()
        {
            data++;
            //File.AppendAllText("C:\\log.txt", 
            //    "Длительная операция была вызвана из потока " + Thread.CurrentThread.ManagedThreadId + "\n"
            //    + "Метод был вызван - " + this.data + " раз.\n");

            // реальная длительная операция. (вместо Thread.Sleep)
            //for (int i = 0; i < 3000; i++) // 20000
            //{
            //    for (int j = 0; j < 1000000; j++)
            //    {
            //        string s = "fdsfd";
            //    }
            //}

            Thread.Sleep(3000);

            return "fdfd";
        }
    }
}
