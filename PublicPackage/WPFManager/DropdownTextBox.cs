using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace WPFManager
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:dropdownTextBoxApp"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:dropdownTextBoxApp;assembly=dropdownTextBoxApp"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:DropdownTextBox/>
    ///
    /// </summary>
    public class DropdownTextBox : Control
    {
        static DropdownTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropdownTextBox), new FrameworkPropertyMetadata(typeof(DropdownTextBox)));
        }

        #region 属性

        /*----------------------------------------------------------------*/
        public static readonly DependencyProperty IsDropdownOpenedProperty = DependencyProperty.Register("IsDropdownOpened", typeof(bool), typeof(DropdownTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(IsDropdownOpenedPropertyChanged)));

        /// <summary>
        /// 下拉框是否已打开
        /// </summary>
        public bool IsDropdownOpened
        {
            get { return (bool)GetValue(IsDropdownOpenedProperty); }
            set { SetValue(IsDropdownOpenedProperty, value); }
        }

        private static void IsDropdownOpenedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool b = (bool)e.NewValue;
            DropdownTextBox db = d as DropdownTextBox;
            db.OnDropdownStateChanged(b);
        }

        protected virtual void OnDropdownStateChanged(bool isopened)
        {
            // 引发路由事件
            RoutedEventArgs args = new RoutedEventArgs();
            if (isopened)
            {
                args.RoutedEvent = DropdownOpenedEvent;
            }
            else
            {
                args.RoutedEvent = DropdownClosedEvent;
            }
            args.Source = this;
            RaiseEvent(args);
        }

        /*-------------------------------------------------------*/
        public static readonly DependencyProperty DropItemsProperty = DependencyProperty.Register("DropItems", typeof(IEnumerable), typeof(DropdownTextBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(DropItemsPropertyChanged)));

        /// <summary>
        /// 获取或设置下拉框中显示的项
        /// </summary>
        public IEnumerable DropItems
        {
            get { return (IEnumerable)GetValue(DropItemsProperty); }
            set { SetValue(DropItemsProperty, value); }
        }

        private static void DropItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropdownTextBox dt = d as DropdownTextBox;
            if (dt != null)
            {
                IEnumerable oldval, newval;
                oldval = (IEnumerable)e.OldValue;
                newval = (IEnumerable)e.NewValue;
                dt.OnDropItemsChanged(oldval, newval);
            }
        }

        protected virtual void OnDropItemsChanged(IEnumerable oldItems, IEnumerable newItems)
        {
            if (oldItems != newItems)
            {
                this.UpdateUIItems(newItems);
            }
        }

        /*------------------------------------------------------*/
        public static readonly DependencyProperty MinDropdownHeightProperty = DependencyProperty.Register("MinDropdownHeighte", typeof(double), typeof(DropdownTextBox));

        /// <summary>
        /// 下拉列表框的最小高度
        /// </summary>
        public double MinDropdownHeighte
        {
            get { return (double)GetValue(MinDropdownHeightProperty); }
            set { SetValue(MinDropdownHeightProperty, value); }
        }

        /*--------------------------------------------------------------*/
        public static readonly DependencyProperty MaxDropdownHeightProperty = DependencyProperty.Register("MaxDropdownHeight", typeof(double), typeof(DropdownTextBox));

        /// <summary>
        /// 下拉列表框的最大高度
        /// </summary>
        public double MaxDropdownHeight
        {
            get { return (double)GetValue(MaxDropdownHeightProperty); }
            set { SetValue(MaxDropdownHeightProperty, value); }
        }

        /*----------------------------------------------------------*/
        public static readonly DependencyProperty TextProperty = TextBox.TextProperty.AddOwner(typeof(DropdownTextBox),new FrameworkPropertyMetadata(new PropertyChangedCallback(TextPropertyChanged)));

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropdownTextBox db = d as DropdownTextBox;
            db.OnTextChanged();
        }

        /// <summary>
        /// 获取或设置控件中显示的文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion

        #region 字段
        /// <summary>
        /// 文本输入框
        /// </summary>
        private TextBox m_InputBox = null;
        /// <summary>
        /// 模板中文本框控件的名字
        /// </summary>
        const string PART_InputBox = "PART_InputBox";
        /// <summary>
        /// 显示项的面板
        /// </summary>
        const string PART_ItemPanel = "PART_ItemPanel";
        /// <summary>
        /// 显示项的面板
        /// </summary>
        private Panel m_itemsPanel = null;
        #endregion

        #region 内部方法
        /// <summary>
        /// 创建项容器
        /// </summary>
        /// <returns></returns>
        protected virtual UIElement CreateItemContainer()
        {
            DropdownItem ue = new DropdownItem();
            AddItemActivedHandler(ue, new RoutedEventHandler(Item_Actived));
            return ue;
        }

        private void Item_Actived(object sender, RoutedEventArgs e)
        {
            if (m_InputBox == null) return;
            DropdownItem item = e.Source as DropdownItem;
            if (item != null)
            {
                string str = item.NormalText;
                m_InputBox.Tag = true;
                m_InputBox.Text = str;
                m_InputBox.Tag = false;
                IsDropdownOpened = false;
            }
        }

        protected virtual void ClearAllItems()
        {
            if (m_itemsPanel != null)
            {
                m_itemsPanel.Children.Clear();
            }
        }

        /// <summary>
        /// 更新项列表UI
        /// </summary>
        private void UpdateUIItems(IEnumerable items)
        {
            if (m_itemsPanel != null)
            {
                ClearAllItems();
                if (items != null)
                {
                    foreach (object item in items)
                    {
                        string strContent = string.Empty;
                        if (item is string)
                            strContent = item as string;
                        else
                            strContent = item.ToString();
                        UIElement ui = CreateItemContainer();
                        ui.SetValue(DropdownItem.NormalTextProperty, strContent);
                        m_itemsPanel.Children.Add(ui);
                    }
                    m_itemsPanel.UpdateLayout();
                    //m_itemsPanel.InvalidateMeasure();
                    //m_itemsPanel.InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// 高亮显示字符
        /// </summary>
        /// <param name="txt"></param>
        private void FilterText(string txt)
        {
            if (m_itemsPanel != null)
            {
                foreach (UIElement ue in m_itemsPanel.Children)
                {
                    ue.SetValue(DropdownItem.FilterTextProperty, txt);
                }
            }
        }

        /*--------------------------------------------------------------------*/
        public override void OnApplyTemplate()
        {
            if (m_InputBox != null)
            {
                m_InputBox.TextChanged -= m_InputBox_TextChanged;
                m_InputBox = null;
            }
            m_InputBox = GetTemplateChild(PART_InputBox) as TextBox;
            if (m_InputBox != null)
                m_InputBox.Tag = false;
            m_InputBox.TextChanged += m_InputBox_TextChanged;

            if (m_itemsPanel != null)
            {
                m_itemsPanel = null;
            }
            m_itemsPanel = GetTemplateChild(PART_ItemPanel) as VirtualizingStackPanel;
            if (m_itemsPanel != null)
            {
                UpdateUIItems(DropItems);
            }

            base.OnApplyTemplate();
        }

        void m_InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool b = (bool)m_InputBox.Tag;
            if (b == false)
            {
                if (IsDropdownOpened == false)
                {
                    IsDropdownOpened = true;
                }
            }

            FilterText(m_InputBox.Text);

            // 引发本类的TextChanged事件
            //OnTextChanged(e);
        }

        protected virtual void OnTextChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(TextChangedEvent, this);
            RaiseEvent(args);
        }

        #endregion

        #region 事件
        public static readonly RoutedEvent ItemActivedEvent = EventManager.RegisterRoutedEvent("ItemActived", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropdownTextBox));

        public static void AddItemActivedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement u = d as UIElement;
            if (u != null)
            {
                u.AddHandler(ItemActivedEvent, handler);
            }
        }

        public static void RemoveItemActivedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement u = d as UIElement;
            if (u != null)
            {
                u.RemoveHandler(ItemActivedEvent, handler);
            }
        }

        /*-------------------------------------------------------------------*/

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropdownTextBox));

        /// <summary>
        /// 当文本框中的内容改变时发生
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {
            add
            {
                AddHandler(TextChangedEvent, value);
            }
            remove
            {
                RemoveHandler(TextChangedEvent, value);
            }
        }

        /*---------------------------------------------------*/
        public static readonly RoutedEvent DropdownOpenedEvent = EventManager.RegisterRoutedEvent("DropdownOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropdownTextBox));

        /// <summary>
        /// 当下拉框打下后发生
        /// </summary>
        public event RoutedEventHandler DropdownOpened
        {
            add { AddHandler(DropdownOpenedEvent, value); }
            remove { RemoveHandler(DropdownOpenedEvent, value); }
        }

        /*---------------------------------------------------------*/
        public static readonly RoutedEvent DropdownClosedEvent = EventManager.RegisterRoutedEvent("DropdownClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropdownTextBox));

        /// <summary>
        /// 当下拉框关闭后发生
        /// </summary>
        public event RoutedEventHandler DropdownClosed
        {
            add { AddHandler(DropdownClosedEvent, value); }
            remove { RemoveHandler(DropdownClosedEvent, value); }
        }

        #endregion
    }
}
