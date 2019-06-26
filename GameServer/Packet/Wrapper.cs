using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.ServerLogic.Interface;
using ShareLib.TcpNetwork.Interface;

namespace GameServer.Packet
{
    public class Wrapper
    {
        public static IRun<IServerPacket, IClientPacket> Convert<Process, RequestPacket, ResponsePacket>()
            where Process : class, IRun<RequestPacket, ResponsePacket>, new()
            where RequestPacket : class, IServerPacket
            where ResponsePacket : class, IClientPacket
        {
            Process process = new Process();
            IRun<IServerPacket, IClientPacket> run = new Holder<Process, RequestPacket, ResponsePacket>(process) as IRun<IServerPacket, IClientPacket>;
            return run;
        }
    }

    public class Holder<Process, RequestPacket, ResponsePacket> : IRun<IServerPacket, IClientPacket>
        where Process : IRun<RequestPacket, ResponsePacket>
        where RequestPacket : class, IServerPacket
        where ResponsePacket : class, IClientPacket
    {
        Process process;

        public Holder(Process process)
        {
            this.process = process;
        }

        public IClientPacket Run(IServerPacket requestPacket, IClientPacket responsePacket)
        {
            RequestPacket request = requestPacket as RequestPacket;
            ResponsePacket response = responsePacket as ResponsePacket;

            return process.Run(request, response);
        }
    }
}
