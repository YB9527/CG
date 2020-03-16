using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using WPFTemplate;

namespace ProgressTask
{
    /// <summary>
    /// 任务对象控件封装
    /// </summary>
    public class MyTask
    {
       private int Max { get; set; }
        public MyTask(string taskName,string Describe, int max)
        {
            this.Max = max;

            this.StateCheckBox = new CheckBox();
            this.StateCheckBox.IsChecked = true;
            this.ProgressBar = new ProgressBar();
            this.ProgressBar.Width = 200;
            this.ProgressBar.Minimum = 0;
            this.ProgressBar.Maximum = max;
            this.ProgressBar.Value = 0;

            this.TaskNameTextBlock = new TextBlock();
            this.TaskNameTextBlock.Text = taskName;
            this.DescribeTextBlock = new TextBlock();
            this.DescribeTextBlock.Text = Describe;
            this.ActionButton = new ModernButton();
            this.ActionButton.Content = "开始";


            StackPanel model = TaskFormModeTemplate.GetMoel();
          
            StackPanel newPanel =Clone(model) as StackPanel;
            newPanel.Children.RemoveAt(0);
            newPanel.Children.Insert(0, this.StateCheckBox);
            newPanel.Children.RemoveAt(1);
            newPanel.Children.Insert(1, this.TaskNameTextBlock);
            newPanel.Children.RemoveAt(2);
            newPanel.Children.Insert(2, this.DescribeTextBlock);
            newPanel.Children.RemoveAt(3);
            newPanel.Children.Insert(3, this.ProgressBar);
            newPanel.Children.RemoveAt(4);
            newPanel.Children.Insert(4, this.ActionButton);
            this.ActionButton.Click += Action_Click;

            newPanel.MouseLeftButtonDown += StackPanelMouseLeftButtonDown;
            this.StackPanel = newPanel;
           

        }
        public static T Clone<T>(T templete)
        {
            string rectXaml = XamlWriter.Save(templete);
            StringReader stringReader = new StringReader(rectXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            object clonedChild = XamlReader.Load(xmlReader);
            return (T)clonedChild;
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {

            Start();
        }


        private static StackPanel StackPanelCurrent = null;
        private void StackPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(StackPanelCurrent != null)
            {
                StackPanelCurrent.Background = null;
            }
            StackPanelCurrent = sender as StackPanel;
            StackPanelCurrent.Background = Brushes.AliceBlue;
        }

        public CheckBox StateCheckBox { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public TextBlock TaskNameTextBlock { get; set; }
        public TextBlock DescribeTextBlock { get; set; }
        public ModernButton ActionButton { get; set; }
        public StackPanel StackPanel { get; set; }

        public Task Task { get; set; }
        public void SetProgressValue(int value,string runningMessage="运行中...",string success="完成")
        {
            this.ActionButton.Content = runningMessage;           
            this.ProgressBar.Value = value ;
            //完成改状态
            if (this.Max == value)
            {
                this.StateCheckBox.IsEnabled = false;
                this.StateCheckBox.IsChecked = null;
                this.ActionButton.Content = success;

            }
        }

        public void Start()
        {
            switch (this.Task.Status)
            {
                case TaskStatus.Created:
                    this.Task.Start();
                    break;
            }

        }
       

        public MyTask Test()
        {
            int count = 5000;

            MyTask myTask = new MyTask("生成标示表", "1230", count);
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task task = new Task(new Action(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    x.BeginInvoke(new Action(() =>
                    {
                        myTask.SetProgressValue(i + 1, i + "/" + count);

                    }), DispatcherPriority.Normal);
                    Thread.Sleep(1);
                }
            }));
            myTask.Task = task;
            return myTask;
        }
    }
}
