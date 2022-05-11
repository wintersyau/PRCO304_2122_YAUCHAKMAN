using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinDefense.ConvertManage;
using WinDefense.KernelManage;
using WinDefense.ProcessControl;
using WinDefense.SQLManage;

namespace WinDefense
{
    /// <summary>
    /// OneAction.xaml exchange logical
    /// </summary>
    public partial class OneAction : Window
    {
        public OneAction()
        {
            InitializeComponent();
        }

        public ProcessInFo CurrentInFo = null;

        public void SetDanger(ProcessInFo OneDanger, List<FileCodeSCanItem> AllSign)
        {
            CurrentInFo = OneDanger;
            ProcessPath.Content = OneDanger.FilePath;
            ProcessName.Content = OneDanger.ProcessName;

            foreach (var GetSign in AllSign)
            {
                ProcessAccess.Content +="-" + GetSign.KeyStr;
                Signs.Items.Add(string.Format("Operation:{0},Sign:{1}:Describe:{2}",GetSign.KeyStr,GetSign.ShortTittle,GetSign.Describe));
            }
          
        }

        private void Signs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSelectStr.Text = ConvertHelper.ObjToStr(Signs.SelectedItem);
        }

  

        private void PassProcess(object sender, RoutedEventArgs e)
        {
            if (SafeEngine.SafeHelper.CheckProcessCache.Contains(CurrentInFo.FilePath))
            {
                SafeEngine.SafeHelper.CheckProcessCache.Remove(CurrentInFo.FilePath);
            }

            DeFine.DangeCount++;
            ProcessOperation.SuperByControlProcess(CurrentInFo.Pid, true);
            this.Close();
        }

        private void KillProcess(object sender, RoutedEventArgs e)
        {
            if (CurrentInFo == null == false)
            {
                try 
                { 
                Ring3ProcessOperation.SuperByKillProcess(CurrentInFo.Pid);
                }
                catch { }
                ProcessOperation.SuperByKillProcess(CurrentInFo.Pid);

                try { 
                if (File.Exists(CurrentInFo.FilePath))
                {
                        if (MessageBox.Show(string.Format("Delete {0} ?", CurrentInFo.ProcessName), "Action", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            FileSystem.DeleteFile(CurrentInFo.FilePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                       
                }

                this.Close();
                }
                catch { }
            }
        }

        private void OpenPath(object sender, RoutedEventArgs e)
        {
            string GetPath = ConvertHelper.ObjToStr(ProcessPath.Content);
            GetPath = GetPath.Substring(0, GetPath.LastIndexOf(@"\"));
            System.Diagnostics.Process.Start("explorer.exe", GetPath);
        }

        private void TrustProcess(object sender, RoutedEventArgs e)
        {
            ProcessOperation.SuperByControlProcess(CurrentInFo.Pid, true);
            this.Close();
        }
    }
}
