using System;
using GameServer.Packet;
using MessagePack;
using ShareLib.TcpNetwork;
using ShareLib.TcpNetwork.Interface;
using ShareLib.TcpNetwork.Packet;

namespace GameServer.ConnetionUser
{
    public class ClientUser : IPeer
    {
        public ClientUser() { }

        public ClientUser(UserToken token)
        {
            token.Peer = this;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        //public void OnMessage(byte[] buffer)
        //{

        //}
        
        public void OnMessage(PacketHeader packetHeader, byte[] packetMessage)
        {
            OnMessage<IServerPacket>(packetHeader, packetMessage);
        }
        
        private void OnMessage<RequestPacket>(PacketHeader packetHeader, byte[] packetMessage)
            where RequestPacket : IServerPacket 
        {
            //Header는 여기서 사용해야할까?
            //var requestPacket = MessagePackSerializer.Deserialize<dynamic>(packetMessage, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            
            //IServerPacket serverPacket = requestPacket as IServerPacket;

            var requestPacket = MessagePackSerializer.Typeless.Deserialize(packetMessage) as IServerPacket; //size 119

            //MessagePackSerializer.Deserialize<IServerPacket>(bytes);

            //여기에는 send로 온.. 데이터만 들어오는데..
            IClientPacket clientPacket = ProcessManager.Run(requestPacket.PacketId, requestPacket);

            byte[] bytes = MessagePack.MessagePackSerializer.Typeless.Serialize(clientPacket);

            PacketHeader packetHeader2 = new PacketHeader
            {
                ContentsVersion = packetHeader.ContentsVersion,
                LogicVersion = packetHeader.LogicVersion,
                PacketID = requestPacket.PacketId,
                PacketSize = bytes.Length,
                EncriptKey = 0,
                EncriptType = 0,
            };
            
            //response
            IRequestItem requestItem = new PacketHolder(PacketHeaderConverter.CreateHeaderToBytes(packetHeader2), requestPacket.PacketId, bytes);
            Send(requestItem);
        }

        public void OnRemoved()
        {
            throw new NotImplementedException();
        }

        public void Send(IRequestItem requestItem)
        {

        }
    }
}
