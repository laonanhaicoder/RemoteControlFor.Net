namespace TerminalCommunication
{
    internal class DefinitionMessage : MessageBase
    {
        /// <summary>
        /// 是否高清模式
        /// </summary>
        public bool IsHighDefinition { get; private set; }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="highdef">是否高清</param>
        public DefinitionMessage(bool highdef) 
            : base(MessageType.Definition)
        {
            IsHighDefinition = highdef;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public DefinitionMessage(byte[] data) : base(data)
        {
            IsHighDefinition = data[HeadLength] != 0;
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 1;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)(IsHighDefinition ? 1 : 0);

            return offset;
        }
    }
}
