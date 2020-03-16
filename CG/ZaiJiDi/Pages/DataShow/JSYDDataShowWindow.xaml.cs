using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using DevExpress.Xpf.Core;
using ReflectManager;
using ZaiJiDi.Pages.ZJDPage;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.Pages.ZJDPage.ExportData;
using ZaiJiDi.ZaiJiDiModel;
using Expression = System.Linq.Expressions.Expression;
using Spire.Doc;
using WordManager;
using ZaiJiDi.Dao;
using Utils = MyUtils.Utils;
using FileManager;

namespace ZaiJiDi.Pages.DataShow
{
    /// <summary>
    /// Interaction logic for JSYDDataShowWindow.xaml
    /// </summary>
    public partial class JSYDDataShowWindow : DXWindow
    {

        JSYD ectypalJSYD;//编辑都用这个
        JSYD JSYD;//保存才更新过来

        ZJDDataSourceViewModel dataSourceViewModel;
        public JSYDDataShowWindow()
        {
            InitializeComponent();

        }

        public JSYDDataShowWindow(JSYD jsyd, ZJDDataSourceViewModel dataSourceViewModel) : this()
        {
            this.JSYD = jsyd;

            ectypalJSYD = JSYDCustom.Clone(jsyd);


            this.dataSourceViewModel = dataSourceViewModel;
            this.DataContext = ectypalJSYD;
            if (MyUtils.Utils.CheckListExists(ectypalJSYD.HZs))
            {
                JTCYTable.ItemsSource = ectypalJSYD.HZs[0].JTCies;
                LXDHTexEdit.Text = JSYD.HZs[0].LXDH;
                JTCYCountTextEdit.Content = JSYD.HZs[0].JTCies.Count;
            }
            //NFTable.ItemsSource = ectypalJSYD.NFs;
            //FloorTable.ItemsSource = ectypalJSYD.Floors;
            //QZBTable.ItemsSource = ectypalJSYD.QZBs;
            //BSBTable.ItemsSource = ectypalJSYD.JZXInfos;
            //签章表
            ContextMenu aMenu = new ContextMenu();
            MenuItem lastMenu = new MenuItem();
            lastMenu.Header = "上行插入";
            lastMenu.Click += QZBTableLastMenu_Click;
            aMenu.Items.Add(lastMenu);

            MenuItem nextMenu = new MenuItem();
            nextMenu.Header = "下行行插入";
            nextMenu.Click += QZBTableNextMenu_Click;
            aMenu.Items.Add(nextMenu);

            MenuItem deleteMenu = new MenuItem();
            deleteMenu.Header = "删除";
            deleteMenu.Click += QZBTableDeleteMenu_Click;
            aMenu.Items.Add(deleteMenu);
            QZBTable.ContextMenu = aMenu;


            //标示表
            ContextMenu bMenu = new ContextMenu();
            MenuItem blastMenu = new MenuItem();
            blastMenu.Header = "上行插入";
            blastMenu.Click += BSBTableLastMenu_Click;
            bMenu.Items.Add(blastMenu);

            MenuItem bnextMenu = new MenuItem();
            bnextMenu.Header = "下行行插入";
            bnextMenu.Click += BSBTableNextMenu_Click;
            bMenu.Items.Add(bnextMenu);

            MenuItem bdeleteMenu = new MenuItem();
            bdeleteMenu.Header = "删除";
            bdeleteMenu.Click += BSBTableDeleteMenu_Click;
            bMenu.Items.Add(bdeleteMenu);
            BSBTable.ContextMenu = bMenu;

            //注入标示表
            PageAddDangAnDdaiQZBAndBSB();
            //检查农房表 与 建设用地表
            GetNF_Floor_Area();
            TipBox1.Text = "未确权颁证\r\n遗失/灭失\r\n已提交原件";
            TipBox2.Text = "其他情形\r\n改建扩建";
            TipBox3.Text = "自建\r\n翻建";

            
        }

