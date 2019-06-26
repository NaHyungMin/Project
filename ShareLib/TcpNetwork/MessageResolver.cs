using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.TcpNetwork.Packet;

namespace ShareLib.TcpNetwork
{
    internal sealed class MessageResolver
    {
        public delegate void CompletedMessageCallback(PacketHeader packetHeader ,byte[] buffer);

        // 메시지 사이즈.  
        Int32 messageSize;

        // 진행중인 버퍼.  
        byte[] messageBuffer = new byte[8092 - PacketHeader.HEADER_SIZE];

        // 현재 진행중인 버퍼의 인덱스를 가리키는 변수.  
        // 패킷 하나를 완성한 뒤에는 0으로 초기화 시켜줘야 한다.  
        Int32 currentPosition;

        // 읽어와야 할 목표 위치.  
        int positionToRead;

        // 남은 사이즈.  
        int remainBytes;

        public MessageResolver()
        {
            messageSize = 0;
            currentPosition = 0;
            positionToRead = 0;
            remainBytes = 0;
        }

        /// <summary>  
        /// 목표지점으로 설정된 위치까지의 바이트를 원본 버퍼로부터 복사한다.  
        /// 데이터가 모자랄 경우 현재 남은 바이트 까지만 복사한다.  
        /// </summary>  
        /// <param name="buffer"></param>  
        /// <param name="offset"></param>  
        /// <param name="transffered"></param>  
        /// <param name="size_to_read"></param>  
        /// <returns>다 읽었으면 true, 데이터가 모자라서 못 읽었으면 false를 리턴한다.</returns>  
        bool ReadUntil(byte[] buffer, ref int srcPosition, int offset, int transffered)
        {
            if (currentPosition >= offset + transffered)
            {
                // 들어온 데이터 만큼 다 읽은 상태이므로 더이상 읽을 데이터가 없다.  
                return false;
            }

            // 읽어와야 할 바이트.  
            // 데이터가 분리되어 올 경우 이전에 읽어놓은 값을 빼줘서 부족한 만큼 읽어올 수 있도록 계산해 준다.  
            int copySize = positionToRead - currentPosition;

            // 앗! 남은 데이터가 더 적다면 가능한 만큼만 복사한다.  
            if (remainBytes < copySize)
            {
                copySize = remainBytes;
            }

            // 버퍼에 복사.  
            Buffer.BlockCopy(buffer, srcPosition, messageBuffer, currentPosition, copySize);
            //Array.Copy(buffer, srcPosition, messageBuffer, currentPosition, copySize);

            // 원본 버퍼 포지션 이동.  
            srcPosition += copySize;

            // 타겟 버퍼 포지션도 이동.  
            currentPosition += copySize;

            // 남은 바이트 수.
            remainBytes -= copySize;

            // 목표지점에 도달 못했으면 false  
            if (currentPosition < positionToRead)
            {
                return false;
            }

            return true;
        }

        /// <summary>  
        /// 소켓 버퍼로부터 데이터를 수신할 때 마다 호출된다.  
        /// 데이터가 남아 있을 때 까지 계속 패킷을 만들어 callback을 호출 해 준다.  
        /// 하나의 패킷을 완성하지 못했다면 버퍼에 보관해 놓은 뒤 다음 수신을 기다린다.  
        /// </summary>  
        /// <param name="buffer"></param>  
        /// <param name="offset"></param>  
        /// <param name="transffered"></param>  
        public void OnReceive(byte[] buffer, int offset, int transffered, CompletedMessageCallback callback)
        {
            // 이번 receive로 읽어오게 될 바이트 수.  
            remainBytes = transffered;

            // 원본 버퍼의 포지션값.  
            // 패킷이 여러개 뭉쳐 올 경우 원본 버퍼의 포지션은 계속 앞으로 가야 하는데 그 처리를 위한 변수이다.  
            int srcPosition = offset;

            // 남은 데이터가 있다면 계속 반복한다.  
            while (remainBytes > 0)
            {
                bool completed = false;
                PacketHeader header = new PacketHeader();

                // 헤더만큼 못읽은 경우 헤더를 먼저 읽는다.  
                if (currentPosition < PacketHeader.HEADER_SIZE)
                {
                    // 목표 지점 설정(헤더 위치까지 도달하도록 설정).  
                    positionToRead = PacketHeader.HEADER_SIZE;

                    completed = ReadUntil(buffer, ref srcPosition, offset, transffered);

                    if (!completed)
                    {
                        // 아직 다 못읽었으므로 다음 receive를 기다린다.  
                        return; 
                    }

                    // 헤더 하나를 온전히 읽어왔으므로 메시지 사이즈를 구한다.  
                    header = PacketHeaderConverter.BytesToHeader(messageBuffer);
                    messageSize = header.PacketSize; //MessageSize(); //get_body_size();

                    // 다음 목표 지점(헤더 + 메시지 사이즈).  
                    this.positionToRead = messageSize + PacketHeader.HEADER_SIZE;
                }

                // 메시지를 읽는다.  
                completed = ReadUntil(buffer, ref srcPosition, offset, transffered);

                if (completed)
                {
                    // 패킷 하나를 완성 했다.
                    byte[] messagePacket = new byte[header.PacketSize];
                    Buffer.BlockCopy(messageBuffer, header.HeaderSize, messagePacket, 0, header.PacketSize);

                    callback(header, messagePacket);

                    clear_buffer();
                }
            }
        }
        
        //private Int32 MessageSize()
        //{
        //    return PacketHeaderConverter.BytesToHeader(messageBuffer).PacketSize;
        //}
        
        int get_body_size()
        {
            // 헤더에서 메시지 사이즈를 구한다.  
            // 헤더 타입은 Int16, Int32두가지가 올 수 있으므로 각각을 구분하여 처리한다.  
            // 사실 헤더가 바뀔 경우는 없을테니 그냥 한가지로 고정하는편이 깔끔할것 같다.  

            //Type type = PacketHeader.HEADER_SIZE.GetType();
            //if (type.Equals(typeof(Int16)))
            //{
            //    return BitConverter.ToInt16(this.messageBuffer, 0);
            //}

            return BitConverter.ToInt32(this.messageBuffer, 0);
        }

        public void clear_buffer()
        {
            Array.Clear(messageBuffer, 0, messageBuffer.Length);

            currentPosition = 0;
            messageSize = 0;
        }
    }
}
