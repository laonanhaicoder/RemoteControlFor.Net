namespace TerminalCommunication
{
    internal sealed class FrameDirtyMessage : MessageBase
    {
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
        public FrameDirtyMessage() 
            : base(MessageType.FrameDirty)
        {
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public FrameDirtyMessage(byte[] data) : base(data)
        {
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 0;
            offset = base.WriteTo(buffer, offset);

            return offset;
        }
    }
}
