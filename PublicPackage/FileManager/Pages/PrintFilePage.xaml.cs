using FirstFloor.ModernUI.Presentation;
using HeibernateManager.Model;
using MyUtils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProgressTask;
using ShowDialogManger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class AddPrintFileModel : NotifyPropertyChanged
    {
        public AddPrintFileModel()
        {
            this.Files = new ObservableCollection<PrintFileModel>();
        }


        public virtual string Fileter { get; set; }
        private IList<PrintFileModel> files;
        public IList<PrintFileModel> Files
        {
            get
            {

                return files;
            }
            set
            {
                files = value;
                OnPropertyChanged("files");
            }
        }
        private bool isDiGuiChaZhao;
        public bool IsDiGuiChaZhao
        {
            get
            {
                return isDiGuiChaZhao;
            }
            set
            {
                if (isDiGuiChaZhao != value)
                {
                    isDiGuiChaZhao = value;
                    OnPropertyChanged("isDiGuiChaZhao");
                }
            }
        }
        private string dir;
        public string Dir
        {
            get
            {
                return dir;
            }
            set
            {
                if (dir != value)
                {
                    dir = value;
                    OnPropertyChanged("dir");
                }
            }
        }

    }
    public partial class PrintFileModel : FileNameCustom
    {
        public PrintFileModel(string path) : base(path)
        {
            
        }
        private int printCount=1;
        /// <summary>
        /// 打印份数
        /// </summary>
        public int PrintCount
        {
            get
            {
                return printCount;
            }
            set
            {
                printCount = value;
                OnPropertyChanged("printCount");
            }
        }

        private bool isTwoPage;
        /// <summary>
        /// 是否双面打印
        /// </summary>
        public bool IsTwoPage
        {
            get
            {
                return isTwoPage;
            }
            set
            {
                isTwoPage = value;
                OnPropertyChanged("isTwoPage");
            }
        }
        private bool isPrintOver;
        /// <summary>
        /// 是否打印完成
        /// </summary>
        public bool IsPrintOver
        {
            get
            {
                return isPrintOver;
            }
            set
            {
                isPrintOver = value;
                OnPropertyChanged("isPrintOver");
            }
        }
    }
    /// <summary>
    /// PrintFilePage.xaml 的交互逻辑
    /// </summary>
    public partial class PrintFilePage : UserControl
    {
        AddPrintFileModel model = new AddPrintFileModel();
        public PagerPageClass<PrintFileModel> addFilePagerPage = new PagerPageClass<PrintFileModel>();
        public PrintFilePage()
        {
            InitializeComponent();

            addFilePagerPage.SetPagerPage<PrintFileModel>(model.Files, GetFieldCustoms(),true,true);
            this.DataContext = model;
            pagerPageGrid.Children.Add(addFilePagerPage);
        }
        public static IList<FieldCustom> GetFieldCustoms()
        {
            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "打印完成", Name = "IsPrintOver", Width = 80 });
            list.Add(new FieldCustom { AliasName = "文件名", Name = "Name", Width = 200 });
            list.Add(new FieldCustom { AliasName = "扩展名", Name = "Extension", Width = 80 });
            list.Add(new FieldCustom { AliasName = "打印分数", Editable = true, Name = "PrintCount", Width = 80 });
            list.Add(new CheckBoxFieldCustom { AliasName = "双面打印", Editable = true, Name = "IsTwoPage", Width = 80 });
            list.Add(new FieldCustom { AliasName = "文件夹", Name = "Dir", Width = 300 });
            return list;
        }
        /// <summary>
        /// 手动添加文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFile_Click(object sender, RoutedEventArgs e)
        {

            IList<string> paths = FileUtils.SelectFiles("", "可以多选文件");
            if (MyUtils.Utils.CheckListExists(paths))
            {
                IList<PrintFileModel> fs = new List<PrintFileModel>();
                foreach (string path in paths)
                {
                    if (MyUtils.Utils.CheckFileExists(path))
                    {
                        PrintFileModel fileNameCustom = new PrintFileModel(path);
                        fs.Add(fileNameCustom);
                    }
                }
                addFilePagerPage.AddObject(fs);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MyUtils.Utils.MessageBoxShow("确定要清空所有的文件吗？"))
            {

                addFilePagerPage.DeleteAll();
            }
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
                if (model.IsDiGuiChaZhao)
                {
                    AddFile(dirClass.FindFileAll());
                }
                else
                {
                    AddFile(dirClass.Files);
                }
            }
        }
        public void AddFile(IList<string> files2)
        {
            IList<PrintFileModel> fileNameCustoms = new List<PrintFileModel>();

            if (!Utils.CheckListExists(files2))
            {
                return;
            }

            for (int a = 0; a < files2.Count; a++)
            {
                PrintFileModel fileNameCustom = new PrintFileModel(files2[a]);
                fileNameCustoms.Add(fileNameCustom);
            }
            addFilePagerPage.AddObject(fileNameCustoms);
        }


        private void ExportTable_Click(object sender, RoutedEventArgs e)
        {
            AddFileViewModel addFileViewModel = new AddFileViewModel();

        }

        private void AddExcelFile_Click(object sender, RoutedEventArgs e)
        {
            string path = FileUtils.SelectSingleExcelFile();
            if (Utils.CheckFileExists(path))
            {

             

                IList<PrintFileModel> fileNameCustoms = SheetToList(path);
                if (fileNameCustoms != null )
                {
                    addFilePagerPage.AddObject(fileNameCustoms);
                }
            }
        }

        private static IList<PrintFileModel> SheetToList(string path)
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
            IList<PrintFileModel> list = new List<PrintFileModel>();
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
                PrintFileModel fileNameCustom = new PrintFileModel(key);

                icell = irow.GetCell(1);
                icell.SetCellType(CellType.String);
                key = icell.StringCellValue;
                int count;
                if (int.TryParse(key,out count))
                {
                    fileNameCustom.PrintCount = count;
                }

                icell = irow.GetCell(2);
                icell.SetCellType(CellType.String);
                key = icell.StringCellValue;
                bool isTwoPage;//双面打印
                if (bool.TryParse(key, out isTwoPage))
                {
                    fileNameCustom.IsTwoPage = isTwoPage;
                }


                list.Add(fileNameCustom);

            }
            return list;
        }
        /// <summary>
        /// 本地打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalPrint_Click(object sender, RoutedEventArgs e)
        {
            IList<PrintFileModel> files = addFilePagerPage.GetChekedAll();
            if (files.Count > 0 && MessageBoxCustom.MessageBoxShow("确要打印：" + files.Count + " 个文件吗？", false, Visibility.Visible))
            {
                IList<MyAction> actions = new List<MyAction>();
                foreach (PrintFileModel model in files)
                {
                    MyAction myAction = new MyAction(new Action(() =>
                    {
                        for (int a = 0; a < model.PrintCount; a++)
                        {
                            PrintUtils.RelativePrint(model.FilePath);
                            model.IsPrintOver = true;
                        }

                    }), model.Name + model.Extension);
                    actions.Add(myAction);
                }
                SingleTaskForm singleTaskForm = new SingleTaskForm(actions);
            }
        }
        /// <summary>
        /// 软件功能打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoftWarePrint_Click(object sender, RoutedEventArgs e)
        {
            IList<PrintFileModel> files = addFilePagerPage.GetChekedAll();
            if (files.Count > 0 && MessageBoxCustom.MessageBoxShow("确定要打印：" + files.Count + " 个文件吗？", false, Visibility.Visible))
            {
                IList<MyAction> actions = new List<MyAction>();
                foreach (PrintFileModel model in files)
                {
                    for (int a = 0; a < model.PrintCount; a++)
                    {
                        MyAction myAction= new MyAction(new Action(() =>
                        {
                            if (PrintUtils.Print(model.FilePath, model.PrintCount))
                            {
                                model.IsPrintOver = true;
                            }

                        }), model.Name+model.Extension);
                        actions.Add(myAction);
                    }
                }
                SingleTaskForm singleTaskForm = new SingleTaskForm(actions);
            }
        }

    }
}
