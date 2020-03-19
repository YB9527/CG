using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Ribbon;
using DevExpress.Xpf.Bars;
using Public;
using HeibernateManager.HibernateDao;
using DevExpress.Xpf.Docking;
using System.Collections.Specialized;
using FileManager;
using ArcMapManager.Pages;
using ArcMapManager.Pages.Views;
using ArcGisManager;
using ESRI.ArcGIS.Geodatabase;
using Config.JTSYQ;
using CG.Pages.JTSYQPage;
using CG.Pages.RegistPage;
using JTSYQManager.Controller;
using JTSYQManager.JTSYQModel;
using ZaiJiDi.Pages.ZJDPage.ExportData;
using ZaiJiDi.Pages.DataSource;
using System;
using DevExpress.Xpf.Core;
using ArcMapManager.Pages.Server;
using CBD.Pages;

namespace CG
{
    public partial class MainWindow : DXRibbonWindow
    {
        public static IJTSYQController JTSYQController = new JTSYQController();
        private static Dictionary<string, int> DocumentGroupTitleDic;
        private MapFormCustom mapFormCustom;
        public static MainWindow mainWindow = null;
        public static bool IsRegist ;
        public MainWindow()
        {
            InitializeComponent();
            //ThemeManager.ApplicationThemeName = "MetropolisLight";
            this.SizeChanged += new System.Windows.SizeChangedEventHandler(MainWindow_Resize);
            DocumentGroupTitleDic = new Dictionary<string, int>();
            for (int a = 0; a < documentGroup.Items.Count; a++)
            {
                var document = documentGroup.Items[a];
                DocumentGroupTitleDic.Add(document.Caption as string, a);
            }
            documentGroup.Items.CollectionChanged += DocumentGropChange;


            Regist regist = new Regist();
            string zcm = regist.Fun();
            if (zcm != null)
            {
                IsRegist = false;
                ///弹出注册窗口
                ZhuCeMaPage zuCeMaPage = new ZhuCeMaPage(zcm);

                int index;
                string title = zuCeMaPage.Uid as string;
                if (DocumentGroupTitleDic.TryGetValue(title, out index))
                {
                    documentGroup.SelectedTabIndex = index;
                    return;
                }
                index = documentGroup.GetChildrenCount();
                DocumentGroupTitleDic.Add(title, index);
                DocumentPanel panel = new DocumentPanel();
                panel.Caption = title;
                documentGroup.Add(panel);
                panel.Content = zuCeMaPage;
                documentGroup.SelectedTabIndex = index;
            }
            else
            {
                IsRegist = true;
                //ZJDDataSourcePage page = new ZJDDataSourcePage();
                //AddDocument(page);
                mainWindow = this;
            }
        }
        public void SwichXZDMDataPage()
        {
            if (tabbedGroup.SelectedItem != XZDMDataPage)
            {
                for (int a = 0; a < tabbedGroup.Items.Count; a++)
                {
                    var item = tabbedGroup.Items[a];
                    if (item == XZDMDataPage)
                    {
                        tabbedGroup.SelectedTabIndex = a;
                    }
                }
            }
        }
        /// <summary>
        /// documentgroup 文档流控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentGropChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewStartingIndex == -1)
            {

                string titile = ((e.OldItems.SyncRoot as object[])[0] as DocumentPanel).Caption as string;
                //移除的
                DocumentGroupTitleDic.Remove(titile);
            }
            else if (e.OldStartingIndex == -1)
            {
                //新加入的

            }
            else
            {
                foreach (string title in DocumentGroupTitleDic.Keys)
                {
                    if (DocumentGroupTitleDic[title].Equals(e.NewStartingIndex))
                    {
                        DocumentGroupTitleDic[title] = e.OldStartingIndex;

                        break;
                    }
                }
                DocumentGroupTitleDic[((e.OldItems.SyncRoot as object[])[0] as DocumentPanel).Caption as string] = e.NewStartingIndex;

            }
        }

        /// <summary>
        /// 窗口宽度变化要 更改map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Resize(object sender, SizeChangedEventArgs e)
        {


        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            PropertyNodeItem current = (sender as CheckBox).DataContext as PropertyNodeItem;
            current.SetIsSelectAll(current.IsSelected.Value);
        }

