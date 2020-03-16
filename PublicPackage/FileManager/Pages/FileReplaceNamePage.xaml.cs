using FirstFloor.ModernUI.Presentation;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProgressTask;
using ShowDialogManger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFTemplate.Views;
using Utils = MyUtils.Utils;
namespace FileManager.Pages
{

    public  class FileReplaceNameModel:FileNameCustom
    {
        private string newFileName;
        public string NewFileName
        {
            get
            {
                return newFileName;
            }
            set
            {
                newFileName = value;
                OnPropertyChanged("newFileName");
            }
        }

        private bool success;
        public bool Success
        {
            get
            {
                return success;
            }
            set
            {
                success = value;
                OnPropertyChanged("success");
            }
        }

    }
   
    /// <summary>
    /// FileReplaceNamePage.xaml 的交互逻辑
    /// </summary>
    public partial class FileReplaceNamePage : UserControl
    {

        public static IList<FieldCustom> GetFieldCustoms()
        {
            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "执行状态", Name = "Success", Width = 100, Editable = false });
            list.Add(new FieldCustom { AliasName = "原文件名", Name = "FilePath", Width = 500, Editable = false });
            list.Add(new FieldCustom { AliasName = "新文件名", Name = "NewFileName", Width = 500, Editable = false });
          
            return list;
        }

        PagerPageClass<FileReplaceNameModel> filePagerPage = new PagerPageClass<FileReplaceNameModel>();
        public FileReplaceNamePage()
        {
            InitializeComponent();
            filePagerPage.SetPagerPage<FileReplaceNameModel>(new ObservableCollection<FileReplaceNameModel>(), GetFieldCustoms());
            pagerPageGrid.Children.Add(filePagerPage);
        }

        private void OpenExcel_Click(object sender, RoutedEventArgs e)
        {
            string path = FileUtils.SelectSingleExcelFile();
            if(Utils.CheckFileExists(path))
            {
                Dictionary<string, string> dic = SheetToDic(path);
                if(dic != null)
                {
                    IList<FileReplaceNameModel> fileReplaceNameModels = new List<FileReplaceNameModel>();
                    foreach (string srcPath in dic.Keys)
                    {
                        FileReplaceNameModel model = new FileReplaceNameModel { FilePath = srcPath, NewFileName = dic[srcPath] };
                        fileReplaceNameModels.Add(model);
                    }
                    filePagerPage.AddObject(fileReplaceNameModels);
                }
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            bool? isCopy = IsCopyRadio.IsChecked;
            string tem;
            if(isCopy.Value)
            {
                tem = "复制";
            }else
            {
                tem = "更名或者移动";
            }
            int successCount = 0;
            int falseCount = 0;
            var fileReplaceNameModels = filePagerPage.GetAllList<FileReplaceNameModel>();
            if(fileReplaceNameModels.Count >0)
            {
                if (MessageBoxCustom.MessageBoxShow("你将"+ tem + fileReplaceNameModels.Count+" 个文件"))
                {
                    foreach (FileReplaceNameModel model in fileReplaceNameModels)
                    {
                        WaitFormCustom.ShowCustom();
                        string path = model.FilePath;
                        string saveName = model.NewFileName;
                        string saveDir = Path.GetDirectoryName(saveName);
                        if (saveDir == "")
                        {
                            model.Success = false;
                            continue;
                        }
                        if (!Utils.CheckDirExists(saveDir))
                        {
                            Directory.CreateDirectory(saveDir);
                        }
                        if (Utils.CheckFileExists(path))
                        {
                            if(isCopy.Value)
                            {
                                File.Copy(path, saveName, true);
                            }else
                            {
                                File.Move(path, saveName);
                            }
                            successCount++;
                            model.Success = true;
                        }
                        else
                        {
                            model.Success = false;
                            falseCount++;
                        }

                    }
                    WaitFormCustom.CloseWaitForm("成功："+ successCount + " 个文件,失败："+ falseCount+" 个文件");
                }
               
            }

           
        }

        private void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            filePagerPage.DeleteAll();
        }
       /// <summary>
       ///  
       /// </summary>
       /// <param name="isheet"></param>
       /// <returns></returns>
        private static Dictionary<string, string> SheetToDic(string path )
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

            if(workbook.NumberOfSheets  == 0)
            {
                return null;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            ICell icell;
            String key;
            String value;
            String outer;
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
                icell = irow.GetCell(1);
                if (icell == null)
                {
                    continue;
                }
                icell.SetCellType(CellType.String);
                value = icell.StringCellValue;
                if (value.Equals(""))
                {
                    continue;
                }
                if (!dic.TryGetValue(key, out outer))
                {
                    dic.Add(key, value);
                }
            }
            return dic;
        }


    }
}
