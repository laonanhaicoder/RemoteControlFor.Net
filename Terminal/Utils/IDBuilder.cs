namespace TerminalCommunication
{
    internal static class IDBuilder
    {
        private static object lockObj = new object();

        private static int nowId = 0;

        public static int NewId
        {
            get
            {
                lock(lockObj)
                {
                    return ++nowId;
                }
            }
        }
    }
}
