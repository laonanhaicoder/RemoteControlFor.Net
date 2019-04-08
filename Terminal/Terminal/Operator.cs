using System.Windows.Forms;
using TerminalCommunication;
using System.Drawing;
using System.Diagnostics;

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TerminalCommunication.Properties;

namespace TerminalCommunication
{
    public sealed partial class Operator : UserControl
    {
        private ITransfer Transfer;
        private Bitmap currentFrame;
        private bool IsConnected = false;
        private object currentFrameLockObj = new object();
        private Size CanvasSize;
        private Size canvasVisualSize;

        private Size CanvasVisualSize
        {
            get { return canvasVisualSize; }
            set
            {
                if(canvasVisualSize.Width != value.Width 
                    || canvasVisualSize.Height != value.Height)
                {
                    canvasVisualSize = value;
                    OnCanvasVisualSizeChanged();
                }
            }
        }

        public Operator()
        {
            InitializeComponent();
            
            Transfer = new TcpTransfer();
            Transfer.Connected += Transfer_Connected;
            Transfer.Received += Transfer_Received;

            keybdHookProc = KeyboardHookProc;

            InstallNotifier();

            InitBkgs();
        }

        #region 对外接口
        
        public void Connect(string host, int port)
        {
            Transfer.Connect(host, port);
        }

        public void ConnectWithToken(string host, int port, string token)
        {
            Transfer.ConnectWithToken(host, port, token);
        }

        public void Shutdown()
        {
            tmrBkg.Stop();

            Transfer.Shutdown();
        }

        public bool AllowControl { get; private set; }

        /// <summary>
        /// 远程已关闭
        /// </summary>
        public event EventHandler<EventArgs> RemoteShutdown;

        #endregion

        #region 显示相关

        private bool scrolled = false;

        private int LastFrameID = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            tmrBkg.Start();
        }
        
        private void Transfer_Received(object sender, TransferEventArgs e)
        {
            OnReceived(e.Message);
        }

        private void Transfer_Connected(object sender, TransferEventArgs e)
        {
            OnConnected();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode || !IsConnected)
            {
                base.OnPaintBackground(e);
                return;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (currentFrame != null)
            {
                lock (currentFrameLockObj)
                {
                    // 计算坐标
                    var dx = CanvasSize.Width > CanvasVisualSize.Width ? 0 : (CanvasVisualSize.Width - CanvasSize.Width) / 2;
                    var dy = CanvasSize.Height > CanvasVisualSize.Height ? 0 : (CanvasVisualSize.Height - CanvasSize.Height) / 2;
                    var sx = CanvasSize.Width > CanvasVisualSize.Width ? hScrollBar.Value : 0;
                    var sy = CanvasSize.Height > CanvasVisualSize.Height ? vScrollBar.Value : 0;
                    var w = CanvasSize.Width > CanvasVisualSize.Width ? CanvasVisualSize.Width : CanvasSize.Width;
                    var h = CanvasSize.Height > CanvasVisualSize.Height ? CanvasVisualSize.Height : CanvasSize.Height;

                    e.Graphics.DrawImage(currentFrame, new Rectangle(dx, dy, w, h), new Rectangle(sx, sy, w, h), GraphicsUnit.Pixel);
                }                
            }
        }

        private void ResetVisualRegion()
        {
            if (Transfer != null)
            {
                var x = hScrollBar.Visible ? hScrollBar.Value : 0;
                var y = vScrollBar.Visible ? vScrollBar.Value : 0;
                var w = CanvasSize.Width > CanvasVisualSize.Width ? CanvasVisualSize.Width : CanvasSize.Width;
                var h = CanvasSize.Height > CanvasVisualSize.Height ? CanvasVisualSize.Height : CanvasSize.Height;

                var reg = new Rectangle(x, y, w, h);
                var msg = new VisualRegionMessage(reg);
                Transfer.Send(msg);
            }
        }
        
        private void OnCanvasVisualSizeChanged()
        {
            ResetVisualRegion();

            ResetControlBarLocation(true);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            ResetScrollBarVisual();
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            scrolled = true;
        }

        private void SetCursor(IntPtr handle)
        {
            Invoke(new Action(() =>
            {
                try
                {
                    Cursor = new Cursor(handle);
                }
                catch (Exception)
                {
                    Cursor = Cursors.Default;
                }
            })); 
        }
        
