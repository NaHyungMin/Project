using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLib.TcpNetwork.Enum
{
    //일반적 열거형 형식 (열거형이 라고도 함)의 이름은 표준 형식 명명 규칙 (PascalCasing 등) 따라야 합니다. https://docs.microsoft.com/ko-kr/dotnet/standard/design-guidelines/names-of-classes-structs-and-interfaces#naming-enumerations
    public enum SocketState
    {
        StandBy = 0,
        Connected = 1,
        ReservationClosing = 2,
        Closed = 3,
    }
}
