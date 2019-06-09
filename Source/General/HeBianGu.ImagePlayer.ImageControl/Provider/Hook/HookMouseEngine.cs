using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeBianGu.ImagePlayer.ImageControl.Hook
{
    /// <summary> 鼠标钩子程序 </summary>
    public class HookMouseEngine
    {
        /// <summary> 鼠标检测活动将被称为每次回调函数 </summary>
        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //Marshall 从回调的数据
                MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                //检测按钮点击
                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                bool mouseDown = false;
                bool mouseUp = false;

                switch (wParam)
                {
                    case HookType.WM_LBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case HookType.WM_LBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case HookType.WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case HookType.WM_RBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case HookType.WM_RBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case HookType.WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                    case HookType.WM_MOUSEWHEEL:
                        //如果消息是WM_MOUSEWHEEL，MouseData成员是滚轮。
                        //一个轮击定义为WHEEL_DELTA，这是120。
                        //(value >> 16) & 0xffff; 从给定的32位值检索高位字。
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);

                        //TODO: X BUTTONS (这个按钮暂时没有测试)
                        break;
                }

                //生成事件
                MouseEventExtArgs e = new MouseEventExtArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.Point.X,
                                                   mouseHookStruct.Point.Y,
                                                   mouseDelta);

                //鼠标弹起
                if (s_MouseUp != null && mouseUp)
                {
                    s_MouseUp.Invoke(null, e);
                }

                //鼠标按下
                if (s_MouseDown != null && mouseDown)
                {
                    s_MouseDown.Invoke(null, e);
                }

                //单击并点击时触发
                if (s_MouseClick != null && clickCount > 0)
                {
                    s_MouseClick.Invoke(null, e);
                }

                //单击并点击时触发
                if (s_MouseClickExt != null && clickCount > 0)
                {
                    s_MouseClickExt.Invoke(null, e);
                }

                //单击或双击时触发
                if (s_MouseDoubleClick != null && clickCount == 2)
                {
                    s_MouseDoubleClick.Invoke(null, e);
                }

                //鼠标滚轮滚动
                if (s_MouseWheel != null && mouseDelta != 0)
                {
                    s_MouseWheel.Invoke(null, e);
                }

                //触发滚轮滚动
                if ((s_MouseMove != null || s_MouseMoveExt != null) && (m_OldX != mouseHookStruct.Point.X || m_OldY != mouseHookStruct.Point.Y))
                {
                    m_OldX = mouseHookStruct.Point.X;
                    m_OldY = mouseHookStruct.Point.Y;
                    if (s_MouseMove != null)
                    {
                        s_MouseMove.Invoke(null, e);
                    }

                    if (s_MouseMoveExt != null)
                    {
                        s_MouseMoveExt.Invoke(null, e);
                    }
                }

                if (e.Handled)
                {
                    return -1;
                }
            }

            //调用下一个钩子
            return HookApi.CallNextHookEx(s_MouseHookHandle, nCode, wParam, lParam);
        }

        /// <summary> 此字段不客观需要的，但我们需要保持一个供参考的将被传递给非托管代码的委托。为了避免GC把它清理干净 </summary>
        private static HookProc s_MouseDelegate;

        /// <summary> 存储句柄的鼠标钩子程序 </summary>
        private static int s_MouseHookHandle;

        private static int m_OldX;
        private static int m_OldY;

        private static void EnsureSubscribedToGlobalMouseEvents()
        {
            // 安装鼠标钩子
            if (s_MouseHookHandle == 0)
            {
                //为了避免GC把它清理干净。
                s_MouseDelegate = MouseHookProc;

                var mo = HookApi.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                //安装钩子
                s_MouseHookHandle = HookApi.SetWindowsHookEx(
                    HookType.WH_MOUSE_LL,
                    s_MouseDelegate,
                    mo,0);//Assembly.GetExecutingAssembly().GetModules()[0])
                //如果SetWindowsHookEx函数将失败。
                if (s_MouseHookHandle == 0)
                {
                    //返回由上一个非托管函数使用平台调用称为具有DllImportAttribute.SetLastError标志设置返回的错误代码。
                    int errorCode = Marshal.GetLastWin32Error();

                    //初始化并抛出初始化Win32Exception类的新实例使用指定的错误。 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private static void TryUnsubscribeFromGlobalMouseEvents()
        {
            //如果没有注册过钩子
            if (s_MouseClick == null &&
                s_MouseDown == null &&
                s_MouseMove == null &&
                s_MouseUp == null &&
                s_MouseClickExt == null &&
                s_MouseMoveExt == null &&
                s_MouseWheel == null)
            {
                ForceUnsunscribeFromGlobalMouseEvents();
            }
        }

        private static void ForceUnsunscribeFromGlobalMouseEvents()
        {
            if (s_MouseHookHandle != 0)
            {
                //卸载钩子
                int result = HookApi.UnhookWindowsHookEx(s_MouseHookHandle);
                //复位无效句柄
                s_MouseHookHandle = 0;
                //释放用于GC
                s_MouseDelegate = null;
                //如果失败，异常必须抛出
                if (result == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #region 鼠标事件

        private static event MouseEventHandler s_MouseMove;

        /// <summary>
        /// 当鼠标指针移动时触发
        /// </summary>
        public static event MouseEventHandler MouseMove
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseMove += value;
            }

            remove
            {
                s_MouseMove -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgs> s_MouseMoveExt;

        /// <summary>
        /// 当鼠标指针移动时触发
        /// </summary>
        /// <remarks>
        /// 此事件提供类型的扩展参数 <see cref="MouseEventArgs"/> 使您可以 
        /// 在其他程序中对鼠标移动做进一步的处理
        /// </remarks>
        public static event EventHandler<MouseEventExtArgs> MouseMoveExt
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseMoveExt += value;
            }

            remove
            {

                s_MouseMoveExt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseClick;

        /// <summary>
        /// 当点击由鼠标完成时触发
        /// </summary>
        public static event MouseEventHandler MouseClick
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseClick += value;
            }
            remove
            {
                s_MouseClick -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgs> s_MouseClickExt;

        /// <summary>
        /// 当点击由鼠标完成时触发
        /// </summary>
        /// <remarks>
        /// 此事件提供类型的扩展参数 <see cref="MouseEventArgs"/> 使您可以 
        /// 在其他程序中对鼠标点击做进一步的操作
        /// </remarks>
        public static event EventHandler<MouseEventExtArgs> MouseClickExt
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseClickExt += value;
            }
            remove
            {
                s_MouseClickExt -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseDown;

        /// <summary>
        /// 当鼠标按下鼠标按钮时触发
        /// </summary>
        public static event MouseEventHandler MouseDown
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseDown += value;
            }
            remove
            {
                s_MouseDown -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseUp;

        /// <summary>
        /// 当松开鼠标按钮时触发
        /// </summary>
        public static event MouseEventHandler MouseUp
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseUp += value;
            }
            remove
            {
                s_MouseUp -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseWheel;

        /// <summary>
        /// 当滑动鼠标滚轮时发生
        /// </summary>
        public static event MouseEventHandler MouseWheel
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                s_MouseWheel += value;
            }
            remove
            {
                s_MouseWheel -= value;
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }


        private static event MouseEventHandler s_MouseDoubleClick;

        //该双击事件不会直接从Hook钩子提供。触发的双击事件需要监测鼠标事件
        //当它是在Windows中定义多次点击时间的时间间隔来决定触发事件

        /// <summary>
        /// 当双击时，由鼠标完成时触发
        /// </summary>
        public static event MouseEventHandler MouseDoubleClick
        {
            add
            {
                EnsureSubscribedToGlobalMouseEvents();
                if (s_MouseDoubleClick == null)
                {
                    //我们创建了一个计时器，用于监测两次点击之间的时间间隔
                    s_DoubleClickTimer = new Timer
                    {
                        //这个时间间隔将被设置为我们从窗户中检索该值。这是一个从窗户控制面板设置。
                        Interval = HookApi.GetDoubleClickTime(),
                        //如果我们不开始计时。当点击时它会被启动
                        Enabled = false
                    };
                    //我们定义回调函数，定时器
                    s_DoubleClickTimer.Tick += DoubleClickTimeElapsed;
                    //我们先来监听鼠标事件。
                    MouseUp += OnMouseUp;
                }
                s_MouseDoubleClick += value;
            }
            remove
            {
                if (s_MouseDoubleClick != null)
                {
                    s_MouseDoubleClick -= value;
                    if (s_MouseDoubleClick == null)
                    {
                        //停止监听鼠标按键弹起
                        MouseUp -= OnMouseUp;
                        //配置定时器
                        s_DoubleClickTimer.Tick -= DoubleClickTimeElapsed;
                        s_DoubleClickTimer = null;
                    }
                }
                TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        //此字段记录鼠标按钮，因为除了在短的时间间隔必须是也相同的按钮点击。
        private static MouseButtons s_PrevClickedButton;
        //定时监测两次点击之间的时间间隔。
        private static Timer s_DoubleClickTimer;

        private static void DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            //定时器过去了，没有第二次点击发生
            s_DoubleClickTimer.Enabled = false;
            s_PrevClickedButton = MouseButtons.None;
        }

        /// <summary>
        /// 这种方法的目的是监视鼠标点击才能触发一个双击事件，如果点击之间的时间间隔足够短。
        /// </summary>
        /// <param name="sender">始终为空</param>
        /// <param name="e">关于点击的一些信息发生.</param>
        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            //这不应该发生
            if (e.Clicks < 1) { return; }
            //如果第二次单击发生在同一个按钮
            if (e.Button.Equals(s_PrevClickedButton))
            {
                if (s_MouseDoubleClick != null)
                {
                    //触发双击事件
                    s_MouseDoubleClick.Invoke(null, e);
                }
                //停止计时器
                s_DoubleClickTimer.Enabled = false;
                s_PrevClickedButton = MouseButtons.None;
            }
            else
            {
                //如果是第一次点击开始计时
                s_DoubleClickTimer.Enabled = true;
                s_PrevClickedButton = e.Button;
            }
        }

        #endregion
    }

    public class MouseEventExtArgs : MouseEventArgs
    {
        /// <summary> 在初始化MouseEventArgs的类的新实例。 </summary>
        public MouseEventExtArgs(MouseButtons buttons, int clicks, int x, int y, int delta)
            : base(buttons, clicks, x, y, delta)
        { }

        /// <summary> 在初始化MouseEventArgs的类的新实例。 </summary>
        internal MouseEventExtArgs(MouseEventArgs e)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        { }

        private bool m_Handled;

        /// <summary> 将此属性设置为<b>true</b>您的事件处理程序中，以防止其它应用程序事件的进一步处理。 </summary>
        public bool Handled
        {
            get { return m_Handled; }
            set { m_Handled = value; }
        }
    }

    /// <summary>
    /// Point结构定义X和Y坐标。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        /// <summary>
        /// 指定的点的横坐标。
        /// </summary>
        public int X;
        /// <summary>
        /// 指定的点的纵坐标。
        /// </summary>
        public int Y;
    }

    /// <summary>
    /// 该MSLLHOOKSTRUCT结构包含有关一个低级别的键盘输入事件信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseLLHookStruct
    {
        /// <summary>
        /// 指定一个Point结构，它包含的X和Y坐标的光标在屏幕坐标。
        /// </summary>
        public Point Point;

        /// <summary>
        /// 滚轮定义
        /// </summary>
        public int MouseData;

        /// <summary>
        ///指定事件注入标志
        ///指定该事件是否被注入。该值是1，如果该事件被注入;否则，它是0
        ///1-15 保留
        /// </summary>
        public int Flags;

        /// <summary>
        /// 指定此消息的时间戳
        /// </summary>
        public int Time;

        /// <summary>
        /// 指定与该消息相关联的额外信息
        /// </summary>
        public int ExtraInfo;
    }
}
