using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;
using ShowDialogManger;

namespace ProgressTask
{
    public partial class WaitFormCustom : DevExpress.XtraWaitForm.WaitForm
    {
        public WaitFormCustom()
        {
            InitializeComponent();
            this.progressPanel1.AutoHeight = true;
            

           // FormBorderStyle = FormBorderStyle.FixedDialog;//固定窗体大小
            ShowInTaskbar = false;//不在任务栏中显示
            ControlBox = false;//隐藏右上角的关闭等按钮
            UseWaitCursor = true;//显示等待光标
            StartPosition = FormStartPosition.CenterScreen;//在屏幕中间显示
        }


        public enum WaitFormCommand
        {
        }
        #region Overrides

        public override void SetCaption(string caption)
        {
            base.SetCaption(caption);
            this.progressPanel1.Caption = caption;
        }
        public override void SetDescription(string description)
        {
            base.SetDescription(description);
            this.progressPanel1.Description = description;
        }
        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion
        static WaitFormCustom form = null;

        private  static object  lockObj = new object();
        public static void ShowCustom(string waitMsg = "Loading...", int sleepTime = 100)
        {
            /*if(waitMsg == null || waitMsg == "")
            {
                waitMsg = "Loading...";
            }
            if (form == null)
            {
                form = new WaitFormCustom();
                ThreadPool.QueueUserWorkItem(state =>
                {
                    form.SetDescription(waitMsg);
                    lock (lockObj)
                    {
                        try
                        {
                            form.ShowDialog();
                        }
                        catch(Exception e)
                        {

                        }
                     
                    }
                });
            }
            System.Threading.Thread.Sleep(sleepTime);*/
        }
        public static void CloseWaitForm(string success = null)
        {

            if (success !="" && success != null)
            {
                MessageBox.Show(success);

            }
            /*
            if (form != null)
            {
                Action ac = new Action(() =>
                {
                    form.Close();
                    form = null;
                    if (success != null)
                    {
                      MessageBox.Show(success) ;
                       
                    }
                });
                try
                {
                    form.Invoke(ac); //在同步方法里面实现更新窗体上的数据
                }
                catch(Exception e)
                {

                }
         
            }
            */

        }

    }
}