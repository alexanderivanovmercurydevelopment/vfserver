using SimpleWCFInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWCFServer.ServiceImplementation
{
    class SimpleService : ISimpleService
    {
        public string SimpleOperation(string input)
        {
            return "ЭТО ОТВЕТ СЕРВЕРА! Вы ввели: " + input;
        }
    }
}
