using FileManager;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Config.JTSYQ
{
    /// <summary>
    /// FileConfigPage.xaml 的交互逻辑
    /// </summary>
    public partial class FileConfigPage : UserControl
    {
        private static FileConfigPageViewModel model;
        public FileConfigPage()
        {
            InitializeComponent();
            model = FileConfigPageViewModel.GetRedis();
            this.DataContext = model;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            model.SaveRedis();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            model = new FileConfigPageViewModel();
            this.DataContext = model;
        }

        private void PhotoSelectDir_Click(object sender, RoutedEventArgs e)
        {
            string path = FileUtils.SelectSingleExcelFile();
            if (!MyUtils.Utils.IsStrNull(path))
            {
                model.PhotoExcelPath = path;
            }
        }

        private void XZDMSelectDir_Click(object sender, RoutedEventArgs e)
        {
            string path = FileUtils.SelectSingleExcelFile();
            if (!MyUtils.Utils.IsStrNull(path))
            {
                model.XZDMExcelPath = path;
            }
        }
    }
}
