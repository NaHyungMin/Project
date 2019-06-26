using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerLib.TcpAsyncNetwork
{
    internal sealed class TCPAsyncListener
    {
        public Socket RunSocket {  get { return socket; } }

        //비동기 정보를 받기 위한 EventArgs 대기자
        private SocketAsyncEventArgs socketAsyncEventArgs;

        //클라이언트 접속 처리 소켓
        private Socket socket;

        // Accept처리의 순서를 제어하기 위한 이벤트 변수
        private AutoResetEvent autoResetEvent;

        private Thread threadForWork;

        //클라이언트가 접속 했을 때 호출되는 Callback
        public delegate void NewClientHandler(Socket socket, object token);
        public event NewClientHandler NewClientCallback;

        bool isAlive = true;

        public TCPAsyncListener()
        {
            NewClientCallback = null;
        }

        public void Start(string listenIP, Int16 listenPort, Int32 backLog, AddressFamily addressFamily)
        {
            switch (addressFamily)
            {
                case AddressFamily.InterNetwork:
                case AddressFamily.InterNetworkV6:
                    StartIPv4(listenIP, listenPort, backLog, addressFamily);
                    break;
                default:
                    new Exception("not support addressFamily");
                    break;
            }
        }

        private void StartIPv4(string listenIP, Int16 listenPort, Int32 backLog, AddressFamily addressFamily)
        {
            socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipAddress = IPAddress.Any;

            if (listenIP.Length > 0)
                ipAddress = IPAddress.Parse(listenIP);

            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, listenPort);

            try
            {
                //host 정보 바인드
                socket.Bind(ipEndPoint);

                //연결을 기다리는 클라이언트 수
                socket.Listen(backLog);

                socketAsyncEventArgs = new SocketAsyncEventArgs();
                //연결 완료되고 난 뒤 처리
                socketAsyncEventArgs.Completed += SocketAsyncEventArgs_Completed;

                //연결 대기
                threadForWork = new Thread(Recieve);
                threadForWork.Start();

                //소켓 연결 이벤트 등록
                //socket.AcceptAsync(socketAsyncEventArgs);
                
                //Recieve();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("StartIPv4 : {0}", ex.Message));
            }
        }

        private void Recieve()
        {
            autoResetEvent = new AutoResetEvent(false);

            while (isAlive)
            {
                // socketAsyncEventArgs 재사용 하기 위해 null로 변경
                socketAsyncEventArgs.AcceptSocket = null;

                bool isPending = true;

                try
                {
                    // 비동기 accept를 호출하여 클라이언트의 접속을 받아들입니다.  
                    // 비동기 매소드 이지만 동기적으로 수행이 완료될 경우도 있으니  
                    // 리턴값을 확인하여 분기시켜야 합니다.  
                    isPending = socket.AcceptAsync(socketAsyncEventArgs);
                }
                catch (Exception ex)
                {
                    continue;
                }

                if (!isPending)
                {
                    SocketAsyncEventArgs_Completed(null, socketAsyncEventArgs);
                }

                autoResetEvent.WaitOne();
            }
        }

        private void SocketAsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                // 소켓을 로컬변수에 저장
                Socket socket = e.AcceptSocket;
                object userToken = e.UserToken;
                
                NewClientCallback?.Invoke(socket, userToken);
            }
            else
            {
                Console.WriteLine("Fialed to accept client");
            }

             //다음 연결 대기
                autoResetEvent.Set();
        }

        ~TCPAsyncListener()
        {
            isAlive = false;
        }
    }
}
