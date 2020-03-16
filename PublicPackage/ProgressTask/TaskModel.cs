using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProgressTask
{
   public class TaskModel
    {
        public TaskModel()
        {

        }
        public TaskModel(MyTask myTask)
        {
            this.Task = Task;
            this.TaskName = myTask.TaskNameTextBlock.Text;
            this.Describe = myTask.DescribeTextBlock.Text;
            this.MyTask = myTask;
           
        }
        public virtual Int64? OBJECTID { get; set; }
        public virtual Task Task { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }
        public virtual string TaskName { get; set; }

        public virtual string Describe { get; set; }
        public virtual bool IsSuccess { get; set; }
        public virtual MyTask MyTask { get; set; }
       
    }
}
