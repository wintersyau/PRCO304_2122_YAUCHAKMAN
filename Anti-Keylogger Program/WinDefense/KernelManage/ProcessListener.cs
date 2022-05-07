using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinDefense.KernelManage
{
    public class ProcessListener
    {
        public static List<int> CreatedProcessIDs= new List<int>();
        public static void Init()
        {
            CreatedProcessIDs.Clear();

            foreach (var GetPr in Process.GetProcesses())
            {
                if (!CreatedProcessIDs.Contains(GetPr.Id))
                {
                    CreatedProcessIDs.Add(GetPr.Id);
                }
            }
        }


        public static bool LockerProcessListenService = false;

      

        public static void StatrProcessListenService(bool Check)
        {
            if (Check)
            {
                if (!LockerProcessListenService)
                {
                    LockerProcessListenService = true;

                    new Thread(() =>
                    {

                        while (LockerProcessListenService)
                        {
                            Thread.Sleep(100);
                            Process[] TempArray = Process.GetProcesses();

                            for (int i = 0; i < TempArray.Length; i++)
                            {
                                if (!CreatedProcessIDs.Contains(TempArray[i].Id))
                                {
                                    CreatedProcessIDs.Add(TempArray[i].Id);

                                    KernelHelper.RecvProcessListen(0, (uint)TempArray[i].Id, (ushort)DateTime.Now.Hour, (ushort)DateTime.Now.Minute, (ushort)DateTime.Now.Second, (ushort)DateTime.Now.Millisecond, false, false);
                           
                                }
                            }
                        }
                    }).Start();
                }
            }
            else
            {
                LockerProcessListenService = false;
            }
        }



   
    }
}
