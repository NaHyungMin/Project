using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkPacket.ClientServerPacket.Enum
{
    public enum ToServerPacketID
    {
        // 1천번 이하는 시스템이나 데이터베이스 관련 오류를 쓰도록 하자.

        // user
        CONNECTION_REQUEST = 1000,
    }
}
