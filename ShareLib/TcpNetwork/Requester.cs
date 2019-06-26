using System;
using ShareLib.TcpNetwork.Interface;
using ShareLib.TcpNetwork.Packet;

namespace ShareLib.TcpNetwork
{
    public class Requester
    {
        public Int32 LogicVersion { get; private set; }
        public Int32 ContentsVersion { get; private set; }
        public byte PacketEncriptType { get; private set; }
        public byte PacketEncriptKey { get; private set; }

        public Requester(Int32 logicVersion, Int32 contentsVersion)
        {
            LogicVersion = logicVersion;
            ContentsVersion = contentsVersion;
        }

        public IRequestItem Request<RequestPacket>(RequestPacket requestPacket)
            where RequestPacket : IServerPacket
        {
            byte[] bytes = MessagePack.MessagePackSerializer.Typeless.Serialize(requestPacket);

            PacketHeader packetHeader = new PacketHeader
            {
                ContentsVersion = ContentsVersion,
                LogicVersion = LogicVersion,
                PacketID = requestPacket.PacketId,
                PacketSize = bytes.Length,
                EncriptKey = 0,
                EncriptType = 0,
            };

            IRequestItem requestItem = new PacketHolder(PacketHeaderConverter.CreateHeaderToBytes(packetHeader), requestPacket.PacketId, bytes);
            return requestItem;
        }
    }
}
