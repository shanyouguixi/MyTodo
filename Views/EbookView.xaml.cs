using MyTodo.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyTodo.Views
{
    /// <summary>
    /// EbookView.xaml 的交互逻辑
    /// </summary>
    public partial class EbookView : UserControl
    {
        private bool _isLoaded = false;
        public EbookView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                string path = (string)btn.Tag;
                ProcessStartInfo processStartInfo = new ProcessStartInfo(path);
                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.StartInfo.UseShellExecute = true;
                process.Start();                

            }


        }
    }
}
