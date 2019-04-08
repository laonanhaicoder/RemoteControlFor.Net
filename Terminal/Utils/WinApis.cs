using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TerminalCommunication
{
    internal static class WinApis
    {
        public static readonly int ScreenWidth;
        public static readonly int ScreenHeight;

        static WinApis()
        {
            ScreenWidth = GetSystemMetrics(SM_CXSCREEN);
            ScreenHeight = GetSystemMetrics(SM_CYSCREEN);
        }

        public static void MouseEvent(MouseEventFlags flags, int x, int y, int delta)
        {
            mouse_event((int)flags, x * 65535 / ScreenWidth,
                                y * 65535 / ScreenHeight, delta, 0);
        }

        public static void KeybdEvent(int vkCode, int scan, int flags)
        {
            keybd_event((byte)vkCode, (byte)scan, flags, 0);
        }

        public static IntPtr GetInstanceHandle(Type t)
        {
            return GetModuleHandle(t.Module.Name);
        }

        public static int InstallGlobalHook(HookType idHook, HookProc lpfn, IntPtr hInstance)
        {
            return SetWindowsHookEx(idHook, lpfn, hInstance, 0);
        }

        public static IntPtr GetCursorHandle()
        {
            CURSORINFO pci = new CURSORINFO();
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
            if (GetCursorInfo(out pci))
            {
                return pci.hCursor;
            }

            return IntPtr.Zero;
        }

        private static Rectangle _ScreenNormalWorkAreaRect = Rectangle.Empty;
        /// <summary>  
        /// 设置全屏或这取消全屏  
        /// </summary>  
        /// <param name="fullscreen">true:全屏 false:恢复</param>  
        /// <param name="rectOld">设置的时候，此参数返回原始尺寸，恢复时用此参数设置恢复</param>  
        /// <returns>设置结果</returns>  
        public static bool SetFormFullScreen(bool fullscreen)//, ref Rectangle rectOld
        {
            Int32 hwnd = 0;
            hwnd = FindWindow("Shell_TrayWnd", null);//获取任务栏的句柄

            if (hwnd == 0) return false;

            if (fullscreen)//全屏
            {
                ShowWindow(hwnd, SW_HIDE);//隐藏任务栏

                SystemParametersInfo(SPI_GETWORKAREA, 0, ref _ScreenNormalWorkAreaRect, SPIF_UPDATEINIFILE);//get  屏幕范围
                Rectangle rectFull = Screen.PrimaryScreen.Bounds;//全屏范围
                SystemParametersInfo(SPI_SETWORKAREA, 0, ref rectFull, SPIF_UPDATEINIFILE);//窗体全屏幕显示
            }
            else//还原 
            {
                SystemParametersInfo(SPI_SETWORKAREA, 0, ref _ScreenNormalWorkAreaRect, SPIF_UPDATEINIFILE);//窗体还原

                ShowWindow(hwnd, SW_SHOW);//显示任务栏
            }
            return true;
        }

        #region APIs

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(int hhk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int CallNextHookEx(int hhk, int nCode, int wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        private static extern bool GetCursorInfo(out CURSORINFO pci);
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern Int32 ShowWindow(Int32 hwnd, Int32 nCmdShow);
        public const Int32 SW_SHOW = 5; public const Int32 SW_HIDE = 0;

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        private static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, ref Rectangle lpvParam, Int32 fuWinIni);
        public const Int32 SPIF_UPDATEINIFILE = 0x1;
        public const Int32 SPI_SETWORKAREA = 47;
        public const Int32 SPI_GETWORKAREA = 48;

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern Int32 FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct CURSORINFO
    {
        public int cbSize;
        public int flags;
        public IntPtr hCursor;
        public POINT ptScreenPos;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class MouseHookStruct
    {
        public POINT pt;
        public int hwnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    /// <summary>  
    /// 声明键盘钩子的封送结构类型  
    /// </summary>  
    [StructLayout(LayoutKind.Sequential)]
    internal class KeyboardHookStruct
    {
        public int vkCode;//表示一个1到254间的虚拟键盘码  
        public int scanCode;//表示硬件扫描码  
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    [Flags]
    internal enum MouseEventFlags
    {
        /// <summary>
        /// 模拟鼠标移动
        /// </summary>
        MOUSEEVENTF_MOVE = 0x0001,
        /// <summary>
        /// 模拟鼠标左键按下
        /// </summary>
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        /// <summary>
        /// 模拟鼠标左键抬起
        /// </summary>
        MOUSEEVENTF_LEFTUP = 0x0004,
        /// <summary>
        /// 鼠标绝对位置
        /// </summary>
        MOUSEEVENTF_ABSOLUTE = 0x8000,
        /// <summary>
        /// 模拟鼠标右键按下 
        /// </summary>
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        /// <summary>
        /// 模拟鼠标右键抬起 
        /// </summary>
        MOUSEEVENTF_RIGHTUP = 0x0010,
        /// <summary>
        /// 模拟鼠标中键按下
        /// </summary>
        MOUSEEVENTF_MIDDLEDOWN = 0x0020,
        /// <summary>
        /// 模拟鼠标中键抬起
        /// </summary>
        MOUSEEVENTF_MIDDLEUP = 0x0040,
        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        MOUSEEVENTF_WHEEL = 0x0800
    }

    /// <summary>
    /// 设置的钩子类型
    /// </summary>
    internal enum HookType : int
    {
        /// <summary>
        /// WH_MSGFILTER 和 WH_SYSMSGFILTER Hooks使我们可以监视菜单，滚动 
        ///条，消息框，对话框消息并且发现用户使用ALT+TAB or ALT+ESC 组合键切换窗口。 
        ///WH_MSGFILTER Hook只能监视传递到菜单，滚动条，消息框的消息，以及传递到通 
        ///过安装了Hook子过程的应用程序建立的对话框的消息。WH_SYSMSGFILTER Hook 
        ///监视所有应用程序消息。 
        /// 
        ///WH_MSGFILTER 和 WH_SYSMSGFILTER Hooks使我们可以在模式循环期间 
        ///过滤消息，这等价于在主消息循环中过滤消息。 
        ///    
        ///通过调用CallMsgFilter function可以直接的调用WH_MSGFILTER Hook。通过使用这 
        ///个函数，应用程序能够在模式循环期间使用相同的代码去过滤消息，如同在主消息循 
        ///环里一样
        /// </summary>
        WH_MSGFILTER = -1,
        /// <summary>
        /// WH_JOURNALRECORD Hook用来监视和记录输入事件。典型的，可以使用这 
        ///个Hook记录连续的鼠标和键盘事件，然后通过使用WH_JOURNALPLAYBACK Hook 
        ///来回放。WH_JOURNALRECORD Hook是全局Hook，它不能象线程特定Hook一样 
        ///使用。WH_JOURNALRECORD是system-wide local hooks，它们不会被注射到任何行 
        ///程地址空间
        /// </summary>
        WH_JOURNALRECORD = 0,
        /// <summary>
        /// WH_JOURNALPLAYBACK Hook使应用程序可以插入消息到系统消息队列。可 
        ///以使用这个Hook回放通过使用WH_JOURNALRECORD Hook记录下来的连续的鼠 
        ///标和键盘事件。只要WH_JOURNALPLAYBACK Hook已经安装，正常的鼠标和键盘 
        ///事件就是无效的。WH_JOURNALPLAYBACK Hook是全局Hook，它不能象线程特定 
        ///Hook一样使用。WH_JOURNALPLAYBACK Hook返回超时值，这个值告诉系统在处 
        ///理来自回放Hook当前消息之前需要等待多长时间（毫秒）。这就使Hook可以控制实 
        ///时事件的回放。WH_JOURNALPLAYBACK是system-wide local hooks，它们不会被 
        ///注射到任何行程地址空间
        /// </summary>
        WH_JOURNALPLAYBACK = 1,
        /// <summary>
        /// 在应用程序中，WH_KEYBOARD Hook用来监视WM_KEYDOWN and  
        ///WM_KEYUP消息，这些消息通过GetMessage or PeekMessage function返回。可以使 
        ///用这个Hook来监视输入到消息队列中的键盘消息
        /// </summary>
        WH_KEYBOARD = 2,
        /// <summary>
        /// 应用程序使用WH_GETMESSAGE Hook来监视从GetMessage or PeekMessage函 
        ///数返回的消息。你可以使用WH_GETMESSAGE Hook去监视鼠标和键盘输入，以及 
        ///其它发送到消息队列中的消息
        /// </summary>
        WH_GETMESSAGE = 3,
        /// <summary>
        /// 监视发送到窗口过程的消息，系统在消息发送到接收窗口过程之前调用
        /// </summary>
        WH_CALLWNDPROC = 4,
        /// <summary>
        /// 在以下事件之前，系统都会调用WH_CBT Hook子过程，这些事件包括： 
        ///1. 激活，建立，销毁，最小化，最大化，移动，改变尺寸等窗口事件； 
        ///2. 完成系统指令； 
        ///3. 来自系统消息队列中的移动鼠标，键盘事件； 
        ///4. 设置输入焦点事件； 
        ///5. 同步系统消息队列事件。
        ///Hook子过程的返回值确定系统是否允许或者防止这些操作中的一个
        /// </summary>
        WH_CBT = 5,
        /// <summary>
        /// WH_MSGFILTER 和 WH_SYSMSGFILTER Hooks使我们可以监视菜单，滚动 
        ///条，消息框，对话框消息并且发现用户使用ALT+TAB or ALT+ESC 组合键切换窗口。 
        ///WH_MSGFILTER Hook只能监视传递到菜单，滚动条，消息框的消息，以及传递到通 
        ///过安装了Hook子过程的应用程序建立的对话框的消息。WH_SYSMSGFILTER Hook 
        ///监视所有应用程序消息。 
        /// 
        ///WH_MSGFILTER 和 WH_SYSMSGFILTER Hooks使我们可以在模式循环期间 
        ///过滤消息，这等价于在主消息循环中过滤消息。 
        ///    
        ///通过调用CallMsgFilter function可以直接的调用WH_MSGFILTER Hook。通过使用这 
        ///个函数，应用程序能够在模式循环期间使用相同的代码去过滤消息，如同在主消息循 
        ///环里一样
        /// </summary>
        WH_SYSMSGFILTER = 6,
        /// <summary>
        /// WH_MOUSE Hook监视从GetMessage 或者 PeekMessage 函数返回的鼠标消息。 
        ///使用这个Hook监视输入到消息队列中的鼠标消息
        /// </summary>
        WH_MOUSE = 7,
        /// <summary>
        /// 当调用GetMessage 或 PeekMessage 来从消息队列种查询非鼠标、键盘消息时
        /// </summary>
        WH_HARDWARE = 8,
        /// <summary>
        /// 在系统调用系统中与其它Hook关联的Hook子过程之前，系统会调用 
        ///WH_DEBUG Hook子过程。你可以使用这个Hook来决定是否允许系统调用与其它 
        ///Hook关联的Hook子过程
        /// </summary>
        WH_DEBUG = 9,
        /// <summary>
        /// 外壳应用程序可以使用WH_SHELL Hook去接收重要的通知。当外壳应用程序是 
        ///激活的并且当顶层窗口建立或者销毁时，系统调用WH_SHELL Hook子过程。 
        ///WH_SHELL 共有５钟情况： 
        ///1. 只要有个top-level、unowned 窗口被产生、起作用、或是被摧毁； 
        ///2. 当Taskbar需要重画某个按钮； 
        ///3. 当系统需要显示关于Taskbar的一个程序的最小化形式； 
        ///4. 当目前的键盘布局状态改变； 
        ///5. 当使用者按Ctrl+Esc去执行Task Manager（或相同级别的程序）。 
        ///
        ///按照惯例，外壳应用程序都不接收WH_SHELL消息。所以，在应用程序能够接 
        ///收WH_SHELL消息之前，应用程序必须调用SystemParametersInfo function注册它自 
        ///己
        /// </summary>
        WH_SHELL = 10,
        /// <summary>
        /// 当应用程序的前台线程处于空闲状态时，可以使用WH_FOREGROUNDIDLE  
        ///Hook执行低优先级的任务。当应用程序的前台线程大概要变成空闲状态时，系统就 
        ///会调用WH_FOREGROUNDIDLE Hook子过程
        /// </summary>
        WH_FOREGROUNDIDLE = 11,
        /// <summary>
        /// 监视发送到窗口过程的消息，系统在消息发送到接收窗口过程之后调用
        /// </summary>
        WH_CALLWNDPROCRET = 12,
        /// <summary>
        /// 监视输入到线程消息队列中的键盘消息
        /// </summary>
        WH_KEYBOARD_LL = 13,
        /// <summary>
        /// 监视输入到线程消息队列中的鼠标消息
        /// </summary>
        WH_MOUSE_LL = 14
    }

    internal delegate int HookProc(int nCode, int wParam, IntPtr lParam);
}
