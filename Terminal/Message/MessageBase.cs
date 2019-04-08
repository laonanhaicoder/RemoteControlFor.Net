using TerminalCommunication;

namespace TerminalCommunication
{
    internal abstract class MessageBase
    {
        public const int HeadLength = 13;

        public MessageBase(MessageType type)
        {
            ID = IDBuilder.NewId;
            Timestamp = TerminalCommunication.Timestamp.Now;
            Type = type;
        }

        public MessageBase(byte[] data)
        {
            Length = (data[0] << 24) + (data[1] << 16) + (data[2] << 8) + data[3];
            Type = (MessageType)data[4];
            ID = ((data[5] << 24) + (data[6] << 16) + (data[7] << 8) + data[8]);
            Timestamp = ((data[9] << 24) + (data[10] << 16) + (data[11] << 8) + data[12]);
        }

        /// <summary>
        /// 消息ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; protected set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public int Timestamp { get; protected set; }

        /// <summary>
        /// 消息长度
        /// </summary>
        public int Length { get; protected set; }

        /// <summary>
        /// 二进制序列化
        /// </summary>
        /// <param name="buffer">缓冲器</param>
        /// <param name="offset">偏移地址</param>
        /// <returns>写入后的偏移地址</returns>
        public virtual int WriteTo(byte[] buffer, int offset)
        {
            var len = Length + HeadLength - 4;
            // len
            buffer[offset++] = (byte)((len >> 24) & 0xFF);
            buffer[offset++] = (byte)((len >> 16) & 0xFF);
            buffer[offset++] = (byte)((len >> 8) & 0xFF);
            buffer[offset++] = (byte)(len & 0xFF);
            // type
            buffer[offset++] = (byte)Type;
            // id
            buffer[offset++] = (byte)((ID >> 24) & 0xFF);
            buffer[offset++] = (byte)((ID >> 16) & 0xFF);
            buffer[offset++] = (byte)((ID >> 8) & 0xFF);
            buffer[offset++] = (byte)(ID & 0xFF);
            // timestamp
            buffer[offset++] = (byte)((Timestamp >> 24) & 0xFF);
            buffer[offset++] = (byte)((Timestamp >> 16) & 0xFF);
            buffer[offset++] = (byte)((Timestamp >> 8) & 0xFF);
            buffer[offset++] = (byte)(Timestamp & 0xFF);

            return offset;
        }
    }
}
