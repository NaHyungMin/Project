using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ShareLib.TcpNetwork.Packet
{
    public class PacketHeaderConverter
    {
        public static PacketHeader BytesToHeader(byte[] bytes)
        {
            PacketHeader header = new PacketHeader();

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    header.PacketSize = BitConverter.ToInt32(reader.ReadBytes(sizeof(Int32)), 0);
                    header.PacketID = BitConverter.ToInt32(reader.ReadBytes(sizeof(Int32)), 0);
                    header.LogicVersion = BitConverter.ToInt32(reader.ReadBytes(sizeof(Int32)), 0);
                    header.ContentsVersion = BitConverter.ToInt32(reader.ReadBytes(sizeof(Int32)), 0);
                    header.EncriptKey = reader.ReadByte();
                    header.EncriptType = reader.ReadByte();
                }
            }

            return header;
        }

        public static byte[] CreateHeaderToBytes(PacketHeader header)
        {
            byte[] bytes;

            using (MemoryStream stream = new MemoryStream(PacketHeader.HEADER_SIZE))
            {
                stream.Write(BitConverter.GetBytes(header.PacketSize), 0, sizeof(Int32));
                stream.Write(BitConverter.GetBytes(header.PacketID), 0, sizeof(Int32));
                stream.Write(BitConverter.GetBytes(header.LogicVersion), 0, sizeof(Int32));
                stream.Write(BitConverter.GetBytes(header.ContentsVersion), 0, sizeof(Int32));
                stream.Write(BitConverter.GetBytes(header.EncriptKey), 0, sizeof(byte));
                stream.Write(BitConverter.GetBytes(header.EncriptType), 0, sizeof(byte));
                
                bytes = stream.GetBuffer();
            }

            return bytes;
        }
    }
}
