using ArcGisManager;
using FileManager;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ArcMapManager.Pages.Views
{
    /// <summary>
    /// MXDToPDFPage.xaml 的交互逻辑
    /// </summary>
    public partial class MXDToPDFPage : UserControl
    {
        public MXDToPDFPage()
        {
            InitializeComponent();
        }

        public void FindMxdPaths()
        {
            string mxdDir = this.MxdDir.Text;
            
            if (MyUtils.Utils.CheckDirExists(mxdDir))
            {
                int length = mxdDir.Length;
                IList<string> mxdPaths = FileUtils.SelectAllDirFiles(mxdDir, ".mxd", FileSelectRelation.EndsWith, false);
                while (ListView.Items.Count > 0)
                {
                    ListView.Items.RemoveAt(0);
                }
                for (int a = 0; a < mxdPaths.Count; a++)
                {
                    string path = mxdPaths[a];
                    ListView.Items.Add("第" + (a + 1) + "条：" + path.Substring(length));
                }
                ListView.DataContext = mxdPaths;

            }
            else
            {
                ListView.DataContext = null;
            }
        }
        /// <summary>
        /// pdf转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            IList<string> mxdPaths = ListView.DataContext as IList<string>;

            if (MyUtils.Utils.CheckListExists(mxdPaths))
            {

                ArcGisUtils.ExportPDFS(mxdPaths);
            }
            else
            {
                MessageBox.Show("无数据转换！！！");
            }



        }
        private void MxdDir_LostFocus(object sender, RoutedEventArgs e)
        {
            FindMxdPaths();
        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            string dir = FileUtils.SelectDir();
            if (!MyUtils.Utils.IsStrNull(dir))
            {
                this.MxdDir.Text = dir;
                FindMxdPaths();
            }
        }
    }
}
