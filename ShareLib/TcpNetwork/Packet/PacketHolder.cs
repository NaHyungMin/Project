using System;
using ShareLib.TcpNetwork.Interface;

namespace ShareLib.TcpNetwork.Packet
{
    public class PacketHolder : IRequestItem
    {
        public byte[] RequestPacketHearder { get; }
        public Int32 PacketId { get; }
        public byte[] PacketBuffer { get; set; }
        
        public PacketHolder(byte[] headerBuffer, Int32 packetId, byte[] packetBuffer)
        {
            RequestPacketHearder = headerBuffer;
            PacketId = packetId;
            PacketBuffer = packetBuffer;
        }
    }
}