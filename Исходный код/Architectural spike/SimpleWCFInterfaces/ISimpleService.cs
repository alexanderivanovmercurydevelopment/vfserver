using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWCFInterfaces
{
    [ServiceContract (CallbackContract=typeof(IWCFEventHandler))]
    public interface ISimpleService
    {
        [OperationContract]
        void SubscribeEvent();

        [OperationContract]
        string SimpleOperation(string input);
    }
}