        private void ResetScrollBarVisual()
        {
            var oldHV = hScrollBar.Visible;
            var oldVV = vScrollBar.Visible;

            hScrollBar.Visible = (CanvasSize.Width > Width
                || (CanvasSize.Height > Height && CanvasSize.Width + vScrollBar.Width > Width));

            vScrollBar.Visible = (CanvasSize.Height > Height
                || (CanvasSize.Width > Width && CanvasSize.Height + hScrollBar.Height > Height));

            if (oldHV != hScrollBar.Visible)
            {
                hScrollBar.Value = 0;
            }

            if (oldVV != vScrollBar.Visible)
            {
                vScrollBar.Value = 0;
            }

            var w = vScrollBar.Visible ? Width - vScrollBar.Width : Width;
            var h = hScrollBar.Visible ? Height - hScrollBar.Height : Height;

            CanvasVisualSize = new Size(w, h);

            if (hScrollBar.Visible)
            {
                hScrollBar.Maximum = CanvasSize.Width - w + hScrollBar.LargeChange - 1;
                if (hScrollBar.Value > CanvasSize.Width - w) hScrollBar.Value = CanvasSize.Width - w;
                hScrollBar.Location = new Point(0, Height - hScrollBar.Height - 1);
                hScrollBar.Width = Width;
            }
            if (vScrollBar.Visible)
            {
                vScrollBar.Maximum = CanvasSize.Height - h + vScrollBar.LargeChange - 1;
                if (vScrollBar.Value > CanvasSize.Height - h) vScrollBar.Value = CanvasSize.Height - h;
                vScrollBar.Location = new Point(Width - vScrollBar.Width - 1, 0);
                vScrollBar.Height = Height;
            }

            picResize.Visible = hScrollBar.Visible || vScrollBar.Visible;
            if(picResize.Visible)
            {
                picResize.Location = new Point(Width - vScrollBar.Width - 1, Height - hScrollBar.Height - 1);
            }
        }

        #endregion

        #region 键鼠钩子

        private int keybdHook = 0;
        private HookProc keybdHookProc;
        private IntPtr hInstance;
        
