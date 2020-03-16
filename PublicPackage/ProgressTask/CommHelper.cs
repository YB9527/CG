using System;
using System.Windows;
using System.Threading.Tasks;
using ProgressTask;
using HeibernateManager.Model;
using MyUtils;
using HeibernateManager.HibernateDao;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;

namespace ProgressTask
{
    public class CommHelper
    {
        private static HibernateUtils hibernate = HibernateUtils.GetInstance();
        /* */
        public static void FastTask1(MyAction myAction)
         {
             WaitForm loading = new WaitForm(myAction.TaskName);
             Task t = new Task(() =>
             {
                 try
                 {

                     myAction.StartTime = DateTime.Now;
                     myAction.EndTime = DateTime.Now;
                     myAction.IsSuccess = true;

                     Application.Current.Dispatcher.BeginInvoke(myAction.Action, DispatcherPriority.Normal);

                     if (!Utils.IsStrNull(myAction.SuccessShowMessage))
                     {
                         MessageBox.Show(myAction.SuccessShowMessage);
                     }


                 }
                 finally
                 {
                     Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                     {
                         loading.Hide();

                     }));
                     //保存任务进入数据库
                     hibernate.SaveEntity(myAction);
                 }
             });
             t.Start();
             loading.ShowDialog();
             ;
         }

        public static void FastTask(MyAction myAction)
        {
            WaitForm loading = new WaitForm(myAction.TaskName);
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task t = new Task(() =>
           {
               try
               {

                   myAction.StartTime = DateTime.Now;
                   myAction.Action();
                   myAction.EndTime = DateTime.Now;
                   myAction.IsSuccess = true;

               }
               finally
               {

                    //保存任务进入数据库
                    hibernate.SaveEntity(myAction);

                   x.BeginInvoke(new Action(() =>
                   {
                       loading.Hide();

                   }));
                 
                   if (!Utils.IsStrNull(myAction.SuccessShowMessage))
                   {
                       MessageBox.Show(myAction.SuccessShowMessage);
                   }

               }
           });
            t.Start();
            loading.ShowDialog();

        }
        /*
        public static void FastTask(MyAction myAction)
         {
             WaitForm loading = WaitForm.GetInstance(myAction.TaskName);
             Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
             Task t = new Task( () =>
             {
                 try
                 {
                     myAction.StartTime = DateTime.Now;
                    
                   
                      x.BeginInvoke(new Action(() =>
                     {
                         myAction.Action();
                         loading.Title = myAction.TaskName;
                         myAction.EndTime = DateTime.Now;
                         myAction.IsSuccess = true;
                         if (!Utils.IsStrNull(myAction.SuccessShowMessage))
                         {
                             MessageBox.Show(myAction.SuccessShowMessage);
                         }

                     }), DispatcherPriority.Normal);
                   
                 }
                 finally
                 {
                     //保存任务进入数据库
                    hibernate.SaveEntity(myAction);

                      x.BeginInvoke(new Action(() =>
                     {
                         loading.Hide();

                     }));

                 }
             });
             t.Start();
             loading.ShowDialog();

         }*/


        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="suceessAction"> 成功任务 </param>
        /// <param name="errorAction"> 失败任务 </param>
        public static void FastTask(Action task, Action suceessAction = null, Action errorAction = null)
        {

            Task t = new Task(() =>
            {
                try
                {
                    task();

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {


                    }));
                    if (suceessAction != null)
                    { suceessAction(); }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {

                        errorAction();
                        MessageBox.Show("失败\r\n" + ex.ToString());//new PopupWindow().ShowThis("绘图失败");
                    }));
                }
            });
            t.Start();

            ;
        }
       
    }
}
