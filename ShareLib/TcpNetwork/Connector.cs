using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Client;

namespace ShareLib.TcpNetwork
{
    public class Connector
    {
        public delegate bool ConnectedHandler(UserToken token);
        public ConnectedHandler Connected { get; set; }
        public ClientNetworkService ClientNetworkService { get; private set; }

        // 원격지 서버와의 연결을 위한 소켓.
        Socket client;

        public Connector(ClientNetworkService clientNetworkService)
        {
            ClientNetworkService = clientNetworkService;
            Connected = null;
        }

        public void Connect(IPEndPoint endpoint)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            // 비동기 접속을 위한 event args.
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.Completed += OnConnectCompleted;
            socketEventArg.RemoteEndPoint = endpoint;

            bool pending = client.ConnectAsync(socketEventArg);

            if (!pending)
            {
                OnConnectCompleted(null, socketEventArg);
            }
        }


        private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("Connect completd!");
                UserToken token = new UserToken();

                // 데이터 수신 준비.
                ClientNetworkService.OnConnectCompleted(client, token);

                Connected?.Invoke(token);
            }
            else
            {
                // failed.
                Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
            }
        }
    }
}
