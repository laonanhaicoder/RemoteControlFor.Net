using System;
using System.Windows.Forms;

namespace TerminalCommunication
{
    internal sealed class ClipboardChangedNotifier : Control
    {
        private IntPtr nextClipboardViewer;

        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;

        public ClipboardChangedNotifier()
        {
            nextClipboardViewer = WinApis.SetClipboardViewer(Handle);
        }

        public event EventHandler<EventArgs> ClipboardChanged;

        private void OnClipboardChanged()
        {
            var handler = ClipboardChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    OnClipboardChanged();
                    WinApis.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        WinApis.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if(nextClipboardViewer != IntPtr.Zero)
            {
                //从观察链中删除本观察窗口
                WinApis.ChangeClipboardChain(Handle, nextClipboardViewer);
                //将WM_DRAWCLIPBOARD消息传递到下一个观察链中的窗口  
                WinApis.SendMessage(nextClipboardViewer, WM_CHANGECBCHAIN, Handle, nextClipboardViewer);      
            }


            base.Dispose(disposing);
        }
    }
}
