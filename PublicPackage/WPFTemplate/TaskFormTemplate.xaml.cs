using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;
using System.Windows.Controls;

namespace WPFTemplate
{
    /// <summary>
    /// Interaction logic for TaskForm.xaml
    /// </summary>
    public partial class TaskFormModeTemplate : ModernWindow
    {
        private static TaskFormModeTemplate taskForm = null;
        private TaskFormModeTemplate()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
          

        }
        private static TaskFormModeTemplate GetInstance()
        {
            if (taskForm == null)
            {
                taskForm = new TaskFormModeTemplate();
            }
            return taskForm;
        }

        public static StackPanel GetMoel()
        {
            return GetInstance().Model;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
