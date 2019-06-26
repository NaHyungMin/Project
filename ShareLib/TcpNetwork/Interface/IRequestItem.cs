using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Packet;

namespace ShareLib.TcpNetwork.Interface
{
    public interface IRequestItem
    {
        byte[] RequestPacketHearder { get; }
        Int32 PacketId { get; }
        byte[] PacketBuffer { get; set; }
    }
}