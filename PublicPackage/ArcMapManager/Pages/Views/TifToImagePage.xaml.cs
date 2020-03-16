using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using FileManager;
using HeibernateManager.Model;
using ProgressTask;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ArcMapManager.Pages.Views
{
    /// <summary>
    /// TifToImagePage.xaml 的交互逻辑
    /// </summary>
    public partial class TifToImagePage : UserControl
    {
        TifToImageViewModel model;
        AxMapControl axMapControl;
        public TifToImagePage()
        {
            InitializeComponent();
            MapForm mapForm = MapForm.GetInstance();
            axMapControl = MapForm.GetAxMapControl();
            model = SoftwareConfig.GetRedis<TifToImageViewModel>(TifToImageViewModel.RedisKey);
            filePagerPage.Init(model, "TIF|*.tif;");
            this.DataContext = model;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            IList<FileNameCustom> list = filePagerPage.GetAllList();
            if (MyUtils.Utils.CheckListExists(list))
            {
                model.Files = new ObservableCollection<FileNameCustom>();
                foreach (FileNameCustom fileNameCustom in list)
                {
                    model.Files.Add(fileNameCustom);
                }
                //执行转换
                TifToImage(model.Files, model.SaveDir);

            }
            else
            {
                MessageBox.Show("没有数据可转换！！！");
            }
            SoftwareConfig.SaveRedis(TifToImageViewModel.RedisKey, model);
        }
        public MyTask GetBiaoShiBaiaoTask2()
        {
            int count = 5000;

            MyTask myTask = new MyTask("生成标示表", "1230", count);
            Dispatcher x = Dispatcher.CurrentDispatcher;//取得当前工作线程
            Task task = new Task(new Action(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    x.BeginInvoke(new Action(() =>
                    {
                        myTask.SetProgressValue(i + 1, i + "/" + count);

                    }), DispatcherPriority.Normal);
                    Thread.Sleep(1);
                }
            }));
            myTask.Task = task;
            return myTask;
        }
        /// <summary>
        /// 执行数据转换
        /// </summary>
        /// <param name="files"></param>
        private void TifToImage(IList<FileNameCustom> list, string saveDir)
        {

            IList<FileNameCustom> fs = new List<FileNameCustom>();


            IList<Task> myTasks = new List<Task>();
            IList<FileNameCustom> rs = new List<FileNameCustom>();
            int count = list.Count / 20;
            for (int a = 0; a < list.Count; a++)
            {
                rs.Add(list[a]);
                if (a % count == 0)
                {
                    myTasks.Add(DownLoadFile_My(rs, saveDir));
                    rs = new List<FileNameCustom>();
                }
            }
            if (rs.Count > 0)
            {
             
                myTasks.Add(DownLoadFile_My(rs, saveDir));
            }
            foreach (Task tak in myTasks)
            {
                tak.Start();
            }

        }
        int aa = 0;
        Task DownLoadFile_My(object obj, string saveDir)
        {
            IWorkspaceFactory pWSF = new RasterWorkspaceFactoryClass();
            Task task = new Task(new Action(() =>
            {
                lock (this)
                {
                    IList<FileNameCustom> lst = obj as IList<FileNameCustom>;
                    foreach (FileNameCustom fileNameCustom in lst)
                    {
                        string path = fileNameCustom.FilePath;
                        if (MyUtils.Utils.CheckFileExists(path))
                        {
                            string name = System.IO.Path.GetFileNameWithoutExtension(path);
                            string saveName = saveDir + "\\" + name + ".img";

                            string pathName = System.IO.Path.GetDirectoryName(path);
                            string fileName = System.IO.Path.GetFileName(path);
                            //定义工作空间工厂并实例化                    
                            IWorkspace pWS = pWSF.OpenFromFile(pathName, 0);
                            IRasterWorkspace pRWS = pWS as IRasterWorkspace;
                            IRasterDataset pRasterDataset = pRWS.OpenRasterDataset(fileName);
                            string str = pRasterDataset.Format;
                            //影像金字塔的判断与创建
                            IRasterPyramid pRasPyrmid = pRasterDataset as IRasterPyramid;
                            if (pRasPyrmid != null)
                            {
                                if (!pRasPyrmid.Present)
                                {
                                    pRasPyrmid.Create();
                                }
                            }
                            IRaster pRaster = pRasterDataset.CreateDefaultRaster();
                            RasterSaveAs(pRaster, saveName);
                            Console.Write(aa+++":");
                            Console.WriteLine(saveName);
                        }
                    }
                }
            }));
            return task;
        }

        private void GetTask(IList<FileNameCustom> list, string saveDir)
        {
            IWorkspaceFactory pWSF = new RasterWorkspaceFactoryClass();
            foreach (FileNameCustom fileNameCustom in list)
            {
                string path = fileNameCustom.FilePath;
                if (MyUtils.Utils.CheckFileExists(path))
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    string saveName = saveDir + "\\" + name + ".img";

                    string pathName = System.IO.Path.GetDirectoryName(path);
                    string fileName = System.IO.Path.GetFileName(path);
                    //定义工作空间工厂并实例化                    
                    IWorkspace pWS = pWSF.OpenFromFile(pathName, 0);
                    IRasterWorkspace pRWS = pWS as IRasterWorkspace;
                    IRasterDataset pRasterDataset = pRWS.OpenRasterDataset(fileName);
                    string str = pRasterDataset.Format;
                    //影像金字塔的判断与创建
                    IRasterPyramid pRasPyrmid = pRasterDataset as IRasterPyramid;
                    if (pRasPyrmid != null)
                    {
                        if (!pRasPyrmid.Present)
                        {
                            pRasPyrmid.Create();
                        }
                    }
                    IRaster pRaster = pRasterDataset.CreateDefaultRaster();
                    RasterSaveAs(pRaster, saveName);
                }
            }
        }
        /// <summary>
        /// 保存栅格到本地
        /// </summary>
        /// <param name="raster">IRaster</param>
        /// <param name="fileName">本地路径全名</param>
        /// <returns>0:成功 !=0:失败</returns>
        private int RasterSaveAs(IRaster raster, string fileName)
        {
            int result = 0;
            try
            {
                IWorkspaceFactory pWKSF = new RasterWorkspaceFactoryClass();
                IWorkspace pWorkspace = pWKSF.OpenFromFile(System.IO.Path.GetDirectoryName(fileName), 0);
                ISaveAs pSaveAs = raster as ISaveAs;
                pSaveAs.SaveAs(System.IO.Path.GetFileName(fileName), pWorkspace, "IMAGINE Image");
                result = 0;
            }
            catch (Exception ex)
            {
                result = 1;
            }
            return result;
        }
        private void SaveDir_Click(object sender, RoutedEventArgs e)
        {
            string dir = FileUtils.SelectDir();
            if (MyUtils.Utils.CheckDirExists(dir))
            {
                model.SaveDir = dir;
            }
        }
        private void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            FileUtils.OpenDir(model.SaveDir);
        }
        public void Export(IExport pExport, string saveName)
        {
            IActiveView pActiveView;
            IEnvelope pPixelBoundsEnv;
            int iOutputResolution;
            int iScreenResolution;
            int hdc;

            pActiveView = axMapControl.ActiveView;
            pExport.ExportFileName = saveName;
            iScreenResolution = 96;
            iOutputResolution = 300;
            pExport.Resolution = iOutputResolution;

            tagRECT pExportFrame;
            pExportFrame = pActiveView.ExportFrame;
            tagRECT exportRECT;
            exportRECT.left = 0;
            exportRECT.top = 0;
            exportRECT.right = pActiveView.ExportFrame.right * (iOutputResolution / iScreenResolution);
            exportRECT.bottom = pActiveView.ExportFrame.bottom * (iOutputResolution / iScreenResolution);

            pPixelBoundsEnv = new EnvelopeClass();
            pPixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.top, exportRECT.right, exportRECT.bottom);

            pExport.PixelBounds = pPixelBoundsEnv;

            hdc = pExport.StartExporting();
            pActiveView.Output(hdc, (int)pExport.Resolution, ref exportRECT, null, null);
            pExport.FinishExporting();
            pExport.Cleanup();
        }
    }
}
