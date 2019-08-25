using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Interface;
using ShareLib.TcpNetwork.Packet;

namespace ShareLib.TcpNetwork.Client
{
    public class ClientPeer : IPeer
    {
        private readonly ClientRequester clientRequester;
        private readonly UserToken token;

        public ClientPeer() { }

        public ClientPeer(ClientRequester clientRequester,  UserToken token)
        {
            this.clientRequester = clientRequester;
            this.token = token;
            this.token.Peer = this;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
        
        public void OnMessage(PacketHeader packetHeader, byte[] packetMessage)
        {
            //token
            IClientPacket responsePacket = MessagePack.MessagePackSerializer.Typeless.Deserialize(packetMessage) as IClientPacket; //size 119
            clientRequester.Response(responsePacket);

            //throw new NotImplementedException();
        }

        public void OnRemoved()
        {
            throw new NotImplementedException();
        }

        public void Send(IRequestItem requestItem)
        {
            if (token == null)
                throw new NullReferenceException();

            this.token.Send(requestItem);
        }
    }
}
