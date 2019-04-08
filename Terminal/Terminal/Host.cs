
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using TerminalCommunication;

namespace Lens.RemoteControl.Terminal
{
    public sealed class Host : IDisposable
    {
        private ITransfer Transfer;
        private Rectangle ScreenRect;
        private Rectangle OperatorRect;
        private Rectangle MoniterRect;
        private bool FrameDirty;
        private bool IsHighDefinition = true;

        private void Transfer_Received(object sender, TransferEventArgs e)
        {
            OnReceived(e.Message);
        }

        private void Transfer_Connected(object sender, TransferEventArgs e)
        {
            OnConnected();
        }
        
        public void Shutdown()
        {
            Transfer.Shutdown();
        }

        public Host()
        {
            Transfer = new TcpTransfer();
            Transfer.Connected += Transfer_Connected; 
            Transfer.Received += Transfer_Received; 

            MoniterRect = ScreenRect = new Rectangle(0, 0, 
                WinApis.ScreenWidth, WinApis.ScreenHeight);

            OperatorRect = MoniterRect;

            InstallNotifier();
        }

        public bool Run(int port)
        {
            return Run(port, string.Empty);
        }

        public bool Run(int port, string host)
        {
            try
            {
                Transfer.Listen(port, host);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public string RunForToken(int port)
        {
            return RunForToken(port, string.Empty);
        }

        public string RunForToken(int port, string host)
        {
            try
            {
                return Transfer.ListenForToken(port, host);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private bool allowControl = true;
        public bool AllowControl
        {
            get { return allowControl; }
            set
            {
                if(allowControl != value)
                {
                    allowControl = value;
                    var msg = new AllowControlMessage(allowControl);
                    Transfer.Send(msg);
                }
            }
        }

        private int FrameDelay
        {
            get { return IsHighDefinition ? 100 : 300; }
        }

        public event EventHandler<EventArgs> Connected;

        /// <summary>
        /// 远程已关闭
        /// </summary>
        public event EventHandler<EventArgs> RemoteShutdown;

        private void OnConnected()
        {
            // 启动抓图线程并发送给控制机
            var smThd = new Thread(ScreenMoniter);
            smThd.SetApartmentState(ApartmentState.STA);
            smThd.IsBackground = true;
            smThd.Start();
            var handler = Connected;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnReceived(MessageBase msg)
        {
            if(msg == null)
            {
                Shutdown();
                OnRemoteShutdown();
                return;
            }

            switch(msg.Type)
            {
                case MessageType.Keyboard:
                    if (AllowControl)
                    {
                        var kMsg = msg as KeyboardMessage;
                        WinApis.KeybdEvent(kMsg.VKey, kMsg.Scan, kMsg.Flags);
                    }
                    break;
                case MessageType.Mouse:
                    if (AllowControl)
                    {
                        var mMsg = msg as MouseMessage;
                        WinApis.MouseEvent(mMsg.Flags, mMsg.X, mMsg.Y, mMsg.Delta);
                    }
                    break;
                case MessageType.FrameDirty:
                    FrameDirty = true;
                    break;
                case MessageType.VisualRegion:
                    OperatorRect = ((VisualRegionMessage)msg).Region;
                    break;
                case MessageType.Clipboard:
                    UninstallNotifier();
                    Clipboard.SetText(((ClipboardMessage)msg).Text);
                    InstallNotifier();
                    break;
                case MessageType.DelayTest:
                    Transfer.Send(msg);
                    break;
                case MessageType.Definition:
                    if(IsHighDefinition != ((DefinitionMessage)msg).IsHighDefinition)
                    {
                        IsHighDefinition = ((DefinitionMessage)msg).IsHighDefinition;
                        FrameDirty = true;
                    }                    
                    break;
            }
        }
        
        private void OnRemoteShutdown()
        {
            var handler = RemoteShutdown;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private unsafe void ScreenMoniter()
        {
            FrameDirty = true;

            // 通知屏幕尺寸
            var siMsg = new ScreenInfoMessage(ScreenRect.Width, ScreenRect.Height);
            while(!Transfer.Send(siMsg));
            
            // 参考帧画布
            var refFrame = new Bitmap(ScreenRect.Width, ScreenRect.Height);
            var refG = Graphics.FromImage(refFrame);            

            // 画布
            var current = new Bitmap(ScreenRect.Width, ScreenRect.Height);
            var curG = Graphics.FromImage(current);

            // 光标句柄
            IntPtr lastCursor = IntPtr.Zero;

            // 上一帧 ID
            int lastFrameId = 0;

            while (true)
            {
                try
                {
                    if (Transfer.IsShutdown)
                    {
                        break;
                    }

                    if (FrameDirty)
                    {// 抓取并发送参考帧
                        refG.CopyFromScreen(0, 0, 0, 0, ScreenRect.Size);

                        // 先抓图,若出现抓不到图时不清楚标记
                        FrameDirty = false;
                        var refMsg = new ScreenFrameMessage(0, 0, refFrame, IsHighDefinition);
                        Transfer.Send(refMsg);
                        lastFrameId = refMsg.ID;
                    }
                    else
                    {
                        var oRect = OperatorRect;
                        if (oRect.Width > 0 && oRect.Height > 0)
                        {
                            // 截操作区图
                            curG.CopyFromScreen(oRect.Location, oRect.Location, oRect.Size);
                            var currentData = current.LockBits(oRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                            var currentPixel = (Int32*)currentData.Scan0;
                            
                            // 锁定参考帧
                            var refData = refFrame.LockBits(oRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                            var refPixel = (Int32*)refData.Scan0;

                            // 匹配像素变化
                            ClearUnaltered(currentPixel, refPixel, oRect.Width, oRect.Height, current.Width);

                            // 清除空白边
                            var rect = FindClipRegion(currentPixel, oRect.Width, oRect.Height, current.Width);

                            current.UnlockBits(currentData);
                            refFrame.UnlockBits(refData);

                            if (rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0)
                            {
                                var tmp = current.Clone(new Rectangle(rect.X + oRect.X, rect.Y + oRect.Y, rect.Width, rect.Height), PixelFormat.Format32bppArgb);
                                var sfMsg = new ScreenFrameMessage(rect.X + oRect.X, rect.Y + oRect.Y, tmp, lastFrameId, IsHighDefinition);
                                Transfer.Send(sfMsg);
                                lastFrameId = sfMsg.ID;
                            }
                        }
                    }

                    // 更新鼠标指针形状
                    var cursor = WinApis.GetCursorHandle();
                    if (lastCursor != cursor)
                    {
                        lastCursor = cursor;
                        var ciMsg = new CursorInfoMessage(cursor);
                        Transfer.Send(ciMsg);
                    }                    
                }
                catch(Exception)
                {
                }

                Thread.Sleep(FrameDelay);
            }
        }

        private unsafe void ClearUnaltered(Int32* currentPixel, Int32* refPixel, int width, int height, int stride)
        {
            var skip = stride - width;
            Int32 vcur;
            while (height-- > 0)
            {
                var w = width;
                while (w-- > 0)
                {
                    vcur = *currentPixel ^ *refPixel;
                    *refPixel = *refPixel ^ vcur;
                    if (vcur == 0)
                    {
                        *currentPixel = vcur;
                    }
                    refPixel++;
                    currentPixel++;
                }
                currentPixel += skip;
                refPixel += skip;
            }
        }

        private unsafe Rectangle FindClipRegion(Int32* currentPixel, int width, int height, int stride)
        {
            int sx = -1,sy = -1,ex = -1,ey = -1;

            // 找左右边
            for (int lx = 0, rx = width - 1; lx <= rx;)
            {
                for (int y = 0; y < height; y++)
                {
                    if (sx == -1 && currentPixel[y * stride + lx] != 0)
                    {
                        sx = lx;
                    }
                    if (ex == -1 && currentPixel[y * stride + rx] != 0)
                    {
                        ex = rx;
                    }
                }
                if (sx == -1) lx++;
                if (ex == -1) rx--;
                if (sx != -1 && ex != -1) break;
            }

            // 找上下边
            for (int ty = 0, by = height - 1; ty <= by;)
            {
                for (int x = 0; x < width; x++)
                {
                    if (sy == -1 && currentPixel[ty * stride + x] != 0)
                    {
                        sy = ty;
                    }
                    if (ey == -1 && currentPixel[by * stride + x] != 0)
                    {
                        ey = by;
                    }
                }
                if (sy == -1) ty++;
                if (ey == -1) by--;
                if (sy != -1 && ey != -1) break;
            }

            return new Rectangle(sx, sy, ex - sx + 1, ey - sy + 1);
        }

        #region 剪切板监控

        private ClipboardChangedNotifier cbdNotifier = new ClipboardChangedNotifier();

        private void InstallNotifier()
        {
            cbdNotifier.ClipboardChanged += CbdNotifier_ClipboardChanged;
        }

        private void UninstallNotifier()
        {
            cbdNotifier.ClipboardChanged -= CbdNotifier_ClipboardChanged;
        }

        private void CbdNotifier_ClipboardChanged(object sender, EventArgs e)
        {
            if (Transfer != null && Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                var msg = new ClipboardMessage(text);
                Transfer.Send(msg);
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; 

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Host() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
