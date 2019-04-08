namespace TerminalCommunication
{
    internal sealed class ScreenInfoMessage : MessageBase
    {
        /// <summary>
        /// 屏幕宽度,低16位有效
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 屏幕高度,低16位有效
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="width">虚拟键码</param>
        /// <param name="height">硬件扫描码</param>
        /// <param name="flags">标志位集</param>
        /// <remarks>
        /// 所有参数实际只有低16位有效  
        /// </remarks>
        public ScreenInfoMessage(int width, int height) 
            : base(MessageType.ScreenInfo)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public ScreenInfoMessage(byte[] data) : base(data)
        {
            Width = (data[HeadLength] << 8) + data[HeadLength + 1];
            Height = (data[HeadLength + 2] << 8) + data[HeadLength + 3];
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 4;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)((Width >> 8) & 0xFF);
            buffer[offset++] = (byte)(Width & 0xFF);
            buffer[offset++] = (byte)((Height >> 8) & 0xFF);
            buffer[offset++] = (byte)(Height & 0xFF);

            return offset;
        }
    }
}
