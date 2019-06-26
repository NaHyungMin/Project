using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Interface;

namespace GameServer.ServerLogic.Interface
{
    public interface IRun<RequestPacket, ResponsePacket>
        where RequestPacket : IServerPacket
        where ResponsePacket : IClientPacket
    {
        ResponsePacket Run(RequestPacket requestPacket, ResponsePacket responsePacket);
    }
}
