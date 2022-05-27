using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyBoardListener.ListenManager
{

    [StructLayout(LayoutKind.Sequential)]

    public class HookStruct
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }
    public class KeyBoardHook
    {

        public delegate void ProcessKeyHandle(HookStruct OneStruct, out bool Handle);

        public delegate int HookHandle(int nCode, int wParam, IntPtr IParam);


        [DllImport("Kernel32.dll")]

        public static extern IntPtr GetModuleHandle(string name);//Retrieves a module handle for the specified module. The module must have been loaded by the calling process.

        [DllImport("User32.dll")]

        public static extern int SetWindowsHookEx(int idHook,HookHandle Hwnd,IntPtr Instance,int ThreadID);//Installs an application-defined hook procedure into a hook chain
        [DllImport("User32.dll")]
        public static extern int CallNextHookEx(int idHook, int Code,int WParam,IntPtr IParam);//Passes the hook information to the next hook procedure in the current hook chain.

        [DllImport("User32.dll")]

        public static extern bool UnhookWindowsHookEx(int idHook);//Removes a hook procedure


        public static int WHKeyBoard = 13;// 13 = Installs a hook procedure that monitors low-level keyboard input events.

        public static int CurrentState = 0;//current state

        public static IntPtr CurrentPtr = IntPtr.Zero;

        private static ProcessKeyHandle MeThod = null;//process return the set parameter
        private static HookHandle CurrentHandle = null;//processs the hook

       
        public static void InstallHook(ProcessKeyHandle OneHandle)
        {
            MeThod = OneHandle;

            if (CurrentState == 0)//CurrentState is prevent the reprocess
            {
                CurrentHandle = new HookHandle(OnHookProcess);

                CurrentPtr = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName); // to get the main process for install hook

                CurrentState = SetWindowsHookEx(WHKeyBoard, CurrentHandle, CurrentPtr, 0);
                
               //Install a hook. CurrentState is same as jason, <=0 is error

                
                if (CurrentState <= 0)
                {
                    UNHook();//unhook if error
                }


            }

        }

        public static int OnHookProcess(int nCode, int wParam, IntPtr IParam)
        {
            if (nCode >= 0)//nCode is Ascii
            {
                HookStruct NHookStruct = Marshal.PtrToStructure(IParam, typeof(HookStruct)) as HookStruct;// Provides a collection of methods for allocating unmanaged memory, copying unmanaged memory blocks, and converting managed to unmanaged types, as well as other miscellaneous methods used when interacting with unmanaged code.
                if (MeThod != null)
                {
                    bool Handle = false;

                    MeThod(NHookStruct, out Handle);

                    if (Handle)
                    {
                        // The return 1 tell the system that we received the key stroke message.  The system will release the hook after it received the returned value.
  
                        return 1;
                    }
                
                }
            }

            //CallNextHookEx - continue monitoring the keystroke

            return CallNextHookEx(CurrentState, nCode, wParam, IParam);
        }


        public static bool UNHook()//unhook
        {
            bool State = false;
            if (CurrentState != 0)//Check the status of hook
            {
                State = UnhookWindowsHookEx(CurrentState);//Unhook

                if (State)
                {
                    CurrentState = 0;//After unhooking, it will restore to inital value otherwise it can not be install the hook againg
                }
            }

            return State;
        }

    }
}
