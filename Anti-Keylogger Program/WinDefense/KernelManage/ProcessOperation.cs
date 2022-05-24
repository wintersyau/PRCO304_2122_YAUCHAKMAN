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
        /// PPLLProtect protect the process from being tampered and ended up.
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
                return KernelHelper.SendMsgToKernelByPPL("P" + Pid.ToString()); 
            }
            else
            {
                return KernelHelper.SendMsgToKernelByPPL("U" + Pid.ToString());
            }

            }
            catch { return false; }
        }

        /// <summary>
        /// Superkill forced to end the process
        /// </summary>
  
        public static bool SuperByKillProcess(int Pid)
        {
            try 
            {
            if (Process.GetProcessById(Pid) == null) return false;
            return KernelHelper.SendMsgToSuperSys("Z"+Pid.ToString()); //empty the process
            }
            catch { return false; }
        }


        /// <summary>
        /// Call the Superkill to elevat the arbitrary processes to DebugSystem
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
        /// Call SuperKill to suspend the process or resume
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
