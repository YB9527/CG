using HeibernateManager.Model;
using MyUtils;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgressTask
{
    /// <summary>
    /// WaitForm.xaml 的交互逻辑
    /// </summary>
    public partial class WaitForm : Window
    {

        private static WaitForm self { get; set; }
        public WaitForm(string taskName = "任务进行中")
        {
            //this.ShowInTaskbar = false;
            InitializeComponent();
            this.TaskName.Content = taskName;


        }
        public WaitForm(MyAction action, bool autoStart = true)
        {
            //this.ShowInTaskbar = false;
            InitializeComponent();
            this.TaskName.Content = action.TaskName;
            CommHelper.FastTask(action);
        }
       
        /*
        internal static WaitForm GetInstance(string  taskName = "任务进行中")
        {
            if(self == null)
            {
                self = new WaitForm();
            }
           
            self.TaskName.Content = taskName;
            return self;
        }*/
    }
}
