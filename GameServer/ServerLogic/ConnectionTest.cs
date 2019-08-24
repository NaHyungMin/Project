using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Packet;
using GameServer.ServerLogic.Interface;
using NetworkPacket.ClientServerPacket.ToClient;
using NetworkPacket.ClientServerPacket.ToServer;
using ShareLib.TcpNetwork.Interface;

namespace GameServer.ServerLogic
{
    public class ConnectionTest :IRun<ConnectionRequest, ConnectionResponse>
    {
        public ConnectionResponse Run(ConnectionRequest requestPacket, ConnectionResponse responsePacket)
        {
            responsePacket.Message = "안녕하세요! 여러분~~";

            return responsePacket;
        }
    }
}