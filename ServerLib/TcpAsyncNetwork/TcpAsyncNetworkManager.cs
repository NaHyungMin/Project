using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLib.TcpNetwork;

namespace ServerLib.TcpAsyncNetwork
{
    /// <summary>
    /// 네트워크 통신이 여러개라면 이걸 가지고 TcpAsyncNetworkManager 여러개를 만들까 생각해야 한다.
    /// </summary>
    public class TcpAsyncNetwork
    {
        public System.Net.Sockets.Socket RunSocket { get { return networkService.RunSocket; } }
        protected string ListenIP { get; private set; }
        protected Int16 ListenPort { get; private set; }
        protected Int32 BackLog { get; private set; }
        protected Int32 MaxConnections { get; private set; }
        protected Int32 SendBufferBlockSize { get; private set; }
        protected Int32 ReceBufferBlockSize { get; private set; }

        public event EventHandler<UserToken> OnSessionEventHandle;
        public NetworkService networkService;
        
        public TcpAsyncNetwork(string listenIP
           , Int16 listenPort
           , Int32 backLog
           , Int32 maxConnections
           , Int32 sendBufferBlockSize
           , Int32 receBufferBlockSize)
        {
            ListenIP = listenIP;
            ListenPort = listenPort;
            BackLog = backLog;
            MaxConnections = maxConnections;
            SendBufferBlockSize = sendBufferBlockSize;
            ReceBufferBlockSize = receBufferBlockSize;
        }

        public bool StartServer()
        {
             networkService = new NetworkService(MaxConnections, SendBufferBlockSize, ReceBufferBlockSize);

            networkService.OnSessionEventHandler += NetworkService_OnSessionEventHandler;

            return networkService.StartListening(ListenIP, ListenPort, BackLog);
        }

        private void NetworkService_OnSessionEventHandler(UserToken token)
        {
            OnSessionEventHandle?.Invoke(this, token);
        }
    }
}
