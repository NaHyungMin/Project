using System;
using MessagePack;
using NetworkPacket.ClientServerPacket.Enum;
using ShareLib.TcpNetwork.Interface;

namespace NetworkPacket.ClientServerPacket.ToClient
{
    [MessagePackObject]
    public class ConnectionResponse : IClientPacket
    {
        [Key(0)]
        public Int32 ResultNumber { get; set; }

        [Key(1)]
        public Int32 Test { get; set; }

        [Key(2)]
        public string Message { get; set; }

        [IgnoreMember]
        public Int32 PacketId { get { return Convert.ToInt32(ToClientPacketID.CONNECTION_RESPONSE); } }
    }
}