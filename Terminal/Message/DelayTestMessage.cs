using System;

namespace TerminalCommunication
{
    internal sealed class DelayTestMessage : MessageBase
    {
        public DateTime CreateTime { get; private set; }

        public DateTime BackTime { get; private set; }

        /// <summary>
        /// 延迟毫秒数
        /// </summary>
        public int Delay { get; private set; }

        public DelayTestMessage() 
            : base(MessageType.DelayTest)
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public DelayTestMessage(byte[] data) : base(data)
        {
            BackTime = DateTime.Now;

            var tick = ((long)data[HeadLength] << 56) + ((long)data[HeadLength + 1] << 48)
                + ((long)data[HeadLength + 2] << 40) + ((long)data[HeadLength + 3] << 32)
                + ((long)data[HeadLength + 4] << 24) + ((long)data[HeadLength + 5] << 16)
                + ((long)data[HeadLength + 6] << 8) + data[HeadLength + 7];
            CreateTime = DateTime.FromBinary(tick);

            Delay = (int)((BackTime - CreateTime).TotalMilliseconds / 2);
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 8;
            var tick = CreateTime.ToBinary();
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)((tick >> 56) & 0xFF);
            buffer[offset++] = (byte)((tick >> 48) & 0xFF);
            buffer[offset++] = (byte)((tick >> 40) & 0xFF);
            buffer[offset++] = (byte)((tick >> 32) & 0xFF);
            buffer[offset++] = (byte)((tick >> 24) & 0xFF);
            buffer[offset++] = (byte)((tick >> 16) & 0xFF);
            buffer[offset++] = (byte)((tick >> 8) & 0xFF);
            buffer[offset++] = (byte)(tick & 0xFF);
            return offset;
        }
    }
}
