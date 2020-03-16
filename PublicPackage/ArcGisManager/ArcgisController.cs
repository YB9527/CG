using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using FileManager;
using HeibernateManager.Model;
using MyUtils;
using NPOI.SS.UserModel;
using ProgressTask;
using System;
using System.Collections.Generic;


namespace ArcGisManager
{
    public class ArcGisController
    {
        private AxMapControl axMapControl;

        public ArcGisController(AxMapControl axMapControl)
        {
            this.axMapControl = axMapControl;
        }

        private static IFeatureWorkspace Workapace { get; set; }
        public  static IFeatureWorkspace GetIFeatureWorkspace(string gdbPath= @"D:\dmSoftwareSource\测试.gdb")
        {
            
            if (Workapace == null)
            {
               
                FileGDBWorkspaceFactoryClass fac = new FileGDBWorkspaceFactoryClass();
                Workapace = (IFeatureWorkspace)fac.OpenFromFile(gdbPath, 0);
            }
            return Workapace;
        }
        public Dictionary<string, IFeatureLayer> MapToLayer(AxMapControl axMapControl)
        {
            Dictionary<string, IFeatureLayer> dic = new Dictionary<string, IFeatureLayer>();
            for (int i = 0; i < axMapControl.Map.LayerCount; i++)
            {
                IFeatureLayer iFeatureLayer = axMapControl.get_Layer(i) as IFeatureLayer;
                dic.Add(iFeatureLayer.Name, iFeatureLayer);
            }
            return dic;
        }
        /// <summary>
        ///多个 excel 转出宅基地shp
        /// </summary>
        /// <param name="paths"></param>
        public void ExcelToShp(IList<string> paths)
        {
            List<MyAction> actions = new List<MyAction>();
            foreach (string path in paths)
            {
               
                Action action = new Action(() =>
                {
                    ExcelToShp(path);
                });
                MyAction myAction = new MyAction(action, path + " SHP转换 ");
                actions.Add(myAction);
            }
            SingleTaskForm task = new SingleTaskForm(actions, "SHP转换 ");
        
        }
       public  void ExcelToShp(string path)
        {
            if(Utils.CheckFileExists(path))
            {
               
                    string zjdShpTemplte = System.AppDomain.CurrentDomain.BaseDirectory + "ArcGisTemplete\\宅基地模板shp";
                    string saveDir = path.Substring(0, path.LastIndexOf(".")) + "SHP";
                    FileUtils.CopyDir(zjdShpTemplte, saveDir, true);
                    while(axMapControl.LayerCount >0)
                    {
                        axMapControl.DeleteLayer(0);
                    }
                    axMapControl.AddShapeFile(saveDir, "ZJD.shp");

                    IFeatureLayer featureLayer = ArcGisUtils.GetFeatureLayer2("ZJD");
                    IFeatureClass featureClass = featureLayer.FeatureClass;
                    IWorkspaceEdit workspaceEdit = ArcGisUtils.StratEdit(featureClass);

                    IFeatureCursor featureCursor = featureClass.Insert(false);
                    IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
                    ISheet sheet = ExcelManager.ExcelRead.ReadExcelSheet(path, 0);
               
                    foreach (NPOI.SS.UserModel.IRow row in sheet)
                    {
                        IPointCollection pc = new Polygon();
                        string[] pcStrArray = row.GetCell(2).StringCellValue.Split(',');
                        foreach (string ptStr in pcStrArray)
                        {
                            string[] ptArray = ptStr.Split(':');
                            IPoint point = new PointClass();
                            point.PutCoords(double.Parse(ptArray[0]), double.Parse(ptArray[1]));
                            pc.AddPoint(point);
                        }
                        IPolygon polygon = ArcGisUtils.CreatePolygon(pc);
                        featureBuffer.Shape = polygon;
                        featureBuffer.Value[3] = row.GetCell(0).StringCellValue;
                        featureBuffer.Value[4] = row.GetCell(1).StringCellValue;
                        featureCursor.InsertFeature(featureBuffer);
                    }

                    //IList<IFeature> list = ArcGisUtils.GetEntitysList("", "ZJD");
                    ArcGisDao.EndEdit(workspaceEdit);
                
            

                

            }
        }
    }
}
