using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLib.TcpAsyncNetwork;
using ServerLib.TcpAsyncNetwork.Interface;
using ShareLib.TcpNetwork;

namespace GameServer.ConnetionUser
{
    public class TcpAsyncNetworkManager : TcpAsyncNetwork, IUserManager
    {
        public ClientUserManager<ClientUser> clientUserManager;

        public TcpAsyncNetworkManager(string listenIP
            , Int16 listenPort
            , Int32 backLog
            , Int32 maxConnections
            , Int32 sendBufferBlockSize
            , Int32 receBufferBlockSize) : base(listenIP, listenPort, backLog, maxConnections, sendBufferBlockSize, receBufferBlockSize)
        {
            base.OnSessionEventHandle += TcpAsyncNetworkManager_OnSessionEventHandle;

            clientUserManager = new ClientUserManager<ClientUser>(maxConnections);
        }

        public void TcpAsyncNetworkManager_OnSessionEventHandle(object sender, UserToken token)
        {
            //유저 접속
            ClientUser user = new ClientUser(token);

            lock (clientUserManager)
            {
                clientUserManager.AddUser(user);
                //접속 시, 후속 처리
            }
        }

        public void RemoveUser(ClientUser user)
        {
            lock (clientUserManager)
            {
                clientUserManager.RemoveUser(user);

                //접속 끊겼을 때, 후속 처리
            }
        }

        public Int32 UserCount()
        {
            return clientUserManager.ClientUserList.Count;
        }
    }
}
