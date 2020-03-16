using DevExpress.Xpf.Editors;
using FileManager;
using HeibernateManager.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ZaiJiDi.Controller;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.ZaiJiDiModel;


namespace ZaiJiDi.Pages.ZJDPage.ExportData
{

    /// <summary>
    /// ExportDataPage.xaml 的交互逻辑
    /// </summary>
    public partial class ZJDExportDataPage : UserControl
    {
        private ZJDController zjdController = new ZJDController();
        public static ZJDExportDataViewModel model;
        public ZJDExportDataPage(ZJDDataSourceViewModel DataSourceViewModel)
        {
            InitializeComponent();

            
            var jsydCustom = JSYDPagerPageViewModel.GetViewModel(zjdController.GetJSYD(DataSourceViewModel));
            pagerPage.SetPagerPage<JSYDPagerPageViewModel>(jsydCustom, JSYDPagerPageViewModel.GetFieldCustoms(), true, true);
            pagerPage.SetDataSourceViewModel(DataSourceViewModel);

           model = SoftwareConfig.GetRedis<ZJDExportDataViewModel>(ZJDExportDataViewModel.RedisKey);
            this.DataContext = model;

        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            string dir = FileManager.FileUtils.SelectDir();
            if (MyUtils.Utils.CheckDirExists(dir))
            {
                model.SaveDir = dir;
            }
        }

        /// <summary>
        /// 导出资料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SoftwareConfig.SaveRedis(ZJDExportDataViewModel.RedisKey, model);
            if (!MyUtils.Utils.CheckDirExists(model.SaveDir))
            {
                MessageBox.Show("没有选择保存的文件夹！！！");
                return;
            }

            IList<JSYDPagerPageViewModel> jsydCustoms = pagerPage.GetChekedAll<JSYDPagerPageViewModel>();
            IList<JSYD> jsyds = new List<JSYD>();
            foreach (JSYDPagerPageViewModel custom in jsydCustoms)
            {
                jsyds.Add(custom.JSYD);
            }

            zjdController.ExportZJD(jsyds, model);
            MessageBox.Show("导出完成");
;        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            
            FileUtils.OpenDir(textEdit.Text);
        }

        private void SaveRedis_Click(object sender, RoutedEventArgs e)
        {
            SoftwareConfig.SaveRedis(ZJDExportDataViewModel.RedisKey, model);
        }
    }
}
