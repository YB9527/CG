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

namespace CG.Pages.RegistPage
{
    /// <summary>
    /// ZhuCeMaPage.xaml 的交互逻辑
    /// </summary>
    public partial class ZhuCeMaPage : UserControl
    {
        private string zcm;

        public ZhuCeMaPage()
        {
            InitializeComponent();
        }

        public ZhuCeMaPage(string zcm):this()
        {
            this.zcm = zcm;
            textBox.Text = zcm;
        }
    }
}
