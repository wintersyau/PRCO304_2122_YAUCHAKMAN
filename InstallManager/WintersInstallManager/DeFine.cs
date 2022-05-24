using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WintersInstallManager
{
    public class DeFine
    {
        public static void AdminRun()
        {
            /**
    *Startup as Administration
    */
            //Get the current user account logo
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            //Create Windows user topic
    
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity); //Identiy the Admin right
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                //run as adminstrator
              
            }
            else
            {
                //Create starting info
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //Set excutable path
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                //set perameters.
                //startInfo.Arguments = String.Join(" ", Args);
                //run as administrator
                startInfo.Verb = "runas";

                try
                {
                    //Other the administrator，activate UAC
                    System.Diagnostics.Process.Start(startInfo);
                    //shutdown
                    Application.Current.Shutdown();
                }
                catch
                {
                }
            }
        }
        public static string GetFullPath(string Path)
        {
            string GetShellPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            return GetShellPath.Substring(0, GetShellPath.LastIndexOf(@"\")) + Path;
        }
    }
}
