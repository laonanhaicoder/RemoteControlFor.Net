using System.Drawing;

namespace TerminalCommunication
{
    internal sealed class VisualRegionMessage : MessageBase
    {
        /// <summary>
        /// 可见区域
        /// </summary>
        public Rectangle Region { get; private set; }

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
        public VisualRegionMessage(Rectangle reg) 
            : base(MessageType.VisualRegion)
        {
            Region = reg;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public VisualRegionMessage(byte[] data) : base(data)
        {
            var x = (data[HeadLength] << 8) + data[HeadLength + 1];
            var y = (data[HeadLength + 2] << 8) + data[HeadLength + 3];
            var w = (data[HeadLength + 4] << 8) + data[HeadLength + 5];
            var h = (data[HeadLength + 6] << 8) + data[HeadLength + 7];
            Region = new Rectangle(x, y, w, h);
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            Length = 8;
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)((Region.X >> 8) & 0xFF);
            buffer[offset++] = (byte)(Region.X & 0xFF);
            buffer[offset++] = (byte)((Region.Y >> 8) & 0xFF);
            buffer[offset++] = (byte)(Region.Y & 0xFF);
            buffer[offset++] = (byte)((Region.Width >> 8) & 0xFF);
            buffer[offset++] = (byte)(Region.Width & 0xFF);
            buffer[offset++] = (byte)((Region.Height >> 8) & 0xFF);
            buffer[offset++] = (byte)(Region.Height & 0xFF);

            return offset;
        }
    }
}
