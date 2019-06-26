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
        private UserToken token = new UserToken();

        public ClientPeer() { }

        public ClientPeer(UserToken token)
        {
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
            throw new NotImplementedException();
        }

        public void OnRemoved()
        {
            throw new NotImplementedException();
        }

        public void Send(IRequestItem requestItem)
        {
            token.Send(requestItem);
        }
    }
}
