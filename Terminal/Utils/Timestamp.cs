namespace TerminalCommunication
{
    internal static class Timestamp
    {
        public static int Now
        {
            get
            {
                var tick = System.DateTime.Now.Ticks;
                return (int)((tick & 0xFFFFFFFF0000) >> 16);
            }
        }
    }
}
