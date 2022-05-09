using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinDefense.ConvertManage;
using WinDefense.DataManage;
using WinDefense.KernelManage;
using WinDefense.SafeEngine;
using WinDefense.SQLManage;
using WinDefense.WinApi;
using static WinDefense.WinApi.WinApiHelper;

namespace WinDefense.ProcessControl
{
    public class ProcessHelper
    {

        public static Queue<ThreadPrRecvItem> PrThreadRecvs = new Queue<ThreadPrRecvItem>();

   

        public static int CurrentProcessSafeCheckThread = 0;

        public static bool LockerProcessProcessService = false;
        public static void StartProcessProcessService(bool Check)
        {
            if (Check)
            {
                if (!LockerProcessProcessService)
                {
                    LockerProcessProcessService = true;

                    new Thread(() => {

                        while (LockerProcessProcessService)
                        {
                            if (PrThreadRecvs.Count > 0)
                            {
                                ThreadPrRecvItem OneItem = PrThreadRecvs.Dequeue();

                            NextWait:

                                if (CurrentProcessSafeCheckThread <= DeFine.MaxProcessSafeCheckThread)
                                {
                                    new Thread(() =>
                                    {
                                        CurrentProcessSafeCheckThread++;
                                        SafeHelper.CheckProcessSafe(ref OneItem);
                                        KernelHelper.MsgPrRecvItems.Enqueue(OneItem);//Displays a message to the front end

                                        CurrentProcessSafeCheckThread--;
                                    }).Start();
                                }
                                else
                                {
                                    Thread.Sleep(100);
                                    goto NextWait;
                                }

                                Thread.Sleep(20);
                            }
                            else
                            {
                                Thread.Sleep(200);
                            }


                        }

                    }).Start();

                }
            }
            else
            {
                LockerProcessProcessService = false;
            }
        }

        public static bool IsSystemProcess(int ID)
        {
            try 
            {
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(ID);
            if (p.StartInfo.UserName == "SYSTEM" || p.StartInfo.UserName == "NETWORK SERVICE" || p.StartInfo.UserName == "LOCAL SERVICE")
            {
                return true;
            }
            return false;
            }
            catch
            { 
                return true;
            }
        }


        public static int GetProcessModuleByID(int PID,ref List<MODULEENTRY32> Modules)
        {
            IntPtr GetShotPicHwnd = WinApiHelper.CreateToolhelp32Snapshot(8, PID);

            if ((int)GetShotPicHwnd > 0)
            {
                MODULEENTRY32 NModule = new MODULEENTRY32();
                NModule.dwSize = 1024;
                IntPtr GetModule = WinApiHelper.Module32First(GetShotPicHwnd,ref NModule);

                while ((int)GetModule != 0)
                {
                    //string GetModuleName = GetEntityName(NModule.szModule);
                    Modules.Add(NModule);
                    GetModule = WinApiHelper.Module32Next(GetShotPicHwnd,ref NModule);
                }

                WinApiHelper.CloseHandle(GetShotPicHwnd);
            }

            return Modules.Count;
        }
        public static string GetProcessPePath(int PID, string ModuleName)
        {
            List<MODULEENTRY32> CurrentModules = new List<MODULEENTRY32>();
            int GetModulesCount = GetProcessModuleByID(PID, ref CurrentModules);
            for (int i = 0; i < GetModulesCount; i++)
            {
                if (CurrentModules[i].szModule.Equals(ModuleName))
                {
                    return CurrentModules[i].szExePath.ToString();
                }
            }

            return string.Empty;
        }

        public static void Ring3EnumWindowsHook()
        {
            foreach (var GetProcess in Process.GetProcesses())
            {
                List<MODULEENTRY32> Moudles = new List<MODULEENTRY32>();
                int GetCount = GetProcessModuleByID(GetProcess.Id, ref Moudles);

                for (int i = 0; i < GetCount; i++)
                {
                    Ring3GetWindowsHook(GetProcess.Id,Moudles[i]);
                }
            }
        }
        public static Dictionary<string, byte[]> LibraryDataCache = new Dictionary<string, byte[]>();


       


