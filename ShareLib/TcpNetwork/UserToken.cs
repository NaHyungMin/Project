using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Enum;
using ShareLib.TcpNetwork.Interface;
using ShareLib.TcpNetwork.Packet;

namespace ShareLib.TcpNetwork
{
    public class UserToken
    {
        public Socket Socket { get; set; }
        public SocketAsyncEventArgs ReceiveArgs { get; set; }
        public SocketAsyncEventArgs SendArgs { get; set; }
        public IPeer Peer { get; set; }
        public SocketState SocketState { get; private set; }
        public Boolean IsCoonected { get { return SocketState == SocketState.Connected; } }

        private object lockObject = new object();
        private Int32 closed;
        private MessageResolver messageResolver = new MessageResolver();
        private Queue<IRequestItem> sendQueue = new Queue<IRequestItem>();

        public delegate void ClosedDelegate(UserToken token);
        public ClosedDelegate OnSesstionClosed;

        public void SetEventArgs(SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            ReceiveArgs = receiveArgs;
            SendArgs = sendArgs;
        }

        public void OnReceive(byte[] buffer, Int32 offset, Int32 transfered)
        {
            messageResolver.OnReceive(buffer, offset, transfered, CompleteCallback);
        }

        public void CompleteCallback(PacketHeader header, byte[] bytes)
        {
            if (Peer == null)
                return;
            //IRequestItem requestItem = MessagePackSerializer.Deserialize<IRequestItem>(bytes);
            //requestItem.RequestPacket.LogicVersion
            //requestItem.RequestPacket.ContentVersion
            OnMessage(header, bytes);
        }

        public void Send(IRequestItem packet)
        {
            lock (lockObject)
            {
                if (sendQueue.Count <= 0)
                {
                    sendQueue.Enqueue(packet);
                    SendStart();
                    return;
                }

                // 큐에 무언가가 들어 있다면 아직 이전 전송이 완료되지 않은 상태이므로 큐에 추가만 하고 리턴한다.  
                // 현재 수행중인 SendAsync가 완료된 이후에 큐를 검사하여 데이터가 있으면 SendAsync를 호출하여 전송해줄 것이다.  
                sendQueue.Enqueue(packet);
            }
        }

        public void SendStart()
        {
            lock (lockObject)
            {
                // 전송이 아직 완료된 상태가 아니므로 데이터만 가져오고 큐에서 제거하진 않는다.  
                IRequestItem packet = sendQueue.Peek();

                //총 크기
                byte[] headerBuffer = packet.RequestPacketHearder;
                byte[] buffer = new byte[headerBuffer.Length + packet.PacketBuffer.Length];

                // 이번에 보낼 패킷 사이즈 만큼 버퍼 크기를 설정하고  
                SendArgs.SetBuffer(SendArgs.Offset, buffer.Length);

                //Copy
                Buffer.BlockCopy(headerBuffer, 0, buffer, 0, headerBuffer.Length);
                Buffer.BlockCopy(packet.PacketBuffer, 0, buffer, headerBuffer.Length, packet.PacketBuffer.Length);

                // 패킷 내용을 SocketAsyncEventArgs버퍼에 복사한다.
                Buffer.BlockCopy(buffer, 0, SendArgs.Buffer, SendArgs.Offset, buffer.Length);

                // 비동기 전송 시작.  
                bool pending = Socket.SendAsync(SendArgs);
                if (!pending)
                {
                    SendProcess(SendArgs);
                }
            }
        }

        public void SendProcess(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                return;
            }

            //lock(lockObject)
            //{
            //    if(sendQueue.Count <= 0)
            //    {
            //        throw new Exception(" Sending Queue Count is less than Zero!");
            //    }
            //}

            // 전송 완료된 패킷을 큐에서 제거한다.  
            sendQueue.Dequeue();

            // 아직 전송하지 않은 대기중인 패킷이 있다면 다시 한번 전송을 요청한다.
            if (sendQueue.Count > 0)
            {
                SendStart();
            }
        }

        private void OnMessage(PacketHeader header, byte[] bytes)
        {
            if (Peer != null)
            {
                Peer.OnMessage(header, bytes);
            }
        }

        public void ConnectionBan()
        {
            try
            {
                //CPacket bye = CPacket.create(SYS_CLOSE_REQ);
                //send(bye);
                //Send();
            }
            catch (Exception)
            {
                Close();
            }
        }

        public void OnConnected()
        {
            SocketState = SocketState.Connected;
            closed = 0;
        }

        public void Disconnect()
        {
            try
            {
                if (sendQueue.Count <= 0)
                    Socket.Shutdown(SocketShutdown.Send);
                else
                    SocketState = SocketState.ReservationClosing;
            }
            catch (Exception)
            {
                Close();
            }
        }

        public void Close()
        {
            if (Interlocked.CompareExchange(ref closed, 1, 0) == 1)
                return;

            if (SocketState.Closed == SocketState)
                return;

            SocketState = SocketState.Closed;
            Socket.Close();
            Socket = null;
            ReceiveArgs.UserToken = null;
            SendArgs.UserToken = null;
            sendQueue.Clear();
            messageResolver.clear_buffer();

            if (Peer != null)
            {

            }
        }
    }
}
