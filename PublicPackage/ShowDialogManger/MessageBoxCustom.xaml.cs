using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;


namespace ShowDialogManger
{
    /// <summary>
    /// Interaction logic for MessageBoxCustom.xaml
    /// </summary>
    public partial class MessageBoxCustom : DXWindow
    {
        private string msg;
        public MessageBoxCustom()
        {
            InitializeComponent();
        }
        public static Window Window { get; set; }
        public static void MessageBosShow(string msg, Window parent)
        {
            MessageBoxCustom messageBoxCustom = new MessageBoxCustom();
            messageBoxCustom.DataContext = msg;
            messageBoxCustom.ShowDialog();
            WindowShow(messageBoxCustom, parent);
        }

        public  bool IsResult{get;set;}
        public static MessageBoxCustom messageBoxCustom;
    /// <summary>
    /// 使用的 已经注入的 window父窗口
    /// </summary>
    /// <param name="msg"></param>
        public static bool MessageBoxShow(string msg, bool textEditable=false, Visibility showFalse = Visibility.Hidden)
        {
             messageBoxCustom = new MessageBoxCustom();
            messageBoxCustom.MsgTextBox.Text = msg;
            messageBoxCustom.NoButton.Visibility = showFalse;

           
            messageBoxCustom.MsgTextBox.IsEnabled = textEditable;

            //messageBoxCustom.ShowDialog();
            WindowShow(messageBoxCustom, Window);
            return messageBoxCustom.IsResult;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            IsResult = false;
            messageBoxCustom.Close();
        }

        private void SimpleButton_Click(object sender, RoutedEventArgs e)
        {
            IsResult = true;
            messageBoxCustom.Close();
        }


        public static void WindowShow(Window window, Window Parent = null)
        {
            if(Parent == null)
            {
                Parent = Window;
            }

            window.Closing += Window_Closing;
            //蒙板
            Grid layer = new Grid() { Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)) };
            //父级窗体原来的内容
            UIElement original = Parent.Content as UIElement;
            Parent.Content = null;
            //容器Grid
            Grid container = new Grid();
            container.Children.Add(original);//放入原来的内容
            container.Children.Add(layer);//在上面放一层蒙板
                                          //将装有原来内容和蒙板的容器赋给父级窗体
            Parent.Content = container;
            window.Owner = Parent;
            window.ShowDialog();
        }

        private static void Window_Closing(object sender, CancelEventArgs e)
        {
            Window window = sender as Window;
            //容器Grid
            Grid grid = window.Owner.Content as Grid;
            //父级窗体原来的内容
            UIElement original = VisualTreeHelper.GetChild(grid, 0) as UIElement;
            //将父级窗体原来的内容在容器Grid中移除
            grid.Children.Remove(original);
            //赋给父级窗体
            window.Owner.Content = original;
        }
    }
}