        public static void Ring3GetWindowsHook(int CurrentProcessPID, MODULEENTRY32 Source)
        {
            IntPtr GetOpenHandle = WinApiHelper.GetModuleHandleA(new StringBuilder(ConvertHelper.GetEntityName(Source.szModule)));

            if ((long)GetOpenHandle != 0)
            {
                string GetModulePath = ConvertHelper.GetEntityName(Source.szExePath);

                if (File.Exists(GetModulePath))
                {
                    string MPath = GetModulePath;
                    string MName = DataHelper.GetPathAndFileName(ref MPath);

                    byte[] CurrentData;//prepare for cached

                    if (LibraryDataCache.ContainsKey(MName))
                    {
                        CurrentData = LibraryDataCache[MName];
                    }
                    else
                    {
                        CurrentData = DataHelper.GetBytesByFilePath(GetModulePath);
                        LibraryDataCache.Add(MName, CurrentData);
                    }


                    if (CurrentData.Length > 0)
                    {
                        byte[] TempData = DataHelper.ReadByteByLength(CurrentData,64);

                        if (TempData.Length == 64)
                        {
                            IMAGE_DOS_HEADER IMAGEDOSHEADER = (IMAGE_DOS_HEADER)new OneRtl(RtlType.IMAGE_DOS_HEADER).GetInstance(TempData, 64);

                            if (IMAGEDOSHEADER.isValid)
                            {
                                TempData = DataHelper.ReadByteByLength(CurrentData, 248, IMAGEDOSHEADER.c_lfanew);
                                
                                if (TempData.Length == 248)
                                {
                                    IMAGE_NT_HEADERS32 IMAGENTHEADERS32 = (IMAGE_NT_HEADERS32)new OneRtl(RtlType.IMAGE_NT_HEADERS32).GetInstance(TempData, 248);

                                    if (IMAGENTHEADERS32.Signature == 17744)
                                    {
                                        if (IMAGENTHEADERS32.OptionalHeader.DataDirectory[2].Size > 0)
                                        {
                                            TempData = DataHelper.ReadByteByLength(CurrentData,(IMAGEDOSHEADER.c_lfanew + 248) + IMAGENTHEADERS32.FileHeader.NumberOfSections * 40);


                                        }
                                    
                                    }
                                }
                            }
                        }
                       
                      

                        GC.Collect();
                      




                    }

                }
                else
                {
                    //Insufficient of read permissions

                }
            }
        }


    }



    public class ProcessInFo
    {
        public string ProcessName;
        public int Pid = 0;
        public int ProtectLevel = 0;
        public string FilePath = "";
        public string CommandLine = "";
        public int DangerValue = 0;
        public List<FileCodeSCanItem> AllSign = new List<FileCodeSCanItem>();
        public Bitmap Image = null;

        public ProcessInFo()
        {
            this.ProtectLevel = 5;
        }

        public ProcessInFo(int Pid)
        {
            this.Pid = Pid;
        }
        public ProcessInFo GetInFo(bool GetImage)
        {
            if (ProcessHelper.IsSystemProcess(this.Pid)) return new ProcessInFo();

            Process CurrentProcess = null;

            this.ProtectLevel = 0;

            try
            {
                CurrentProcess = Process.GetProcessById(this.Pid);
            }
            catch { this.ProtectLevel = 5; }

            try
            {
                this.ProcessName = CurrentProcess.ProcessName;
                this.ProtectLevel = 0;

                this.CommandLine = CurrentProcess.StartInfo.Arguments;
            }
            catch { this.ProtectLevel = 1; }

            try
            {
                this.FilePath = CurrentProcess.MainModule.FileName;
            }
            catch { this.ProtectLevel = 2; }


            if (GetImage)
            {
                if (this.FilePath.Trim().Length > 0 && File.Exists(this.FilePath))
                {
                    this.Image = System.Drawing.Icon.ExtractAssociatedIcon(this.FilePath).ToBitmap();
                }
            }

            return this;
        }
    }
    public enum ProcessType
    {
        Null = 0, Parent = 1, This = 2
    }
    public class ThreadPrRecvItem
    {
        public DateTime ShellTime;
        public int ParentPid = 0;
        public int Pid = 0;
        public string CurrentProcessName = "";
        public string ParentProcessName = "";
        public bool ParentSystem;
        public bool ThisSystem;

        public List<ProcessInFo> ProcessInFos = new List<ProcessInFo>();

        public ThreadPrRecvItem(uint ParentPid, uint Pid, ushort Hour, ushort Minute, ushort Second, ushort Milliseconds, bool ParentSystem, bool ThisSystem)
        {

            this.ParentSystem = ParentSystem;
            this.ThisSystem = ThisSystem;
            ShellTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)Hour, (int)Minute, (int)Second, (int)Milliseconds);
            this.ParentPid = Convert.ToInt32(ParentPid);
            this.Pid = Convert.ToInt32(Pid);

            ProcessInFos.Add(new ProcessInFo(this.ParentPid));
            ProcessInFos.Add(new ProcessInFo(this.Pid));
        }

        public ProcessInFo GetProcessInFo(ProcessType OneType, bool GetImage = false)
        {
            if (OneType == ProcessType.Parent)
            {
                if (this.ProcessInFos.Count > 0)
                {
                    return this.ProcessInFos[0].GetInFo(GetImage);
                }
            }
            else
            if (OneType == ProcessType.This)
            {
                if (this.ProcessInFos.Count > 1)
                {
                    return this.ProcessInFos[1].GetInFo(GetImage);
                }
            }

            return new ProcessInFo();
        }
    }
}
