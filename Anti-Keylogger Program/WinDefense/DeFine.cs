using JsonCore;
using Microsoft.Win32;
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

        public static SystemSetting<WhiteItemInFo> LocalSetting = new SystemSetting<WhiteItemInFo>();

        public static int DangeCount = 0;
        public static bool SCaning = false;

        public static MainWindow WorkingWin = null;

        private const bool TestStart = false;

        public static bool AdminRun()
        {
            if (TestStart) return true;
            
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity); //判断当前登录用户是否为管理员
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                
                return true;

            }
            else
            {
               
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                
                startInfo.Verb = "runas";

                try
                {

                    System.Diagnostics.Process.Start(startInfo);
                  
                }
                catch
                {
                }

                return false;
            }
        }
        public static void ClearDange()
        {
            foreach (var Get in SafeHelper.WaitProcessDangers)
            {
                ProcessOperation.SuperByKillProcess(Get.Pid);
            }

            DangeCount = 0;
        }


        public static void SCanPowerOn()
        {
            RegistryKey PowerOns = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");

            foreach (string Item in PowerOns.GetValueNames())
            {
                string Path = PowerOns.GetValue(Item).ToString();

                foreach (var GetProcess in Process.GetProcessesByName(Item))
                {
                    ThreadPrRecvItem OneItem = new ThreadPrRecvItem(0, (uint)GetProcess.Id, 0, 0, 0, 0, false, true);
                    ProcessHelper.PrThreadRecvs.Enqueue(OneItem);
                }

            }
        }

        public static int MaxProcessSafeCheckThread = 7;

        public const string DrivePath = @"\Driver\Tools\";
        public static string GetFullPath(string Path,string Name)
        {
            if (!Path.EndsWith(@"\")) Path += @"\";

            string GetShellPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            return GetShellPath.Substring(0, GetShellPath.LastIndexOf(@"\")) + Path+Name;
        }

        public static void Initialization()
        {
            LocalSetting = LocalSetting.GetLocal();

            AutoSave.StartAutoSaveService(true);

            //using (SQLiteConnection connection = new SQLiteConnection(SQLiteHelper.connectionString))
            //{
            //    connection.Open();
            //    connection.ChangePassword("!@#$%Z^&^&*()12333AseRkjhjlaf!#$%");
            //}

            SQLiteHelper.SetConnectionString(DeFine.GetFullPath("", @"\SafeDB.db"), "!@#$%Z^&^&*()12333AseRkjhjlaf!#$%");

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

            //FristStart Scan All SCanPowerOn Process Report Virus
            SCanPowerOn();

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


            ProcessOperation.UPLevel(Process.GetCurrentProcess().Id);
        }

       
        public static void AnyExit()
        {
            LocalSetting.SetLocal();

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

    public static class WhiteItemInFoExtend
    {
        private static bool WhiteListAction(this List<WhiteItemInFo> Sources, Action<List<WhiteItemInFo>, WhiteItemInFo> OneAction, string CRC)
        {
            for (int i = 0; i < Sources.Count; i++)
            {
                if (Sources[i].CRC.Equals(CRC))
                {
                    OneAction.Invoke(Sources, Sources[i]);
                    return true;
                }
            }
           
            return false;
        }

        public static WhiteItemInFo WhiteListAction(this List<WhiteItemInFo> Sources, Action<List<WhiteItemInFo>, WhiteItemInFo> OneAction, string CRC,ref bool State)
        {
            for (int i = 0; i < Sources.Count; i++)
            {
                if (Sources[i].CRC.Equals(CRC))
                {
                    OneAction.Invoke(Sources, Sources[i]);
                    State = true;
                    return Sources[i];
                }
            }

            State = false;
            return null;
        }


        public static bool Remove(this List<WhiteItemInFo> Sources,string CRC)
        {
            return WhiteListAction(Sources,new Action<List<WhiteItemInFo>, WhiteItemInFo>((a, b) =>
            {
                b.TrustByUser = false;
            }), CRC);
        }

        public static string GetProcessName(this WhiteItemInFo Source)
        {
            return Source.ProcessPath.Substring(Source.ProcessPath.LastIndexOf(@"\") + @"\".Length);
        }

        public static bool AddWhite(this List<WhiteItemInFo> Sources, string CRC)
        {
            return WhiteListAction(Sources, new Action<List<WhiteItemInFo>, WhiteItemInFo>((a, b) =>
            {
                b.TrustByUser = true;
            }), CRC);
        }

        public static bool CheckWhiteList(this List<WhiteItemInFo> Sources,string CRC)
        {
            return WhiteListAction(Sources, new Action<List<WhiteItemInFo>, WhiteItemInFo>((a, b) => {}), CRC);
        }

        public static bool CheckWhiteList(this List<WhiteItemInFo> Sources, string CRC,ref WhiteItemInFo OneInFo)
        {
            bool State = false;
            OneInFo = WhiteListAction(Sources, new Action<List<WhiteItemInFo>, WhiteItemInFo>((a, b) => { }), CRC,ref State);
            return State;
        }
    }

    public class WhiteItemInFo
    {
        public string ProcessPath = "";
        public string CRC = "";
        public int DangeScore = 0;
        public bool TrustByUser = false;
        public List<FileCodeSCanItem> Accesss = null;
        public WhiteItemInFo() { }

        public WhiteItemInFo(string ProcessPath,ref string CRC32)
        {
            if (File.Exists(ProcessPath))
            {
                this.ProcessPath = ProcessPath;
                this.CRC = FileToCRC32.GetFileCRC32(this.ProcessPath);
                CRC32 = this.CRC;
            }
        }
    }

    public class SystemSetting<T> where T : new()
    {
        public List<T> WhiteList = new List<T>();

        public SystemSetting() { }

        public SystemSetting(List<T> A)
        {
            this.WhiteList = A;
        }


        public string SetLocal()
        {
            string GetJson = JsonHelper.GetJson(this);
            GetJson = PINHelper.AESEncrypt(GetJson);
            DataHelper.WriteFile(DeFine.GetFullPath(@"\", "Setting.dat"),Encoding.UTF8.GetBytes(GetJson));

            return GetJson;
        }

        public SystemSetting<T> GetLocal()
        {
            string GetJson = DataHelper.ReadFileByStr(DeFine.GetFullPath(@"\", "Setting.dat"),Encoding.UTF8);
            GetJson = PINHelper.AESDecrypt(GetJson);
            var Source = JsonHelper.ProcessToJson<SystemSetting<T>>(GetJson);
            if (Source == null)
            {
                return new SystemSetting<T>();
            }
            return Source;
        }
    }
}
