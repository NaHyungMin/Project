using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ServerLib.TcpAsyncNetwork
{
    /// <summary>  
    /// This class creates a single large buffer which can be divided up and assigned to SocketAsyncEventArgs objects for use  
    /// with each socket I/O operation.  This enables bufffers to be easily reused and gaurds against fragmenting heap memory.  
    ///   
    /// The operations exposed on the BufferManager class are not thread safe.  
    /// </summary>  
    internal sealed class BufferManager
    {
        readonly int m_BufferTotalBytes;         // the total number of bytes controlled by the buffer pool  
        byte[] m_Buffer;                // the underlying byte array maintained by the Buffer Manager
        Stack<int> m_FreeOffsetPool;
        int m_CurrentOffset;
        readonly int m_BufferBlockSize;

        public BufferManager(int bufferBlockSize, Int32 maxBufferBlockCount)
        {
            m_BufferTotalBytes = bufferBlockSize * maxBufferBlockCount;
            m_CurrentOffset = 0;
            m_BufferBlockSize = bufferBlockSize;
            m_FreeOffsetPool = new Stack<int>(maxBufferBlockCount);
            // create one big large buffer and divide that out to each SocketAsyncEventArg object  
            m_Buffer = new byte[m_BufferTotalBytes];
        }

        // /// <summary>  
        // /// Allocates buffer space used by the buffer pool  
        // /// </summary>  
        //public void InitBuffer()
        //{
        //    // create one big large buffer and divide that out to each SocketAsyncEventArg object  
        //    m_Buffer = new byte[m_BufferTotalBytes];
        //}

        /// <summary>  
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object  
        /// </summary>  
        /// <returns>true if the buffer was successfully set, else false</returns>  
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (m_FreeOffsetPool.Count > 0)
            {
                args.SetBuffer(m_Buffer, m_FreeOffsetPool.Pop(), m_BufferBlockSize);
            }
            else
            {
                if ((m_BufferTotalBytes - m_BufferBlockSize) < m_CurrentOffset)
                {
                    return false;
                }
                args.SetBuffer(m_Buffer, m_CurrentOffset, m_BufferBlockSize);
                m_CurrentOffset += m_BufferBlockSize;
            }
            return true;
        }
        /// <summary>  
        /// Removes the buffer from a SocketAsyncEventArg object.  This frees the buffer back to the   
        /// buffer pool  
        /// </summary>  
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            m_FreeOffsetPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
