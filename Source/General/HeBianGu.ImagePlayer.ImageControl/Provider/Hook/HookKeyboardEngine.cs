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
    /// <summary> 键盘钩子程序 </summary>
    public class HookKeyboardEngine
    {
        /// <summary> 此字段不客观需要的，但我们需要保持一个供参考的将被传递给非托管代码的委托。 为了避免GC把它清理干净。 </summary>
        private static HookProc s_KeyboardDelegate;

        /// <summary> 存储句柄键盘钩子程序。 </summary>
        private static int s_KeyboardHookHandle;

        /// <summary> 键盘检测活动将被称为每次回调函数。 </summary>
        private static int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
         {
            //表示如有underlaing事件设置e.Handled标志
            bool handled = false;

            if (nCode >= 0)
            {
                //在lParam中读取KeyboardHookStruct结构
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (s_KeyDown != null && (wParam == HookType.WM_KEYDOWN || wParam == HookType.WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.VirtualKeyCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    s_KeyDown.Invoke(null, e);
                    handled = e.Handled;
                }

                // 按键按下
                if (s_KeyPress != null && wParam == HookType.WM_KEYDOWN)
                {
                    bool isDownShift = ((HookApi.GetKeyState(HookType.VK_SHIFT) & 0x80) == 0x80 ? true : false);
                    bool isDownCapslock = (HookApi.GetKeyState(HookType.VK_CAPITAL) != 0 ? true : false);

                    byte[] keyState = new byte[256];
                    HookApi.GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (HookApi.ToAscii(MyKeyboardHookStruct.VirtualKeyCode,
                              MyKeyboardHookStruct.ScanCode,
                              keyState,
                              inBuffer,
                              MyKeyboardHookStruct.Flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        KeyPressEventArgs e = new KeyPressEventArgs(key);
                        s_KeyPress.Invoke(null, e);
                        handled = handled || e.Handled;
                    }
                }

                // 按键弹起
                if (s_KeyUp != null && (wParam == HookType.WM_KEYUP || wParam == HookType.WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.VirtualKeyCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    s_KeyUp.Invoke(null, e);
                    handled = handled || e.Handled;
                }
            }

            //如果事件在应用程序处理的不换手到其他听众
            if (handled)
                return -1;

            //转发到其它应用程序
            return HookApi.CallNextHookEx(s_KeyboardHookHandle, nCode, wParam, lParam);
        }

        private static void EnsureSubscribedToGlobalKeyboardEvents()
        {
            // 安装键盘钩子，只有当它没有安装，必须安装
            if (s_KeyboardHookHandle == 0)
            {
                //var ss = Assembly.GetExecutingAssembly().GetModules();

                var mo = HookApi.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

                //为了避免GC把它清理干净。
                s_KeyboardDelegate = KeyboardHookProc;

                //安装钩子
                s_KeyboardHookHandle = HookApi.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, s_KeyboardDelegate, mo, 0);

                // Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0])

                //如果SetWindowsHookEx函数将失败。
                if (s_KeyboardHookHandle == 0)
                {
                    //返回由上一个非托管函数使用平台调用称为具有DllImportAttribute.SetLastError标志设置返回的错误代码. 
                    int errorCode = Marshal.GetLastWin32Error();

                    //初始化并抛出初始化Win32Exception类的新实例使用指定的错误。
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private static void TryUnsubscribeFromGlobalKeyboardEvents()
        {
            //如果没有subsribers从钩注册unsubsribe
            if (s_KeyDown == null &&
                s_KeyUp == null &&
                s_KeyPress == null)
            {
                ForceUnsunscribeFromGlobalKeyboardEvents();
            }
        }

        private static void ForceUnsunscribeFromGlobalKeyboardEvents()
        {
            if (s_KeyboardHookHandle != 0)
            {
                //卸载钩子
                int result = HookApi.UnhookWindowsHookEx(s_KeyboardHookHandle);
                //重置句柄
                s_KeyboardHookHandle = 0;
                //清理
                s_KeyboardDelegate = null;
                //如果失败，异常必须抛出
                if (result == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }


        #region 键盘事件

        private static event KeyPressEventHandler s_KeyPress;

        /// <summary> 当一个键被按下时触发 </summary>

        public static event KeyPressEventHandler KeyPress
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                s_KeyPress += value;
            }
            remove
            {
                s_KeyPress -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }


        private static event KeyEventHandler s_KeyUp;

        /// <summary> 当释放键时触发 </summary>
        public static event KeyEventHandler KeyUp
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                s_KeyUp += value;
            }
            remove
            {
                s_KeyUp -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler s_KeyDown;

        /// <summary> 当一个键被按下时触发 </summary>
        public static event KeyEventHandler KeyDown
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                s_KeyDown += value;
            }
            remove
            {
                s_KeyDown -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }


        #endregion
    }

    /// <summary>
    /// 该KBDLLHOOKSTRUCT结构包含一个低级别的键盘输入事件的信息
    /// </summary>
    /// <remarks>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyboardHookStruct
    {
        /// <summary>
        /// 指定一个虚拟键码。该代码必须是一个值范围为1〜254
        /// </summary>
        public int VirtualKeyCode;

        /// <summary>
        /// 指定了密钥的硬件扫描码
        /// </summary>
        public int ScanCode;

        /// <summary>
        /// 指定扩展键标志，事件注入标志，上下文代码，以及过渡态的标志
        /// </summary>
        public int Flags;

        /// <summary>
        /// 指定此消息时间戳
        /// </summary>
        public int Time;

        /// <summary>
        /// 指定与消息有关的附加信息
        /// </summary>
        public int ExtraInfo;
    }


    /// <summary> 钩子触发的委托 </summary>
    internal delegate int HookProc(int nCode, int wParam, IntPtr lParam);
}
