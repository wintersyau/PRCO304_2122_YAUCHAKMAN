using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WinDefense.DataManage;
using WinDefense.FormManage;
using WinDefense.KernelManage;
using WinDefense.ProcessControl;
using WinDefense.SafeEngine;
using WinDefense.SQLManage;

namespace WinDefense
{
    public class DeFine
    {
        public static int DangeCount = 0;
        public static bool SCaning = false;

        public static MainWindow WorkingWin = null;
        public static void ClearDange()
        {
            foreach (var Get in SafeHelper.WaitProcessDangers)
            {
                ProcessOperation.SuperByKillProcess(Get.Pid);
            }

            DangeCount = 0;
        }

        public static int MaxProcessSafeCheckThread = 5;

        public const string DrivePath = @"\Driver\Tools\";
        public static string GetFullPath(string Path,string Name)
        {
            if (!Path.EndsWith(@"\")) Path += @"\";

            string GetShellPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            return GetShellPath.Substring(0, GetShellPath.LastIndexOf(@"\")) + Path+Name;
        }

        public static void Initialization()
        {
            //using (SQLiteConnection connection = new SQLiteConnection(SQLiteHelper.connectionString))
            //{
            //    connection.Open();
            //    connection.ChangePassword("abc123");
            //}

            SQLiteHelper.SetConnectionString(DeFine.GetFullPath("", @"\SafeDB.db"), "abc123");

            DriveLoader.NewDrive("ProcessListen.sys");

            DriveLoader.NewDrive("Superkill.sys");

            DriveLoader.NewDrive("PPLLProtect.sys");


            if (!DriveLoader.GetDrive("Superkill.sys").StartDrive())
            {
                MessageBox.Show("Error Not Install Super Extend!");
            }
            else
            {
               // ProcessOperation.UPLevel(Process.GetCurrentProcess().Id);
                //ProcessOperation.SuperByKillProcess(7452);

            }

            if (DriveLoader.GetDrive("PPLLProtect.sys").StartDrive())
            {
                //ProcessOperation.ProtectProcess(Process.GetCurrentProcess().Id); INJECT too much PPL will causing 

            }

            //FristStart Scan All Process Report Virus
            

            foreach (var GetProcess in Process.GetProcesses())
            {
                try {

                    if (GetProcess.ProcessName == "syscrb")
                    {
                        ThreadPrRecvItem OneItem = new ThreadPrRecvItem(0, (uint)GetProcess.Id, 0, 0, 0, 0, false, true);
                        ProcessHelper.PrThreadRecvs.Enqueue(OneItem);
                    }
              
                }
                catch { }
            }


            if (DriveLoader.GetDrive("ProcessListen.sys").StartDrive())
            {
                KernelHelper.InstallProcessRecvHandle(new KernelHelper.ProcessProc(KernelHelper.RecvProcessListen));

                var StartState = KernelHelper.StartProcessListenService(true);

                if (StartState == false)
                {
                    MessageBox.Show("ProcessListen - LoadDriveError!");
                }

                ProcessHelper.StartProcessProcessService(true);
            }
            else
            {
                MessageBox.Show("ProcessListen.sys RegCallBack Error!");
                ProcessListener.Init();
                ProcessListener.StatrProcessListenService(true);
                ProcessHelper.StartProcessProcessService(true);
            }
        }

       
        public static void AnyExit()
        {
         

            DeFine.WorkingWin.Dispatcher.Invoke(new Action(() => {
                DeFine.WorkingWin.Close();
            }));

            try
            {
            KernelHelper.StartProcessListenService(false);
            ProcessHelper.StartProcessProcessService(false);
            }
            catch { }

            try 
            { 
            DriveLoader.GetDrive("ProcessListen.sys").StopDrive();
            DriveLoader.GetDrive("ProcessListen.sys").CloseDrive();
            }
            catch { }

            try
            {
                DriveLoader.GetDrive("PPLLProtect.sys").StopDrive();
                DriveLoader.GetDrive("PPLLProtect.sys").CloseDrive();
            }
            catch { }

            try
            { 
            DriveLoader.GetDrive("Superkill.sys").StopDrive();
            DriveLoader.GetDrive("Superkill.sys").CloseDrive();
            }
            catch { }

            try
            {

                NotifyIconHelper.OneNotifyIcon.Dispose();

            }
            catch { }


            System.Environment.Exit(System.Environment.ExitCode);

            DeFine.WorkingWin.Dispatcher.BeginInvoke(new Action(()=> {
                WorkingWin.ProcessList.Items.RemoveAt(99999);
            }));

        }
    }


}
