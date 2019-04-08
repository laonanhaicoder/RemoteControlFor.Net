using System;
using TerminalCommunication;

namespace TerminalCommunication
{
    internal interface ITransfer
    {
        bool Send(MessageBase msg);

        event EventHandler<TransferEventArgs> Connected;

        event EventHandler<TransferEventArgs> Received;

        bool IsShutdown { get; }

        void Listen(int port, string host);

        void Listen(int port);

        string ListenForToken(int port);

        string ListenForToken(int port, string host);

        void Connect(string host, int port);

        void ConnectWithToken(string host, int port, string token);

        void Shutdown();
        
        long ReceiveCount { get; set; }

        long SendCount { get; set; }

    }
}
