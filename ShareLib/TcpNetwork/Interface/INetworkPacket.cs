using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace ShareLib.TcpNetwork.Interface
{
    public interface IPacket
    {
        Int32 PacketId { get; }
    }
    
    public interface IServerPacket : IPacket
    {
        //Int64 TransactionSerial { get; set; }
        //Int64 UserKey { get; set; }
    }
    
    public interface IClientPacket : IPacket
    {
        Int32 ResultNumber { get; set; }
    }
    
    public interface IClientUserPacket : IClientPacket
    {
        //Int64 TransactionSerial { get; set; }
    }
}
