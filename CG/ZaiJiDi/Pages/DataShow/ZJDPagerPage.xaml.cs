using DevExpress.Xpf.Grid;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFTemplate.Views;
using ZaiJiDi.Pages.DataShow;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.Pages.ZJDPage.ExportData;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Pages.ZJDPage.DataShow
{
    /// <summary>
    /// Interaction logic for PagerPage.xaml
    /// </summary>
    public partial class ZJDPagerPage : UserControl
    {
        public ZJDPagerPage()
        {
            InitializeComponent();
            lst = new List<object>();

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
        ZJDDataSourceViewModel dataSourceViewModel;
        public void SetDataSourceViewModel(ZJDDataSourceViewModel dataSourceViewModel)
        {
            this.dataSourceViewModel = dataSourceViewModel;
        }

        public GridControl GetTabGrid()
        {
            return this.tabGrid;
        }

        /// <summary>
        /// 得到所有被选中的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetChekedAll<T>()
        {
            IList<T> list = new List<T>();
            MethodInfo m = null;
            if (lst.Count > 0)
            {
                m = lst[0].GetType().GetMethod("get_IsChecked");
                if (m != null)
                {
                    bool flag;
                    foreach (Object obj in lst)
                    {
                        flag = (bool) m.Invoke(obj, null)  ;
                        if(flag)
                        {
                            list.Add((T)obj);
                        }
                    }
                   
                }
            }
            return list;
        }

        /// <summary>
        /// 隐藏脚本、并设置界面1万的数量
        /// </summary>
        public void HideFloot()
        {
            PageCount = 10000;
            dPager.Height = 1;
            view.ShowFixedTotalSummary = false;
            dPager.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 添加对象并刷新页面
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject<T>(T t)
        {
            if (!lst.Contains(t))
            {
                lst.Insert(0, t);
                DgvListBind(pageIndex, num, ref count);
            }

        }
        /// <summary>
        /// 批量添加对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void AddObject<T>(IList<T> list)
        {

            foreach (T t in list)
            {
                if (!lst.Contains(t))
                {
                    lst.Insert(0, t);
                }
            }
            DgvListBind(pageIndex, num, ref count);
        }
        /// <summary>
        /// 移除全部的对象
        /// </summary>
        public void DeleteAll()
        {
            if (lst.Count > 0)
            {
                lst.RemoveRange(0, lst.Count);
            }
            DgvListBind(pageIndex, num, ref count);
        }
        /// <summary>
        /// 返回所有已被添加的对象
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAllList<T>()
        {
            return lst.OfType<T>().ToList();
        }
        private IList<FieldCustom> FieldCustoms;
        public virtual void Table_Click(object sender, MouseButtonEventArgs e)
        {
           

            GridControl gridControl = sender as GridControl;
            JSYDPagerPageViewModel model = gridControl.CurrentItem as JSYDPagerPageViewModel;
            if(model != null)
            {
                JSYD jsyd = model.JSYD;
                JSYDDataShowWindow window = new JSYDDataShowWindow(jsyd,dataSourceViewModel);
                window.ShowDialog();
              
                model.QLRMC = jsyd.QLRMC;
                model.ZDNUM = jsyd.ZDNUM;
                model.BZ = jsyd.BZ;
            }
           
        }

        /// <summary>
        /// 创建页面
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        /// <returns></returns>
        public void SetPagerPage(IList<JToken> list, IList<FieldCustom> fieldCustoms)
        {


            foreach (FieldCustom fieldCustom in fieldCustoms)
            {
                GridColumn column = new GridColumn { Width = 100, Header = fieldCustom.AliasName, Binding = new Binding("[" + fieldCustom.Name + "]") { Mode = BindingMode.TwoWay } };



                this.tabGrid.Columns.Add(column);
            }
            foreach (JToken t in list)
            {
                lst.Add((Object)t);
            }
            this.Init(fieldCustoms);
        }

        /// <summary>
        /// 所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        public void SetPagerPage<T>(IList<T> list, IList<FieldCustom> fieldCustoms, bool isAutoObjectID = true, bool isHiddenSelected = false)
        {

            //添加要使用的index
            int count = fieldCustoms.Count;
            int[] indexs = new int[count];
            select.Visible = isHiddenSelected;
           
            //添加标题
            for (int a = 0; a < count; a++)
            {
                FieldCustom fieldCustom = fieldCustoms[a];
                GridColumn column = new GridColumn
                {
                    Width = fieldCustom.Width,
                    Header = fieldCustom.AliasName,
                    Binding = new Binding(fieldCustom.Name) { Mode = BindingMode.TwoWay }
                };
                //EditSettings="{dxe:ComboBoxSettings DisplayMember=UserRole, ValueMember=UserRole,ItemsSource={x:Static local:db_PMSData.Data}}"/>

                if (fieldCustom.Editable)
                {
                    column.AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                }

                indexs[a] = fieldCustom.Index;
                this.tabGrid.Columns.Add(column);
            }
            if (list != null)
            {
                lst = list.OfType<object>().ToList();

            }

            this.Init();

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
        void dPager_OnPageGetList(int start, int num, ref int count)
        {
            List<Object> lstGrid = new List<Object>();
            for (int i = start; i < (start + num); i++)
            {
                try
                {
                    lstGrid.Add(lst[i]);
                }
                catch (Exception)
                {
                    break;
                }

            }
            tabGrid.ItemsSource = lstGrid;
            count = lst.Count;
        }



        /// <summary>
        /// 每次创建页面必须 new
        /// </summary>
        private static List<Object> lst;

        public static IList<TestGrid> GetList(int v)
        {
            IList<TestGrid> list = new List<TestGrid>();
            TestGrid t = new TestGrid();
            for (int i = 1; i < v; i++)
            {
                t = new TestGrid();
                t.OrderID = i;
                t.chose = false;
                t.Country = "美国";
                t.City = "纽约";
                t.UnitPrice = 16.2;
                t.Quantity = i;
                t.Birth = DateTime.Now.AddDays(-19);
                list.Add(t);
            }
            return list;
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
        public void CheckedAll(bool flag)
        {
           
            MethodInfo m = null;
            if (lst.Count > 0)
            {
                m = lst[0].GetType().GetMethod("set_IsChecked");
                if (m != null)
                {
                  
                    object[] paramters = new object[] { flag };
                    foreach (Object obj in lst)
                    {
                        m.Invoke(obj, paramters);
                    }
                }

            }
        }

        private void TabGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }


    public class TestGrid
    {
        public bool chose { set; get; }
        public int OrderID { set; get; }
        public string Country { set; get; }
        public string City { set; get; }
        public Double UnitPrice { set; get; }
        public DateTime Birth { set; get; }
        public int Quantity { set; get; }
    }
}