using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWCFInterfaces
{
    public interface IWCFEventHandler
    {
        [OperationContract (IsOneWay=true)]
        void HandleEvent(string message);
    }
}
