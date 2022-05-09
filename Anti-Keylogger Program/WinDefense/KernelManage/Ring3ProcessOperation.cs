using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDefense.KernelManage
{
    class Ring3ProcessOperation
    {


        /// <summary>
        /// Call Superkill to end the process
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public static bool SuperByKillProcess(int Pid)
        {
            try
            {

                Process ProcessItem = Process.GetProcessById(Pid);


                if (ProcessItem != null)
                {
                    ProcessItem.Kill();
                    ProcessItem.Dispose();
                    return true;
                }

            }
            catch { return false; }


            return false;
        }


       
    }
}
