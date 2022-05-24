using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinDefense.ConvertManage;
using WinDefense.ProcessControl;
using WinDefense.SafeEngine;
using WinDefense.SQLManage;

namespace WinDefense.KernelManage
{
    public class KernelHelper
    {
        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SendMsgToKernelByPPL(StringBuilder pInStr, int len);

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SendMsgToKernel(StringBuilder pInStr, int len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]

        public delegate void ProcessProc(uint ParentPid, uint Pid, ushort Hour, ushort Minute, ushort Second, ushort Milliseconds,bool ParentSystem,bool ThisSystem);

        public static ProcessProc ProcessListenProc;

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetProcessCallback(IntPtr FunctionAdd);

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartProcessListenService(int Check);

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ProcessListenServiceLoop();

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool installDvr(StringBuilder drvPath, StringBuilder serviceName);

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool startDvr(StringBuilder serviceName);

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool stopDvr(StringBuilder serviceName);

        [DllImport(@"ProtectControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool unloadDvr(StringBuilder serviceName);


        public static Queue<ThreadPrRecvItem> MsgPrRecvItems = new Queue<ThreadPrRecvItem>(); 


        [HandleProcessCorruptedStateExceptions]

        public static bool SendMsgToSuperSys(string Msg)
        {
            StringBuilder NStringBuilder = new StringBuilder(Msg);

            if (SendMsgToKernel(NStringBuilder, 255) != 0)
            {
                return true;
            }
            return false;
        }

        public static bool SendMsgToKernelByPPL(string Msg)
        {
            StringBuilder NStringBuilder = new StringBuilder(Msg);

            if (SendMsgToKernelByPPL(NStringBuilder, 255) != 0)
            {
                return true;
            }
            return false;
        }



        #region ProcessListen.sys
        public static void InstallProcessRecvHandle(ProcessProc OneAddress)
        {
            ProcessListenProc = OneAddress;
            SetProcessCallback(Marshal.GetFunctionPointerForDelegate(ProcessListenProc));
        }


        public static Thread ProcessListenService = null;
        public static bool? StartProcessListenService(bool Check)
        {
            if (Check)
            {
                if (ProcessListenService != null)
                {
                    ProcessListenService.Abort();
                    ProcessListenService = null;
                    StartProcessListenService(0);

                    return null;
                }

                if (StartProcessListenService(1))
                {
                    ProcessListenService = new Thread(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(20);

                            ProcessListenServiceLoop();
                        }
                    });

                    ProcessListenService.Start();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (ProcessListenService != null)
                {
                    ProcessListenService.Abort();
                    ProcessListenService = null;
                    StartProcessListenService(0);

                    return true;
                }
            }

            return false;
        }

        public static void RecvProcessListen(uint ParentPid, uint Pid, ushort Hour, ushort Minute, ushort Second, ushort Milliseconds,bool ParentSystem, bool ThisSystem)
        {
            ThreadPrRecvItem OneItem = new ThreadPrRecvItem(ParentPid, Pid, Hour, Minute, Second, Milliseconds,ParentSystem,ThisSystem);

            ProcessHelper.PrThreadRecvs.Enqueue(OneItem);
        }

        #endregion

    }


}
