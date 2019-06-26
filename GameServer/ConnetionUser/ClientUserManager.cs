using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.ConnetionUser
{
    public class ClientUserManager<T> where T : new()
    {
        public List<T> ClientUserList { get; private set; }

        public ClientUserManager(Int32 initCapacity)
        {
            ClientUserList = new List<T>(initCapacity);
        }

        internal void AddUser(T clientUser)
        {
            ClientUserList.Add(clientUser);
        }

        internal void RemoveUser(T clientUser)
        {
            ClientUserList.Remove(clientUser);
        }
    }
}
