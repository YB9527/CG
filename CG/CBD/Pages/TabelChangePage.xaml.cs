using CBD.CBDModels;
using FileManager;
using FileManager.Pages;
using ProgressTask;
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

namespace CBD.Pages
{
    /// <summary>
    /// TabelChange.xaml 的交互逻辑
    /// </summary>
    public partial class TabelChangePage : UserControl
    {
        private AddFileViewModel addFileViewModel = new AddFileViewModel();
        public TabelChangePage()
        {
            InitializeComponent();
            filePagerPage.Init(addFileViewModel, FileUtils.ExcelFileter);
          
        }

        private void ChengBaoSuXingBiaoChange_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ZhongDiShuXingBiaoChange_Click(object sender, RoutedEventArgs e)
        {
            IList<FileNameCustom> files = addFileViewModel.Files;
            if (files.Count > 0)
            {
                foreach (FileNameCustom fileNameCustom in files)
                {
                   
                  
                }

            }
            else
            {
                MessageBox.Show("你还没有添加任何文件！！！");
            }
        }

        private void TableChange_Click(object sender, RoutedEventArgs e)
        {

            IList<FileNameCustom> files = addFileViewModel.Files;
            if (files.Count > 0)
            {
                string shpmjPath = FileUtils.SelectSingleExcelFile("选择shp面积表格");
                if (shpmjPath == null || shpmjPath == "")
                {
                    return;
                }
                string saveDir = FileManager.FileUtils.SeleFileDir("选择保存的文件夹");
                if (saveDir == null || saveDir == "")
                {
                    return;
                }

                WaitFormCustom.ShowCustom();
                Dictionary<string, string> dkbmMJDic = ExcelManager.ExcelRead.ReadExcelToDic(shpmjPath, 0);
                foreach (FileNameCustom fileNameCustom in files)
                {
                    bool flag = true;
                    IList<JTCY> jTCies = JTCYCustom.ReadJTCYExcel(fileNameCustom.FilePath);
                    IList<JTCY> hzs = JTCYCustom.ExtractHZs(jTCies);
                    if(hzs == null)
                    {
                        return;
                    }
                    IList<DK> dks = DKCustom.ReadDKExcel(fileNameCustom.FilePath);
                    string area;
                    string dkbm;
                   
                    JTCYCustom.SetJTCY_DK(hzs, dks);

                    foreach(JTCY hz in  hzs)
                    {
                        hz.CBFBM = System.IO.Path.GetFileNameWithoutExtension(fileNameCustom.FilePath) + hz.CBFBM;
                    }
                    string zh = "512081"+System.IO.Path.GetFileNameWithoutExtension(fileNameCustom.FilePath).Substring(6,8);
                    foreach (DK dk in dks)
                    {
                        dkbm = dk.DKBM;
                        if (MyUtils.Utils.IsStrNull(dkbm))
                        {
                            MessageBox.Show("文件：" + fileNameCustom.FilePath + ",地块编码有为空");
                            flag = false;
                            break;
                        }
                        if (dkbmMJDic.TryGetValue(zh +dkbm, out area))
                        {
                            double areaMJ;
                            if (double.TryParse(area, out areaMJ))
                            {
                                dk.SCMJ = areaMJ.ToString("f2");
                            }
                          
                        }else
                        {
                            {
                                MessageBox.Show("地块编码：" + zh + dkbm + ",在shp面积中没有找到");
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (hzs != null && flag)
                    {
                        //导出数据
                        fileNameCustom.IsSuccess = JTCYCustom.ExportBsicInformationExcel(hzs, saveDir);
                    }
                }
                WaitFormCustom.CloseWaitForm();
            }
            else
            {
                MessageBox.Show("你还没有添加任何文件！！！");
            }
        }

       
    }
}
