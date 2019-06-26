using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.TcpAsyncNetwork
{
    class AsyncNetworkManager
    {
        private BufferManager sendBufferManager;
        private BufferManager recvBufferManager;

        // 접속 후, 첫번째 패킷 받을 때까지 시간
        private TimeSpan connectionFirstReceiveTimeOut;

        // 두번째 패킷부터 대기 시간
        private TimeSpan receiveTimeout;

        public AsyncNetworkManager()
        {
            NetworkService service = new NetworkService();

            // 이벤트 연결
            service.OnSessionEventHandler += OnSessionEventHandler;

            //초기화
            BufferManager
        }

        private void OnSessionEventHandler(UserToken token)
        {
            throw new NotImplementedException();
        }
    }
}
