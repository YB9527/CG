using FileManager;
using HeibernateManager.Model;
using ProgressTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFTemplate.Views;
using Utils = MyUtils.Utils;

namespace ExcelManager.Pages
{
    /// <summary>
    /// ExcelMerge.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelMergePage : UserControl
    {
        ExcelMergeModel model;
   
       
        public ExcelMergePage()
        {
            InitializeComponent();
            model = SoftwareConfig.GetRedis<ExcelMergeModel>(ExcelMergeModel.RedisKey);
            this.DataContext = model;
            //filePagerPage.SetPagerPage<FileNameCustom>(model.FileNameCustoms, FileNameCustom.GetFieldCustoms());
            filePagerPage.Init(model, FileUtils.ExcelFileter);


        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            string dir = FileUtils.SelectDir();
            if (!Utils.IsStrNull(dir))
            {
                model.Dir = dir;
                IList<string> list = FileUtils.SelectAllDirFiles(dir, ".xls", FileSelectRelation.Contains, false);
                AddFile(list);
            }
            
        }
        public void AddFile(IList<string> files)
        {
            if(!Utils.CheckListExists(files))
            {
                return;
            }
        
            for (int a = 0; a < files.Count; a++)
            {
                FileNameCustom custom = new FileNameCustom(files[a]);
                model.FileNameCustoms.Add(custom);
              
            }
         
          
        }
        /// <summary>
        /// 手动添加多个文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            IList<string> files = FileUtils.SelectExcelFiles();
            AddFile(files);
        }

   

        /// <summary>
        /// 执行合并
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {

            ///list<object> 转换成正确类型
            IList<FileNameCustom> currencies = model.FileNameCustoms;
            model.SetPaths(currencies);
           
            
            int count = model.FileNameCustoms.Count;
            for (int a =0; a < count; a++)
            {
                FileNameCustom custom = model.FileNameCustoms[a];
                if (!Utils.CheckFileExists(custom.FilePath))
                {
                    model.FileNameCustoms.RemoveAt(a);
                    a--;
                    count--;
                }
            }
            MyAction myAction = new MyAction(new Action(()=>
            {
                ExcelWrite.MergeExcel(model);

            }), "合并表格","完成");
            CommHelper.FastTask(myAction);
            SoftwareConfig.SaveRedis(ExcelMergeModel.RedisKey, model);
        }

        /// <summary>
        /// Excel解析路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileAna_Click(object sender, RoutedEventArgs e)
        {
            /*
            string excelPath = FileUtils.SelectSingleExcelFile();
            if(Utils.IsStrNull(excelPath))
            {
                return;
            }
            
            ExcelPathText.Text = excelPath;
            IList<string> list = ExcelRead.FileToList(excelPath, 0);
            int count = list.Count;
            for(int a =0; a <count;a++)
            {
                string path = list[a];
                if(path!= null && !path.Contains(".xls"))
                {
                    list.RemoveAt(a);
                    a--;
                    count--;
                }
            }
            AddFile(list);*/
        }

        private void SaveName_Click(object sender, RoutedEventArgs e)
        {
            string saveName = FileUtils.SaveFileName("合并.xlsx", FileUtils.ExcelFileter);
            model.SaveName = saveName;

        }
    }
}
