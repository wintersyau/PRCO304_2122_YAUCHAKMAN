using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using File = System.IO.File;

namespace WintersInstallManager.InstallManager
{
    public class InstallHelper
    {
        public static bool Agreement = false;
        public static void StartProcess(string FilePath, string Args)
        {
            ProcessStartInfo ProcessInfo;

            Process NewProcess;

            ProcessInfo = new ProcessStartInfo();

            ProcessInfo.FileName = Path.GetFileName(FilePath);

            ProcessInfo.WindowStyle = ProcessWindowStyle.Normal;

            ProcessInfo.WorkingDirectory = Path.GetDirectoryName(FilePath);

            ProcessInfo.UseShellExecute = true;

            ProcessInfo.Arguments = Args;

            NewProcess = new Process();

            NewProcess.StartInfo = ProcessInfo;

            NewProcess.Start();
        }
        public static void CheckFile()
        {
            NextTry:
            Thread.Sleep(100);

            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\ICSharpCode.SharpZipLib.dll"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\ICSharpCode.SharpZipLib.dll");
                }
                if (File.Exists(Environment.CurrentDirectory + @"\ICSharpCode.SharpZipLib.xml"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\ICSharpCode.SharpZipLib.xml");
                }
                if (File.Exists(Environment.CurrentDirectory + @"\WintersInstallManager.exe.config"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\WintersInstallManager.exe.config");
                }


                if (File.Exists(Environment.CurrentDirectory + @"\WintersInstallManager.pdb"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\WintersInstallManager.pdb");
                } 
                if (File.Exists(Environment.CurrentDirectory + @"\ICSharpCode.SharpZipLib.pdb"))
                {
                    File.Delete(Environment.CurrentDirectory + @"\ICSharpCode.SharpZipLib.pdb");
                }
             
            }
            catch { goto NextTry; }
        }

        public static List<string> GetUserDrives()
        {
            List<string> DeviceIDs = new List<string>();
            System.IO.DriveInfo[] Drives = System.IO.DriveInfo.GetDrives();
            foreach (var Get in Drives)
            {
                DeviceIDs.Add(Get.Name);
            }

            return DeviceIDs;
        }
        public static void ExtractResFile(string resFileName, string outputFile)
        {
            BufferedStream inStream = null;
            FileStream outStream = null;
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly(); 
                inStream = new BufferedStream(asm.GetManifestResourceStream("WintersInstallManager" + ".Application." + resFileName));
                outStream = new FileStream(outputFile+ resFileName, FileMode.Create, FileAccess.Write);

                byte[] buffer = new byte[1024];
                int length;

                while ((length = inStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, length);
                }
                outStream.Flush();
            }
            finally
            {
                if (outStream != null)
                {
                    outStream.Close();
                }
                if (inStream != null)
                {
                    inStream.Close();
                }
            }
        }

        public static Stream GetResFile(string ResFileName)
        {
            try
            {
                string ProjectName = Assembly.GetExecutingAssembly().GetName().ToString();
                Assembly Asm = Assembly.GetExecutingAssembly(); 
                var GetData = Asm.GetManifestResourceStream("WintersInstallManager" + ".Application." + ResFileName);
                return GetData;
            }
            catch
            {
                return null;
            }
        }


        public static bool CreateDesktopShortcut(string FileName, string exePath)
        {
            try
            {
                string deskTop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
                if (System.IO.File.Exists(deskTop + FileName + ".lnk"))  //
                {
                    System.IO.File.Delete(deskTop + FileName + ".lnk");//Delete desktop shortcut
                }
                WshShell shell = new WshShell();

                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(deskTop + "WinDefense" + ".lnk");
                shortcut.TargetPath = exePath + "\\" + FileName; //Target shortcut
                shortcut.WorkingDirectory = exePath;
                shortcut.WindowStyle = 1; //Setting window style【1,3,7】
                shortcut.Description = "Winters"; 
                shortcut.IconLocation = exePath + "\\logo.ico";  //icon location
                shortcut.Arguments = "";
                //shortcut.Hotkey = "CTRL+ALT+F11"; 
                shortcut.Save(); //save
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




    }
}
