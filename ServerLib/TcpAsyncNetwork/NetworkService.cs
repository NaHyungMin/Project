using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork;

namespace ServerLib.TcpAsyncNetwork
{
    public class NetworkService
    {
        public Socket RunSocket { get { return listener.RunSocket; } }

        // 메시지 수신용 풀.  
        private SocketAsyncEventArgsPool receiveEventArgsPool;

        // 메시지 전송용 풀.
        private SocketAsyncEventArgsPool sendEventArgsPool;

        private BufferManager sendBufferManager;
        private BufferManager receBufferManager;

        private TCPAsyncListener listener;

        public delegate void SessionHandler(ref UserToken token);
        public event SessionHandler OnSessionEventHandler;

        #region Init Network

        public NetworkService() { }

        public NetworkService(Int32 maxConnections, Int32 sendBufferBlockSize, Int32 receBufferBlockSize)
        {
            receiveEventArgsPool = new SocketAsyncEventArgsPool(maxConnections);
            sendEventArgsPool = new SocketAsyncEventArgsPool(maxConnections);

            //buffer 생성, 프로토콜 전송, 받기에 따라 용량을 다르게 잡는다.
            sendBufferManager = new BufferManager(sendBufferBlockSize, maxConnections);
            receBufferManager = new BufferManager(receBufferBlockSize, maxConnections);

            SendSocket(maxConnections);
            ReceiveSocket(maxConnections);
        }

        private void SendSocket(Int32 maxConnections)
        {
            for (Int32 i = 0; i < maxConnections; i++)
            {
                // send
                SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                arg.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
                arg.UserToken = new UserToken();

                sendBufferManager.SetBuffer(arg);

                sendEventArgsPool.Push(arg);
            }
        }

        private void ReceiveSocket(Int32 maxConnections)
        {
            for (Int32 i = 0; i < maxConnections; i++)
            {
                // receive
                SocketAsyncEventArgs arg = new SocketAsyncEventArgs();
                arg.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
                arg.UserToken = new UserToken();

                receBufferManager.SetBuffer(arg);

                receiveEventArgsPool.Push(arg);
            }
        }

        #endregion
        
        public bool StartListening(string listenIP, Int16 listenPort, Int32 backLog)
        {
            listener = new TCPAsyncListener();
            listener.NewClientCallback += Listener_NewClientCallback;
            listener.Start(listenIP, listenPort, backLog, AddressFamily.InterNetwork);
            
            return true;
        }

        private void Listener_NewClientCallback(Socket clientSocket, object token)
        {
            // 풀에서 하나씩 가져온다.
            SocketAsyncEventArgs receiveSocketAsyncEventArgs = receiveEventArgsPool.Pop();
            SocketAsyncEventArgs sendSocketAsyncEventArgs = sendEventArgsPool.Pop();

            if (OnSessionEventHandler != null)
            {
                UserToken userToken = receiveSocketAsyncEventArgs.UserToken as UserToken;
                OnSessionEventHandler(ref userToken); 
            }

            // 클라이언트 데이터 수신 준비
            BeginReceive(clientSocket, receiveSocketAsyncEventArgs, sendSocketAsyncEventArgs);
        }

        private void BeginReceive(Socket socket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 된다. 둘다 동일한 CUserToken을 물고 있다.  
            UserToken token = receiveArgs.UserToken as UserToken;
            token.SetEventArgs(receiveArgs, sendArgs);

            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용한다.  
            token.Socket = socket;
            token.OnConnected();

            // 데이터를 받을 수 있도록 소켓 매소드를 호출해준다.  
            // 비동기로 수신할 경우 워커 스레드에서 대기중으로 있다가 Completed에 설정해놓은 매소드가 호출된다.  
            // 동기로 완료될 경우에는 직접 완료 매소드를 호출해줘야 한다.  
            bool pending = socket.ReceiveAsync(receiveArgs);
            
            if (!pending)
            {
                ProcessReceive(receiveArgs);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            // check if the remote host closed the connection  
            UserToken token = e.UserToken as UserToken;

            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // 이후의 작업은 CUserToken에 맡긴다.  
                token.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);

                // 다음 메시지 수신을 위해서 다시 ReceiveAsync매소드를 호출한다.  
                bool pending = token.Socket.ReceiveAsync(e);

                if (!pending)
                {
                    ProcessReceive(e);
                }
            }
            else
            {
                token.Close();
                Console.WriteLine(string.Format("error {0},  transferred {1}", e.SocketError, e.BytesTransferred));
                //close_clientsocket(token);
            }
        }

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(e);
                return;
            }
        }

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            UserToken userToken = e.UserToken as UserToken;
            userToken.SendProcess(e);
        }
    }
}
