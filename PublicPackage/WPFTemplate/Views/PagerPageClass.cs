using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Utils = MyUtils.Utils;
namespace WPFTemplate.Views
{
    public class PagerPageClass<T> :PagerPage
    {
        public void AddObject(T t)
        {
            if(!lst.Contains(t))
            {
                lst.Add(t);
                btnFirst_Click(null, null);
            }
        }
        public void AddObjectAll(T t)
        {
            
                lst.Add(t);
                btnFirst_Click(null, null);
            
        }
        public void AddObject(IList<T> ts)
        {
            if(ts != null)
            {
                foreach(T t in ts)
                {
                    if (!lst.Contains(t))
                    {
                        lst.Add(t);
                    }
                }
                btnFirst_Click(null, null);
            }
           
        }

        public void AddObjectAll(IList<T> ts)
        {
            if (ts != null)
            {
                foreach (T t in ts)
                {
                   
                     lst.Add(t);
                }
                btnFirst_Click(null, null);
            }
        }

        public  IList<T> lst { get; set; }
     

        /// <summary>
        /// 所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        public override void  SetPagerPage<T1>(IList<T1> list, IList<FieldCustom> fieldCustoms, bool isAutoObjectID = true, bool isHiddenSelected = false, Visibility ExportBtuGridIsShow = Visibility.Hidden)
        {

            ExportBtuGrid.Visibility = ExportBtuGridIsShow;
            //添加要使用的index
            int count = fieldCustoms.Count;
            int[] indexs = new int[count];
            if(!isHiddenSelected)
            {
                select.Visible = isHiddenSelected;
                //this.tabGrid.Columns.Remove(select);
            }
           

            //添加标题
            for (int a = 0; a < count; a++)
            {
                FieldCustom fieldCustom = fieldCustoms[a];
                GridColumn column = new GridColumn
                {
                    Width = fieldCustom.Width,
                    Header = fieldCustom.AliasName,
                    Binding = new Binding(fieldCustom.Name) { Mode = fieldCustom.Mode },
                 
                };
              
                
                if (fieldCustom.Editable)
                {
                    column.AllowEditing = DevExpress.Utils.DefaultBoolean.True;

                }
                if (fieldCustom is ComBoxFieldCustom)
                {
                    ComBoxFieldCustom comBoxFieldCustom = (ComBoxFieldCustom)fieldCustom;
                    ComboBoxEditSettings comboBoxEditSettings = new ComboBoxEditSettings {  ItemsSource = comBoxFieldCustom.Items, DisplayMember = comBoxFieldCustom.DisplayMember, ValueMember = comBoxFieldCustom.ValueMember };
                    //comboBoxEditSettings.IsTextEditable = true;
                    comboBoxEditSettings.ValidateOnTextInput = false;
                    comboBoxEditSettings.AcceptsReturn = false;                  
                    comboBoxEditSettings.PreviewTextInput += ComboxTextChange;
                    column.EditSettings = comboBoxEditSettings;
                   
                }else if(fieldCustom is CheckBoxFieldCustom)
                {
                    //-------------添加选择列
                    //--dgrid为DataGrid实例对象,Row_Checked是响应事件。
                    //GridColumn dgtc = new GridColumn();
                    //dgtc.Header = "选择123";
                    DataTemplate dt = new DataTemplate();
                    //定义子元素
                    FrameworkElementFactory fef = new FrameworkElementFactory(typeof(CheckBox));
                     var binding = new Binding(fieldCustom.Name);
                    fef.SetValue(CheckBox.IsCheckedProperty, binding);
                    //fef.SetValue(CheckBox.ForegroundProperty, Brushes.Red);
                    fef.SetValue(CheckBox.ContentProperty, fieldCustom.AliasName);
                    fef.SetValue(CheckBox.NameProperty, fieldCustom.Name);
                    //关键部分，为每一项添加事件理解AddHandler的用法
                    fef.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(Row_Checked));
                    fef.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(Row_Checked));
                    dt.VisualTree = fef;//添加子元素 
                    column.HeaderTemplate = dt;
                    column.DataContext = fieldCustom.Name;
                    var vc  = this.tabGrid.Columns[0];
                    column.MouseLeftButtonDown += CheckLeftClick;
                    fef.SetValue(CheckBox.NameProperty, fieldCustom.Name);
                }
                indexs[a] = fieldCustom.Index;
                this.tabGrid.Columns.Add(column);
               
            }
            this.lst = (IList<T>)list;

