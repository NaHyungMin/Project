using System;
using System.Collections.Concurrent;
using System.Net;
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
                server = new ClientPeer(this, serverToken);
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

        public void Response<ResponsePacket>(ResponsePacket responsePacket)
            where ResponsePacket : IClientPacket
        {
            //네트워크 환경으로 인해, 서버에서 뒤죽박죽 들어온다면? 결국 시리얼 넘버를 보내야 하나...
            //어차피 클라이언트도 똑같은 패킷을 두 개 받는다면 다 쓰진 않을것이다.
            //그럼 하나만 등록시켜놓고 데이터가 오지 않으면 정해진 횟수만큼 서버에 보내고, 데이터가 돌아오면 다 삭제를 해버릴까?
            concurrentQueue.
            //concurrentQueue
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