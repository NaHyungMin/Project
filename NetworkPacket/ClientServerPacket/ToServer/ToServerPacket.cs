using System;
using MessagePack;
using NetworkPacket.ClientServerPacket.Enum;
using ShareLib.TcpNetwork.Interface;

namespace NetworkPacket.ClientServerPacket.ToServer
{
    [MessagePackObject(true)]
    public class ConnectionRequest : IServerPacket
    {
        public Int32 Test { get; set; }
        
        public string Message { get; set; }
        
        public Int32 PacketId { get { return Convert.ToInt32(ToServerPacketID.CONNECTION_REQUEST); } }
    }
}