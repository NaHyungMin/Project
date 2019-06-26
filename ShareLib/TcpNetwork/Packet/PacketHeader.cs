using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace ShareLib.TcpNetwork.Packet
{
    public sealed class PacketHeader
    {
        private Int32 packetId;
        private Int32 packetSize;
        private byte encriptType;
        private byte encriptKey;
        private Int32 logicVersion;
        private Int32 contentsVersion;

        //Id, encriptType, encriptKey, logicVersion, contentsVersion, Size
        public const Int32 HEADER_SIZE = (sizeof(Int32) + sizeof(byte) + sizeof(byte) + sizeof(Int32) + sizeof(Int32) + sizeof(Int32));

        public Int32 PacketID { get { return packetId; } set { packetId = value; } }
        public Int32 PacketSize { get { return packetSize; } set { packetSize = value; } }
        public Int32 HeaderSize { get { return HEADER_SIZE; } }
        public byte EncriptType { get { return encriptType; } set { encriptType = value; } }
        public byte EncriptKey { get { return encriptKey; } set { encriptKey = value; } }
        public Int32 LogicVersion { get { return logicVersion; } set { logicVersion = value; } }
        public Int32 ContentsVersion { get { return contentsVersion; } set { contentsVersion = value; } }

        public PacketHeader Clone()
        {
            return (PacketHeader)MemberwiseClone();
        }

        public static Int32 GetCalculateDataFieldOffset(Int32 startOffset)
        {
            return startOffset + HEADER_SIZE;
        }

        public static Int32 GetCalculateDataFieldLength(Int32 length)
        {
            return length - HEADER_SIZE;
        }
    }
}
