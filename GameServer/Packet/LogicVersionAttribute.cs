using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Packet
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class LogicVersionAttribute : Attribute
    {
        public Int32 MinVersion { get; private set; }
        public Int32 MaxVersion { get; private set; }

        public LogicVersionAttribute(Int32 minVersion, Int32 maxVersion)
        {
            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }
    }
}
