using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Interface;

namespace ShareLib.TcpNetwork.Client
{
    public class ClientRequester
    {
        //private static List<IPeer> gameServers = new List<IPeer>();
        public delegate void ResponseCallback<RequestPacket, ResponsePacket>(RequestPacket requestPacket, ResponsePacket responsePacket);

        private object lockObejct = new object();
        private IPeer server;
        private Requester requester;
        private ConcurrentQueue<RequestHolder> concurrentQueue = new ConcurrentQueue<RequestHolder>();
         
        public ClientRequester(string ip, Int16 port, Int32 logicVersion, Int32 contentsVersion)
        {
            ClientNetworkService networkService = new ClientNetworkService(2048, 8192);
            Connector connector = new Connector(networkService)
            {
                Connected = OnConnectedGameserver
            };

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));
            connector.Connect(endpoint);
            requester = new Requester(logicVersion, contentsVersion);
        }

        private bool OnConnectedGameserver(UserToken serverToken)
        {
            lock (lockObejct)
            {
                server = new ClientPeer(serverToken);
                serverToken.OnConnected();

                Console.WriteLine("Connected!");

                return true;
            }
        }  
        
        public bool Request<RequestPacket, ResponsePacket>(RequestPacket requestPacket, ResponseCallback<RequestPacket, ResponsePacket> responseCallback)
            where RequestPacket : IServerPacket
            where ResponsePacket : class, IClientPacket, new()
        {
            IRequestItem requestItem = requester.Request<RequestPacket>(requestPacket);
            server.Send(requestItem);

            RequestHolder requestHolder = new RequestHolder();
            requestHolder.Add<RequestPacket, ResponsePacket>(requestPacket, responseCallback);
            concurrentQueue.Enqueue(requestHolder);

            //responseCallback();
            //responseCallback(requestPacket, new ResponsePacket());

            return true;
        }

        public void Response()
        {

        }

        private class RequestHolder
        {
            internal void Add<RequestPacket, ResponsePacket>(RequestPacket requestPacket, ResponseCallback<RequestPacket, ResponsePacket> responseCallback)
                where RequestPacket : IServerPacket
                where ResponsePacket : class, IClientPacket, new()
            {
                //throw new NotImplementedException();
            }

            public void Call<RequestPacket, ResponsePacket>(ResponseCallback<RequestPacket, ResponsePacket> responseCallback)
                where RequestPacket : IServerPacket
                where ResponsePacket : class, IClientPacket, new()
            {
                //foreach(ResponseCallback < ResponsePacket> call in )
            }
        }
    }
}