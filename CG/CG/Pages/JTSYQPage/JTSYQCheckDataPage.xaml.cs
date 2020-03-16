using FirstFloor.ModernUI.Presentation;
using HeibernateManager.Model;
using JTSYQManager.Controller;
using JTSYQManager.JTSYQModel;
using ProgressTask;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CG.Pages.JTSYQPage
{
    /// <summary>
    /// JTSYQCheckDataPage.xaml 的交互逻辑
    /// </summary>
    public partial class JTSYQCheckDataPage : UserControl
    {
        public static IJTSYQController JTSYQController = new JTSYQController();
        JTSYQCheckDataViewModel model;
        public JTSYQCheckDataPage()
        {
            InitializeComponent();
            model = SoftwareConfig.GetRedis<JTSYQCheckDataViewModel>(JTSYQCheckDataViewModel.RedisKey);
            this.DataContext = model;
           
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            
            List<MyAction> actions = new List<MyAction>();
            SoftwareConfig.SaveRedis(JTSYQCheckDataViewModel.RedisKey, model);
            IList<JTSYQ> jtsyqs = MainWindow.mainWindow.GetSelectJTSYQS();

            if(!MyUtils.Utils.CheckListExists(jtsyqs))
            {
                MessageBox.Show("你还没有选择行政区！！！");
                return;
            }

            if (model.BasicData)
            {
                actions.AddRange(JTSYQCustom.CheckDataActions(jtsyqs));
            }
            if(model.Angle)
            {

                actions.AddRange(JTSYQCustom.CheckAngle(jtsyqs, model.AngleValue));
            }
            SingleTaskForm taskForm = new SingleTaskForm(actions,"检查");
        }
    }
     class JTSYQCheckDataViewModel: NotifyPropertyChanged
    {
        public static string RedisKey = "JTSYQCheckData";

       

        private bool basicData;
        public bool BasicData
        {
            get { return this.basicData; }
            set
            {
                if (this.basicData != value)
                {
                    this.basicData = value;
                    OnPropertyChanged("basicData");
                }
            }
        }
        private bool angle;
        public bool Angle
        {
            get { return this.angle; }
            set
            {
                if (this.angle != value)
                {
                    this.angle = value;
                    OnPropertyChanged("angle");
                }
            }
        }
        private double angleValue;
        public double AngleValue
        {
            get { return this.angleValue; }
            set
            {
                if (this.angleValue != value)
                {
                    this.angleValue = value;
                    OnPropertyChanged("angleValue");
                }
            }
        }

      
    }
}
