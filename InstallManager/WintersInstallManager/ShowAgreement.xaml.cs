using WintersInstallManager.InstallManager;
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

namespace WintersInstallManager
{
    /// <summary>
    /// ShowAgreement.xaml 
    /// </summary>
    public partial class ShowAgreement : Window
    {
        public ShowAgreement()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OneText.Text = OneText.Text.Replace("}", "\r\n");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InstallHelper.Agreement = false;
        }

        private void OneText_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
