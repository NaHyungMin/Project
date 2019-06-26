using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Packet;

namespace ShareLib.TcpNetwork.Interface
{
    public interface IPeer
    {
        //void OnMessage(byte[] buffer);
        void OnMessage(PacketHeader packetHeader, byte[] packetMessage);
        void OnRemoved();
        void Disconnect();
        void Send(IRequestItem requestItem);
    }
}