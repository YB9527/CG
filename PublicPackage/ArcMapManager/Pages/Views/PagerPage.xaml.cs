using ArcGisManager;
using ArcMapManager.Pages.Views;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ArcMapManager.Views
{
    /// <summary>
    /// Interaction logic for PagerPage.xaml
    /// </summary>
    public partial class PagerPage: UserControl
    { 
        public PagerPage()
        {
            InitializeComponent();
            lst = new List<object>();

        }
        /// <summary>
        /// 设置完页面，要有些动作
        /// </summary>
        /// <param name="fieldCustoms"></param>
        public void Init(IList<FieldCustom> fieldCustoms =null)
        {
            OnPageGetList += dPager_OnPageGetList;
            cboPageSize.SelectionChanged += cboPageSize_SelectionChanged;
            // MainWindow_Loaded(null, null);
             
            this.UpdateLayout(); //重新布局
            btnFirst_Click(null, null);
            this.tabGrid.MouseDoubleClick += Table_Click;
            this.tabGrid.MouseLeftButtonDown += Table_SingleClick;
           //this.view.AllowEditing = true;
           if (fieldCustoms != null)
            {
                this.FieldCustoms = fieldCustoms;
            }

        }



        private IList<FieldCustom> FieldCustoms;
        public virtual void Table_Click(object sender, MouseButtonEventArgs e)
        {
            
        }
        public virtual void Table_SingleClick(object sender, MouseButtonEventArgs e)
        {
            
        }
        /// <summary>
        /// 创建页面
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        /// <returns></returns>
        public  void SetPagerPage(IList<JToken> list, IList<FieldCustom> fieldCustoms)
        {
          
            foreach(FieldCustom fieldCustom in fieldCustoms)
            {
                
                GridColumn column = new GridColumn { Width = 100, Header = fieldCustom.AliasName, Binding = new Binding("[" + fieldCustom.Name + "]") { Mode = BindingMode.TwoWay } };
                if(fieldCustom.Editable)
                {
                    column.AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                  
                }
                this.tabGrid.Columns.Add(column);
            }
            foreach (JToken t in list)
            {
                lst.Add((Object)t);
            }
            this.Init(fieldCustoms);
        }

        public void SetPagerPage(IList<MapTabDictoryCustom> list, IList<FieldCustom> fieldCustoms)
        {
            foreach (FieldCustom fieldCustom in fieldCustoms)
            {

                GridColumn column = new GridColumn { Width = 100, Header = fieldCustom.AliasName, Binding = new Binding("Dic[" + fieldCustom.Name + "]") { Mode = BindingMode.TwoWay } };
                if (fieldCustom.Editable)
                {
                    column.AllowEditing = DevExpress.Utils.DefaultBoolean.True;

                }
                this.tabGrid.Columns.Add(column);
            }

            foreach (MapTabDictoryCustom t in list)
            {
                
                lst.Add(t);
            }
            this.Init(fieldCustoms);
        }
        public  void SetPagerPage<T>(IList<T> list, IList<FieldCustom> fieldCustoms)
        {
          
            //添加要使用的index
            int count = fieldCustoms.Count;
            int[] indexs = new int[count];
            //添加标题
            for (int a = 0; a < count; a++)
            {
                FieldCustom fieldCustom = fieldCustoms[a];
                indexs[a] = fieldCustom.Index;
                this.tabGrid.Columns.Add(new GridColumn { Width = 100, Header = fieldCustom.AliasName, Binding = new Binding("Value[3]") { Mode = BindingMode.OneWay } });
            }
            foreach (var t in list)
            {
                lst.Add((Object)t);
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
                }catch
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
        private  static List<Object> lst;

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