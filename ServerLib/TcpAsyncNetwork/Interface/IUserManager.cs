using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork;

namespace ServerLib.TcpAsyncNetwork.Interface
{
    public interface IUserManager
    {
        void TcpAsyncNetworkManager_OnSessionEventHandle(object sender, UserToken token);
        Int32 UserCount();
    }
}
