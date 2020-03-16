using ExcelManager.Pages.Replace;
using FileManager;
using HeibernateManager.Model;
using MyUtils;
using ProgressTask;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WordManager.Pages.Repalce
{
    /// <summary>
    /// WordReplacePage.xaml 的交互逻辑
    /// </summary>
    public partial class WordReplacePage : UserControl
    {

        PagerPageClass<ReplaceViewModel> replacePagerPage = new PagerPageClass<ReplaceViewModel>();
        WordReplacePageViewModel model;
        public WordReplacePage()
        {
            InitializeComponent();
            model = SoftwareConfig.GetRedis<WordReplacePageViewModel>(WordReplacePageViewModel.RedisKey);
            model.Fileter = FileUtils.WordFileter;
            this.DataContext = model;


            //if(model.ReplaceViewModels == null)
            //{
            //    
            //}
            model.ReplaceViewModels = new ObservableCollection<ReplaceViewModel>();
           
         
            replacePagerPage.SetPagerPage<ReplaceViewModel>(model.ReplaceViewModels, ReplaceCustom.GetWordReplaceFieldCustoms(), true);
            replacePagerPage.HideFloot();

            ReplacePagerPageGrid.Children.Add(replacePagerPage);
            addFilePage.Init(model, model.Fileter);


            ContextMenu menu = new ContextMenu();

            MenuItem lookOldEmploeeItem = new MenuItem();
            lookOldEmploeeItem.Header = "删除";
            lookOldEmploeeItem.Click += DeleteItem_Click;
            menu.Items.Add(lookOldEmploeeItem);
            replacePagerPage.ContextMenu = menu;

        }

        /// <summary>
        /// 删除替换内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
         
            replacePagerPage.DeleteEntry(replacePagerPage.GetTabGrid().CurrentItem as ReplaceViewModel);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            //Spire.Doc.Document document = new  Spire.Doc.Document();
            //document.LoadFromFile("d:/123.doc");
            //document.SaveToFile("d:/123.doc");

            SaveRedis_Click(null, null);
            //执行替换
            var paths = addFilePage.GetAllList();
            if (Utils.CheckListExists(paths))
            {
                WaitFormCustom.ShowCustom("运行中。。。");
                SpireDocUtils.ReplaceText(model.ReplaceViewModels, paths);
                WaitFormCustom.CloseWaitForm("完成");
            }
            else
            {
                MessageBox.Show("没有选择任何的文件来替换");
            }
        }

        private void SaveRedis_Click(object sender, RoutedEventArgs e)
        {
            SoftwareConfig.SaveRedis(WordReplacePageViewModel.RedisKey,model);
        }

        private void ReplaceContentRest_Click(object sender, RoutedEventArgs e)
        {
            replacePagerPage.DeleteAll();
            AddReplaceViewModel_Click(null, null);
        }

        private void AddReplaceViewModel_Click(object sender, RoutedEventArgs e)
        {
            //添加替换内容
            ReplaceViewModel viewModel = new ReplaceViewModel();
            replacePagerPage.AddObject(viewModel);
        }

        private void SelectRepalceFile_Click(object sender, RoutedEventArgs e)
        {
            string path = FileUtils.SelectSingleExcelFile();
            if(Utils.CheckFileExists(path))
            {
                Dictionary<string, string> replaceDic = ExcelManager.ExcelRead.ReadExcelToDic(path,0);
                IList<ReplaceViewModel> list = new List<ReplaceViewModel>();
                foreach(string oldText in replaceDic.Keys)
                {
                    ReplaceViewModel viewModel = new ReplaceViewModel { OldText = oldText, NewText = replaceDic[oldText] };
                    list.Add(viewModel);
                }
                replacePagerPage.AddObject(list);
            }
            
        }
    }
}
