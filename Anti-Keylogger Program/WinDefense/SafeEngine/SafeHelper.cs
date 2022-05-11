using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WinDefense.DataManage;
using WinDefense.FormManage;
using WinDefense.KernelManage;
using WinDefense.ProcessControl;
using WinDefense.SQLManage;

namespace WinDefense.SafeEngine
{
   

    public class SafeHelper
    {
        public static List<FileCodeSCanItem> LocalFileCodeSCanItems = new List<FileCodeSCanItem>();
        public static void Initialization()
        {
            //return;
            //A=pay attention B=significant C=take action immediatelys
            try { 
            new FileCodeSCanItem("SetWindowsHookEx", 30, "Install Overall Hook !", "Hook.C").InsertDateToDB();
            new FileCodeSCanItem("CallNextHookEx", 20, "Passes the hook information to the next hook procedure in the current hook chain.", "Hook.A").InsertDateToDB();
            new FileCodeSCanItem("SendMessage", 3, "This Api Can Control other program", "Control.B").InsertDateToDB();
            new FileCodeSCanItem("PostMessage", 2, "This Api Can Control other program or Process Communication", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("SendKeys", 5, "This Api Can Control Your Computer KeyBoard", "Control.C").InsertDateToDB();
            new FileCodeSCanItem("SetWindowPos", 2, "Keep the top layer", "Harass.A").InsertDateToDB();
            new FileCodeSCanItem("WinSock", 1, "Network Communication", "Network").InsertDateToDB();
            new FileCodeSCanItem("Socket", 1, "Network Communication", "Network").InsertDateToDB();
            new FileCodeSCanItem("SHFormatDrive", 30, "Hard Disk Format", "Format.C").InsertDateToDB();
            new FileCodeSCanItem("FormatDrive", 30, "Hard Disk Format ", "Format.C").InsertDateToDB();
            new FileCodeSCanItem("CreateRemoteThread", 5, "Inject Hook", "Hook.B").InsertDateToDB();
            new FileCodeSCanItem("ReadProcessMemory", 2, "Read Memory", "Memory.A").InsertDateToDB();
            new FileCodeSCanItem("LoadLibrary", 6, "This Api Can Read Normal Dll Address", "Hook.A").InsertDateToDB();
            new FileCodeSCanItem("GetWindowText", 2, "This Api Can Get Other Windows Tittle", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("SetWindowText", 3, "This Api Can Set Other Windows Tittle", "Control.C").InsertDateToDB();
            new FileCodeSCanItem("FindWindow", 2, "This Api Can Find Other Windows HWND", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("OpenProcess", 2, "This Api Can Open Other program", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("TerminateProcess", 3, "This Api Can Close Other program", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("URLDownloadToFile", 2, "This Api Can Download HttpFile", "Network.A").InsertDateToDB();
            new FileCodeSCanItem("CreateToolhelp32Snapshot", 2, "This Api Can Look Other program Info", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("Process32First", 2, "This Api Can enum SystemProcess", "Harass.A").InsertDateToDB();
            new FileCodeSCanItem("Process32Next", 2, "This Api Can enum SystemProcess", "Harass.A").InsertDateToDB();
            new FileCodeSCanItem("WriteProcessMemory", 2, "This Api Can Change Other program", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("RegOpenKeyEx", 2, "This Api Can Set Automatic Power on", "Control.A").InsertDateToDB();
            new FileCodeSCanItem("RegDeleteValue", 7, "This Api Can Damage System", "Harass.B").InsertDateToDB();
            new FileCodeSCanItem("StartService", 3, "This Api Can Load KernelDrive", "Harass.B").InsertDateToDB();
            new FileCodeSCanItem("OpenService", 3, "This Api Can Load KernelDrive", "Harass.B").InsertDateToDB();
            new FileCodeSCanItem("CreateServicee", 3, "This Api Can Load KernelDrive", "Harass.B").InsertDateToDB();
            new FileCodeSCanItem("OpenSCManager", 3, "This Api Can Load KernelDrive And Change Win Service", "Harass.B").InsertDateToDB();

            }
            catch {

               
            }

            //
            //
            //LoadLibrary
            //WinSock
            GC.Collect();
        }


        public static bool CheckProcessSafe(ref ThreadPrRecvItem OneProcess)
        {
            var PParent = OneProcess.GetProcessInFo(ProcessType.Parent);
            var PThis = OneProcess.GetProcessInFo(ProcessType.This);

            string ParentPath = PParent.FilePath;
            string TargetPath = PThis.FilePath;


            OneProcess.ProcessInFos[0].DangerValue = -5;

            if (File.Exists(ParentPath))
            {
                int DangerScore = 20;

                List<FileCodeSCanItem> AllSign = new List<FileCodeSCanItem>();
                SafeHelper.SCANProcess(false,OneProcess.ProcessInFos[0], ParentPath, ref DangerScore, ref AllSign);

                OneProcess.ProcessInFos[0].AllSign.AddRange(AllSign);

                OneProcess.ProcessInFos[0].DangerValue += DangerScore;

                if (DangerScore > 50)
                {

                }
            }

            OneProcess.ProcessInFos[1].DangerValue = -5;

            if (File.Exists(TargetPath))
            {
                int DangerScore = 20;
                List<FileCodeSCanItem> AllSign = new List<FileCodeSCanItem>();

                try
                {
                    if (OneProcess.ProcessInFos[0].ProcessName.Equals("explorer"))
                    {
                        SafeHelper.SCANProcess(false, OneProcess.ProcessInFos[1], TargetPath, ref DangerScore, ref AllSign);
                    }
                    else
                    {
                        SafeHelper.SCANProcess(true, OneProcess.ProcessInFos[1], TargetPath, ref DangerScore, ref AllSign);
                    }
                }
                catch {
                    SafeHelper.SCANProcess(true, OneProcess.ProcessInFos[1], TargetPath, ref DangerScore, ref AllSign);
                }

                OneProcess.ProcessInFos[1].AllSign.AddRange(AllSign);

                OneProcess.ProcessInFos[1].DangerValue += DangerScore;

                if (DangerScore > 50)
                {

                }
            }
           


            return true;
        }

        /// <summary>
        /// provide the path of the digital signature
        /// </summary>
        /// <param name="providedFilePath"></param>
        /// <returns></returns>
        public static bool VerifyAuthenticodeSignature(string providedFilePath,ref string Subject)
        {
            bool isSigned = true;
            string fileName = Path.GetFileName(providedFilePath);
            string calculatedFullPath = Path.GetFullPath(providedFilePath);

            if (File.Exists(calculatedFullPath))
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    ps.AddCommand("Get-AuthenticodeSignature", true);
                    ps.AddParameter("FilePath", calculatedFullPath);
                    var cmdLetResults = ps.Invoke();
                    foreach (PSObject result in cmdLetResults)
                    {
                        Signature s = (Signature)result.BaseObject;

                        isSigned = s.Status.Equals(SignatureStatus.Valid);

                        if (isSigned == false)
                        {
                            isSigned = false;
                        }
                        else
                        {
                            
                            if (s.SignerCertificate != null)
                            {
                                if (s.SignerCertificate.Subject != null)
                                {
                                    Subject = s.SignerCertificate.Subject;
                                }
                            }

                            isSigned = true;

                            break;
                        }
                       
                    }
                }
            }
            else
            {
                
                isSigned = false;
            }

            return isSigned;
        }


        public static List<FileCodeSCanItem> NewSCan(string FilePath)
        {
            List<FileCodeSCanItem> FileCodeSCanItems = new List<FileCodeSCanItem>();

            try {

                FileCodeSCanItems = SQLiteStruct.GetAllBlock();
            }
            catch {
                
                FileCodeSCanItems = new List<FileCodeSCanItem>();
                FileCodeSCanItems.AddRange(SafeHelper.LocalFileCodeSCanItems);
            
            }

            string GetFileCode = DataHelper.ReadFileByStr(FilePath,DataHelper.GetFileEncodeType(FilePath));
            GetFileCode = Regex.Replace(GetFileCode, @"[^a-zA-Z0-9]", " ");
            GetFileCode = Regex.Replace(GetFileCode, @"^[\d\-\u4e00-\u9fa5]*$", "");
            GetFileCode = GetFileCode.Replace(",", " ");
            GetFileCode = Regex.Replace(GetFileCode, "[\\s\\p{P}\n\r=<>$>+￥^]", " ");
            GetFileCode = Regex.Replace(GetFileCode, "\\s{2,}", ".");
            GetFileCode = GetFileCode.Replace(" ", ".");

            List<FileCodeSCanItem> AllType = new List<FileCodeSCanItem>();

            foreach (var GetBlock in FileCodeSCanItems)
            {
                if (GetFileCode.Contains(GetBlock.KeyStr))
                { 
                   AllType.Add(GetBlock);
                }
            }
          
            return AllType;
           
        }

        public static bool IsSystemProcess(string Name)
        {
            if (Name.Trim().Length == 0)
            {
                return true;
            }
            if (Name.ToLower() == "explorer".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "Taskmgr".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "svchost".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "taskhostw".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "lsass".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "ctfmon".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "devenv".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "conhost".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "cmd".ToLower())
            {
                return true;
            }
            if (Name.ToLower() == "msedge".ToLower())
            {
                return true;
            }

            return false;
        }

        public static List<string> CheckProcessCache = new List<string>();

        public static List<ProcessInFo> WaitProcessDangers = new List<ProcessInFo>();
        public static bool SCANProcess(bool IsBackGround,ProcessInFo OneInFo,string FilePath,ref int DangerScore,ref List<FileCodeSCanItem> AllSign)
        {
            if (CheckProcessCache.Contains(OneInFo.FilePath))
            {
                return true;
            }
            else
            {
                CheckProcessCache.Add(OneInFo.FilePath);
            }

            WhiteListItem FileCrc = null;
            string CRC32 = "";

            try 
            {
                CRC32 = FileToCRC32.GetFileCRC32(FilePath);
                FileCrc = SQLiteStruct.FindWhiteList(CRC32);

            } catch { }

            if (FileCrc == null)
            {
                if (CRC32.Trim().Length > 0)
                {
                    if (!IsSystemProcess(OneInFo.ProcessName))
                    {
                        DangerScore = 0;//Default= threat low  >=100 High

                        if (IsBackGround)
                        {
                            DangerScore += 7;
                        }

                        if (File.Exists(FilePath))
                        {
                            bool IsStop = false;
  
                            string Subject = string.Empty;

                            if (VerifyAuthenticodeSignature(FilePath, ref Subject))
                            {
                                DangerScore -= 5;
                            }
                            else
                            {
                                IsStop = true;
                                ProcessOperation.SuperByControlProcess(OneInFo.Pid);
                                DangerScore += 5;
                            }

                            AllSign = NewSCan(FilePath);

                            //Check Process Dll
                          
                            
                            string DLLFilePath = FilePath.Substring(0, FilePath.LastIndexOf(@"\")) + @"\" + OneInFo.ProcessName + ".dll";
                            if (File.Exists(DLLFilePath))
                            {
                                SafeExtend.ScanFileByProcessPath(DLLFilePath, ref AllSign);
                            }
                            else
                            {
                                //Is Not Net Core Program
                                string ChildFilePath = FilePath.Substring(0, FilePath.LastIndexOf(@"\")) + @"\";

                                foreach (var Get in Directory.GetFiles(ChildFilePath))
                                {
                                    if (Get.ToLower().EndsWith(".dll"))
                                    {
                                        SafeExtend.ScanFileByProcessPath(Get, ref AllSign);
                                    }
                                }
                               
                            }

                            SafeExtend.ScanFileByProcessPath(FilePath,ref AllSign);

                            foreach (var GetSign in AllSign)
                            {
                                DangerScore += GetSign.DangerPoint;
                            }

                            if (DangerScore < 45)
                            {
                                try
                                { 

                                if (SQLiteStruct.AddWhiteList(new WhiteListItem(CRC32, OneInFo.ProcessName, "System", "NoCheck", "NoCheck")))
                                {
                                    if (IsStop)
                                    {
                                        ProcessOperation.SuperByControlProcess(OneInFo.Pid, true);
                                    }
                                }

                                }
                                catch { }
                            }
                            else
                            {

                                if (!IsStop)
                                {
                                    ProcessOperation.SuperByControlProcess(OneInFo.Pid);
                                }

                                List<FileCodeSCanItem> NFileCodeSCanItem = new List<FileCodeSCanItem>();
                                NFileCodeSCanItem.AddRange(AllSign);

                                FormHelper.WorkingWin.Dispatcher.Invoke(new Action(()=> {
                                    OneAction NOneAction = new OneAction();
                                    NOneAction.Hide();
                                    NOneAction.SetDanger(OneInFo, NFileCodeSCanItem);

                                    NOneAction.Show();
                                }));
                               


                            }

                        }
                    }
                    else
                    {
                        DangerScore -= 25;
                    }
                }
            }
            else
            {
                //White list
                DangerScore = 0;
                AllSign.Clear();
            }


            return false;
        }
    }
}
