using System;
using System.Collections.Generic;
using GameServer.ServerLogic.Interface;
using NetworkPacket.ClientServerPacket.Enum;
using ServerLib.Common;
using ShareLib.TcpNetwork.Interface;

namespace GameServer.Packet
{
    public class ProcessManager
    {
        public Int32 PacketCount { get; private set; }

        private static Dictionary<Int32, IRun<IServerPacket, IClientPacket>> ProcessList = new Dictionary<Int32, IRun<IServerPacket, IClientPacket>>();
        private static PacketRegister packetRegister;

        public ProcessManager()
        {
            PacketCount = CommonClass.EnumCount<ToServerPacketID>();
            packetRegister = new PacketRegister(PacketCount, ref ProcessList);
        }
        
        public static IClientPacket Run<RequestPacket>(Int32 packetId, RequestPacket requestPacket)
               where RequestPacket : IServerPacket
        {
            IClientPacket clientPacket = packetRegister.ClientPacketList[packetId];

            if (ProcessList.Count > 0)
            {
                Run<RequestPacket, IClientPacket>(packetId, requestPacket, ref clientPacket);
            }

            return clientPacket;
        }

        //내부 콜
        private static ResponsePacket Run<RequestPacket, ResponsePacket>(Int32 packetId, RequestPacket requestPacket, ref ResponsePacket responsePacket)
            where RequestPacket : IServerPacket
            where ResponsePacket : IClientPacket
        {
            if (ProcessList.ContainsKey(packetId) == true)
            {
                IRun<IServerPacket, IClientPacket> process = ProcessList[packetId];
                return (ResponsePacket)process.Run(requestPacket, responsePacket);

                //여기에서 바로 return response를 해줄것인가?
                //responsePacket
            }
            else
            {
                throw new InvalidOperationException("ProcessManager.PacketList Find Failed, Not Exist PacketId");
            }
        }
    }
}
