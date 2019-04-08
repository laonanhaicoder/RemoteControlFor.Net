namespace TerminalCommunication
{
    internal sealed class AllowControlMessage : MessageBase
    {
        /// <summary>
        /// 允许控制
        /// </summary>
        public bool AllowControl { get; private set; }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="allowControl">是否允许控制</param>
        public AllowControlMessage(bool allowControl) 
            : base(MessageType.AllowControl)
        {
            AllowControl = allowControl;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public AllowControlMessage(byte[] data) : base(data)
        {
            AllowControl = data[HeadLength] != 0;
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 1;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)(AllowControl ? 1 : 0);

            return offset;
        }
    }
}
