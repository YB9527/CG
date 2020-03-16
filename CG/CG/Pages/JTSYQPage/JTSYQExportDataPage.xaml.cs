using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Editors;
using FileManager;
using JTSYQManager.Controller;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CG.Pages.JTSYQPage
{
    /// <summary>
    /// Interaction logic for ExportDataPage.xaml
    /// </summary>
    public partial class JTSYQExportDataPage : UserControl
    {
        private static ExportDataPageViewModel model ;
        private static Dictionary<string, Clazz> clazzDic;
        private IJTSYQController JTSYQController = new JTSYQController();
        public JTSYQExportDataPage()
        {
            InitializeComponent();
            model = ExportDataPageViewModel.GetRedis();
            this.DataContext = model;
            clazzDic = ReflectUtils.MethodToFunction<ExportDataPageViewModel>();

        }

    
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //保存缓存
            model.SaveRedis();
            if(MyUtils.Utils.IsStrNull(model.SaveDir))
            {
                MessageBox.Show("没有选择保存的位置");
            }else if(MyUtils.Utils.CheckDirExists(model.SaveDir))
            {
                if(MyUtils.Utils.MessageBoxShow("确定导出资料"))
                {

                    //输出文件
                    JTSYQCustom.ReflushJTSYQ(XZDMCustom.GetSelect(MainWindow.mainWindow.tvProperty.Items));

                    IList <XZDM> xzdms = XZDMCustom.GetSelect(MainWindow.mainWindow.tvProperty.Items);
                    JTSYQController.Export(model, xzdms);
                }
              
            }else
            {
                MessageBox.Show("保存的位置不存在，请重新选择！！！");
            }

          
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ZuZiLiao.IsChecked = false;
            CunZiLiao.IsChecked = false;
            SetAllChecked("Zu", false);
            SetAllChecked("Cun", false);
            
        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            string dir = FileUtils.SelectDir();
            if (!MyUtils.Utils.IsStrNull(dir))
            {
                model.SaveDir = dir;
            }
        }

        private void ZuZiLiao_Click(object sender, RoutedEventArgs e)
        {
            bool flag = (sender as CheckBox).IsChecked.Value;
            SetAllChecked("Zu", flag);
        }
        private void CunZiLiao_Click(object sender, RoutedEventArgs e)
        {
            bool flag = (sender as CheckBox).IsChecked.Value;
            SetAllChecked("Cun", flag);
        }
        private void SetAllChecked(string startStr,bool flag)
        {
            object[] trueArray = new object[]
           {
                true
           };
            object[] falseArray = new object[]
           {
                false
           };
            foreach (string methodName in clazzDic.Keys)
            {
                if (methodName.StartsWith(startStr))
                {
                    if (flag)
                    {
                        clazzDic[methodName].SetMethodInfo.Invoke(model, trueArray);
                    }
                    else
                    {
                        clazzDic[methodName].SetMethodInfo.Invoke(model, falseArray);
                    }
                }
            }
        }

        private void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            FileUtils.OpenDir(textEdit.Text);
        }
    }
}
