using ShareLib.TcpNetwork.Interface;

namespace NetworkPacket.ClientServerPacket
{
    public class ClientPacketHolder<T> where T : IClientPacket
    {
        public T Clone()
        {
            return (T)this.MemberwiseClone();
        }
    }
}
