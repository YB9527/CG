using FileManager;
using HeibernateManager.Model;
using NPOI.SS.UserModel;
using ProgressTask;
using Spire.Xls;
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
namespace ExcelManager.Pages.Replace
{
    /// <summary>
    /// ReplacePage.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelReplacePage : UserControl
    {
        PagerPageClass<ReplaceViewModel> replacePagerPage = new PagerPageClass<ReplaceViewModel>();
   
        RepalcePageViewModel model;
        public ExcelReplacePage()
        {
            
            InitializeComponent();
            model = SoftwareConfig.GetRedis<RepalcePageViewModel>(RepalcePageViewModel.RedisKey);
            this.DataContext = model;

            //IList<ReplaceViewModel> test = new List<ReplaceViewModel>();
            //test.Add(new ReplaceViewModel { OldText = "1", NewText = "345", Rel = Relation.Contains });
            //model.ReplaceViewModels = test;
          
            replacePagerPage.SetPagerPage<ReplaceViewModel>(model.ReplaceViewModels, ReplaceCustom.GetFieldCustoms(),true);
            replacePagerPage.HideFloot();
            ReplacePagerPageGrid.Children.Add(replacePagerPage);

            filePagerPage.Init(model, FileUtils.ExcelFileter);
         
        }

        private void AddReplaceViewModel_Click(object sender, RoutedEventArgs e)
        {
            replacePagerPage.AddObject(new ReplaceViewModel());
        }
        /// <summary>
        /// 内容，1为开始，2为包含，3结束，4相等,不填为相等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectRepalceFile_Click(object sender, RoutedEventArgs e)
        {
            IList<string> paths = FileUtils.SelectExcelFiles();
            if(Utils.CheckListExists(paths))
            {
                IList<ReplaceViewModel> list = new List<ReplaceViewModel>();
                foreach(string path in paths)
                {
                    if(Utils.CheckFileExists(path))
                    {
                        ISheet sheet = ExcelRead.ReadExcelSheet(path, 0);
                        foreach(IRow row in sheet)
                        {
                            var replaceModel = new ReplaceViewModel();
                            for (int a =0; a <3;a ++)
                            {
                                ICell cell = row.GetCell(a);
                                if(cell == null || cell.CellType == CellType.Blank)
                                {
                                    if(a == 3)
                                    {
                                        cell = row.CreateCell(a);
                                        cell.SetCellValue(4);
                                    }
                                    break;
                                }
                                cell.SetCellType(CellType.String);
                                string value = cell.StringCellValue;
                                switch(a)
                                {
                                    case 0:
                                        replaceModel.OldText = value;
                                        break;
                                    case 1:
                                        replaceModel.NewText = value;
                                        break;
                                    case 2:
                                        int gx;
                                        if(int.TryParse(value,out gx))
                                        {
                                            switch(gx)
                                            {
                                                case 0:
                                                    replaceModel.StrRelation = MyUtils.StrRelation.StartsWith;
                                                        break;
                                                case 1:
                                                    replaceModel.StrRelation = MyUtils.StrRelation.Contains;
                                                    break;
                                                case 2:
                                                    replaceModel.StrRelation = MyUtils.StrRelation.EndsWith;
                                                    break;
                                                case 3:
                                                    replaceModel.StrRelation = MyUtils.StrRelation.Equals;
                                                    break;
                                            }
                                        }else
                                        {
                                            replaceModel.StrRelation = MyUtils.StrRelation.Equals;
                                        }
                                      
                                        break;


                                }
                            }
                            list.Add(replaceModel);

                        }
                    }
                }
                replacePagerPage.AddObject(list);

            }
        }

        private void ReplaceContentRest_Click(object sender, RoutedEventArgs e)
        {
            replacePagerPage.DeleteAll();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SaveRedis_Click(null, null);
          
            //执行替换
            model.Files = filePagerPage.GetAllList();
            if (Utils.CheckListExists(model.Files))
            {
                WaitFormCustom.ShowCustom("运行中。。。");
                ExcelWrite.ReplaceText(model, model.Files);
                WaitFormCustom.CloseWaitForm("完成");
            }
            else
            {
                MessageBox.Show("没有选择任何的文件来替换");
            }

        }

        private void SaveRedis_Click(object sender, RoutedEventArgs e)
        {
            model.Files = filePagerPage.GetAllList();
            SoftwareConfig.SaveRedis(RepalcePageViewModel.RedisKey, model);
        }
    }
}
