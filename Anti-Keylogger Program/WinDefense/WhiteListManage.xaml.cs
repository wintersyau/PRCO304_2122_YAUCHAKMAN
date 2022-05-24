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

                            if (SelectedValue.Contains(">"))
                            {
                                DeFine.LocalSetting.WhiteList.Remove(SelectedValue.Split('>')[1]);
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

            foreach (var Item in DeFine.LocalSetting.WhiteList)
            {
                if (Item.TrustByUser)
                {
                    Whites.Items.Add(string.Format("{0}>{1}",Item.ProcessPath,Item.CRC));
                }
             
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }
    }
}
