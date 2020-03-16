
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
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
namespace FileManager.Pages
{
    /// <summary>
    /// AddFilePage.xaml 的交互逻辑
    /// </summary>
    public partial class AddFilePage : UserControl
    {

        public PagerPageClass<FileNameCustom> addFilePagerPage = new PagerPageClass<FileNameCustom>();

        private string Filter;
        public AddFilePage()
        {
            InitializeComponent();
       
        }
        private AddFileViewModel model;


   

        public virtual void Init(AddFileViewModel model,string Filter, bool isShowSelect=false, Visibility SelectDirIsShow=Visibility.Visible)
        {
            this.model = model;
            this.DataContext = model;
            addFilePagerPage.SetPagerPage(model.Files, FileNameCustom.GetFieldCustoms(), true, isShowSelect);
            this.Filter = Filter;
            pagerPageGrid.Children.Add(addFilePagerPage);
            selectDirStackPanel.Visibility = SelectDirIsShow;
            addFilePagerPage.HideFloot();

            ContextMenu menu = new ContextMenu();
            MenuItem taskAllocationMenu = new MenuItem();
            taskAllocationMenu.Header = "打开文件";
            taskAllocationMenu.Click += OpenFile_Click;
            menu.Items.Add(taskAllocationMenu);

            addFilePagerPage.ContextMenu = menu;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FileNameCustom fileNameCustom = addFilePagerPage.GetTabGrid().CurrentItem as FileNameCustom;
            if(fileNameCustom != null)
            {
                string path = fileNameCustom.FilePath;
                FileUtils.OpenFile(path);
            }
        }

        /// <summary>
        /// 手动添加文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {

            IList<string> paths = FileUtils.SelectFiles(Filter, "可以多选文件");
            if(MyUtils.Utils.CheckListExists(paths))
            {
                IList<FileNameCustom> fs = new List<FileNameCustom>();
                foreach(string path in paths)
                {
                    if(MyUtils.Utils.CheckFileExists(path))
                    {
                        FileNameCustom fileNameCustom = new FileNameCustom(path);
                        fs.Add(fileNameCustom);
                    } 
                }
                addFilePagerPage.AddObject(fs);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if(MyUtils.Utils.MessageBoxShow("确定要清空所有的文件吗？"))
            {

                addFilePagerPage.DeleteAll();
            }
        }
        public IList<FileNameCustom> GetAllList()
        {
            return model.Files;
        }

        private void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            FileUtils.OpenDir(model.Dir);
            
        }

        private void AddDirFile_Click(object sender, RoutedEventArgs e)
        {
           

            string dir = FileUtils.SelectDir();
            if (!Utils.IsStrNull(dir))
            {
                model.Dir = dir;
                DirClass dirClass = new DirClass(dir);
                if(model.IsDiGuiChaZhao)
                {
                    AddFile(dirClass.FindFileAll());
                }else
                {
                    AddFile(dirClass.Files);
                }
            }
        }
        public void AddFile(IList<string> files2)
        {
            IList<FileNameCustom> fileNameCustoms = new List<FileNameCustom>();

            if (!Utils.CheckListExists(files2))
            {
                return;
            }
            if(Utils.IsStrNull(Filter))
            {
                for (int a = 0; a < files2.Count; a++)
                {
                    FileNameCustom custom = new FileNameCustom(files2[a]);
                    fileNameCustoms.Add(custom);
                }
                addFilePagerPage.AddObject(fileNameCustoms);
                return;
            }
            IList<string> files = new List<string>();
           int count = files2.Count;
            string[] filters = FileterToEndWith();
            for (int a = 0; a < count; a++)
            {
                string path = files2[a];
                string ex = System.IO.Path.GetExtension(path);
                if (path != null && Array.IndexOf(filters,ex) !=-1)
                {
                    files.Add(path);

                }
            }

           
            for (int a = 0; a < files.Count; a++)
            {
                FileNameCustom custom = new FileNameCustom(files[a]);
                fileNameCustoms.Add(custom);
            }
            addFilePagerPage.AddObject(fileNameCustoms);
        }


       
    
        /// <summary>
        /// 文件过滤器 转 单个格式集合   样式Excel(*.xls, *.xlsx) | *.xls; *.xls
        /// </summary>
        /// <returns></returns>
        public string[] FileterToEndWith()
        {
            string[] array = Filter.Split('|');
            if (array.Length == 2)
            {
                return array[1].Replace("*", "").Split(';');
            }
            else
            {
                throw new Exception("程序设置的过滤器有问题");
            }

        }

        private void ExportTable_Click(object sender, RoutedEventArgs e)
        {
            AddFileViewModel addFileViewModel = new AddFileViewModel();
            
        }

        private void AddExcelFile_Click(object sender, RoutedEventArgs e)
        {
            string path = FileUtils.SelectSingleExcelFile();
            if(Utils.CheckFileExists(path))
            {
                IList<string> paths = SheetToList(path);
                if(paths != null)
                {
                    AddFile(paths);
                }
            }
        }

        private static IList<string> SheetToList(string path)
        {
            IWorkbook workbook = null;

            //FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (path.IndexOf(".xlsx") > 0) // 2007版本
            {
                workbook = new XSSFWorkbook(fileStream);  //xlsx数据读入workbook
            }
            else if (path.IndexOf(".xls") > 0) // 2003版本
            {
                workbook = new HSSFWorkbook(fileStream);  //xls数据读入workbook
            }
            fileStream.Close();

            if (workbook.NumberOfSheets == 0)
            {
                return null;
            }
            IList<string> list = new List<string>();
            ICell icell;
            String key;
            foreach (IRow irow in workbook.GetSheetAt(0))
            {
                icell = irow.GetCell(0);
                if (icell == null)
                {
                    continue;
                }
                icell.SetCellType(CellType.String);
                key = icell.StringCellValue;
                if (key.Equals(""))
                {
                    continue;
                }
                list.Add(key);
            }
            return list;
        }
    }
}
