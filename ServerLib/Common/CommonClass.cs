using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.Common
{
    public class CommonClass
    {
        public static Int32 EnumCount<T>()
        { 
            if(typeof(T).IsEnum == false)
                throw new ArgumentException("T must be an enumerated type");

            var values = Enum.GetValues(typeof(T));

            return values.Length;
        }
    }
}
