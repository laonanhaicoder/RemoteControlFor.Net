using TerminalCommunication;
using System;

namespace TerminalCommunication
{
    internal sealed class TransferEventArgs : EventArgs
    {
        public new static readonly TransferEventArgs Empty = new TransferEventArgs(null);

        public TransferEventArgs(MessageBase msg)
        {
            Message = msg;
        }

        public MessageBase Message { get; private set; }
    }
}
