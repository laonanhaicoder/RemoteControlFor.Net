namespace TerminalCommunication
{
    internal static class MessageFactory
    {
        /// <summary>
        /// 反序列化消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>反序列化后的消息,失败返回null</returns>
        public static MessageBase Parse(byte[] data)
        {
            var type = (MessageType)data[4];
            switch(type)
            {
                case MessageType.Mouse:
                    return new MouseMessage(data);
                case MessageType.Keyboard:
                    return new KeyboardMessage(data);
                case MessageType.ScreenInfo:
                    return new ScreenInfoMessage(data);
                case MessageType.ScreenFrame:
                    return new ScreenFrameMessage(data);
                case MessageType.AllowControl:
                    return new AllowControlMessage(data);
                case MessageType.VisualRegion:
                    return new VisualRegionMessage(data);
                case MessageType.FrameDirty:
                    return new FrameDirtyMessage(data);
                case MessageType.Clipboard:
                    return new ClipboardMessage(data);
                case MessageType.CursorInfo:
                    return new CursorInfoMessage(data);
                case MessageType.DelayTest:
                    return new DelayTestMessage(data);
                case MessageType.Definition:
                    return new DefinitionMessage(data);
                default:
                    return null;
            }
        }
    }
}
