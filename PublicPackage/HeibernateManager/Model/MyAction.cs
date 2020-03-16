using FirstFloor.ModernUI.Presentation;
using MyUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace HeibernateManager.Model
{
    public class MyAction
    {

        public MyAction()
        {

        }

        public MyAction(Action Action, string TaskName)
        {
            this.Action = Action;
            this.TaskName = TaskName;
        }

        public MyAction(Action Action, string TaskName,string SuccessShowMessage)
        {
            this.Action = Action;
            this.TaskName = TaskName;
            this.SuccessShowMessage = SuccessShowMessage;
        }
        public virtual Int64? OBJECTID { get; set; }
        public virtual Action Action { get; set; }
        public virtual DateTime StartTime { get; set; }
        
        public virtual DateTime EndTime { get; set; }

        public virtual  string TaskName { get; set; }
       
        public virtual string SuccessShowMessage { get; set; } 
        public virtual bool IsSuccess { get; set; }
        public virtual MyAction SuccessAction { get; set; }
        public virtual MyAction ErrorAction { get; set; }
        public virtual ProgressBar Progressbar { get; set; }

       
    }
 }
