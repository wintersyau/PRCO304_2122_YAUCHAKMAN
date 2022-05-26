using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinDefense.DataManage
{
    public class AutoSave
    {
        public static int GlobalArrayHashID = 0;

        public static Thread MainThread = null;
        public static bool LockerAutoSaveService = false;
        public static void StartAutoSaveService(bool Check)
        {
            if (Check)
            {
                if (!LockerAutoSaveService)
                {
                    LockerAutoSaveService = true;

                    MainThread = new Thread(() =>
                    {
                        while (LockerAutoSaveService)
                        {
                            Thread.Sleep(1000);
                            string Content = JsonCore.JsonHelper.GetJson(DeFine.LocalSetting.WhiteList);
                            if (GlobalArrayHashID != Content.GetHashCode())
                            {
                                GlobalArrayHashID = Content.GetHashCode();
                                DeFine.LocalSetting.SetLocal();
                            }
                           
                        }
                    });
                    MainThread.Start();
                }
            }
            else
            {
                if (MainThread != null)
                {
                    try 
                    {
                    MainThread.Abort();
                    }
                    catch { }

                    LockerAutoSaveService = false;
                }
            }

        }    


       
            
            }
}