        /// <summary>
        /// 得到 分层表 与农房表 面积之差
        /// </summary>
        /// <returns></returns>
        private double GetNF_Floor_Area()
        {
            double area =0;
            double tem = 0;
            if (Utils.CheckListExists(ectypalJSYD.Floors) && Utils.CheckListExists(ectypalJSYD.NFs))
            {
                foreach (Floor floor in ectypalJSYD.Floors)
                {
                    area += Math.Round(floor.CJZMJ, 2);
                }
                 tem = Math.Round(area - ectypalJSYD.NFs[0].JZMJ, 2);
                AreaDxTextEdit.Text = tem+"";
            }
            return tem;
            
           
        }
        IList<JZXInfo> dangAnDaiJZXs;
        /// <summary>
        /// 档案袋中加入 签章表 标示表
        /// </summary>
        private void PageAddDangAnDdaiQZBAndBSB()
        {
            StringBuilder sb = new StringBuilder();
            //检查是否有界址线
            if (!MyUtils.Utils.CheckListExists(ectypalJSYD.JZXInfos))
            {
                sb.Append("MDB没有界址线数据");
            }
            else
            {
                int jzxLength = ectypalJSYD.JZXInfos.Count;
                dangAnDaiJZXs = GetDangAnDaiJZXs();


                //读取档案中的 界址标示表


                //1、对比长度
                if (!MyUtils.Utils.CheckListExists(dangAnDaiJZXs))
                {
                    sb.Append("档案袋没有界址线数据");

                }
                else
                {
                    DangAnDaiBSBTable.ItemsSource = dangAnDaiJZXs;
                    int dangJZXLength = dangAnDaiJZXs.Count;
                    if (jzxLength != dangJZXLength)
                    {
                        sb.Append("档案袋界址线共：" + dangJZXLength+" 条,");
                    }
                    //2、对比内容，只要有一条不一样就返回结果
                    int length = jzxLength < dangJZXLength ? jzxLength : dangJZXLength;//谁短取谁
                    for (int a = 0; a < length; a++)
                    {
                        JZXInfo jzx = ectypalJSYD.JZXInfos[a];
                        JZXInfo dangJZX = dangAnDaiJZXs[a];
                        if (jzx.QDH != dangJZX.QDH)
                        {
                            //起点号
                            sb.Append("第 " + (a + 1) + " 条界址线起点号不相等");
                        }
                        else if (jzx.ZDH != dangJZX.ZDH)
                        {
                            //终点号
                            sb.Append("第 " + (a + 1) + " 条界址线终点号不相等");
                        }
                        else if (jzx.TSBC != dangJZX.TSBC)
                        {
                            //距离
                            sb.Append("第 " + (a+1) + " 条界址线距离不相等...");
                        }
                        //else if (jzx.JZXLB != dangJZX.JZXLB)
                        //{
                        //    //界址线类别
                        //    sb.Append("第 " + a + " 条界址线类别不相等");
                        //}
                        //else if (jzx.JZXWZ != dangJZX.JZXWZ)
                        //{
                        //    //界址线位置
                        //    sb.Append("第 " + a + " 条界址线位置不相等");
                        //}
                    }

                }

            }
            //3、显示结果
            errorLabel.Content = sb.ToString();

        }

