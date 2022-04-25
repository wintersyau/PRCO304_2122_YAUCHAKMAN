using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinDefense.ConvertManage;
using WinDefense.DataManage;
using WinDefense.FormManage;
using WinDefense.KernelManage;
using WinDefense.ProcessControl;
using WinDefense.SafeEngine;
using WinDefense.SQLManage;

namespace WinDefense
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //private void TestEngine()
        //{
        //    SQLiteHelper.InitSqliteConfig("SafeDB");//装载本地离线木马库
        //    var TryGetAccess = SafeHelper.NewSCan(@"C:\Users\Lab\source\repos\KeyBoardListener\KeyBoardListener\bin\Debug\net5.0-windows\KeyBoardListener.dll");

        //    int DangeValue = 0;
        //    foreach (var get in TryGetAccess) DangeValue += get.DangerPoint;
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TestEngine()
            //return;



            NotifyIconHelper.Init(this, "WinDefense");

            DeFine.Initialization();
            SafeHelper.Initialization();
            FormHelper.SetActiveWin(this);

            FormHelper.ShowRealTimeProcessReport();//显示内存防护系统处理摘要
        }

        private void StartQuickSCan(object sender, MouseButtonEventArgs e)
        {

            string GetBtnText = ConvertHelper.ObjToStr(CenterBtn.Content);

            if (GetBtnText == "QuickProcess")
            {
                DeFine.ClearDange();
            }
            else
            if(GetBtnText == "FastSCan")
            {
                string RichTextBox = "";

                CenterBtn.Content = "PleaseWait!";
                DeFine.SCaning = true;

                new Thread(() =>
                {
                    Thread.Sleep(1500);

                    int GetMaxCount = Process.GetProcesses().Length;

                    int ProcessCount = 0;

                    foreach (var Get in Process.GetProcesses())
                    {
                        ProcessCount++;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            CenterBtn.Content =string.Format("PleaseWaitP({0}/{1})",ProcessCount,GetMaxCount) ;
                        }));

                        try
                        {
                            var GetSign = SafeHelper.NewSCan(Get.MainModule.FileName);
                            RichTextBox += "\r\nProcess:" + Get.ProcessName + ",Access:";

                            int TotalDangePoint = 0;

                            foreach (var OSign in GetSign)
                            {
                                RichTextBox += "-" + OSign.KeyStr;
                                TotalDangePoint += OSign.DangerPoint;
                            }

                            RichTextBox += ",DangePoint:" + TotalDangePoint;
                        }
                        catch { }
                    }

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        new SCANEND().ShowMsg(RichTextBox);
                    }));

                    DeFine.SCaning = false;

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        if (DeFine.DangeCount == 0)
                        {
                            CenterBtn.Content = "FastSCan";
                        }
                        else
                        {
                            CenterBtn.Content = "QuickProcess";
                        }
                    }));
                }).Start();
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void AnyExit(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            NotifyIconHelper.ShowMsgInNotifyIcon("WinDefense", "The program is hidden in the background",1);
        }


        private void ProcessClick(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
         
        }
    }
}
