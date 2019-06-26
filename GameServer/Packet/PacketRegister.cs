using System;
using System.Collections.Generic;
using GameServer.ServerLogic;
using GameServer.ServerLogic.Interface;
using NetworkPacket.ClientServerPacket.ToClient;
using NetworkPacket.ClientServerPacket.ToServer;
using ShareLib.TcpNetwork.Interface;

namespace GameServer.Packet
{
    public class PacketRegister
    {
        public Dictionary<Int32, IClientPacket> ClientPacketList { get; private set; }
        public bool Commited { get; set; }

        private Dictionary<Int32, IRun<IServerPacket, IClientPacket>> ProcessList { get; }

        public PacketRegister(Int32 packetCount, ref Dictionary<Int32, IRun<IServerPacket, IClientPacket>> processList)
        {
            ClientPacketList = new Dictionary<Int32, IClientPacket>(packetCount);
            ProcessList = processList;

            Regist();
        }

        private void Regist()
        {
            if (Commited == true)
                throw new InvalidOperationException("ProcessManager.Regist Failed, already commited");

            RegistPacket();

            Commited = true;
        }

        private void Regist<Process, RequestPacket, ResponsePacket>()
            where Process : class, IRun<RequestPacket, ResponsePacket>, new()
            where RequestPacket : class, IServerPacket, new()
            where ResponsePacket : class, IClientPacket, new()
        {
            if (Commited == true)
                throw new InvalidOperationException("ProcessManager.Regist Failed, already commited");

            Process process = new Process();
            RequestPacket requestPacket = new RequestPacket();
            IRun<IServerPacket, IClientPacket> run = Wrapper.Convert<Process, RequestPacket, ResponsePacket>();

            ProcessList.Add(requestPacket.PacketId, run);
            ClientPacketList[requestPacket.PacketId] = new ResponsePacket();
        }

        private void RegistPacket()
        {
            Regist<ConnectionTest, ConnectionRequest, ConnectionResponse>();
        }
    }
}
