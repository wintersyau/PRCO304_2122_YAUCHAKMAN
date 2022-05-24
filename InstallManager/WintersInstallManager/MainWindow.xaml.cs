
using Microsoft.Win32;
using WintersInstallManager.InstallManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace WintersInstallManager
{
    /// <summary>
    /// MainWindow.xaml exchange logic
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeFine.AdminRun();

            ProcessBackground.Width = this.ActualWidth-(15*2);

            string Disk = "";
            DriveInfo[] Drives = DriveInfo.GetDrives();


            foreach (DriveInfo GetDrive in Drives)
            {

                if (GetDrive.DriveType == DriveType.Fixed && string.Compare(System.IO.Path.GetPathRoot(Environment.SystemDirectory), GetDrive.Name, true) != 0) //固定硬盘且是非系统盘
                {
                    Disk = GetDrive.Name.Replace(@"\", "");
                    break;
                }

            }
            if (Disk.Trim() == "")
            {
                Disk = Environment.GetEnvironmentVariable("systemdrive");
            }

            if (!Disk.EndsWith(":"))
            {
                Disk += ";";
            }


            InstallPath.Text = Disk + @"\Program Files (x86)\" + @"Winters\";
        }


        public bool CanInstall = false;
        public bool Installing = false;


        public void CreatUnInstall(string UnPath)
        {
            try
            {

                //RegistryKey localMachin64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                //RegistryKey key = localMachin64.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\", true);

                //key.CreateSubKey("WinDefense");
                //key.Close();

                //RegistryKey key1 = localMachin64.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\WinDefense", true);
                //key1.SetValue("DisplayName", "WinDefense");
                //key1.SetValue("UninstallString", UnPath + @"uninstall.exe");
                //key1.SetValue("DisplayIcon", UnPath + @"WinDefense.exe");
                //key1.SetValue("DisplayVersion", "V1.0");
                //key1.SetValue("Publisher", "Winters");
                //key1.Close();

            }
            catch (Exception Ex) 
            {
            
            
            }
        }
        public void InstallEnd()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                InstallHelper.StartProcess(InstallPath.Text + @"\WinDefense.exe", "");
            }));

            Application.Current.Shutdown();
        }

        public static string InstallSucessPath = "";

        public static int ThisThread = 0;
        public object LockerInstall = new object();
        public void InstallPack(string InstallPath)
        {
            string Disk = "";
            DriveInfo[] Drives = DriveInfo.GetDrives();


            foreach (DriveInfo GetDrive in Drives)
            {

                if (GetDrive.DriveType == DriveType.Fixed && string.Compare(System.IO.Path.GetPathRoot(Environment.SystemDirectory), GetDrive.Name, true) != 0) //固定硬盘且是非系统盘
                {
                    Disk = GetDrive.Name.Replace(@"\","");
                    break;
                }

            }
            if (Disk.Trim() == "")
            {
                Disk = Environment.GetEnvironmentVariable("systemdrive");
            }

            if (!Disk.EndsWith(":"))
            {
                Disk += ";";
            }

            if (InstallPath.Trim().Length == 0)
            {
                InstallPath = Disk + @"\Program Files (x86)\" + @"Winters\";
            }

            new Thread(() =>
            {
                if (!InstallPath.EndsWith(@"\"))
                {
                    InstallPath += @"\";
                }
                if (!InstallPath.EndsWith("Winters" + @"\"))
                {
                    InstallPath += @"Winters" + @"\";
                }

                if (!Directory.Exists(InstallPath))
                {
                    try
                    {
                        Directory.CreateDirectory(InstallPath);
                    }
                    catch { MessageBox.Show("Insufficient permission, please select another drive"); return; }
                }
                else 
                {
                    Process[] GetPr = Process.GetProcessesByName("WinDefense");

                    foreach (var Get in GetPr)
                    {
                        Get.Kill();
                        Get.Dispose();
                    }
                }

                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.InstallPath.Text = InstallPath;
                }));

                double GetBarOffset = 0;
                double MaxOffset = 0;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    GetBarOffset = ProcessBar.Width;
                    MaxOffset = this.ActualWidth - (15 * 2);

                }));

                while (GetBarOffset < MaxOffset)
                {
                    GetBarOffset++;

                    ProcessBar.Dispatcher.Invoke(new Action(() =>
                    {
                        ProcessBar.Width = GetBarOffset;
                    }));

                    if (GetBarOffset < Convert.ToInt32(MaxOffset * 0.1))
                    {
                        this.Dispatcher.Invoke(new Action(()=> {
                            CurrentText.Content = "Installation is in preparation";
                        }));
                        
                    }
                    else
                    if (GetBarOffset < Convert.ToInt32(MaxOffset * 0.2))
                    {
                        this.Dispatcher.Invoke(new Action(() => {
                            CurrentText.Content = "Extracting files";
                        }));
                    }
                    else
                    if (GetBarOffset == Convert.ToInt32(MaxOffset * 0.3))
                    {
                        this.Dispatcher.Invoke(new Action(() => {
                            CurrentText.Content = "Installation is in Progress";
                        }));

                        new Thread(() =>
                        {
                            lock (LockerInstall)
                            {
                                if (ThisThread > 0) return;

                                ThisThread++;

                                var CurrentData = InstallHelper.GetResFile("Main.zip");

                                try {
                                DataHelper.UnZip(CurrentData, InstallPath);
                                }
                                catch { MessageBox.Show("Insufficient permission, please select another drive"); return; }

                                if (InstallHelper.CreateDesktopShortcut("WinDefense.exe", InstallPath))
                                {

                                }
                               
                                Thread.Sleep(3000);

                                GetBarOffset = MaxOffset;

                                ProcessBar.Dispatcher.Invoke(new Action(() =>
                                {
                                    ProcessBar.Width = GetBarOffset;
                                }));

                                Thread.Sleep(1000);

                                //InstallSucess

                                this.Dispatcher.Invoke(new Action(() => {
                                    CurrentText.Visibility = Visibility.Collapsed;
                                    ShowProcess.Visibility = Visibility.Collapsed;
                                    InstallSucessBtn.Visibility = Visibility.Visible;
                                }));

                                InstallSucessPath = InstallPath;

                                if (InstallSucessPath.EndsWith(@"\"))
                                {

                                }
                                else
                                {
                                    InstallSucessPath += @"\";
                                }

                                CreatUnInstall(InstallSucessPath);
                            }

                           
                          
                        }).Start();
                    }
                    

                    Thread.Sleep(5);
                }

            }).Start();

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string CurrentName = "";

            if (sender is Border)
            {
                Border LockerBorder = (Border)sender;
                CurrentName = LockerBorder.Name;
            }

            if (sender is Image)
            {
                Image LockerImg = (Image)sender;
                CurrentName = LockerImg.Name;
            }

            if (sender is Label)
            {
                Label LockerLab = (Label)sender;
                CurrentName = LockerLab.Name;
            }

            if (CurrentName == "UNCheck")
            {
                if (Installing) return;
                UNCheck.Visibility = Visibility.Collapsed;
                Checked.Visibility = Visibility.Visible;
                CheckedF.Visibility = Visibility.Visible;
                InstallBtn.Background = new SolidColorBrush(Color.FromRgb(234, 150, 20));
                InstallCap.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                CanInstall = true;
            }
            else
            if (CurrentName == "Checked")
            {
                if (Installing) return;
                UNCheck.Visibility = Visibility.Visible;
                Checked.Visibility = Visibility.Collapsed;
                CheckedF.Visibility = Visibility.Hidden;
                InstallBtn.Background = new SolidColorBrush(Color.FromRgb(75, 75, 75));
                InstallCap.Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 31));
                CanInstall = false;
            }
            else
            if (CurrentName == "CheckedF")
            {
                if (Installing) return;
                UNCheck.Visibility = Visibility.Visible;
                Checked.Visibility = Visibility.Collapsed;
                CheckedF.Visibility = Visibility.Hidden;
                InstallBtn.Background = new SolidColorBrush(Color.FromRgb(75, 75, 75));
                InstallCap.Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 31));
                CanInstall = false;
            }
            else
            {
                if (CanInstall)
                {
                    ReadyInstall.Visibility = Visibility.Collapsed;
                    NowInstall.Visibility = Visibility.Visible;
                    DivContent.Visibility = Visibility.Collapsed;

                    InstallPack(InstallPath.Text);

                    Installing = true;

                    OneSelectCon.Visibility = Visibility.Hidden;
                }
                else
                { 
                
                }
            }

        }

        private void DivSelect(object sender, MouseButtonEventArgs e)
        {
            if (DivContent.Content.ToString() == "Custom installation")
            {
                DivPath.Visibility = Visibility.Visible;
                DivContent.Content = "Fast installation";
            }
            else
            {
                DivPath.Visibility = Visibility.Hidden;
                DivContent.Content = "Custom installation";
            }
        }

        private void SelectPath(object sender, MouseButtonEventArgs e)
        {
            InstallPath.Text = DataHelper.SelectPath();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProcessBackground.Width = this.ActualWidth - (15 * 2);
        }

        private void EndInstall(object sender, MouseButtonEventArgs e)
        {
            InstallEnd();
        }

        private void Agreement(object sender, MouseButtonEventArgs e)
        {
            if (!InstallHelper.Agreement)
            {
                InstallHelper.Agreement = true;
                new ShowAgreement().Show();
            }
           
        }

        private void CloseThis(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown(-1);
        }

        private void MinThis(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
