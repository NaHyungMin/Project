using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NetworkPacket.ClientServerPacket;
using NetworkPacket.ClientServerPacket.ToClient;
using NetworkPacket.ClientServerPacket.ToServer;
using ShareLib.TcpNetwork;
using ShareLib.TcpNetwork.Client;
using ShareLib.TcpNetwork.Interface;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.Tcp);

            //IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(7979));
            //s.Connect(endpoint);

            ConnectTest();
        }

        private static void ConnectTest()
        {
            ClientRequester clientRequester = new ClientRequester("127.0.0.1", Config.listenPort, 1, 1);

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();

                if (line == "q")
                {
                    break;
                }

                ConnectionRequest connectionRequest = new ConnectionRequest
                {
                    Message = line,
                    Test = 0
                };

                clientRequester.Request<ConnectionRequest, ConnectionResponse>(connectionRequest, Response);

                //TestRequest testRequest = new TestRequest
                //{
                //    Message = "안녕하세요"
                //};

                //clientRequester.Request<TestRequest, TestResponse>(testRequest, Response2);
            }
        }

        private static void Response(ConnectionRequest request, ConnectionResponse response)
        {
            Console.WriteLine(response.Message);
        }
    }
    
    //실제 서버에서는 Config/Message 형식으로 받아서 처리해야 한다.
    public class Config
    {
        public const string listenIP = "0.0.0.0";
        public const Int16 listenPort = 7979;
        public const Int32 backLog = 100;

        public const Int32 maxConnections = 2000;
        public const Int32 sendBufferBlockSize = 2048;
        public const Int32 receBufferBlockSize = 8192;
    }
}