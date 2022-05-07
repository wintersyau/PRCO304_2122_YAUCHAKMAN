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

namespace WinDefense
{
    /// <summary>
    /// SCANEND.xaml 的交互逻辑
    /// </summary>
    public partial class SCANEND : Window
    {
        public SCANEND()
        {
            InitializeComponent();
        }

        public void ShowMsg(string Text)
        {
            SCANDSP.Text = Text;
            this.Show();
        }

        private void SCANDSP_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
