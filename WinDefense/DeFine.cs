using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public static void ClearDange()
        {
            foreach (var Get in SafeHelper.WaitProcessDangers)
            {
                ProcessOperation.SuperByKillProcess(Get.Pid);
            }

            DangeCount = 0;
        }

        public static int MaxProcessSafeCheckThread = 5;//最大并行检测数量

        public const string DrivePath = @"\Driver\Tools\";//驱动文件
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
            //    connection.ChangePassword("123abc");设置密码
            //}

            SQLiteHelper.SetConnectionString(DeFine.GetFullPath("", @"\SafeDB.db"), "123abc");//装载本地离线木马库

            DriveLoader.NewDrive("ProcessListen.sys");//安装并启动监听服务

            DriveLoader.NewDrive("Superkill.sys");//安装并启动高级删除服务 结束进程 强制删除文件等

            DriveLoader.NewDrive("PPLLProtect.sys");//安装并启动核心保护程序


            if (!DriveLoader.GetDrive("Superkill.sys").StartDrive())
            {
                MessageBox.Show("Error Not Install Super Extend!");
            }
            else
            {
                //ProcessOperation.SuperByKillProcess(7452);

            }

            if (DriveLoader.GetDrive("PPLLProtect.sys").StartDrive())
            {
                //ProcessOperation.ProtectProcess(Process.GetCurrentProcess().Id); 千万不要这样做 因为我们本身注入了很多东西启用PPL会造成一些大问题

            }

            if (DriveLoader.GetDrive("ProcessListen.sys").StartDrive())
            {
                KernelHelper.InstallProcessRecvHandle(new KernelHelper.ProcessProc(KernelHelper.RecvProcessListen));

                var StartState = KernelHelper.StartProcessListenService(true);

                if (StartState == false)
                {
                    MessageBox.Show("LoadDriveError!");
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

        }
    }


}
