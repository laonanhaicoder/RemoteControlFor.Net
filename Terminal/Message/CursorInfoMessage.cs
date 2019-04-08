using System;

namespace TerminalCommunication
{
    internal sealed class CursorInfoMessage : MessageBase
    {
        /// <summary>
        /// 允许控制
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="allowControl">是否允许控制</param>
        public CursorInfoMessage(IntPtr handle) 
            : base(MessageType.CursorInfo)
        {
            Handle = handle;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public CursorInfoMessage(byte[] data) : base(data)
        {
            var tmp = (data[HeadLength] << 24) + (data[HeadLength + 1] << 16)
                        + (data[HeadLength + 2] << 8) + data[HeadLength + 3];
            Handle = (IntPtr)tmp;
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 4;
            offset = base.WriteTo(buffer, offset);
            var tmp = (int)Handle;
            buffer[offset++] = (byte)((tmp >> 24) & 0xFF);
            buffer[offset++] = (byte)((tmp >> 16) & 0xFF);
            buffer[offset++] = (byte)((tmp >> 8) & 0xFF);
            buffer[offset++] = (byte)(tmp & 0xFF);

            return offset;
        }
    }
}
