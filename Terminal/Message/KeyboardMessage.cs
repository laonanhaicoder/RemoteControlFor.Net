namespace TerminalCommunication
{
    internal sealed class KeyboardMessage : MessageBase
    {
        /// <summary>
        /// 虚拟键码
        /// </summary>
        public int VKey { get; private set; }

        /// <summary>
        /// 硬件扫描码
        /// </summary>
        public int Scan { get; private set; }

        /// <summary>
        /// 标志位集,0 为Down,1为Up
        /// </summary>
        public int Flags { get; private set; }
        
        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="vkey">虚拟键码</param>
        /// <param name="scan">硬件扫描码</param>
        /// <param name="flags">标志位集</param>
        /// <remarks>
        /// 所有参数实际只有低8位有效  
        /// 参数详解详情参见 "keybd_event" API.
        /// </remarks>
        public KeyboardMessage(int vkey, int scan, int flags) 
            : base(MessageType.Keyboard)
        {
            VKey = vkey;
            Scan = scan;
            Flags = flags;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public KeyboardMessage(byte[] data) : base(data)
        {
            VKey = data[HeadLength];
            Scan = data[HeadLength + 1];
            Flags = data[HeadLength + 2];
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 3;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)VKey;
            buffer[offset++] = (byte)Scan;
            buffer[offset++] = (byte)Flags;

            return offset;
        }
    }
}
