using System.Text;

namespace TerminalCommunication
{
    internal sealed class ClipboardMessage : MessageBase
    { 
        public string Text { get; private set; }

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
        public ClipboardMessage(string text) 
            : base(MessageType.Clipboard)
        {
            Text = (text.Length > 65535) ? text.Substring(0, 65535) : text;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public ClipboardMessage(byte[] data) : base(data)
        {
            var len = (data[HeadLength] << 8) + data[HeadLength + 1];
            Text = Encoding.UTF8.GetString(data, HeadLength + 2, len);
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            var len = Encoding.UTF8.GetByteCount(Text);
            Length = len + 2;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)((len >> 8) & 0xFF);
            buffer[offset++] = (byte)(len & 0xFF);
            offset += Encoding.UTF8.GetBytes(Text, 0, Text.Length, buffer, offset);
            
            return offset;
        }
    }
}
