using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ShareLib.TcpNetwork.Client
{
    public class ClientNetworkService
    {
        public Int32 SendBufferBlockSize { get; private set; }
        public Int32 ReceBufferBlockSize { get; private set; }

        public ClientNetworkService(Int32 sendBufferBlockSize, Int32 receBufferBlockSize)
        {
            SendBufferBlockSize = sendBufferBlockSize;
            ReceBufferBlockSize = receBufferBlockSize;
        }

        public void OnConnectCompleted(Socket socket, UserToken token)
        {
            token.OnSesstionClosed += OnSesstionClosed;
            //this.usermanager.add(token);

            SocketAsyncEventArgs receive_event_arg = new SocketAsyncEventArgs();
            receive_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(Receive_Completed);
            receive_event_arg.UserToken = token;
            receive_event_arg.SetBuffer(new byte[ReceBufferBlockSize], 0, ReceBufferBlockSize);

            SocketAsyncEventArgs send_event_arg = new SocketAsyncEventArgs();
            send_event_arg.Completed += new EventHandler<SocketAsyncEventArgs>(Send_Completed);
            send_event_arg.UserToken = token;
            send_event_arg.SetBuffer(new byte[SendBufferBlockSize], 0, SendBufferBlockSize);

            BeginReceive(socket, receive_event_arg, send_event_arg);
        }

        private void BeginReceive(Socket socket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            // receive_args, send_args 아무곳에서나 꺼내와도 된다. 둘다 동일한 CUserToken을 물고 있다.  
            UserToken token = receiveArgs.UserToken as UserToken;
            token.SetEventArgs(receiveArgs, sendArgs);

            // 생성된 클라이언트 소켓을 보관해 놓고 통신할 때 사용한다.  
            token.Socket = socket;

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

        private void OnSesstionClosed(UserToken token)
        {
            token.SetEventArgs(null, null);
        }
    }
}
