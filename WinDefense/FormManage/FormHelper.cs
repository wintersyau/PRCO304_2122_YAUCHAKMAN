using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinDefense.KernelManage;
using WinDefense.ProcessControl;

namespace WinDefense.FormManage
{
    public class FormHelper
    {
        public static MainWindow WorkingWin = null;
        public static void SetActiveWin(MainWindow Win)
        {
            if (WorkingWin == null)
            {
                WorkingWin = Win;
            }
        }

        public static bool LockerRealTimeProcessReport = false;
        public static long CheckOffset = 0;
        public static void ShowRealTimeProcessReport()
        {
            if (WorkingWin == null == false)
            {
                if (!LockerRealTimeProcessReport)
                {
                    LockerRealTimeProcessReport = true;

                    new Thread(() => {

                        while (true)
                        {
                            Thread.Sleep(100);

                            if (KernelHelper.MsgPrRecvItems.Count > 0)
                            {
                                var GetRecv = KernelHelper.MsgPrRecvItems.Dequeue();
                                CheckOffset++;
                                var Parent = GetRecv.GetProcessInFo(ProcessType.Parent);
                                var Target = GetRecv.GetProcessInFo(ProcessType.This);

                                if (Parent == null) Parent = new ProcessInFo();
                                if (Target == null) Target = new ProcessInFo();

                                if (Target.FilePath.Trim().Length > 0 || Parent.FilePath.Trim().Length > 0)
                                {
                                        WorkingWin.Dispatcher.Invoke(new Action(() => 
                                        {
                                            WorkingWin.RSourceProcess.Content = Parent.FilePath;
                                            WorkingWin.RTargetProcess.Content = Target.FilePath;
                                            WorkingWin.RScore.Content = Parent.DangerValue + "-" + Parent.DangerValue;
                                            WorkingWin.RProtectLevel.Content = Parent.ProtectLevel + "-" + Parent.ProtectLevel;


                                            if (WorkingWin.ProcessList.Items.Count > 100)
                                            {
                                                WorkingWin.ProcessList.Items.Clear();
                                            }

                                            if (Parent==null==false)
                                            if (Parent.ProcessName == null == false)
                                            if (Parent.ProcessName.Trim().Length > 0)
                                            WorkingWin.ProcessList.Items.Add(new
                                            {
                                                ID = CheckOffset,
                                                PName = Parent.ProcessName,
                                                DScore = Parent.DangerValue,
                                                PPath = Parent.FilePath,
                                                Access = Parent.AllSign.Count,
                                                Time = GetRecv.ShellTime.ToString()
                                            }) ;

                                            if (Target == null == false)
                                            if (Target.ProcessName == null == false)
                                            if (Target.ProcessName.Trim().Length>0)
                                            WorkingWin.ProcessList.Items.Add(new
                                            {
                                                ID = CheckOffset,
                                                PName = Target.ProcessName,
                                                DScore = Target.DangerValue,
                                                PPath = Target.FilePath,
                                                Access = Target.AllSign.Count,
                                                Time = GetRecv.ShellTime.ToString()
                                            });
                                        }));

                                   
                                        Thread.Sleep(1500);
                                }
                              
                            }
                        }


                    }).Start();

                    new Thread(() => {

                        while (true)
                        {
                            Thread.Sleep(30);

                            try { 
                            WorkingWin.Dispatcher.Invoke(new Action(() => {

                                WorkingWin.RLoadBearing.Content = string.Format("Doing:{0},Max:{1},Pass:{2}", ProcessHelper.CurrentProcessSafeCheckThread, DeFine.MaxProcessSafeCheckThread, ProcessHelper.PrThreadRecvs.Count);

                            }));
                            }
                            catch { Thread.Sleep(5000); }
                        }


                    }).Start();


                    new Thread(() => {

                        while (true)
                        {
                            Thread.Sleep(1000);

                            try
                            {
                                WorkingWin.Dispatcher.Invoke(new Action(() => {

                                    if (!DeFine.SCaning)
                                    {
                                        if (DeFine.DangeCount > 0)
                                        {
                                            WorkingWin.MainCaption.Content = "Find Danger!";
                                            WorkingWin.CenterBtn.Content = "QuickProcess";
                                        }
                                        else
                                        {
                                            WorkingWin.MainCaption.Content = "You Are Protected";
                                            WorkingWin.CenterBtn.Content = "FastSCan";
                                        }
                                    }

                                }));
                            }
                            catch { Thread.Sleep(5000); }
                        }


                    }).Start();
                }
              
            }
         
        }


    }
}
