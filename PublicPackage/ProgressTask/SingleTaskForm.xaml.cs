using HeibernateManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgressTask
{
    /// <summary>
    /// SingleTaskForm.xaml 的交互逻辑
    /// </summary>
    public partial class SingleTaskForm : Window
    {
       

        public SingleTaskForm()
        {
           // this.ShowInTaskbar = false;
            InitializeComponent();
        }

        public SingleTaskForm(IList<Action> actions) :this()
        {
            Start(actions);
        }
        public SingleTaskForm(IList<MyAction> myActions, string taskName= "任务进行中",int sleepTime=1) : this()
        {
            Start(myActions, taskName, sleepTime);
        }
        public void Start(IList<MyAction> actions, string taskName, int sleepTime)
        {
            string waitStr = "  进行中...";
            string okStr   = "  完成";
            this.Title = taskName;
            int count = actions.Count;
            this.proBar.Maximum = count;
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task task = new Task(new Action(async () =>
            {
                for (int i = 0; i < count; i++)
                {

                    await x.BeginInvoke(new Action(() =>
                    {
                        this.proBar.Value++;
                        this.txt.Text = (i + 1) + " \\ " + count;
                        if (i == 0)
                        {
                            log.Text = "第 " + (i + 1) + " 个任务：" + actions[i].TaskName + waitStr;
                        } else
                        {
                            log.Text ="第 " + (i + 1) + " 个任务：" + actions[i].TaskName + waitStr+ "\n" + log.Text.Replace(waitStr, okStr) ;
                        }

                    }), DispatcherPriority.Normal);
                    await x.BeginInvoke(actions[i].Action, DispatcherPriority.Normal);
                    Thread.Sleep(sleepTime);

                }
                
                await x.BeginInvoke(new Action(() =>
                {
                    log.Text = log.Text.Replace(waitStr, okStr);
                }), DispatcherPriority.Normal);
               
            }));
            task.Start();
            this.ShowDialog();

        }


        public SingleTaskForm(Action action, string taskName="运行中...") : this()
        {
            this.proBar.Minimum = 0;
            this.proBar.Maximum = 1;
            this.proBar.IsIndeterminate = true;
            this.Title = taskName;
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task task = new Task(new Action( () =>
            { 
                x.BeginInvoke(action, DispatcherPriority.Normal);
                    x.BeginInvoke(new Action(() =>
                {
                    this.Close();
                }), DispatcherPriority.Normal);

            }));
            task.Start();
            this.ShowDialog();
        }

        public void Start(IList<Action> actions)
        {
           
            int count = actions.Count;
            this.proBar.Maximum = count;
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task task = new Task(new Action(async () =>
            {
                for (int i = 0; i < count; i++)
                {
                   await x.BeginInvoke(actions[i], DispatcherPriority.Normal);
                    await x.BeginInvoke(new Action(()=>
                    {
                        this.proBar.Value++;
                        this.txt .Text = (i + 1)+" \\ " + count;
                        this.Title = this.txt.Text;
                    }), DispatcherPriority.Normal);
                    Thread.Sleep(1);
                }
             
                await x.BeginInvoke(new Action(() =>
                {
                    //Thread.Sleep(3000);
                    //this.Close();

                }));
            }));
            task.Start();
            this.ShowDialog();

        }
    }
}
