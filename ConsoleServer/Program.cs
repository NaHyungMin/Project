using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer.ConnetionUser;
using GameServer.Packet;

namespace ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessManager processManager = new ProcessManager();
            TcpAsyncNetworkManager tcpAsyncNetworkManager = new TcpAsyncNetworkManager(Config.listenIP, Config.listenPort, Config.backLog, Config.maxConnections, Config.sendBufferBlockSize, Config.receBufferBlockSize);
            tcpAsyncNetworkManager.StartServer();
           
            Console.WriteLine("Server Start!");

            while(true)
            {
                string input = Console.ReadLine();

                if (input == "users")
                    Console.WriteLine("user count : {0}", tcpAsyncNetworkManager.UserCount());

                if(input == "check")
                    Console.WriteLine("socket Check : {0}", tcpAsyncNetworkManager.RunSocket.Connected);

                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    //실제 서버에서는 Config/Message 형식으로 받아서 처리해야 한다.
    public class Config
    {
        public const string listenIP = "0.0.0.0";
        public const Int16 listenPort = 7979;
        public const Int32 backLog = 100;

        public const Int32 maxConnections = 20;
        public const Int32 sendBufferBlockSize = 2048;
        public const Int32 receBufferBlockSize = 8192;
    }
}
