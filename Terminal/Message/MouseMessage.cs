
namespace TerminalCommunication
{
    internal sealed class MouseMessage : MessageBase
    {
        public MouseEventFlags Flags { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Delta { get; private set; }
        
        /// <summary>
        /// 由事件参数初始化,通常用于发包
        /// </summary>
        /// <param name="flags">标志位集</param>
        /// <param name="dx">X坐标</param>
        /// <param name="dy">Y坐标</param>
        /// <param name="delta">滚轮数据</param>
        /// <remarks>
        /// 所有参数实际只有低16位有效  
        /// 参数详解详情参见 "mouse_event" API.
        /// </remarks>
        public MouseMessage(MouseEventFlags flags, int dx, int dy, int delta) 
            : base(MessageType.Mouse)
        {
            Flags = flags;
            X = dx;
            Y = dy;
            Delta = delta > 0 ? delta : (-delta | 0x8000);
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public MouseMessage(byte[] data) : base(data)
        {
            Flags = (MouseEventFlags)(data[HeadLength] << 8) + data[HeadLength + 1];
            X = (data[HeadLength + 2] << 8) + data[HeadLength + 3];
            Y = (data[HeadLength + 4] << 8) + data[HeadLength + 5];
            var delta = (data[HeadLength + 6] << 8) + data[HeadLength + 7];
            Delta = ((delta & 0x8000) == 0) ? delta : -(delta & 0x7FFF);
        }

        public override int WriteTo(byte[] buffer, int offset)
        {//定长消息,8byte,不含len字段本身,采用大端模式序列化
            Length = 8;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)(((int)Flags >> 8) & 0xFF);
            buffer[offset++] = (byte)((int)Flags & 0xFF);
            buffer[offset++] = (byte)((X >> 8) & 0xFF);
            buffer[offset++] = (byte)(X & 0xFF);
            buffer[offset++] = (byte)((Y >> 8) & 0xFF);
            buffer[offset++] = (byte)(Y & 0xFF);
            buffer[offset++] = (byte)((Delta >> 8) & 0xFF);
            buffer[offset++] = (byte)(Delta & 0xFF);

            return offset;
        }
    }
}
