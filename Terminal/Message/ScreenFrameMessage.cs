using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TerminalCommunication
{
    internal sealed class ScreenFrameMessage : MessageBase
    {
        /// <summary>
        /// X 坐标偏移量,低16位有效
        /// </summary>
        public int OffsetX { get; private set; }

        /// <summary>
        /// Y 坐标偏移量,低16位有效
        /// </summary>
        public int OffsetY { get; private set; }
        
        /// <summary>
        /// 图像数据
        /// </summary>
        public Bitmap Data { get; private set; }

        /// <summary>
        /// 上一帧的消息ID
        /// </summary>
        public int LastFrameID { get; private set; }

        public bool IsHighDefinition { get; private set; }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="offsetX">X 坐标偏移量,低16位有效</param>
        /// <param name="offsetY">Y 坐标偏移量,低16位有效</param>
        /// <param name="data">图像数据</param>
        public ScreenFrameMessage(int offsetX, int offsetY, Bitmap data)
            : this(offsetX, offsetY, data, 0, true)
        {
        }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="offsetX">X 坐标偏移量,低16位有效</param>
        /// <param name="offsetY">Y 坐标偏移量,低16位有效</param>
        /// <param name="data">图像数据</param>
        public ScreenFrameMessage(int offsetX, int offsetY, Bitmap data, bool isHighDef)
            : this(offsetX, offsetY, data, 0, isHighDef)
        {
        }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="offsetX">X 坐标偏移量,低16位有效</param>
        /// <param name="offsetY">Y 坐标偏移量,低16位有效</param>
        /// <param name="data">图像数据</param>
        /// <param name="lfId">上一帧的消息ID</param>
        public ScreenFrameMessage(int offsetX, int offsetY, Bitmap data, int lfId) 
            : this(offsetX, offsetY, data, lfId, true)
        {
        }

        /// <summary>
        /// 由参数初始化,通常用于发包
        /// </summary>
        /// <param name="offsetX">X 坐标偏移量,低16位有效</param>
        /// <param name="offsetY">Y 坐标偏移量,低16位有效</param>
        /// <param name="data">图像数据</param>
        /// <param name="lfId">上一帧的消息ID</param>
        /// <param name="isHighDef">是否高清消息</param>
        public ScreenFrameMessage(int offsetX, int offsetY, Bitmap data, int lfId, bool isHighDef)
            : base(MessageType.ScreenFrame)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Data = data;
            LastFrameID = lfId;
            IsHighDefinition = isHighDef;
        }

        /// <summary>
        /// 由数据包初始化,通常用于解包
        /// </summary>
        /// <param name="data">数据包</param>
        public ScreenFrameMessage(byte[] data) : base(data)
        {
            OffsetX = (data[HeadLength] << 8) + data[HeadLength + 1];
            OffsetY = (data[HeadLength + 2] << 8) + data[HeadLength + 3];
            LastFrameID = ((data[HeadLength + 4] << 24) + (data[HeadLength + 5] << 16)
                + (data[HeadLength + 6] << 8) + data[HeadLength + 7]);
            IsHighDefinition = data[HeadLength + 8] != 0;
            var index = HeadLength + 9;
            var stream = new MemoryStream(data, index, data.Length - index);
            Data = new Bitmap(stream);
        }

        public override int WriteTo(byte[] buffer, int offset)
        {
            var index = offset + HeadLength + 9;
            var stream = new MemoryStream(buffer, index, buffer.Length - index);
            if (IsHighDefinition)
            {
                Data.Save(stream, ImageFormat.Png);
            }
            else
            {
                Data.Clone(new Rectangle(Point.Empty, Data.Size), PixelFormat.Format16bppArgb1555).Save(stream, ImageFormat.Png);
            }
            stream.Flush();
            Length = (int)stream.Position + 9;
#if DEBUG
            Debug.WriteLine($"now at {DateTime.Now}, Screen frame size is {stream.Position} byte.");
#endif
            offset = base.WriteTo(buffer, offset);
            buffer[offset++] = (byte)((OffsetX >> 8) & 0xFF);
            buffer[offset++] = (byte)(OffsetX & 0xFF);
            buffer[offset++] = (byte)((OffsetY >> 8) & 0xFF);
            buffer[offset++] = (byte)(OffsetY & 0xFF);
            buffer[offset++] = (byte)((LastFrameID >> 24) & 0xFF);
            buffer[offset++] = (byte)((LastFrameID >> 16) & 0xFF);
            buffer[offset++] = (byte)((LastFrameID >> 8) & 0xFF);
            buffer[offset++] = (byte)(LastFrameID & 0xFF);
            buffer[offset++] = (byte)(IsHighDefinition ? 1 : 0);

            return (int)(offset + stream.Position);
        }
    }
}
