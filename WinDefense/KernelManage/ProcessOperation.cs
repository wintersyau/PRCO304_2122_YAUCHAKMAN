using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDefense.KernelManage
{
    public class ProcessOperation
    {
        /// <summary>
        /// 调用PPLLProtect保护指定进程防止被结束被篡改
        /// </summary>
        /// <param name="Pid"></param>
        /// <param name="Protected"></param>
        /// <returns></returns>
        public static bool ProtectProcess(int Pid,bool Protected=true)
        {
            try 
            {

            if (Process.GetProcessById(Pid) == null) return false;

            if (Protected)
            {
                return KernelHelper.SendMsgToKernelByPPL("P" + Pid.ToString()); //启用护盾
            }
            else
            {
                //已弃用
                return KernelHelper.SendMsgToKernelByPPL("U" + Pid.ToString()); //停用护盾有攻击性的还可以停止其他软件的PPL
            }

            }
            catch { return false; }
        }

        /// <summary>
        /// 调用 Superkill 强制结束进程
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public static bool SuperByKillProcess(int Pid)
        {
            try 
            {
            if (Process.GetProcessById(Pid) == null) return false;
            return KernelHelper.SendMsgToSuperSys("Z"+Pid.ToString()); //执行内存清0
            }
            catch { return false; }
        }


        /// <summary>
        ///  调用 Superkill强制提权任意进程到DebugSystem
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public static bool UPLevel(int Pid)
        {
            try 
            { 
            if (Process.GetProcessById(Pid) == null) return false;
            return KernelHelper.SendMsgToSuperSys("U" + Pid.ToString());
            }
            catch { return false; }
        }

        /// <summary>
        /// 调用SuperKill挂起进程或者恢复
        /// </summary>
        /// <param name="Pid"></param>
        /// <param name="Keep"></param>
        /// <returns></returns>
        public static bool SuperByControlProcess(int Pid,bool Keep=false)
        {
            try 
            { 
            if (Process.GetProcessById(Pid) == null) return false;
            if (!Keep)
            {
                return KernelHelper.SendMsgToSuperSys("S" + Pid.ToString()); 
            }
            else
            {
                return KernelHelper.SendMsgToSuperSys("M" + Pid.ToString()); 
            }
            }
            catch { return false; }
        }
    }
}