        private IList<JZXInfo> GetDangAnDaiJZXs()
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string quanJiDiaoChaTablePath = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.QJDCBDocPath);
            if (MyUtils.Utils.CheckFileExists(quanJiDiaoChaTablePath))
            {
                //读取档案中的 界址标示表
               // Document doc = SpireDocUtils.ReadWord(quanJiDiaoChaTablePath);
               // IList<JZXInfo> dangAnDaiJZXs = JZXInfoCustom.ReadBSBByDangAnDai(doc, JSYD.ZDNUM);
                return null;
            }
            return null;
        }

        private void BSBTableLastMenu_Click(object sender, RoutedEventArgs e)
        {
            JZXInfo old = BSBTable.CurrentItem as JZXInfo;
            if (old != null)
            {
                int index = ectypalJSYD.JZXInfos.IndexOf(old);
                AddBSB(index);
            }
        }
        private void BSBTableNextMenu_Click(object sender, RoutedEventArgs e)
        {

            JZXInfo old = BSBTable.CurrentItem as JZXInfo;
            if (old != null)
            {
                int index = ectypalJSYD.JZXInfos.IndexOf(old) + 1;
                AddBSB(index);
            }
        }
        private void BSBTableDeleteMenu_Click(object sender, RoutedEventArgs e)
        {

            JZXInfo old = BSBTable.CurrentItem as JZXInfo;
            if (old != null)
            {
                ectypalJSYD.JZXInfos.Remove(old);
            }
        }

        public void AddBSB(int index)
        {
            JZXInfo newBSB = ectypalJSYD.JZXInfos[0].Clone();
            newBSB.QDH = "";
            newBSB.ZDH = "";
            newBSB.TSBC = 0;
            if (index == ectypalJSYD.QZBs.Count)
            {
                ectypalJSYD.JZXInfos.Add(newBSB);

            }
            else
            {
                ectypalJSYD.JZXInfos.Insert(index, newBSB);
            }

        }



        /// <summary>
        /// 上一处添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QZBTableLastMenu_Click(object sender, RoutedEventArgs e)
        {

            if (QZBTable.CurrentItem is QZB old)
            {
                int index = ectypalJSYD.QZBs.IndexOf(old);
                AddQZB(index);
            }
        }
        private void QZBTableNextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (QZBTable.CurrentItem is QZB old)
            {
                int index = ectypalJSYD.QZBs.IndexOf(old) + 1;
                AddQZB(index);
            }
        }
        /// <summary>
        /// 添加一个全新的签章表
        /// </summary>
        /// <param name="index"></param>
        public void AddQZB(int index)
        {
            QZB newQZB = ectypalJSYD.QZBs[0].Clone();
            newQZB.QDH = "";
            newQZB.ZDH = "";
            newQZB.LZDZJR = "";
            if (index == ectypalJSYD.QZBs.Count)
            {
                ectypalJSYD.QZBs.Add(newQZB);

            }
            else
            {
                ectypalJSYD.QZBs.Insert(index, newQZB);
            }

        }

        private void QZBTableDeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            QZB old = QZBTable.CurrentItem as QZB;
            if (old != null)
            {
                ectypalJSYD.QZBs.Remove(old);
            }
        }


        private void Num1TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            double num1 = 0;
            double num2 = 0;
            if (double.TryParse(num1TextEdit.Text, out num1))
            {
                if (double.TryParse(num2TextEdit.Text, out num2))
                {
                    num3TextEdit.Text = (num1 - num2) + "";
                }
            }


        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 保存内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            double areaDX = GetNF_Floor_Area();
            if ((areaDX ==0 && Utils.MessageBoxShow("确定保存吗？")) || Utils.MessageBoxShow("农房表与分层表，面积之差："+areaDX+",确定保存吗？"))
            {
                JSYDCustom.SavejSYDRow(ectypalJSYD, dataSourceViewModel);

                //电话号码设置
                if (MyUtils.Utils.CheckListExists(JSYD.HZs))
                {
                    JSYD.HZs[0].LXDH = LXDHTexEdit.Text;
                }



                if (MyUtils.Utils.CheckListExists(JSYD.QZBs))
                {
                    //删除以前的签章表
                    MDBUtils.DeleteBySql(dataSourceViewModel.QZ_BSMDBPath, "Delete From " + QZBCustom.QZBTableName + "  Where BZDH ='" + JSYD.ZDNUM + "'");
                    //保存现在的
                    MDBUtils.WriteData(dataSourceViewModel.QZ_BSMDBPath, QZBCustom.QZBTableName, ectypalJSYD.QZBs);
                }
                if (MyUtils.Utils.CheckListExists(JSYD.JZXInfos))
                {
                    //删除以前的标示表
                    MDBUtils.DeleteBySql(dataSourceViewModel.QZ_BSMDBPath, "Delete From " + JZXInfoCustom.JZXTableName + " Where BZDH ='" + JSYD.ZDNUM + "'");
                    //保存现在的
                    MDBUtils.WriteData(dataSourceViewModel.QZ_BSMDBPath, JZXInfoCustom.JZXTableName, ectypalJSYD.JZXInfos);
                }
                //对象复制
                ReflectUtils.ClassCopy(ectypalJSYD, JSYD);
            }
        }

        private void DXWindow_Closed(object sender, EventArgs e)
        {

        }

        private void LXDHTexEdit_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (LXDHTexEdit.Text != null && MyUtils.Utils.CheckListExists(ectypalJSYD.HZs))
            {
                ectypalJSYD.HZs[0].LXDH = LXDHTexEdit.Text;
            }
        }

        /// <summary>
        /// 使用 档案袋中 界址线数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlushJZXFromDangAnDai_Click(object sender, RoutedEventArgs e)
        {
            if (MyUtils.Utils.MessageBoxShow("你确定要更新为档案袋的 界址线"))
            {
               
                if (dangAnDaiJZXs != null && dangAnDaiJZXs.Count != 0)
                {
                    while (ectypalJSYD.JZXInfos.Count > 0)
                    {
                        ectypalJSYD.JZXInfos.RemoveAt(0);
                    }
                    foreach (JZXInfo dangAnDaiJZX in dangAnDaiJZXs)
                    {
                        ectypalJSYD.JZXInfos.Add(dangAnDaiJZX);
                    }
                }
                else
                {
                    MessageBox.Show("没有找到 档案袋中的 界址线，无法更新");
                }
            }

        }

        /// <summary>
        /// 重新检查界址线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JZXFromDangAnDai_Click(object sender, RoutedEventArgs e)
        {
            PageAddDangAnDdaiQZBAndBSB();
        }

        private void FlushAreaDX_Click(object sender, RoutedEventArgs e)
        {
            GetNF_Floor_Area();
        }
        /// <summary>
        /// 打开档案袋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDangAnDai_Click(object sender, RoutedEventArgs e)
        {

            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.DanAnDaiDocPath);
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开审批表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSPB_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.SPBDocPath);
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开委托书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenWTS_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.WTSDocPath);
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开声明书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSMS_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.SMSDocPath);
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开权籍调查表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenQJDCB_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.QJDCBDocPath);
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开测绘报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCHBG_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, JSYDCustom.CHBGDocPath);
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开宗地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCT_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, "6_"+ectypalJSYD.ZDNUM+".PDF");
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 宗地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenZDT_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, "7_" + ectypalJSYD.ZDNUM + ".PDF");
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 平面图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenPMT_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string path = ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, "7_" + ectypalJSYD.ZDNUM + "F.PDF");
            FileUtils.OpenFile(path);
        }
        /// <summary>
        /// 打开户文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHuDir_Click(object sender, RoutedEventArgs e)
        {
            ZJDExportDataViewModel exportDataViewModel = ZJDExportDataPage.model;
            string dir = System.IO.Path.GetDirectoryName(ZJDDao.GetZJDSaveFileName(ectypalJSYD, exportDataViewModel.SaveDir, ""));
            FileUtils.OpenDir(dir);
        }
    }
}
