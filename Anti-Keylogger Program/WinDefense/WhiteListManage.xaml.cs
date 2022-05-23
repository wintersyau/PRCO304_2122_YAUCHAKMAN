using System;
using System.Collections.Generic;
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
using WinDefense.SafeEngine;

namespace WinDefense
{
    /// <summary>
    /// Interaction logic for WhiteListManage.xaml
    /// </summary>
    public partial class WhiteListManage : Window
    {
        public WhiteListManage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AnyBtnDown(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                Button LockerBtn = sender as Button;

                switch (ConvertHelper.ObjToStr(LockerBtn.Content))
                {
                    case "Remove":
                        {
                            string SelectedValue = ConvertHelper.ObjToStr(Whites.SelectedValue);

                            if (SafeHelper.CheckProcessCache.Contains(SelectedValue))
                            {
                                SafeHelper.CheckProcessCache.Remove(SelectedValue);
                            }

                            if (DeFine.Trusts.Contains(SelectedValue))
                            {
                                DeFine.Trusts.Remove(SelectedValue);
                            }

                            ReloadData();
                        }
                        break;
                }
            }
        }


        public void ReloadData()
        {
            Whites.Items.Clear();

            foreach (var Item in SafeHelper.CheckProcessCache)
            {
               if (DeFine.Trusts.Contains(Item))
                {
                    Whites.Items.Add(Item);
                }
             
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }
    }
}
