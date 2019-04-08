using TerminalCommunication;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TerminalCommunication
{
    internal sealed class TcpTransfer : ITransfer
    {
        private string token = string.Empty;

        public bool IsLinsten { get; set; } = false;

        public void Listen(int port)
        {
            Listen(port, null);
        }

        public void Listen(int port, string host)
        {
            if(IsLinsten)
            {
                return;
            }

            IsLinsten = true;

            IsShutdown = false;

            IPAddress address = string.IsNullOrEmpty(host)
                                ? IPAddress.Any
                                : IPAddress.Parse(host);

            var endPoint = new IPEndPoint(address, port);
            listener = new TcpListener(endPoint);
            listener.Start();
            listener.BeginAcceptTcpClient(ar =>
            {
                var listener = ar.AsyncState as TcpListener;
                client = listener.EndAcceptTcpClient(ar);
                stream = client.GetStream();
                OnConnected();
            }, listener);
        }

        public string ListenForToken(int port)
        {
            return ListenForToken(port, null);
        }

        public string ListenForToken(int port, string host)
        {
            if (IsLinsten)
            {
                return token;
            }

            IsLinsten = true;

            IsShutdown = false;

            IPAddress address = string.IsNullOrEmpty(host) 
                                ? IPAddress.Any
                                : IPAddress.Parse(host);

            var endPoint = new IPEndPoint(address, port);
            listener = new TcpListener(endPoint);
            listener.Start();

            token = Guid.NewGuid().ToString();

            listener.BeginAcceptTcpClient(AcceptForToken, listener);
            
            return token;
        }

        private void AcceptForToken(IAsyncResult ar)
        {
            var listener = ar.AsyncState as TcpListener;
            var clt = listener.EndAcceptTcpClient(ar);
            var stm = clt.GetStream();
            stm.ReadTimeout = 5000;
            var reader = new StreamReader(stm, Encoding.ASCII);
            if (reader.ReadLine() == token)
            {// 令牌相符
                var writer = new StreamWriter(stm, Encoding.ASCII);
                writer.WriteLine(token);
                writer.Flush();

                client = clt;
                stream = stm;
                stm.ReadTimeout = Timeout.Infinite;
                OnConnected();
            }
            else
            {// 令牌不符
                reader.Close();
                stm.Close();
                listener.BeginAcceptTcpClient(AcceptForToken, listener);
            }
        }

        public void Connect(string host, int port)
        {
            client = new TcpClient();
            client.BeginConnect(host, port, ar =>
            {
                IsShutdown = false;

                var client = ar.AsyncState as TcpClient;
                client.EndConnect(ar);
                stream = client.GetStream();
                OnConnected();
            }, client);
        }

        public void ConnectWithToken(string host, int port, string token)
        {
            client = new TcpClient();
            client.BeginConnect(host, port, ar =>
            {
                IsShutdown = false;

                var client = ar.AsyncState as TcpClient;
                client.EndConnect(ar);
                stream = client.GetStream();
                
                var writer = new StreamWriter(stream, Encoding.ASCII);
                writer.WriteLine(token);
                writer.Flush();
                stream.ReadTimeout = 10000;
                var reader = new StreamReader(stream, Encoding.ASCII);
                if(reader.ReadLine() == token)
                {
                    stream.ReadTimeout = Timeout.Infinite;
                    OnConnected();
                }                
                else
                {
                    reader.Close();
                    stream.Close();
                }
            }, client);
        }

        public TcpTransfer()
        {
            msgBuffer = new byte[BUFFER_SIZE];
            sndBuffer = new byte[BUFFER_SIZE];
            msgBufOffset = 0;

            recBuffer = new byte[BUFFER_SIZE];
        }

        public long ReceiveCount { get; set; }

        public long SendCount { get; set; }

        private TcpListener listener;

        private TcpClient client;

        private NetworkStream stream;

        private byte[] recBuffer;
        private byte[] msgBuffer;
        private byte[] sndBuffer;
        private int sndBufOffset;
        private int msgBufOffset;
        private const int BUFFER_SIZE = 5 * 1024 * 1024;

        private int offset = 0;
        
        #region 收发线程

        private void ReceiveData()
        {
            while (true)
            {
                if (IsShutdown)
                {// 结束
                    break;
                }

                try
                {
                    offset = 0;

                    // 读取指令长度
                    while (offset < 4)
                        offset += stream.Read(recBuffer, offset, 4 - offset);
                    
                    var len = (int)((recBuffer[0] << 24) + (recBuffer[1] << 16) + (recBuffer[2] << 8) + recBuffer[3]);

                    // 读取指令数据
                    while (offset < len)
                        offset += stream.Read(recBuffer, offset, len + 4 - offset);
                    
                    // 流量统计
                    ReceiveCount += offset;

                    // 反序列化
                    var msg = MessageFactory.Parse(recBuffer);
#if DEBUG
                    Debug.WriteLine("rec:" + DateTime.Now.ToString("hh:mm:ss.ffff"));
#endif
                    OnReceived(msg);
                }
                catch (IOException)
                {// 连接断开
                    OnReceived(null);
                }
                catch (Exception)
                {//TODO:异常处理
                }

                //Thread.Sleep(Configure.MessageDelay);
            }
        }

        private void SendBuffer()
        {
            while (true)
            {
                if (IsShutdown)
                {// 结束
                    break;
                }
                     
                lock(msgBuffer)
                {
                    if(msgBufOffset <=0)
                    {// 无数据可发,等待
                        Monitor.Wait(msgBuffer);
                    }

                    msgBuffer.CopyTo(sndBuffer, 0);
                    sndBufOffset = msgBufOffset;
                    msgBufOffset = 0;
                }
                try
                {
                    stream.Write(sndBuffer, 0, sndBufOffset);
                    

                    // 流量统计
                    SendCount += sndBufOffset;
#if DEBUG
                    Debug.WriteLine("snd:" + DateTime.Now.ToString("hh:mm:ss.ffff"));
#endif
                }
                catch (IOException)
                {// 连接断开
                    OnReceived(null);
                }
                catch (Exception)
                {//TODO:异常处理
                }

                //Thread.Sleep(Configure.MessageDelay);
            }
        }

        #endregion

        #region ITransfer 成员

        public bool Send(MessageBase msg)
        {
            lock (msgBuffer)
            {
                try
                {
                    msgBufOffset = msg.WriteTo(msgBuffer, msgBufOffset);
                    Monitor.Pulse(msgBuffer);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        public event EventHandler<TransferEventArgs> Connected;
        public event EventHandler<TransferEventArgs> Received;

        public bool IsShutdown { get; private set; }

        private void OnConnected()
        {
            // 启动收发线程
            var recThd = new Thread(ReceiveData);
            recThd.SetApartmentState(ApartmentState.STA);
            recThd.IsBackground = true;
            recThd.Start();

#if DEBUG
            Debug.WriteLine("Receive Data Thread Id=" + recThd.ManagedThreadId);
#endif 

            var sndThd = new Thread(SendBuffer);
            sndThd.SetApartmentState(ApartmentState.STA);
            sndThd.IsBackground = true;
            sndThd.Start();

#if DEBUG
            Debug.WriteLine("Send Buffer Thread Id=" + sndThd.ManagedThreadId);
#endif 

            var handler = Connected;
            handler?.Invoke(this, TransferEventArgs.Empty);
        }

        private void OnReceived(MessageBase msg)
        {
            var handler = Received;
            handler?.Invoke(this, new TransferEventArgs(msg));
        }

        public void Shutdown()
        {
            try
            {
                IsShutdown = true;

                // 关闭连接
                client.Client.Shutdown(SocketShutdown.Both);
                stream.Close();
                client.Close();
                listener.Stop();

                // 唤醒等待的线程
                lock (msgBuffer)
                {
                    Monitor.PulseAll(msgBuffer);
                }
            }
            catch
            {
            }            
        }

        #endregion
    }
}
