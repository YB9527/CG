using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using FileManager;
using FileManager.Pages;
using HeibernateManager.Model;
using ProgressTask;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFTemplate.Views;
using ZaiJiDi.Controller;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Pages.DataSource
{
    /// <summary>
    /// ZJDDataSourcePage.xaml 的交互逻辑
    /// </summary>
    public partial class ZJDDataSourcePage : UserControl
    {
        private ZJDController zjdController = new ZJDController();
        public static ZJDDataSourceViewModel model;

        PagerPageClass<DirPagerCustom> pagerPage = new PagerPageClass<DirPagerCustom>();
        public ZJDDataSourcePage()
        {
            InitializeComponent();
            model = SoftwareConfig.GetRedis<ZJDDataSourceViewModel>(ZJDDataSourceViewModel.RedisKey);


            pagerPage.SetPagerPage<DirPagerCustom>(model.DirPagerCustoms, DirPagerCustom.GetFieldCustoms());
            pagerPage.HideFloot();
            this.DataContext = model;
            pagerPage.AddObject(model.DirPagerCustoms);
            this.pagerPageGird.Children.Add(pagerPage);
            pagerPage.GetTabGrid().MouseDoubleClick += PagerPage_DBClick;
        }



        private void PagerPage_DBClick(object sender, MouseButtonEventArgs e)
        {
            DirPagerCustom dirPagerCustom = (sender as GridControl).CurrentItem as DirPagerCustom;
            if (dirPagerCustom != null)
            {

                DirAna(dirPagerCustom.Dir);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            model.DirPagerCustoms = pagerPage.GetAllList<DirPagerCustom>();
            SoftwareConfig.SaveRedis(ZJDDataSourceViewModel.RedisKey, model);
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            Save_Click(null, null);
            if (!MyUtils.Utils.CheckFileExists(model.QZ_BSMDBPath))
            {
                MessageBox.Show("两表，路径有问题");
                return;
            }

            else
            if (!MyUtils.Utils.CheckFileExists(model.ZdinfoMDBPath))
            {
                MessageBox.Show("宗地表，路径有问题");
                return;
            }
            else
           if (!MyUtils.Utils.CheckFileExists(model.JTCYTablePath))
            {
                MessageBox.Show("家庭成员表，路径有问题");
                return;
            }
            else
            if (!MyUtils.Utils.CheckFileExists(model.JSYDTablePath))
            {
                MessageBox.Show("建设用地表，路径有问题");
                return;
            }
            else
            if (!MyUtils.Utils.CheckFileExists(model.NFTablePath))
            {
                MessageBox.Show("农房表，路径有问题");
                return;
            }
            else if (!MyUtils.Utils.CheckFileExists(model.ZJDXZDMTablePath))
            {
                MessageBox.Show("行政代码表，路径有问题");
                return;
            }
            else
            {
                IList<string> list = zjdController.CheckZJD(model);
                this.ErrorListBox.ItemsSource = list;

            }

        }

        private void SelectJTCY_Click(object sender, RoutedEventArgs e)
        {
            string excelPath = FileUtils.SelectSingleExcelFile("选择家庭成员表");
            if (!MyUtils.Utils.IsStrNull(excelPath))
            {
                model.JTCYTablePath = excelPath;
            }

        }



        private void SelectJSYD_Click(object sender, RoutedEventArgs e)
        {
            string excelPath = FileUtils.SelectSingleExcelFile("选择建设用地表");
            if (!MyUtils.Utils.IsStrNull(excelPath))
            {
                model.JSYDTablePath = excelPath;
            }
        }

        private void SelectNF_Click(object sender, RoutedEventArgs e)
        {
            string excelPath = FileUtils.SelectSingleExcelFile("选择农房表");
            if (!MyUtils.Utils.IsStrNull(excelPath))
            {
                model.NFTablePath = excelPath;
            }
        }

        private void OpenDir(object sender, RoutedEventArgs e)
        {
            string dir = FileUtils.SelectDir();
            DirAna(dir);

        }

        private void DirAna(string dir)
        {
            if (!MyUtils.Utils.CheckDirExists(dir))
            {
                return;
            }
            model.JTCYTablePath = "";
            model.JSYDTablePath = "";
            model.NFTablePath = "";
            model.ZdinfoMDBPath = "";
            model.QZ_BSMDBPath = "";

            DirPagerCustom dirPagerCustom = new DirPagerCustom(dir);
            pagerPage.AddObject(dirPagerCustom);

            DirClass dirClass = new DirClass(dir);
            foreach (string path in dirClass.FindFileAll())
            {
                string name = System.IO.Path.GetFileName(path);
                if (name.Contains("家庭"))
                {
                    model.JTCYTablePath = path;
                }
                else if (name.Contains("建设"))
                {
                    model.JSYDTablePath = path;
                }
                else if (name.Contains("房"))
                {
                    model.NFTablePath = path;
                }
                else if (name.Contains("宗地") && name.EndsWith(".mdb"))
                {
                    model.ZdinfoMDBPath = path;
                }
                else if (name.Contains("两表") && name.EndsWith(".mdb"))
                {
                    model.QZ_BSMDBPath = path;
                }
                else if (name.Contains("地籍") && name.EndsWith(".dwg"))
                {
                    model.DWGPath = path;
                }
            }
        }

        private void EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            if (textEdit != null && textEdit.Text != null)
            {
                textEdit.SelectionStart = textEdit.Text.Length;
            }
        }

        private void SelectZJDXZDMTable_Click(object sender, RoutedEventArgs e)
        {
            string excelPath = FileUtils.SelectSingleExcelFile("选择行政代码表");
            if (!MyUtils.Utils.IsStrNull(excelPath))
            {
                model.ZJDXZDMTablePath = excelPath;
            }

        }

        private void SelectMDBPath_Click(object sender, RoutedEventArgs e)
        {
            string mdbPath = FileUtils.SelectSingleFileMDB();
            if (!MyUtils.Utils.IsStrNull(mdbPath))
            {
                model.ZdinfoMDBPath = mdbPath;
            }
        }

        private void SelectQZ_BS_MDB_Click(object sender, RoutedEventArgs e)
        {
            string mdbPath = FileUtils.SelectSingleFileMDB();
            if (!MyUtils.Utils.IsStrNull(mdbPath))
            {
                model.QZ_BSMDBPath = mdbPath;
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            FileUtils.OpenFile(textEdit.Text);
        }

        /// <summary>
        /// 签章表更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DangAnDaiRefeshMDB_Click(object sender, RoutedEventArgs e)
        {
            string selectDir = FileUtils.SelectDir();
            int a = 0;
            if (!MyUtils.Utils.IsStrNull(selectDir))
            {
                MyAction myAction= new MyAction(new Action(() =>
                {
                    DirClass dirClass = new DirClass(selectDir);
                    foreach (string dir in dirClass.Dirs)
                    {
                        DirClass dangAnDaiDir = new DirClass(dir);
                        string path = dangAnDaiDir.FindFileAllSelectOne("权籍", FileSelectRelation.Contains);

                        if (MyUtils.Utils.CheckFileExists(path))
                        {
                            string[] array = System.IO.Path.GetFileName(dir).Split('(');
                            string zdnum = array[0];
                            IList<QZB> qzbs = QZBCustom.DocToDaAnDaiQZB(path,zdnum);
                            if (MyUtils.Utils.CheckListExists(qzbs))
                            {
                                //删除以前的签章表
                                MDBUtils.DeleteBySql(model.QZ_BSMDBPath, "Delete From " + QZBCustom.QZBTableName + "  Where BZDH ='" + zdnum + "'");
                                //保存现在的
                                MDBUtils.WriteData(model.QZ_BSMDBPath, QZBCustom.QZBTableName, qzbs);
                                a++;
                            }
                        }
                    }
                })
                , "更新签章表");
                CommHelper.FastTask(myAction);
            }
        }
    }
}