        protected override void OnMouseLeave(EventArgs e)
        {
            UninstallHook();

            base.OnLostFocus(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            InstallHook();

            base.OnGotFocus(e);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Focus();
        }

        private void InstallHook()
        {
            if (keybdHook == 0)
            {
                hInstance = WinApis.GetInstanceHandle(typeof(Operator));

                keybdHook = WinApis.InstallGlobalHook(HookType.WH_KEYBOARD_LL, keybdHookProc, hInstance);
#if DEBUG
                var errorCode = keybdHook == 0 ? Marshal.GetLastWin32Error() : 0;
                Debug.WriteLine($"keybdHook={keybdHook},errorCode={errorCode}");
#endif
            }
        }

        private void UninstallHook()
        {
            if(keybdHook != 0)
            {
                WinApis.UnhookWindowsHookEx(keybdHook);
                keybdHook = 0;
#if DEBUG
                Debug.WriteLine($"Keyboard hook uninstalled.");
#endif
            }
        }
        
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && AllowControl)
            {
                KeyboardHookStruct kbArg = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                var vkey = kbArg.vkCode;
                var scan = kbArg.scanCode;
                var flags = (wParam & 0x1) << 1; 
#if DEBUG
                Debug.WriteLine($"now at {DateTime.Now.ToString("hh:mm:ss.ffff")},Keyboard Message vkey={vkey},scan={scan},flags={flags},wParam={wParam},lParam={lParam}");
#endif
                var msg = new KeyboardMessage(vkey, scan, flags);
                Transfer.Send(msg);

                return 1;
            }
            else
            {
                return WinApis.CallNextHookEx(keybdHook, nCode, wParam, lParam);
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (AllowControl)
            {
                var flag = MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        flag |= MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
                        break;
                    case MouseButtons.Middle:
                        flag |= MouseEventFlags.MOUSEEVENTF_MIDDLEDOWN;
                        break;
                    case MouseButtons.Right:
                        flag |= MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
                        break;
                }

                var x = CanvasSize.Width > canvasVisualSize.Width 
                        ? hScrollBar.Value + e.X 
                        : e.X - (canvasVisualSize.Width - CanvasSize.Width) / 2;
                var y = CanvasSize.Height > canvasVisualSize.Height
                        ? vScrollBar.Value + e.Y
                        : e.Y - (canvasVisualSize.Height - CanvasSize.Height) / 2;

                if (x < 0 || y < 0) return;

#if DEBUG
                Debug.WriteLine($"now at {DateTime.Now.ToString("hh:mm:ss.ffff")},Mouse down on ({x},{y}),flag is {flag}");
#endif
                var msg = new MouseMessage(flag, x, y, 0);
                Transfer.Send(msg);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (AllowControl)
            {
                var flag = MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        flag |= MouseEventFlags.MOUSEEVENTF_LEFTUP;
                        break;
                    case MouseButtons.Middle:
                        flag |= MouseEventFlags.MOUSEEVENTF_MIDDLEUP;
                        break;
                    case MouseButtons.Right:
                        flag |= MouseEventFlags.MOUSEEVENTF_RIGHTUP;
                        break;
                }

                var x = CanvasSize.Width > canvasVisualSize.Width
                        ? hScrollBar.Value + e.X
                        : e.X - (canvasVisualSize.Width - CanvasSize.Width) / 2;
                var y = CanvasSize.Height > canvasVisualSize.Height
                        ? vScrollBar.Value + e.Y
                        : e.Y - (canvasVisualSize.Height - CanvasSize.Height) / 2;

                if (x < 0 || y < 0) return;

#if DEBUG
                Debug.WriteLine($"now at {DateTime.Now.ToString("hh:mm:ss.ffff")},Mouse up on ({x},{y}),flag is {flag}");
#endif
                var msg = new MouseMessage(flag, x, y, 0);
                Transfer.Send(msg);
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (AllowControl)
            {
                var flag = MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_MOVE;

                var x = CanvasSize.Width > canvasVisualSize.Width
                        ? hScrollBar.Value + e.X
                        : e.X - (canvasVisualSize.Width - CanvasSize.Width) / 2;
                var y = CanvasSize.Height > canvasVisualSize.Height
                        ? vScrollBar.Value + e.Y
                        : e.Y - (canvasVisualSize.Height - CanvasSize.Height) / 2;

                if (x < 0 || y < 0) return;

#if DEBUG
                Debug.WriteLine($"now at {DateTime.Now.ToString("hh:mm:ss.ffff")},Mouse move on ({x},{y}),flag is {flag}");
#endif
                var msg = new MouseMessage(flag, x, y, 0);
                Transfer.Send(msg);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (AllowControl)
            {
                var flag = MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_WHEEL;

                var x = CanvasSize.Width > canvasVisualSize.Width
                        ? hScrollBar.Value + e.X
                        : e.X - (canvasVisualSize.Width - CanvasSize.Width) / 2;
                var y = CanvasSize.Height > canvasVisualSize.Height
                        ? vScrollBar.Value + e.Y
                        : e.Y - (canvasVisualSize.Height - CanvasSize.Height) / 2;

                if (x < 0 || y < 0) return;

#if DEBUG
                Debug.WriteLine($"now at {DateTime.Now.ToString("hh:mm:ss.ffff")},Mouse wheel on ({x},{y}),flag is {flag}");
#endif
                var msg = new MouseMessage(flag, x, y, e.Delta);
                Transfer.Send(msg);
            }
            // 屏蔽默认滚轮处理
            //base.OnMouseWheel(e);
        }

        #endregion

        #region 消息处理

        private int frameCount = 0;

        private void OnConnected()
        {
            IsConnected = true;

            AllowControl = true;
        }

        private void OnRemoteShutdown()
        {
            var handler = RemoteShutdown;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnReceived(MessageBase msg)
        {
            if (msg == null)
            {// 远程断开,本地生成空消息
                Shutdown();
                OnRemoteShutdown();
                return;
            }

            switch (msg.Type)
            {
                case MessageType.AllowControl:
                    AllowControl = ((AllowControlMessage)msg).AllowControl;
                    break;
                case MessageType.ScreenInfo:
                    // 设置虚拟工作区大小
                    var siMsg = msg as ScreenInfoMessage;
                    lock (currentFrameLockObj)
                    {
                        currentFrame = new Bitmap(siMsg.Width, siMsg.Height);
                    }
                    Invoke(new Action(() =>
                    {
                        CanvasSize = new Size(siMsg.Width, siMsg.Height);
                        ResetScrollBarVisual();

                        Invalidate();
                    }));
                    break;
                case MessageType.ScreenFrame:
                    var sfMsg = msg as ScreenFrameMessage;
                    frameCount++;
                    if (sfMsg.LastFrameID != LastFrameID)
                    {// 数据流断帧,请求全幅重发
                        LastFrameID = 0;
                        var fdMsg = new FrameDirtyMessage();
                        Transfer.Send(fdMsg);
                    }
                    else
                    {// 更新上一帧的ID
                        LastFrameID = sfMsg.ID;
                    }
                    lock (currentFrameLockObj)
                    {
                        var canvas = currentFrame;
                        if (canvas != null)
                        {// 不管帧是否连续都绘制
                            using (var g = Graphics.FromImage(canvas))
                            {
                                g.DrawImage(sfMsg.Data, new Point(sfMsg.OffsetX, sfMsg.OffsetY));
                            }
                        }
                    }
                    Invalidate();
                    break;
                case MessageType.Clipboard:
                    UninstallNotifier();
                    Clipboard.SetText(((ClipboardMessage)msg).Text);
                    InstallNotifier();
                    break;
                case MessageType.CursorInfo:
                    SetCursor(((CursorInfoMessage)msg).Handle);
                    break;
                case MessageType.DelayTest:
                    Invoke(new Action(() => { NetworkDelay = ((DelayTestMessage)msg).Delay; }));
                    break;
            }
        }

        #endregion

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
            if(Transfer != null && Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                var msg = new ClipboardMessage(text);
                Transfer.Send(msg);
            }
        }

        #endregion

        #region 控制栏

        private string remoteName;

        private DateTime ctrlBarUnlockTime = DateTime.MinValue;

        public string RemoteName
        {
            get { return remoteName; }
            set
            {
                remoteName = value;
                lblMsg.Text = $"正在控制 {remoteName} 的电脑";
            }
        }
        
        private void btnLock_Click(object sender, EventArgs e)
        {
            ResetControlBarLocation(false);
        }

        private void ResetControlBarLocation(bool realSet)
        {
            if (btnLock.IsChecked)
            {
                pnlControl.Location = new Point((CanvasVisualSize.Width - pnlControl.Width) / 2, 0);
            }
            else 
            {
                if(realSet)
                {
                    pnlControl.Location = new Point((CanvasVisualSize.Width - pnlControl.Width) / 2, -30);

                    ctrlBarUnlockTime = DateTime.MinValue;
                }
                else
                {
                    pnlControl.Location = new Point((CanvasVisualSize.Width - pnlControl.Width) / 2, 0);
                    ctrlBarUnlockTime = DateTime.Now;
                }
            }
        }
        
        private void pnlControl_MouseEnter(object sender, EventArgs e)
        {
            if(!btnLock.IsChecked)
            {
                ResetControlBarLocation(false);
            }
        }
        
        private void btnMode_Click(object sender, EventArgs e)
        {
            if (Transfer != null)
            {
                var msg = new DefinitionMessage(btnMode.IsChecked);
                Transfer.Send(msg);
            }
        }

        private DockStyle DockBackup;
        private int LeftBackup;
        private int TopBackup;
        private int WidthBackup;
        private int HeightBackup;
        private IntPtr ParentHandleBackup;

        private void btnFullScreen_Click(object sender, EventArgs e)
        {
            var form = FindForm();
            SuspendLayout();
            if (btnFullScreen.IsChecked)//全屏 ,按特定的顺序执行
            {
                WinApis.SetFormFullScreen(true);

                form.FormClosed += Form_FormClosed;

                DockBackup = Dock;
                LeftBackup = Left;
                TopBackup = Top;
                WidthBackup = Width;
                HeightBackup = Height;
                ParentHandleBackup = Parent == null ? IntPtr.Zero : Parent.Handle;

                Dock = DockStyle.None;
                Left = 0;
                Top = 0;
                Width = Screen.PrimaryScreen.Bounds.Width;
                Height = Screen.PrimaryScreen.WorkingArea.Height;
                WinApis.SetParent(Handle, IntPtr.Zero);
            }
            else//还原，按特定的顺序执行——窗体状态，窗体边框，设置任务栏和工作区域
            {
                WinApis.SetFormFullScreen(false);

                form.FormClosed -= Form_FormClosed;

                Dock = DockBackup;
                Left = LeftBackup;
                Top = TopBackup;
                Width = WidthBackup;
                Height = HeightBackup;
                WinApis.SetParent(Handle, ParentHandleBackup);
            }
            ResetScrollBarVisual();
            ResumeLayout(false);
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (btnFullScreen.IsChecked)
            {
                WinApis.SetFormFullScreen(false);
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            Shutdown();
            FindForm()?.Close();
        }
        
        // 上次更新时间
        private DateTime lastUpdateTime = DateTime.MinValue;

        private int fps;
        public int Fps
        {
            get { return fps; }
            private set
            {
                fps = value;
                lblFps.Text = $"{fps} fps";
            }
        }

        private int networkDelay;
        public int NetworkDelay
        {
            get { return networkDelay; }
            private set
            {
                networkDelay = value;
                lblDelay.Text = $"{networkDelay} ms";
                if(networkDelay > 500)
                {
                    lblDelay.ForeColor = Color.Red;
                }
                else
                {
                    lblDelay.ForeColor = Color.LightGray;
                }
            }
        }

        private int networkFlow;
        public int NetworkFlow
        {
            get { return networkFlow; }
            private set
            {
                networkFlow = value;
                if(networkFlow < 512 * 1024)
                {
                    lblFlow.Text = $"{((double)networkFlow / 1024).ToString("F1")} KB/s";
                    lblFlow.ForeColor = Color.LightGray;
                }
                else
                {
                    lblFlow.Text = $"{((double)networkFlow / 1024 / 1024).ToString("F1")} MB/s";
                    lblFlow.ForeColor = Color.Red;
                }
            }
        }

        #endregion

        #region 连接背景

        private int bkgIndex = 0;

        private List<Image> Bkgs = new List<Image>();
        private void InitBkgs()
        {
            Bkgs.Add(Resources.bkg1);
            Bkgs.Add(Resources.bkg2);
            Bkgs.Add(Resources.bkg3);
            Bkgs.Add(Resources.bkg4);
            Bkgs.Add(Resources.bkg5);
            Bkgs.Add(Resources.bkg6);
            Bkgs.Add(Resources.bkg7);
            Bkgs.Add(Resources.bkg8);
            Bkgs.Add(Resources.bkg9);
            Bkgs.Add(Resources.bkg10);
            Bkgs.Add(Resources.bkg11);
        }

        #endregion

        #region 综合定时器

        private void tmrBkg_Tick(object sender, EventArgs e)
        {
            if (!IsConnected)
            {
                BackgroundImage = Bkgs[bkgIndex];
                bkgIndex++;
                bkgIndex %= Bkgs.Count;
            }
            if (scrolled)
            {
                scrolled = false;

                Invalidate();

                ResetVisualRegion();

                ResetControlBarLocation(true);
            }

            if(lastUpdateTime == DateTime.MinValue)
            {
                lastUpdateTime = DateTime.Now;
                frameCount = 0;
                if(Transfer != null)
                {
                    Transfer.ReceiveCount = 0;
                }
            }
            else if(IsConnected)
            {
                var now = DateTime.Now;
                var interval = (int)(now - lastUpdateTime).TotalMilliseconds;
                if(interval > 1000)
                {
                    lastUpdateTime = now;
                    Fps = frameCount;
                    frameCount = 0;
                    NetworkFlow = (int)((Transfer.ReceiveCount + Transfer.SendCount) * 1000 / interval);
                    if(Transfer.ReceiveCount > 100 && Transfer.SendCount > 100)
                    {
                        picFlowState.Image = Resources.flowupdown;
                    }
                    else if(Transfer.ReceiveCount > 100)
                    {
                        picFlowState.Image = Resources.flowdown;
                    }
                    else if (Transfer.SendCount > 100)
                    {
                        picFlowState.Image = Resources.flowup;
                    }
                    else if(Transfer.ReceiveCount + Transfer.SendCount > 100)
                    {
                        picFlowState.Image = Resources.flowupdown;
                    }
                    else
                    {
                        picFlowState.Image = Resources.flownone;
                    }

                    Transfer.ReceiveCount = 0;
                    Transfer.SendCount = 0;

                    // 测算延迟
                    var dtMsg = new DelayTestMessage();
                    Transfer.Send(dtMsg);
                }
            }

            if(!btnLock.IsChecked && ctrlBarUnlockTime != DateTime.MinValue && DateTime.Now - ctrlBarUnlockTime > TimeSpan.FromSeconds(3))
            {
                ResetControlBarLocation(true);
            }
        }

        #endregion
    }
}
