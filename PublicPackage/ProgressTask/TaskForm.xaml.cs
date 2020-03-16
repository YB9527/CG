using FirstFloor.ModernUI.Windows.Controls;
using HeibernateManager.Model;
using MyUtils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ProgressTask
{
    /// <summary>
    /// Interaction logic for TaskForm.xaml
    /// </summary>
    public partial class TaskForm : ModernWindow
    {
        public static TaskForm taskForm = null;
        public IList<MyTask> MyTasks = new List<MyTask>();
        private TaskForm()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

        }

            public static TaskForm GetInstance()
        {
            if (taskForm == null)
            {
                taskForm = new TaskForm();
            }
            return taskForm;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="myTask"></param>
        public void AddTask(MyTask myTask)
        {
            
           
            this.StackTasks.Children.Insert(0,myTask.StackPanel);
            this.MyTasks.Add(myTask);
        }
     
       
            private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

        }
        /// <summary>
        /// 彻底关闭窗口
        /// </summary>
        public static void Close_Click()
        {
            GetInstance().Close();
        }

        public void StartAction(MyAction myAction)
        {

            CommHelper.FastTask(myAction.Action, GetSuccessAction(), GetErrorAction());

        }
        /// <summary>
        /// 成功的方法
        /// </summary>
        /// <returns></returns>
        public Action GetSuccessAction()
        {
            return new Action(() =>
            {

                MessageBox.Show("成功了");
            });
        }
        /// <summary>
        /// 失败的方法
        /// </summary>
        /// <returns></returns>
        public Action GetErrorAction()
        {
            return new Action(() =>
            {
                // MessageBox.Show("失败了");
            });
        }

        private void ModernButton_Click(object sender, RoutedEventArgs e)
        {
            MyAction myAction = (sender as Control).DataContext as MyAction;
            // StartAction(myAction);
            //Thread thread = new Thread(new ThreadStart(fun));
            //thread.Start();


            //fun();
        }
        public Task TaskStrart(ProgressBar parBar)
        {
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程          
            Task task = new Task(new Action(() =>
            {
                for (int i = 0; i <= 10; i++)
                {
                    x.BeginInvoke(new Action(() =>
                    {
                        parBar.Value = i;
                    }), DispatcherPriority.Normal);
                    i++;
                    Thread.Sleep(500);
                }
            }));
            task.Start();
            return task;
        }

        private void AllStart_Click(object sender, RoutedEventArgs e)
        {
            //得到所有能够运行的任务
           
            foreach(MyTask myTask in this.MyTasks)
            {
                CheckBox checkBox = myTask.StateCheckBox;
                if (checkBox.IsChecked != null && checkBox.IsChecked.Value)
                {
                    //TaskStrart(myTask.MyTaskControl.ProgressBar);
                    
                    myTask.Start();
                }
                
            }
           // TaskStrart(ta[0].Progressbar);
        }

        

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 全选全不选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAll_Click(object sender, RoutedEventArgs e)
        {
            var list = StackTasks.Children;
            if (list.Count == 0)
            {
                return;
            }
            SetAllCheckBox(list, (sender as CheckBox).IsChecked.Value);

        }
        /// <summary>
        /// 设置全部任务的checkbox
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        private void SetAllCheckBox(UIElementCollection list, bool value)
        {
            foreach (StackPanel stackPanel in list)
            {
                //第一个checkbox 如果可以编辑，就全部选择
                UIElement ui = stackPanel.Children[0];
                CheckBox checkBox = ui as CheckBox;
                if (checkBox.IsEnabled)
                {
                    checkBox.IsChecked = value;
                }

            }
        }
        public void Start(MyAction action)
        {

            this.Title = action.TaskName;
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task task = new Task(new Action(async () =>
            {
                await x.BeginInvoke(action.Action, DispatcherPriority.Normal);
                Thread.Sleep(1);
                await x.BeginInvoke(new Action(()=>
                {
                    if(Utils.IsStrNull(action.SuccessShowMessage))
                    {
                        MessageBox.Show(action.SuccessShowMessage);
                    }
                    this.Close();
                })
                    , DispatcherPriority.Normal);
            }));
            task.Start();
            this.ShowDialog();

        }
    }
}