        private void TextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PropertyNodeItem current = (sender as TextBlock).DataContext as PropertyNodeItem;
            current.SetIsSelectAll(!current.IsSelected.Value);
        }
        public IList<PropertyNodeItem> GetSelectItem()
        {
            return PropertyNodeItem.FindSelect(tvProperty.Items);
        }


        public TreeView GetTvProperty()
        {
            return tvProperty;
        }
        /// <summary>
        /// 窗口关闭要做的事情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DXRibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定退出程序？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            ApplicationThemeHelper.SaveApplicationThemeName();
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                //TaskForm.Close_Click();
                IList<PropertyNodeItem> items = tvProperty.ItemsSource as IList<PropertyNodeItem>;
                if (items != null)
                {
                    PropertyNodeItemCustom.SaveDMToHibernate(items);
                    HibernateUtils.GetInstance().CloseSession();
                }
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        /// <summary>
        ///检查数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBasicData_Click(object sender, ItemClickEventArgs e)
        {
            JTSYQCheckDataPage page = new JTSYQCheckDataPage();
            AddDocument(page);

        }

        private void ExportData_Click(object sender, ItemClickEventArgs e)
        {
            SwichXZDMDataPage();
            Pages.JTSYQPage.JTSYQExportDataPage page = new Pages.JTSYQPage.JTSYQExportDataPage();
            //dockLayoutManager
            AddDocument(page);
        }
        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="userControl"></param>
        public void AddDocument(UserControl userControl)
        {
            if(!IsRegist)
            {
                return;
            }
            int index;
            string title = userControl.Uid as string;
            if (DocumentGroupTitleDic.TryGetValue(title, out index))
            {
                documentGroup.SelectedTabIndex = index;
                return;
            }
            index = documentGroup.GetChildrenCount();
            DocumentGroupTitleDic.Add(title, index);
            DocumentPanel panel = new DocumentPanel();
            panel.Caption = title;
            documentGroup.Add(panel);
            panel.Content = userControl;
            documentGroup.SelectedTabIndex = index;


        }
        /// <summary>
        /// 双击关闭 DocumentPanel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DocumentPanel_DBClick(object sender, MouseButtonEventArgs e)
        {
            documentGroup.Items.Remove(sender as DocumentPanel);
        }

        private void OpenMxd_Click(object sender, ItemClickEventArgs e)
        {
            string mxdPath = FileUtils.SelectSingleFile("选择mxd文件", MapForm.MXDFilter);
            if (!MyUtils.Utils.IsStrNull(mxdPath))
            {
                MapForm.GetInstance().LoadMXD(mxdPath);
                AddDocument(MapFormCustom.GetInstance());
            }

        }

        private static bool createTuFuGuangFlag = false;
        private static string createTuFuGuangStr = "";
        private void CreateTuFuGuang_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            createTuFuGuangFlag = !createTuFuGuangFlag;
            BarButtonItem barButtonItem = sender as BarButtonItem;
            if (createTuFuGuangFlag)
            {
                MapForm.MapMouseDownFlag = 3;
                createTuFuGuangStr = barButtonItem.Content as string;
                barButtonItem.Content = "停止生成";
            }
            else
            {
                MapForm.MapMouseDownFlag = -1;
                barButtonItem.Content = createTuFuGuangStr;

            }
        }

        private void OpenMxd2_Click(object sender, ItemClickEventArgs e)
        {
            //集体所有权添加行政代码view

            mapFormCustom = MapFormCustom.GetInstance();
            //添加地图文档
            AddDocument(mapFormCustom);
            MapTableCommand.dataPage = dataPage;
            MapTableCommand.tabbedGroup = tabbedGroup;
            MapTableCustom.SetPageSource(pageView);
           
            try
            {
               
                tvProperty.ItemsSource = JTSYQCustom.GetXZDMTree();
                if(tvProperty.ItemsSource == null)
                {
                    MessageBox.Show("请重新配置工程文件！！！");
                    return ;
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show("行政代码表有问题：" + e1.Message);
            }
          

            AddDocument(MapFormCustom.GetInstance());

            UserControl map = mapFormCustom;
            int index;
            if (DocumentGroupTitleDic.TryGetValue(map.Uid as string, out index))
            {
                documentGroup.SelectedTabIndex = index;
            }
            else
            {
                AddDocument(map);
            }


        }

        private void SaveMxd_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            MapForm.MXDSave();
        }

        private void MXDToPDF_Click(object sender, ItemClickEventArgs e)
        {
            MXDToPDFPage page = new MXDToPDFPage();
            AddDocument(page);
        }

        private void FlushTuFu_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            IList<JTSYQ> jtsyqs = GetSelectJTSYQS();
            if (jtsyqs != null)
            {
                List<IFeature> list = new List<IFeature>();
                foreach (JTSYQ jtsyq in jtsyqs)
                {
                    foreach (JTSYQ child in jtsyq.GroupJTSYQ)
                    {
                        list.Add(child.Feature);
                    }

                }
                TuFu.SetTuFu(ArcGisUtils.GetFeatureLayer("TuFu"), list);
            }
            else
            {
                MessageBox.Show("你没有选择行政区");
            }

        }
        /// <summary>
        /// 多部件是合并的，指向同一个行政代码
        /// </summary>
        /// <returns></returns>
        public IList<JTSYQ> GetSelectJTSYQS()
        {
            return JTSYQCustom.GetSelectJTSYQ(PropertyNodeItem.FindSelect(tvProperty.Items));
        }
        /// <summary>
        /// 包括所有多部件
        /// </summary>
        /// <returns></returns>
        public IList<JTSYQ> GetSelectJTSYQSAll()
        {
            IList<JTSYQ> jtsyqs = GetSelectJTSYQS();
            List<JTSYQ> list = new List<JTSYQ>();
            if (jtsyqs == null)
            {
                return list;
            }
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                list.AddRange(jtsyq.GroupJTSYQ);

            }
            return list;
        }
        private void DeleteTuFuKuang_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            MapForm.DeleteTuFuKuang();
        }

        private static bool siZiFlag = false;
        private static string siZhiStr = "";
        /// <summary>
        /// 四至开始编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SiZiStart_Click(object sender, ItemClickEventArgs e)
        {

            siZiFlag = !siZiFlag;
            BarButtonItem barButtonItem = sender as BarButtonItem;
            if (siZiFlag)
            {
                AddDocument(MapFormCustom.GetInstance());
                MapForm.MapMouseDownFlag = 4;
                siZhiStr = barButtonItem.Content as string;
                barButtonItem.Content = "四至停止编辑";
            }
            else
            {
                MapForm.MapMouseDownFlag = -1;
                barButtonItem.Content = siZhiStr;

            }
        }


        /// <summary>
        /// 提取界址点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateJZD_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            IList<JTSYQ> jtsyqs = GetSelectJTSYQS();
            if (jtsyqs == null)
            {
                MessageBox.Show("你还没有选择要生成的组");
            }
            else
            {
                JTSYQController.ExtractJZD_Intersectant(jtsyqs);
                ArcGisUtils.axMapControl.ActiveView.Refresh();
            }
        }
        /// <summary>
        /// 界址点编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JZDHNumber_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            IList<JTSYQ> jtsyqs = GetSelectJTSYQS();
            if (jtsyqs == null)
            {
                MessageBox.Show("你还没有选择要编码的组");
            }
            else
            {
                JTSYQController.JZDBM(jtsyqs);
                ArcGisUtils.axMapControl.ActiveView.Refresh();
            }



        }
        /// <summary>
        /// 删除界址点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteJZD_Click(object sender, ItemClickEventArgs e)
        {
            AddDocument(MapFormCustom.GetInstance());
            IList<JTSYQ> jtsyqs = GetSelectJTSYQS();
            if (jtsyqs == null)
            {
                MessageBox.Show("你还没有选择要删除界址点的组");
            }
            else
            {
                JTSYQController.DeleteJZD(jtsyqs);
                ArcGisUtils.axMapControl.ActiveView.Refresh();
            }


        }
        /// <summary>
        /// 文件配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileConfig_Click(object sender, ItemClickEventArgs e)
        {
            FileConfigPage page = new FileConfigPage();
            AddDocument(page);

        }

        private void ZJDDataSourcePage_Click(object sender, ItemClickEventArgs e)
        {
            ZJDDataSourcePage page = new ZJDDataSourcePage();
            AddDocument(page);
        }

        private void ZJDExportData_Click(object sender, ItemClickEventArgs e)
        {
            if (ZJDDataSourcePage.model == null)
            {
                MessageBox.Show("主点击数据源设置页面！！！");
                return;
            }
            try
            {
                ZJDExportDataPage page = new ZJDExportDataPage(ZJDDataSourcePage.model);
                AddDocument(page);
            }catch
            {

            }

        }

        /// <summary>
        /// 在arcgis 中 tif 转 image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TifToImage_Click(object sender, ItemClickEventArgs e)
        {
            TifToImagePage page = new TifToImagePage();
            AddDocument(page);

        }

        private void MapOnServer_Click(object sender, ItemClickEventArgs e)
        {
            MapOnServerPage page = new MapOnServerPage();
            AddDocument(page);
        }

        /// <summary>
        /// Excel 转 shp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcelToMap_Click(object sender, ItemClickEventArgs e)
        {
            IList<string> paths = FileUtils.SelectExcelFiles();
            if(MyUtils.Utils.CheckListExists(paths))
            {
                MapFormCustom mapFormCustom = MapFormCustom.GetInstance();
                ArcGisController arcGisController = new ArcGisController(MapForm.GetAxMapControl());
                try
                {
                    arcGisController.ExcelToShp(paths);
                }
                catch (Exception e1)
                {
                    MessageBox.Show("表格有问题，请注意检查！！！");
                }
            }       
        }

        private void CBDTableChange_Click(object sender, ItemClickEventArgs e)
        {
            TabelChangePage page = new TabelChangePage();
            AddDocument(page);
        }

        /// <summary>
        /// 将shp导出 成单个的 mxd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpliceSHPToMxd_Click(object sender, ItemClickEventArgs e)
        {
            //打开map

            //得到选中的地块  
        
            IList<IFeature> features = ArcGisUtils.GetInstance().GetSelectFeature();
            //得到模板mxd

            //导出mxd


        }

        private void ArcgisTool_OpenMxd_Click(object sender, ItemClickEventArgs e)
        {
            //string mxdPath = FileUtils.SelectSingleFile("选择mxd文件", MapForm.MXDFilter);
            string mxdPath = @"D:\桌面\所有权测试数据及样本\高新测试.mxd";
            if (!MyUtils.Utils.IsStrNull(mxdPath))
            {
                MapForm.GetInstance().LoadMXD(mxdPath);                  
                AddDocument(MapFormCustom.GetInstance());
                
            }
        }
    }

}