            if(this.lst == list)
            {

            }
            this.Init();

        }

        private void Row_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            bool isChecked = checkBox.IsChecked.Value;
            MethodInfo m = null;
            int count = this.GetTabGrid().VisibleRowCount;
            if (count > 0)
            {
                var table = this.GetTabGrid();
                m = lst[0].GetType().GetMethod("set_"+ checkBox.Name);
                if (m != null)
                {
                    object[] paramters = new object[] { isChecked };
                    for (int a = 0; a < count; a++)
                    {
                        Object obj = table.GetRow(a);
                        m.Invoke(obj, paramters);
                    }

                }

            }
        }

        public List<T1> GetChildObjects<T1>(DependencyObject obj, string name) where T1 : FrameworkElement
        {
            DependencyObject child = null;
            List<T1> childList = new List<T1>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T1)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T1)child);
                }
                childList.AddRange(GetChildObjects<T1>(child, ""));//指定集合的元素添加到List队尾
            }
            return childList;
        }

        public IList<T> DeleteCurrentItem()
        {
            IList<T> list = new List<T>();
            var items = tabGrid.SelectedItems;
            if (items != null && items.Count > 0 &&  Utils.MessageBoxShow("确定要删除共"+items.Count + " 条数据吗？"))
            {
              
                foreach(T t in items)
                {
                    list.Add(t);
                }
                DeleteEntry(list);

            }
            return list;
        }

        public T1 FindFirstVisualChild<T1>(DependencyObject obj, string childName) where T1 : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    return (T1)child;
                }
                else
                {
                    T1 childOfChild = FindFirstVisualChild<T1>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }


        private void CheckLeftClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        public static T Clone<T>(T templete)
        {
            string rectXaml = XamlWriter.Save(templete);
            StringReader stringReader = new StringReader(rectXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            object clonedChild = XamlReader.Load(xmlReader);
            return (T)clonedChild;
        }

        public void DeleteEntry(T t)
        {
            if(t != null)
            {
                lst.Remove(t);
                Refulsh();
            }
        }
        public void DeleteEntry(IList<T> ts)
        {
            if(ts != null && ts.Count >0)
            {
                foreach(T t in ts)
                {
                   bool bl =  lst.Remove(t);
                }
                Refulsh();
            }

        }

        private void ComboxTextChange(object sender, TextCompositionEventArgs e)
        {
          
        }
        /// <summary>
        /// 得到所有被选中的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetChekedAll()
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
                        flag = (bool)m.Invoke(obj, null);
                        if (flag)
                        {
                            list.Add((T)obj);
                        }
                    }

                }
            }
            return list;
        }

      

        public void ClearChecked()
        {
            IList<T> list = new List<T>();
            MethodInfo m = null;
            object[] paramters = new object[]
            {
                false
            };
            if (lst.Count > 0)
            {
                m = lst[0].GetType().GetMethod("set_IsChecked");
                if (m != null)
                {
                    foreach (Object obj in lst)
                    {
                       m.Invoke(obj, paramters);
                        
                    }

                }
            }
          
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
       
        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            bool flag = (sender as CheckBox).IsChecked.Value;
            CheckedAll(flag);

        }
        public override void dPager_OnPageGetList(int start, int num, ref int count)
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
        /// 创建页面
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fieldCustoms"></param>
        /// <returns></returns>
        public virtual void SetPagerPage(IList<JToken> list, IList<FieldCustom> fieldCustoms)
        {


            foreach (FieldCustom fieldCustom in fieldCustoms)
            {
                GridColumn column = new GridColumn { Width = 100, Header = fieldCustom.AliasName, Binding = new Binding("[" + fieldCustom.Name + "]") { Mode = BindingMode.TwoWay } };



                this.tabGrid.Columns.Add(column);
            }
            foreach (JToken t in list)
            {
                Object t1 = (Object)t;
                lst.Add(((T)t1));
            }
            this.Init(fieldCustoms);
        }


        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            bool flag = (sender as CheckBox).IsChecked.Value;
            CheckedAll(flag);
        }
        public override void CheckedAll(bool flag)
        {

            MethodInfo m = null;
            int count = this.GetTabGrid().VisibleRowCount;
            if (count > 0)
            {
                var table = this.GetTabGrid();
                m = lst[0].GetType().GetMethod("set_IsChecked");
                if (m != null)
                {
                    object[] paramters = new object[] { flag };
                    for (int a =0; a < count; a++)
                    {
                        Object obj = table.GetRow(a);
                        m.Invoke(obj, paramters);
                    }
 
                }

            }
        }
        public override void DeleteAll()
        {
            if(lst != null)
            {
                while(lst.Count >0)
                {
                    lst.RemoveAt(0);
                }
            }
            btnFirst_Click(null, null);
        }

    }
}
