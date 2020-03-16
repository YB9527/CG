using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WPFTemplate.Views
{
    /// <summary>
    /// Interaction logic for PagerPage.xaml
    /// </summary>
    public partial class PagerPage : System.Windows.Controls.UserControl
    {
        public PagerPage()
        {
            InitializeComponent();
            this.view.IndicatorWidth = 30;//设置显示行号的列宽
            //this.view.TotalSummary.Add(new GridSummaryItem()
            //{
            //    SummaryType = DevExpress.Data.SummaryItemType.Count,
            //    DisplayFormat = "总记录数：{0:0}"
                
            //}
            //);

        }
        /// <summary>
        /// 设置完页面，要有些动作
        /// </summary>
        /// <param name="fieldCustoms"></param>
        public void Init(IList<FieldCustom> fieldCustoms = null)
        {
            OnPageGetList += dPager_OnPageGetList;
            cboPageSize.SelectionChanged += cboPageSize_SelectionChanged;
            // MainWindow_Loaded(null, null);
            // ThemeManager.ApplicationThemeName = "MetropolisLight";     
            this.UpdateLayout(); //重新布局
            btnFirst_Click(null, null);
            this.tabGrid.MouseDoubleClick += Table_Click;
            //this.view.AllowEditing = true;
            if (fieldCustoms != null)
            {
                this.FieldCustoms = fieldCustoms;
            }
          
           // this.tabGrid.EnableSmartColumnsGeneration = true;
           
        }
        public GridControl GetTabGrid()
        {
            return this.tabGrid;
        }

       

        /// <summary>
        /// 隐藏脚本、并设置界面1万的数量
        /// </summary>
        public void HideFloot()
        {
            //cboPageSize.SelectedIndex = cboPageSize.Items.Count - 1;
            dPager.Height = 1;
            view.ShowFixedTotalSummary = false;
            dPager.Visibility = Visibility.Hidden;
            dPager.Width = 0;
        }



        private IList<FieldCustom> FieldCustoms;

        public virtual void DeleteAll()
        {
           
        }

        public virtual void Table_Click(object sender, MouseButtonEventArgs e)
        {
            GridControl gridControl = sender as GridControl;
            GridColumn column = gridControl.CurrentColumn as GridColumn;
            if (column.VisibleIndex == -1)
            {
                //图形缩放
            }
        }

        /// <summary>
        /// 创建页面
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        /// <returns></returns>
        public virtual void SetPagerPage(IList<JToken> list, IList<FieldCustom> fieldCustoms)
        {


           
        }

       

        /// <summary>
        /// 所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        public virtual  void SetPagerPage<T>(IList<T> list, IList<FieldCustom> fieldCustoms, bool isAutoObjectID = true, bool isHiddenSelected = false, Visibility ExportBtuGridIsShow = Visibility.Hidden)
        {

           
           
        }

        public delegate void BackToCallback(int start, int num, ref int count);
        public event BackToCallback OnPageGetList;

        public int pageIndex = 1;
        public int num = 100;
        public int PageCount = 1;
        public int count;

     

        /// <summary>
        /// 设置第页最大数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cboPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                ComboBoxItem cbb = cboPageSize.SelectedItem as ComboBoxItem;
                num = Convert.ToInt32(cbb.Content);
                int iEnd = count % num == 0 ? count / num : count / num + 1;
                pageIndex = Convert.ToInt32(txtJumpPage.Text);
                if (pageIndex > iEnd)
                {
                    pageIndex = iEnd;
                }

                DgvListBind(pageIndex, num, ref count);
                ImgControl(pageIndex, count);

                //
                // this.PageCount = 
            }
            catch (Exception ex)
            {
            }
        }

        private void DgvListBind(int pageIndex, int num, ref int count)
        {
            if (OnPageGetList != null)
            {
                int start = (pageIndex - 1) * num;
                OnPageGetList(start, num, ref count);
                if (count == 0)
                {
                    lblPageCount.Content = "1";
                    lblTotal.Content = count.ToString();
                }
                else
                {
                    lblPageCount.Content = (count % num == 0 ? count / num : count / num + 1).ToString();
                    lblTotal.Content = count.ToString();
                }
                try
                {
                    if (int.Parse(txtJumpPage.Text as string) > int.Parse(lblPageCount.Content as string))
                    {
                        txtJumpPage.Text = lblPageCount.Content as string;
                    }
                }
                catch
                {
                    txtJumpPage.Text = "1";
                }

            }
        }

        private void ImgControl(int pageIndex, int pageCount)
        {
            pageCount = pageCount / num == 0 ? pageCount / num : (pageCount / num) + 1;
            if (pageIndex <= 1 && pageCount <= 1)
            {
                btnFirst.IsEnabled = false;
                btnLeft.IsEnabled = false;
                btnRight.IsEnabled = false;
                btnLast.IsEnabled = false;
            }
            else if (pageIndex == 1 && pageCount > 1)
            {
                btnFirst.IsEnabled = false;
                btnLeft.IsEnabled = false;
                btnRight.IsEnabled = true;
                btnLast.IsEnabled = true;
            }
            else if (pageIndex > 1 && pageIndex < pageCount)
            {
                btnFirst.IsEnabled = true;
                btnLeft.IsEnabled = true;
                btnRight.IsEnabled = true;
                btnLast.IsEnabled = true;
            }
            else if (pageIndex > 1 && pageIndex == pageCount)
            {
                btnFirst.IsEnabled = true;
                btnLeft.IsEnabled = true;
                btnRight.IsEnabled = false;
                btnLast.IsEnabled = false;
            }
        }
        public void Refulsh()
        {
            pageIndex = 1;
            txtJumpPage.Text = "1";
            num = Convert.ToInt32(cboPageSize.Text);
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }

        /// <summary>
        /// 刷新到第一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            pageIndex = 1;
            txtJumpPage.Text = "1";
            num = Convert.ToInt32(cboPageSize.Text);
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }



        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            pageIndex -= 1;
            txtJumpPage.Text = pageIndex.ToString();
            num = Convert.ToInt32(cboPageSize.Text);
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            pageIndex += 1;
            txtJumpPage.Text = pageIndex.ToString();
            num = Convert.ToInt32(cboPageSize.Text);
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }



        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            num = Convert.ToInt32(cboPageSize.Text);
            pageIndex = count % num == 0 ? count / num : count / num + 1;
            DgvListBind(pageIndex, num, ref count);
            //也许有新数据所以要更新
            pageIndex = count % num == 0 ? count / num : count / num + 1;
            txtJumpPage.Text = pageIndex.ToString();
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }

        private void btnJump_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            try
            {
                index = Convert.ToInt32(txtJumpPage.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("请输入数字!");
                return;
            }
            num = Convert.ToInt32(cboPageSize.Text);
            int end = count % num == 0 ? count / num : count / num + 1;
            if (index > end)
            {
                index = end;
                txtJumpPage.Text = end.ToString();
            }
            pageIndex = index;
            DgvListBind(pageIndex, num, ref count);
            txtJumpPage.Text = pageIndex.ToString();
            DgvListBind(pageIndex, num, ref count);
            ImgControl(pageIndex, count);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            DgvListBind(pageIndex, num, ref count);
        }
       public virtual void dPager_OnPageGetList(int start, int num, ref int count)
        {
           
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (sender as CheckBox).IsChecked.Value;
            CheckedAll(flag);

        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            bool flag = (sender as CheckBox).IsChecked.Value;
            CheckedAll(flag);
        }
        public virtual void CheckedAll(bool flag)
        {
          
          
        }

        private void ExportTable_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.RestoreDirectory = true;
            System.Windows.Forms.DialogResult dr = saveFileDialog1.ShowDialog();
            
            if (dr == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName.Length > 0)
            {
                string saveName = saveFileDialog1.FileName;
                string key = (FormatComboBox.SelectedItem as ComboBoxEditItem).Content as string;

                switch (key)
                {
                    case "XLS":
                        saveName = saveName + ".xls";
                        view.ExportToXls(saveName);
                     
                        break;
                    case "XLSX":
                        saveName = saveName + ".xlsx";
                        view.ExportToXlsx(saveName);
                        break;
                    case "PDF":
                        saveName = saveName + ".pdf";
                        view.ExportToPdf(saveName);
                        break;
                    case "Html":
                        saveName = saveName + ".html";
                        view.ExportToHtml(saveName);
                        break;
                    case "Image":
                        saveName = saveName + ".jpg";
                        view.ExportToImage(saveName);
                        break;
                }

            }
         
          
        }
    }

}