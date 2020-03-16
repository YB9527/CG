using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.Windows;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using stdole;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;
using ReflectManager;
using MyUtils;
using HeibernateManager.Model;
using ProgressTask;

namespace ArcGisManager
{
    public class ArcGisUtils
    {

        public static string MXDPath { get; set; }
        public static AxMapControl axMapControl { get; set; }
        public static AxPageLayoutControl axPageLayoutControl { get; set; }
        private static ArcGisUtils instance;
      

        public ArcGisUtils(AxMapControl axMapControl)
        {
            ArcGisUtils.axMapControl = axMapControl;

        }
        public static void Refresh()
        {
            axMapControl.LoadMxFile(MXDPath);
            axMapControl.ActiveView.Refresh();
        }

        public static IList<IFeature> GetFeatures(string layerName, string sql= "")
        {
            IFeatureCursor featureCursor = GetEntitys(sql, layerName);
            IList<IFeature> features = ArcGisUtils.CursorToList(featureCursor);
            return features;
        }

        public static IList<IFeature> GetFeatures(ILayer pLayer,string sql = "")
        {
            IFeatureCursor featureCursor = GetEntitys(sql,pLayer);
            IList<IFeature> features = ArcGisUtils.CursorToList(featureCursor);
            return features;
        }

        /// <summary>
        /// 检查图形 夹角小于度数的点
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static IList<IPoint> CheckAngle(IGeometry shape, double angle)
        {
            IList<IPoint> list = new List<IPoint>();
            IPointCollection pc = shape as IPointCollection;
            //pc.RemovePoints(pc.PointCount - 1, 1);
            pc.AddPoint(pc.Point[1]);
            int count = pc.PointCount;

            for (int a =1;a < count-1; a++)
            {
              double  dl = ArcGisUtils.Angle(pc.Point[a], pc.Point[a-1], pc.Point[a+1]);
                if(dl <= angle)
                {
                    list.Add(pc.Point[a]);
                }
            } 
            
            return list;
        }

        public static void ExtentShp(IFeature feature, ILayer pLayer, double scale=1)
        {
         
            IGeometry geometry = feature.Shape;
            
        
            //PositionFlashElement(geometry);
            if (geometry is IPoint)
            {
                axMapControl.Extent = geometry.Envelope;
            }
            else
            {
                if(scale == 1)
                {
                    axMapControl.Extent = geometry.Envelope;
                }
                else
                {
                    axMapControl.Extent = ArcGisUtils.ScaleFeatrue(geometry, scale).Envelope;
                  
                }
               
            }
            axMapControl.Map.ClearSelection();
            axMapControl.Map.SelectFeature( pLayer, feature);//第三个参数为是否只选中一个
            axMapControl.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null); //选中要素高亮显示
            axMapControl.ActiveView.Refresh();

        }

        private ArcGisUtils()
        {
            
        }

        /// <summary>
        /// 得到图层中所有feature
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public  static IList<IFeature> GetEntitysList(string sql, string layerName)
        {
            IFeatureCursor featureCursor = GetEntitys(sql,layerName);
            
            return FeaturesToList(featureCursor);
        }

       

        /// <summary>
        /// IFeatureCursor 转list
        /// </summary>
        /// <param name="featureCursor"></param>
        /// <returns></returns>
        public static IList<IFeature> FeaturesToList(IFeatureCursor featureCursor)
        {
            if (featureCursor == null)
            {
                return null;
            }
            IList<IFeature> list = new List<IFeature>();
            IFeature feature = featureCursor.NextFeature();
            while(feature != null)
            {
                list.Add(feature);
                feature = featureCursor.NextFeature();
            }
            return list;
        }
        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();
        public static void ChangeMapScale(MapDocumentClass mapDocument, double scaleValue)
        {

            IPageLayout pageLayout = mapDocument.PageLayout;
            IActiveView activeView = (IActiveView)pageLayout;
            IMap map = activeView.FocusMap;

            activeView = (IActiveView)mapDocument.PageLayout;
            activeView.Activate(GetDesktopWindow()); // 注意，调用了这个函数，才使得设置 MapScale 生效

            map.MapScale = scaleValue;
            activeView.Refresh();

            mapDocument.Save(true, true);
        }


        public static ArcGisUtils GetInstance()
        {
          if(instance == null)
            {
                instance = new ArcGisUtils();
            }
            return instance;
        }
        /// <summary>
        /// 删除图层
        /// </summary>
        public void DeleteLayer()
        {
            try
            {
                for (int i = axMapControl.LayerCount - 1; i > 0; i++)
                {
                    axMapControl.DeleteLayer(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除图层失败！！！" + ex.ToString());
            }
        }

 

        public void MoveLayer()
        {
            bool flag = axMapControl.LayerCount > 0;
            if (flag)
            {
                try
                {
                    axMapControl.MoveLayerTo(axMapControl.LayerCount - 1, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("移动图层失败！！！" + ex.ToString());
                }
            }
        }


        /// <summary>
        /// 添加地块
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="geometry"></param>
        public IFeatureBuffer AddFeature(string layerName, IGeometry geometry)
        {
            ILayer layer = null;
            for (int i = 0; i < axMapControl.LayerCount; i++)
            {
                layer = axMapControl.Map.get_Layer(i);
                bool flag = layer.Name == layerName;
                if (flag)
                {
                    break;
                }
            }
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IDataset dataset = (IDataset)featureClass;
            IWorkspace workspace = dataset.Workspace;
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            IFeatureCursor featureCursor = featureClass.Insert(true);
            featureBuffer.Shape = geometry;
            object obj = featureCursor.InsertFeature(featureBuffer);
            featureCursor.Flush();
            return featureBuffer;
        }

        /// <summary>
        /// 导出多个pdf
        /// </summary>
        /// <param name="mxdPaths"></param>
        public static void ExportPDFS(IList<string> mxdPaths)
        {
            List<MyAction> actions = new List<MyAction>();
            foreach(string mxdPath in mxdPaths)
            {
             Action action = new Action(() =>
             {
                 MapDocumentClass mapDocument = new MapDocumentClass();
                mapDocument.Open(mxdPath, "");
                ExportActiveViewParameterizedPDF(mapDocument.ActiveView, mxdPath.Replace("mxd", "pdf"));

             });
                MyAction myAction = new MyAction(action,mxdPath);
                actions.Add(myAction);
            }
            SingleTaskForm task = new SingleTaskForm(actions, "mxd转PDF ");
        }

        public void AddEntity()
        {
            IGeometryCollection geometryCollection = new MultipointClass();
            object missing = Type.Missing;
            for (int i = 0; i < 10; i++)
            {
                IPoint point = new PointClass();
                point.PutCoords((double)(i * 2), (double)(i * 2));
                geometryCollection.AddGeometry(point, ref missing, ref missing);
            }
            IMultipoint multipoint = geometryCollection as IMultipoint;
            this.AddFeature("multipoint", multipoint);
            axMapControl.Extent = multipoint.Envelope;
            axMapControl.Refresh();
        }

        public IFeature GetPolyLineFeature(string layerName)
        {
            ILayer layer = null;
            for (int i = 0; i < axMapControl.LayerCount; i++)
            {
                layer = axMapControl.Map.get_Layer(i);
                bool flag = layer.Name.ToLower() == layerName;
                if (flag)
                {
                    break;
                }
            }
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IFeatureCursor featureCursor = featureClass.Search(new QueryFilterClass
            {
                WhereClause = ""
            }, true);
            IFeature feature = featureCursor.NextFeature();
            bool flag2 = feature != null;
            IFeature result;
            if (flag2)
            {
                result = feature;
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 转换成list
        /// </summary>
        /// <param name="featureCursor"></param>
        /// <returns></returns>
        public static IList<IFeature> CursorToList(IFeatureCursor featureCursor)
        {
            IList<IFeature> list = new List<IFeature>();
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                list.Add(feature);
                feature = featureCursor.NextFeature();
            }
            return list;
        }

        /// <summary>
        /// 得到图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static  ILayer GetLayer(string layerName)
        {

            ILayer layer = null;
            for (int i = 0; i < axMapControl.LayerCount; i++)
            {
                layer = axMapControl.Map.get_Layer(i);
                
                bool flag = layer.Name.ToLower() == layerName;
                if (flag)
                {
                    break;
                }
            }
            return layer;
        }
        /// <summary>
        /// 得到图层
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayer(string layerName)
        {

            ILayer layer = null;
            for (int i = 0; i < axMapControl.LayerCount; i++)
            {
                layer = axMapControl.Map.get_Layer(i);
                bool flag = layer.Name == layerName;
                if (flag)
                {
                    break;
                }

            }
           
            return (IFeatureLayer)layer;
        }
        public static  IFeatureLayer GetFeatureLayer2(string layerName)
        {

            ILayer layer = null;
            for (int i = axMapControl.LayerCount-1; i >= 0; i--)
            {
                layer = axMapControl.Map.get_Layer(i);
                bool flag = layer.Name == layerName;
                if (flag)
                {
                    break;
                }

            }

            return (IFeatureLayer)layer;
        }
        public static Dictionary<string, IList<IFeature>> GetFeatureDicByLayer(IList<IFeature> features)
        {
           
            Dictionary<string, IList<IFeature>> dic = new Dictionary<string, IList<IFeature>>();
            IList<IFeature> save;
            foreach (IFeature feature in features)
            {
              string layerName =   feature.Class.AliasName;
               if(dic.TryGetValue(layerName ,out save))
                {
                    save.Add(feature);
                }
                else
                {
                    save = new List<IFeature>();
                    save.Add(feature);
                    dic.Add(layerName, save);
                }
            }
            return dic;
        }

        

        public static bool IsBei(IPoint point1, IPoint point2, IPoint point3)
        {
            double num = PointAngle(point1, point2);
            double num2 = PointAngle(point2, point3);
            double num3 = CalculateTwoPt(point1, point2) / CalculateTwoPt(point2, point3);
            double num4 = num2 - num;
            bool flag = num < 10.0;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                bool flag2 = num4 < 10.0;
                result = flag2;
            }
            return result;
        }


        public IList GetPointFeature(string layerName)
        {
            IFeatureCursor entitys = GetEntitys("", layerName);
            IList list = new ArrayList();
            IFeature feature = entitys.NextFeature();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = "";
            while (feature != null)
            {
                list.Add(feature);
                ITable table = feature.Table;
                for (int i = 0; i < table.RowCount(queryFilter); i++)
                {
                    IRow row = table.GetRow(i);
                    for (int j = 0; j < row.Fields.FieldCount; j++)
                    {
                        object obj = row.get_Value(j);
                        Console.WriteLine(obj.ToString());
                    }
                }
                feature = entitys.NextFeature();
            }
            return list;
        }

        public static  IFeatureCursor GetEntitys(string sql, string layerName)
        {
            ILayer layer = GetLayer(layerName);
            return GetEntitys(sql, layer);
        }

        public static IFeatureCursor GetEntitys(string sql, ILayer layer)
        {
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            return GetEntitysByFeatureLayer(sql, featureLayer);
        }

        private static IFeatureCursor GetEntitysByFeatureLayer(string sql, IFeatureLayer featureLayer)
        {
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IQueryFilter queryFilter = new QueryFilterClass();
            if(!Utils.IsStrNull(sql))
            {
                queryFilter.WhereClause = sql;
            }
          
            IFeatureCursor featureCursor = featureClass.Update(queryFilter, false);
           
            return featureCursor;
        }
        /// <summary>
        /// 图层转换为DataTable
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable CreateDataTableByLayer(ILayer pLayer, string tableName)
        {
            DataTable dataTable = new DataTable(tableName);
            ITable table = pLayer as ITable;
            for (int i = 0; i < table.Fields.FieldCount; i++)
            {
                IField field = table.Fields.get_Field(i);
                DataColumn dataColumn = new DataColumn(field.Name);
                bool flag = field.Name == table.OIDFieldName;
                if (flag)
                {
                    dataColumn.Unique = true;
                }
                dataColumn.AllowDBNull = field.IsNullable;
                dataColumn.Caption = field.AliasName;
                dataColumn.DataType = Type.GetType(ParseFieldType(field.Type));
                dataColumn.DefaultValue = field.DefaultValue;
                bool flag2 = field.VarType == 8;
                if (flag2)
                {
                    dataColumn.MaxLength = field.Length;
                }
                dataTable.Columns.Add(dataColumn);
            }
            return dataTable;
        }
        /// <summary>
        /// arcgis 字段类型 转换成C# 基本类型
        /// </summary>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static string ParseFieldType(esriFieldType fieldType)
        {
            string result;
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeSmallInteger:
                    result = "System.Int32";
                    break;
                case esriFieldType.esriFieldTypeInteger:
                    result = "System.Int32";
                    break;
                case esriFieldType.esriFieldTypeSingle:
                    result = "System.Single";
                    break;
                case esriFieldType.esriFieldTypeDouble:
                    result = "System.Double";
                    break;
                case esriFieldType.esriFieldTypeString:
                    result = "System.String";
                    break;
                case esriFieldType.esriFieldTypeDate:
                    result = "System.DateTime";
                    break;
                case esriFieldType.esriFieldTypeOID:
                    result = "System.String";
                    break;
                case esriFieldType.esriFieldTypeGeometry:
                    result = "System.String";
                    break;
                case esriFieldType.esriFieldTypeBlob:
                    result = "System.String";
                    break;
                case esriFieldType.esriFieldTypeRaster:
                    result = "System.String";
                    break;
                case esriFieldType.esriFieldTypeGUID:
                    result = "System.String";
                    break;
                case esriFieldType.esriFieldTypeGlobalID:
                    result = "System.String";
                    break;
                default:
                    result = "System.String";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 得到图层是 Point  Polyline Polygon字符串
        /// </summary>
        /// <param name="pLayer"></param>
        /// <returns></returns>
        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer featureLayer = (IFeatureLayer)pLayer;
            string result;
            switch (featureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    result = "Point";
                    return result;
                case esriGeometryType.esriGeometryPolyline:
                    result = "Polyline";
                    return result;
                case esriGeometryType.esriGeometryPolygon:
                    result = "Polygon";
                    return result;
            }
            result = "";
            return result;
        }




        public static IFeatureCursor GetIFeatureCursor(IFeatureLayer pFeatureLayer, string sql)
        {
            IFeatureClass featureClass = pFeatureLayer.FeatureClass;
            return featureClass.Update(new QueryFilterClass
            {
                WhereClause = sql
            }, false);
        }



        /// <summary>
        /// 开启编辑
        /// </summary>
        /// <param name="pFeatureLayer"></param>
        /// <returns></returns>
        public static IWorkspaceEdit StratEdit(IFeatureLayer pFeatureLayer)
        {
            IFeatureClass featureClass = pFeatureLayer.FeatureClass;
            return StratEdit(featureClass);
        }
        /// <summary>
        ///  开启编辑
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <returns></returns>
        public static IWorkspaceEdit StratEdit(IFeatureClass pFeatureClass)
        {
            IDataset dataset = (IDataset)pFeatureClass;
            IWorkspace workspace = dataset.Workspace;
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();
            return workspaceEdit;
        }
        /// <summary>
        /// 结束编辑
        /// </summary>
        /// <param name="workspaceEdit"></param>
        public static void EndEdit(IWorkspaceEdit workspaceEdit)
        {
            workspaceEdit.StopEditing(true);
            workspaceEdit.StopEditOperation();
        }



        public static IList<IFeature> GetSeartchFeatures(IFeatureLayer pFeatureLayer, IGeometry pGeometry, esriSpatialRelEnum spatial)
        {
            IList<IFeature> result;
            try
            {
                IList<IFeature> list = new List<IFeature>();
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                IQueryFilter queryFilter = spatialFilter;
                spatialFilter.Geometry = pGeometry;
                spatialFilter.SpatialRel = spatial;
                IFeatureCursor featureCursor = pFeatureLayer.Search(queryFilter, false);
                for (IFeature feature = featureCursor.NextFeature(); feature != null; feature = featureCursor.NextFeature())
                {
                    list.Add(feature);
                }
                Marshal.ReleaseComObject(featureCursor);
                result = list;
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }


        /// <summary>
        /// 创建多段线
        /// </summary>
        /// <param name="PolylineList"></param>
        /// <returns></returns>
        public static IPolyline CreatePolyline(IList<IPoint> PolylineList)
        {
            object missing = Type.Missing;
            ISegmentCollection segmentCollection = new PathClass();
            for (int i = 0; i < PolylineList.Count - 1; i++)
            {
                ILine line = CreateLine(PolylineList[i], PolylineList[i + 1]);
                ISegment inSegment = line as ISegment;
                segmentCollection.AddSegment(inSegment, ref missing, ref missing);
            }
            IGeometryCollection geometryCollection = new PolylineClass();
            geometryCollection.AddGeometry(segmentCollection as IGeometry, ref missing, ref missing);
            return geometryCollection as IPolyline;
        }
        /// <summary>
        /// 点创建线
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static ILine CreateLine(IPoint from, IPoint to)
        {
            ILine line = new LineClass();
            line.PutCoords(from, to);
            return line;
        }
        public static IPolygon CreatePolygon(IPointCollection points)
        {
            return CreatePolygon(CreatePolyline(points));
        }


       
        public static IPolyline PolygonToPolyline(IPolygon polygon)
        {
         
            ISegmentCollection segmentCollction = polygon as ISegmentCollection;
            ISegmentCollection polyline = new Polyline() as ISegmentCollection;
            object missing = Type.Missing;
            for (int m = 0; m < segmentCollction.SegmentCount; m++)
            {
                ISegment pSeg = segmentCollction.get_Segment(m);
                polyline.AddSegment(pSeg, ref missing, ref missing);
            }
           return polyline as IPolyline;
         
          
        }

        /// 点创建多段线线
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPolyline CreatePolyline(IPointCollection points)
        {
            IPointCollection pLinePc = new PolylineClass();//一定要设置为线类


            object missing = Type.Missing;
            ISegmentCollection segmentCollection = new PathClass();
            for (int i = 0; i < points.PointCount - 1; i++)
            {
                ILine line = CreateLine(points.get_Point(i), points.get_Point(i + 1));
                ISegment inSegment = line as ISegment;
                segmentCollection.AddSegment(inSegment, ref missing, ref missing);
            }
            IGeometryCollection geometryCollection = new PolylineClass();
            geometryCollection.AddGeometry(segmentCollection as IGeometry, ref missing, ref missing);
            return geometryCollection as IPolyline;
        }
        /// <summary>
        /// 点创建线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static IPolyline CreatePolyline(IPointCollection points, int count)
        {
            object missing = Type.Missing;
            ISegmentCollection segmentCollection = new PathClass();
            for (int i = 0; i < points.PointCount - 1; i++)
            {
                ILine line = CreateLine(points.get_Point(i), points.get_Point(i + 1));
                ISegment inSegment = line as ISegment;
                segmentCollection.AddSegment(inSegment, ref missing, ref missing);
            }
            IGeometryCollection geometryCollection = new PolylineClass();
            geometryCollection.AddGeometry(segmentCollection as IGeometry, ref missing, ref missing);
            return geometryCollection as IPolyline;
        }
        /// <summary>
        /// 线创建面
        /// </summary>
        /// <param name="pPolyline"></param>
        /// <returns></returns>
        public static IPolygon CreatePolygon(IPolyline pPolyline)
        {
            IGeometryCollection geometryCollection = new PolygonClass();
            bool flag = pPolyline != null && !pPolyline.IsEmpty;
            if (flag)
            {
                IGeometryCollection geometryCollection2 = pPolyline as IGeometryCollection;
                ISegmentCollection segmentCollection = new RingClass();
                object missing = Type.Missing;
                for (int i = 0; i < geometryCollection2.GeometryCount; i++)
                {
                    ISegmentCollection segmentCollection2 = geometryCollection2.get_Geometry(i) as ISegmentCollection;
                    for (int j = 0; j < segmentCollection2.SegmentCount; j++)
                    {
                        ISegment inSegment = segmentCollection2.get_Segment(j);
                        segmentCollection.AddSegment(inSegment, ref missing, ref missing);
                    }
                    geometryCollection.AddGeometry(segmentCollection as IGeometry, ref missing, ref missing);
                }
            }
            return geometryCollection as IPolygon;
        }

        public static  int[] GetFourPointsIndex(IPolygon polygon)
        {
            IPointCollection fourPoint =GetFourPoint(polygon);
            IPointCollection pointCollection = polygon as IPointCollection;
            int num = PolyDiPoly(pointCollection, fourPoint);
            int[] array = new int[4];
            bool flag = num > 3;
            if (flag)
            {
                int[] array2 = FindPointsDic(pointCollection);
                int[] array3 = FindPointsDic(pointCollection, fourPoint);
                int num2 = SelectPc1PC2(new int[][]
				{
					array2,
					array3
				}, pointCollection);
                bool flag2 = num2 == 0;
                if (flag2)
                {
                    array = array2;
                }
                else
                {
                    bool flag3 = num2 == 1;
                    if (flag3)
                    {
                        array = array3;
                    }
                }
            }
            else
            {
                array = FindPointsDic(pointCollection, fourPoint);
            }
            bool flag4 = array[0] == array[3];
            if (flag4)
            {
                array[3] = array[3] - 1;
            }
            return array;
        }

        private int[] FindAreaMax(IPointCollection points, int[] pointIndex1)
        {
            int[] array = this.CopyArray<int>(pointIndex1, 4);
            int[] array2 = this.CopyArray<int>(array, 4);
            double num = this.SelectPointsArea(points, array);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < points.PointCount; j++)
                {
                    array2[i] = j;
                    double num2 = this.SelectPointsArea(points, array2);
                    bool flag = num < num2;
                    if (flag)
                    {
                        array[i] = j;
                        num = num2;
                    }
                }
                array2 = this.CopyArray<int>(array, 4);
            }
            array = this.IfXmax(points, array);
            num = this.SelectPointsArea(points, array);
            return array;
        }

        public int[] IfXmax(IPointCollection points, int[] pointIndex)
        {
            bool flag = points.get_Point(pointIndex[0]).X < points.get_Point(pointIndex[1]).X;
            int[] result;
            if (flag)
            {
                result = pointIndex;
            }
            else
            {
                int[] array = new int[]
				{
					pointIndex[0],
					0,
					0,
					0,
					pointIndex[0]
				};
                array[1] = pointIndex[3];
                array[2] = pointIndex[2];
                array[3] = pointIndex[1];
                result = array;
            }
            return result;
        }

        private T[] CopyArray<T>(T[] array, int length)
        {
            T[] array2 = new T[length];
            for (int i = 0; i < length; i++)
            {
                array2[i] = array[i];
            }
            return array2;
        }
        /// <summary>
        /// 集合复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public T[] CopyArray<T>(T[] array)
        {
            int num = array.Length;
            T[] array2 = new T[num];
            for (int i = 0; i < num; i++)
            {
                array2[i] = array[i];
            }
            return array2;
        }

        public double SelectPointsArea(IPointCollection points, int[] pointIndex)
        {
            IPointCollection pointCollection = new PolygonClass();
            for (int i = 0; i < pointIndex.Length; i++)
            {
                int i2 = pointIndex[i];
                IPointCollection arg_2D_0 = pointCollection;
                IPoint arg_2D_1 = points.get_Point(i2);
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                arg_2D_0.AddPoint(arg_2D_1, ref missing, ref missing2);
            }
            return PointsArea(pointCollection);
        }

        internal int[] GetMaxArea(IPointCollection points, int[] pointIndex, int[] pointIndex2)
        {
            double num = this.SelectPointsArea(points, pointIndex);
            double num2 = this.SelectPointsArea(points, pointIndex2);
            double num3 = num - num2;
            bool flag = Math.Abs(num3) / num < 0.3 || num3 > 0.0;
            int[] result;
            if (flag)
            {
                result = pointIndex;
            }
            else
            {
                pointIndex = new int[]
				{
					pointIndex2[0],
					0,
					0,
					0,
					pointIndex2[0]
				};
                pointIndex[1] = pointIndex2[1];
                pointIndex[2] = pointIndex2[2];
                pointIndex[3] = pointIndex2[3];
                result = pointIndex;
            }
            return result;
        }

        public static int SelectPc1PC2(int[][] max, IPointCollection pc)
        {
            IPointCollection pc2 = SelectPoints(max[0], pc);
            IPointCollection pc3 = SelectPoints(max[1], pc);
            double num = PointsArea(pc2);
            double num2 = PointsArea(pc3);
            double num3 = Math.Round(num - num2,2);
            if(num3 ==0)
            {
                return 1;
            }
            bool flag = Math.Abs(num3) / num < 0.04 || num3 > 0.0;
            int result;
            if (flag)
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return result;
        }

        private static IPointCollection SelectPoints(int[] max, IPointCollection pc)
        {
            IPointCollection pointCollection = new PolygonClass();
            for (int i = 0; i < max.Length; i++)
            {
                IPointCollection arg_26_0 = pointCollection;
                IPoint arg_26_1 = pc.get_Point(max[i]);
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                arg_26_0.AddPoint(arg_26_1, ref missing, ref missing2);
            }
            return pointCollection;
        }
        /// <summary>
        /// 点集合换算成面积
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public static double PointsArea(IPointCollection pc)
        {
            IPolyline pPolyline = CreatePolyline(pc);
            IPolygon polygon = CreatePolygon(pPolyline);
            polygon.Close();
            IArea area = polygon as IArea;
            return area.Area;
        }
        /// <summary>
        /// 最小点的坐标
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pointsLength"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static  int GetPointMinIndex(IPointCollection points, int pointsLength, IPoint point)
        {
            double num = CalculateTwoPt(points.get_Point(0), point);
            int result = 0;
            for (int i = 1; i < pointsLength; i++)
            {
                double num2 = CalculateTwoPt(points.get_Point(i), point);
                bool flag = num > num2;
                if (flag)
                {
                    num = num2;
                    result = i;
                }
            }
            return result;
        }

        private static int[] FindPointsDic(IPointCollection points, IPointCollection fourPoints)
        {
            int[] array = new int[5];
            int pointCount = points.PointCount;
            array[0] = GetPointMinIndex(points, pointCount, fourPoints.get_Point(0));
            array[4] = array[0];
            array[1] = GetPointMinIndex(points, pointCount, fourPoints.get_Point(1));
            array[2] = GetPointMinIndex(points, pointCount, fourPoints.get_Point(2));
            array[3] = GetPointMinIndex(points, pointCount, fourPoints.get_Point(3));
            return array;
        }

        public static int[] FindPointsDic(IPointCollection points)
        {
            int[] array = new int[5];
            double[] array2 = new double[5];
            array2[0] = points.get_Point(0).X;
            array2[1] = points.get_Point(0).X;
            array2[2] = points.get_Point(0).Y;
            array2[3] = points.get_Point(0).Y;
            for (int i = 1; i < points.PointCount; i++)
            {
                bool flag = array2[0] >= points.get_Point(i).X;
                if (flag)
                {
                    array2[0] = points.get_Point(i).X;
                    array[3] = i;
                }
            }
            for (int j = 1; j < points.PointCount; j++)
            {
                bool flag2 = array2[1] < points.get_Point(j).X;
                if (flag2)
                {
                    array2[1] = points.get_Point(j).X;
                    array[1] = j;
                }
            }
            for (int k = 0; k < points.PointCount; k++)
            {
                bool flag3 = points.get_Point(k).Y > array2[2];
                if (flag3)
                {
                    array2[2] = points.get_Point(k).Y;
                    array[0] = k;
                    array[4] = k;
                }
            }
            for (int l = 0; l < points.PointCount; l++)
            {
                bool flag4 = array2[3] > points.get_Point(l).Y;
                if (flag4)
                {
                    array2[3] = points.get_Point(l).Y;
                    array[2] = l;
                }
            }
            return array;
        }

        private static  IPointCollection GetFourPoint(IPolygon polygon)
        {
            IEnvelope envelope = polygon.Envelope;
            IPointCollection pointCollection = new PolygonClass();
            IPointCollection arg_25_0 = pointCollection;
            IPoint arg_25_1 = envelope.UpperLeft;
            object missing = Type.Missing;
            object missing2 = Type.Missing;
            arg_25_0.AddPoint(arg_25_1, ref missing, ref missing2);
            IPointCollection arg_42_0 = pointCollection;
            IPoint arg_42_1 = envelope.UpperRight;
            missing2 = Type.Missing;
            missing = Type.Missing;
            arg_42_0.AddPoint(arg_42_1, ref missing2, ref missing);
            IPointCollection arg_5F_0 = pointCollection;
            IPoint arg_5F_1 = envelope.LowerRight;
            missing = Type.Missing;
            missing2 = Type.Missing;
            arg_5F_0.AddPoint(arg_5F_1, ref missing, ref missing2);
            IPointCollection arg_7C_0 = pointCollection;
            IPoint arg_7C_1 = envelope.LowerLeft;
            missing2 = Type.Missing;
            missing = Type.Missing;
            arg_7C_0.AddPoint(arg_7C_1, ref missing2, ref missing);
            return pointCollection;
        }
        /// <summary>
        /// 线之间最近的距离
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="pc2"></param>
        /// <returns></returns>
        public static int PolyDiPoly(IPointCollection pc, IPointCollection pc2)
        {
            int num = 0;
            for (int i = 0; i < pc2.PointCount; i++)
            {
                bool flag = i == pc2.PointCount - 1;
                double num2;
                if (flag)
                {
                    num2 = PolyDistance(pc, pc2.get_Point(i), pc2.get_Point(0));
                }
                else
                {
                    num2 = PolyDistance(pc, pc2.get_Point(i), pc2.get_Point(i + 1));
                }
                bool flag2 = num2 < 0.02;
                if (flag2)
                {
                    num++;
                }
            }
            return num;
        }

        private static double PolyDistance(IPointCollection pc, IPoint startPont, IPoint endPoint)
        {
            double num = 100000.0;
            for (int i = 0; i < pc.PointCount; i++)
            {
                IPoint point = pc.get_Point(i);
                double num2 = CalculateDi(point, startPont, endPoint);
                bool flag = num > num2;
                if (flag)
                {
                    num = num2;
                }
            }
            return num;
        }

        private static double CalculateDi(IPoint point, IPoint startPoint, IPoint endPoint)
        {
            double num = CalculateTwoPt(startPoint, endPoint);
            double num2 = CalculateTwoPt(endPoint, point);
            double num3 = CalculateTwoPt(point, startPoint);
            double num4 = 0.5 * (num + num2 + num3);
            double num5 = Math.Sqrt(num4 * (num4 - num) * (num4 - num2) * (num4 - num3));
            return 2.0 * num5 / num3;
        }

        public static double CalculateTwoPt(IPoint pt1, IPoint pt2)
        {
            double num = pt1.X - pt2.X;
            double num2 = pt1.Y - pt2.Y;
            return Math.Sqrt(num * num + num2 * num2);
        }
        /// <summary>
        /// 得到两个点的中点
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static IPoint GetZongDian(IPoint pt1, IPoint pt2)
        {
            double num = pt1.X + pt2.X;
            double num2 = pt1.Y + pt2.Y;
            IPoint pt = new PointClass();
            pt.PutCoords(num / 2, num2 / 2);
            return pt;
        }



        internal IPointCollection[] GetFourPoints(IPointCollection pointCollection, int[] array)
        {
            IPointCollection[] array2 = new Polygon[4];
            IPointCollection[] array3 = array2;
            array3[0] = this.SelectPoints(pointCollection, array, 0);
            array3[1] = this.SelectPoints(pointCollection, array, 1);
            array3[2] = this.SelectPoints(pointCollection, array, 2);
            array3[3] = this.SelectPoints(pointCollection, array, 3);
            return array3;
        }

        public IPointCollection SelectPoints(IPointCollection pointCollection, int[] array, int startIndex)
        {
            IPointCollection pointCollection2 = new PolygonClass();
            int num = array[startIndex];
            int num2 = array[startIndex + 1];
            bool flag = num > num2;
            if (flag)
            {
                pointCollection2 = this.SelectPoints(pointCollection, num, pointCollection.PointCount);
                pointCollection2.AddPointCollection(this.SelectPoints(pointCollection, 0, num2 + 1));
            }
            else
            {
                pointCollection2.AddPointCollection(this.SelectPoints(pointCollection, num, num2 + 1));
            }
            return pointCollection2;
        }
        /// <summary>
        /// 选择点集合 从startIndex 到 endIndex
        /// </summary>
        /// <param name="pointCollection"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public IPointCollection SelectPoints(IPointCollection pointCollection, int startIndex, int endIndex)
        {
            IPointCollection pointCollection2 = new PolygonClass();
            for (int i = startIndex; i < endIndex; i++)
            {
                IPointCollection arg_24_0 = pointCollection2;
                IPoint arg_24_1 = pointCollection.get_Point(i);
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                arg_24_0.AddPoint(arg_24_1, ref missing, ref missing2);
            }
            return pointCollection2;
        }
        /// <summary>
        /// 两点角度
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double PointAngle(IPoint startPoint, IPoint endPoint)
        {
            double x = startPoint.X;
            double y = startPoint.Y;
            double x2 = endPoint.X;
            double y2 = endPoint.Y;
            double num = x2 - x;
            double num2 = y2 - y;
            double value = num2 / num;
            double num3 = Math.Tanh(Math.Abs(value));
            return num3 * 180.0 / 3.1415926535897931;
        }
        /// <summary>
        /// 创建偏移的点
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="offsetDi"></param>
        /// <param name="scale"></param>
        /// <param name="basicPoint"></param>
        /// <param name="scRealy"></param>
        /// <returns></returns>
        public IPolygon PlCreateOffsetPolygon(IPointCollection pc, double offsetDi, double scale, IPoint basicPoint, double scRealy)
        {
            IPointCollection pointCollection = new PolygonClass();
            pointCollection.AddPointCollection(pc);
            int pointCount = pc.PointCount;
            IPointCollection arg_46_0 = pointCollection;
            IPoint arg_46_1 = this.PointOffset(pc.get_Point(pointCount - 1), pc.get_Point(pointCount - 2), esriSegmentExtension.esriExtendAtTo, -offsetDi, scale);
            object missing = Type.Missing;
            object missing2 = Type.Missing;
            arg_46_0.AddPoint(arg_46_1, ref missing, ref missing2);
            for (int i = pointCount - 1; i > 0; i--)
            {
                IPointCollection arg_82_0 = pointCollection;
                IPoint arg_82_1 = this.PointOffset(pc.get_Point(i), pc.get_Point(i - 1), esriSegmentExtension.esriExtendAtFrom, offsetDi, scale);
                missing2 = Type.Missing;
                missing = Type.Missing;
                arg_82_0.AddPoint(arg_82_1, ref missing2, ref missing);
            }
            pointCollection = ScalePoints(pointCollection, basicPoint, scRealy);
            pointCollection = this.ReservPoint(pointCollection);
            IPolyline pPolyline = CreatePolyline(pointCollection);
            IPolygon polygon = CreatePolygon(pPolyline);
            polygon.Close();
            return polygon;
        }
        /// <summary>
        /// 集合反转
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public IPointCollection ReservPoint(IPointCollection pc)
        {
            IPointCollection pointCollection = new PolygonClass();
            for (int i = pc.PointCount - 1; i >= 0; i--)
            {
                IPointCollection arg_2B_0 = pointCollection;
                IPoint arg_2B_1 = pc.get_Point(i);
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                arg_2B_0.AddPoint(arg_2B_1, ref missing, ref missing2);
            }
            return pointCollection;
        }
        /// <summary>
        /// 得到点集合
        /// </summary>
        /// <param name="points"></param>
        /// <param name="basicPoint"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static IPointCollection ScalePoints(IPointCollection points, IPoint basicPoint, double scale)
        {
            IPoint point = new PointClass();
            IPointCollection pointCollection = new PolygonClass();
            double x = basicPoint.X;
            double y = basicPoint.Y;
            IPointCollection result;
            for (int i = 0; i < points.PointCount; i++)
            {
                try
                {
                    double num = points.get_Point(i).X - x;
                    double num2 = points.get_Point(i).Y - y;
                    point.X = num * scale + x;
                    point.Y = num2 * scale + y;
                    IPointCollection arg_72_0 = pointCollection;
                    IPoint arg_72_1 = point;
                    object missing = Type.Missing;
                    object missing2 = Type.Missing;
                    arg_72_0.AddPoint(arg_72_1, ref missing, ref missing2);
                }
                catch
                {
                    result = points;
                    return result;
                }
            }
            result = pointCollection;
            return result;
        }
        /// <summary>
        /// 得到中点
        /// </summary>
        /// <param name="onePoint"></param>
        /// <param name="twoPoint"></param>
        /// <returns></returns>
        public IPoint GetZpoint(IPoint onePoint, IPoint twoPoint)
        {
            return new PointClass
            {
                X = (onePoint.X + twoPoint.X) / 2.0,
                Y = (onePoint.Y + twoPoint.Y) / 2.0
            };
        }

        private IPoint PointOffset(IPoint pt1, IPoint pt2, esriSegmentExtension e, double distace, double direction)
        {
            IConstructPoint constructPoint = new PointClass();
            IPolyline curve = CreatePolyline(new List<IPoint>
			{
				pt1,
				pt2
			});
            constructPoint.ConstructOffset(curve, e, distace, true, direction);
            return constructPoint as IPoint;
        }

        internal IList<IFeature> GetSeartchGeometry(IGeometry geometry, IFeatureLayer pFeatureLayer, double compareArea)
        {
            IList<IFeature> seartchFeatures = GetSeartchFeatures(pFeatureLayer, geometry, esriSpatialRelEnum.esriSpatialRelIntersects);
            IList<IFeature> list = new List<IFeature>();
            foreach (IFeature current in seartchFeatures)
            {
                IGeometry geometry2 = GeometryClip(geometry, current.ShapeCopy);
                bool flag = this.CalculateGeometryArea(geometry2) > compareArea;
                if (flag)
                {
                    list.Add(current);
                }
            }
            return list;
        }


        private int FeatureOverlayAreaShort(IList<IGeometry> geometryList, IList<IFeature> fs)
        {
            IList<IFeature> list = new List<IFeature>();
            double num = this.CalculateGeometryArea(geometryList[0]);
            int num2 = 0;
            for (int i = 1; i < geometryList.Count; i++)
            {
                double num3 = this.CalculateGeometryArea(geometryList[i]);
                bool flag = num3 > num;
                if (flag)
                {
                    num = num3;
                    num2 = i;
                }
            }
            bool flag2 = num < 0.05;
            int result;
            if (flag2)
            {
                result = -1;
            }
            else
            {
                result = num2;
            }
            return result;
        }
        /// <summary>
        /// 计算图形面积
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public double CalculateGeometryArea(IGeometry geometry)
        {
            IPointCollection pc = geometry as IPointCollection;
            return PointsArea(pc);
        }

        private IList<IGeometry> GeometryClips(IList<IFeature> fs, IPolygon po)
        {
            return GeometryClips(fs, po);
        }

        public static IList<IGeometry> GeometryClips(IList<IFeature> fs, IGeometry geometry)
        {
            IList<IGeometry> list = new List<IGeometry>();
            foreach (IFeature current in fs)
            {
                list.Add(GeometryClip(current.ShapeCopy, geometry));
            }
            return list;
        }

        public static IList<IGeometry> GeometryClips(IGeometry geometry, IList<IFeature> fs)
        {
            IList<IGeometry> list = new List<IGeometry>();
            foreach (IFeature current in fs)
            {
                list.Add(GeometryClip(geometry, current.ShapeCopy));
            }
            return list;
        }
        /// <summary>
        /// 面裁剪
        /// </summary>
        /// <param name="sourceGeometry">被裁剪的</param>
        /// <param name="clipGeometry"></param>
        /// <returns></returns>
        public static IGeometry GeometryClip(IGeometry sourceGeometry, IGeometry clipGeometry)
        {
            bool flag = sourceGeometry.SpatialReference != clipGeometry.SpatialReference;
            if (flag)
            {
                clipGeometry.Project(sourceGeometry.SpatialReference);
            }
            esriGeometryType geometryType = sourceGeometry.GeometryType;
            IGeometry result;
            if (geometryType != esriGeometryType.esriGeometryPolyline)
            {
                if (geometryType != esriGeometryType.esriGeometryPolygon)
                {
                    result = sourceGeometry;
                }
                else
                {
                    ITopologicalOperator2 topologicalOperator = sourceGeometry as ITopologicalOperator2;
                    topologicalOperator.IsKnownSimple_2 = true;
                    topologicalOperator.Simplify();
                    result = topologicalOperator.Difference(topologicalOperator.Difference(clipGeometry));
                }
            }
            else
            {
                ITopologicalOperator2 topologicalOperator = sourceGeometry as ITopologicalOperator2;
                topologicalOperator.IsKnownSimple_2 = true;
                topologicalOperator.Simplify();
                result = topologicalOperator.Intersect(topologicalOperator.Intersect(clipGeometry, esriGeometryDimension.esriGeometry1Dimension), esriGeometryDimension.esriGeometry1Dimension);
            }
            return result;
        }
        /// <summary>
        /// 得到圆心
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPoint GetPolyCore(IPointCollection points)
        {
            double num = 0.0;
            double num2 = 0.0;
            int pointCount = points.PointCount;
            for (int i = 0; i < pointCount; i++)
            {
                num += points.get_Point(i).X;
                num2 += points.get_Point(i).Y;
            }
            return new PointClass
            {
                X = num / (double)pointCount,
                Y = num2 / (double)pointCount
            };
        }
        /// <summary>
        /// 离此点最近的点
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static IPoint GetDiMin(IPointCollection pc, IPoint pt)
        {
            IPoint point = pc.get_Point(0);
            for (int i = 1; i < pc.PointCount; i++)
            {
                IPoint point2 = pc.get_Point(i);
                bool flag = CalculateTwoPt(point, pt) > CalculateTwoPt(point2, pt);
                if (flag)
                {
                    point = point2;
                }
            }
            return point;
        }
        /// <summary>
        /// 离圆心最近的点
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public IPoint GetDiMinCore(IPointCollection pc)
        {
            int pointCount = pc.PointCount;
            IPoint polyCore = GetPolyCore(pc);
            IPoint diMaxPl = this.GetDiMaxPl(pc, pointCount);
            bool flag = diMaxPl == null;
            IPoint result;
            if (flag)
            {
                result = polyCore;
            }
            else
            {
                IPoint point = pc.get_Point(0);
                IPoint point2 = pc.get_Point(pointCount - 1);
                IPoint diMin = GetDiMin(pc, polyCore);
                double num = CalculateDi(diMin, pc.get_Point(pointCount - 1), pc.get_Point(0));
                double num2 = CalculateDi(diMaxPl, pc.get_Point(pointCount - 1), pc.get_Point(0));
                bool flag2 = num > num2;
                if (flag2)
                {
                    result = diMin;
                }
                else
                {
                    result = diMaxPl;
                }
            }
            return result;
        }

        internal IPoint GetDiMaxPl(IPointCollection pc, int length)
        {
            IPoint pt = pc.get_Point(0);
            IPoint point = pc.get_Point(length - 1);
            double num = 0.0;
            IPoint result = null;
            bool flag = length > 2;
            if (flag)
            {
                for (int i = 1; i < length - 2; i++)
                {
                    IPoint point2 = pc.get_Point(i);
                    double num2 = CalculateTwoPt(point2, pt) + CalculateTwoPt(point2, pt);
                    bool flag2 = num2 > num;
                    if (flag2)
                    {
                        num = num2;
                        result = point2;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 三点形成角度
        /// </summary>
        /// <param name="cen"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static double Angle(IPoint cen, IPoint first, IPoint second)
        {
            double num = first.X - cen.X;
            double num2 = first.Y - cen.Y;
            double num3 = second.X - cen.X;
            double num4 = second.Y - cen.Y;
            double num5 = num * num3 + num2 * num4;
            double num6 = Math.Sqrt(num * num + num2 * num2);
            double num7 = Math.Sqrt(num3 * num3 + num4 * num4);
            double d = num5 / (num6 * num7);
            return Math.Acos(d) * 180.0 / 3.1415926535897;
        }
        /// <summary>
        /// 图形面积
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public static double AreaFeature(IFeature feature)
        {
            IPolygon polygon = feature.Shape as IPolygon;
            return (polygon as IArea).Area;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enveplope"></param>
        /// <returns></returns>
        internal static IPoint GetEnvelopeCore(IEnvelope enveplope)
        {
            IPointCollection pointCollection = new PolygonClass();
            IPointCollection arg_1E_0 = pointCollection;
            IPoint arg_1E_1 = enveplope.UpperLeft;
            object missing = Type.Missing;
            object missing2 = Type.Missing;
            arg_1E_0.AddPoint(arg_1E_1, ref missing, ref missing2);
            IPointCollection arg_3B_0 = pointCollection;
            IPoint arg_3B_1 = enveplope.UpperRight;
            missing2 = Type.Missing;
            missing = Type.Missing;
            arg_3B_0.AddPoint(arg_3B_1, ref missing2, ref missing);
            IPointCollection arg_58_0 = pointCollection;
            IPoint arg_58_1 = enveplope.LowerRight;
            missing = Type.Missing;
            missing2 = Type.Missing;
            arg_58_0.AddPoint(arg_58_1, ref missing, ref missing2);
            IPointCollection arg_75_0 = pointCollection;
            IPoint arg_75_1 = enveplope.LowerLeft;
            missing2 = Type.Missing;
            missing = Type.Missing;
            arg_75_0.AddPoint(arg_75_1, ref missing2, ref missing);
            return GetPolyCore(pointCollection);
        }
        /// <summary>
        /// 图形缩放
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static IPolygon ScaleFeatrue(IGeometry geometry, double scale)
        {
            IPointCollection pointCollection = geometry as IPointCollection;
            IPoint polyCore = GetPolyCore(pointCollection);
            double x = polyCore.X;
            double y = polyCore.Y;
            IPointCollection pointCollection2 = new PolygonClass();
            for (int i = 0; i < pointCollection.PointCount; i++)
            {
                IPoint point = pointCollection.get_Point(i);
                double num = point.X - x;
                double num2 = point.Y - y;
                point = new PointClass();
                point.X = x + num * scale;
                point.Y = y + num2 * scale;
                IPointCollection arg_83_0 = pointCollection2;
                IPoint arg_83_1 = point;
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                arg_83_0.AddPoint(arg_83_1, ref missing, ref missing2);
            }
            return CreatePolygon(CreatePolyline(pointCollection2));
        }

        internal static IPolygon ScaleFeatrue(IPointCollection pc, double scale)
        {
            IPoint polyCore = GetPolyCore(pc);
            return ScaleFeatrue(pc, polyCore, scale);
        }
        public static IPolygon ScaleEnvelope(IEnvelope envelope,double scale)
        {
           return  ScaleFeatrue(GetEnvelopePc(envelope), scale);
        }
            /// <summary>
            /// 图形 外切矩形点集合
            /// </summary>
            /// <param name="envelope"></param>
            /// <returns></returns>
            public static IPointCollection GetEnvelopePc(IEnvelope envelope)
        {
            IPointCollection pointCollection = new PolygonClass();
            IPointCollection arg_1E_0 = pointCollection;
            IPoint arg_1E_1 = envelope.UpperLeft;
            object missing = Type.Missing;
            object missing2 = Type.Missing;
            arg_1E_0.AddPoint(arg_1E_1, ref missing, ref missing2);
            IPointCollection arg_3B_0 = pointCollection;
            IPoint arg_3B_1 = envelope.UpperRight;
            missing2 = Type.Missing;
            missing = Type.Missing;
            arg_3B_0.AddPoint(arg_3B_1, ref missing2, ref missing);
            IPointCollection arg_58_0 = pointCollection;
            IPoint arg_58_1 = envelope.LowerRight;
            missing = Type.Missing;
            missing2 = Type.Missing;
            arg_58_0.AddPoint(arg_58_1, ref missing, ref missing2);
            IPointCollection arg_75_0 = pointCollection;
            IPoint arg_75_1 = envelope.LowerLeft;
            missing2 = Type.Missing;
            missing = Type.Missing;
            arg_75_0.AddPoint(arg_75_1, ref missing2, ref missing);
            IPointCollection arg_92_0 = pointCollection;
            IPoint arg_92_1 = envelope.UpperLeft;
            missing = Type.Missing;
            missing2 = Type.Missing;
            arg_92_0.AddPoint(arg_92_1, ref missing, ref missing2);
            return pointCollection;
        }
        /// <summary>
        /// 图形缩放
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="core"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static IPolygon ScaleFeatrue(IPointCollection pc, IPoint core, double scale)
        {
            double x = core.X;
            double y = core.Y;
            IPointCollection pointCollection = new PolygonClass();
            for (int i = 0; i < pc.PointCount; i++)
            {
                IPoint point = pc.get_Point(i);
                double num = point.X - x;
                double num2 = point.Y - y;
                point = new PointClass();
                point.X = x + num * scale;
                point.Y = y + num2 * scale;
                IPointCollection arg_73_0 = pointCollection;
                IPoint arg_73_1 = point;
                object missing = Type.Missing;
                object missing2 = Type.Missing;
                arg_73_0.AddPoint(arg_73_1, ref missing, ref missing2);
            }
            return CreatePolygon(CreatePolyline(pointCollection));
        }

        internal static double[] AddScale(double x, double y, IPoint core, double scale)
        {
            double[] array = new double[2];
            double x2 = core.X;
            double y2 = core.Y;
            IPoint point = new PointClass();
            bool flag = x > x2;
            if (flag)
            {
                array[0] = x + scale * 1.5;
            }
            else
            {
                array[0] = x - scale * 1.5;
            }
            bool flag2 = y > y2;
            if (flag2)
            {
                array[1] = y + scale;
            }
            else
            {
                array[1] = y - scale;
            }
            return array;
        }

        public static double isLeft(IPoint P0, IPoint P1, IPoint P2)
        {
            return (P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y);
        }

        private static bool PointInFences(IPoint pnt1, IPointCollection pc)
        {
            int pointCount = pc.PointCount;
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < pointCount; i++)
            {
                bool flag = i == pointCount - 1;
                if (flag)
                {
                    num2 = 0;
                }
                else
                {
                    num2++;
                }
                bool flag2 = pc.get_Point(i).Y <= pnt1.Y;
                if (flag2)
                {
                    bool flag3 = pc.get_Point(num2).Y > pnt1.Y;
                    if (flag3)
                    {
                        bool flag4 = isLeft(pc.get_Point(i), pc.get_Point(num2), pnt1) > 0.0;
                        if (flag4)
                        {
                            num++;
                        }
                    }
                }
                else
                {
                    bool flag5 = pc.get_Point(num2).Y <= pnt1.Y;
                    if (flag5)
                    {
                        bool flag6 = isLeft(pc.get_Point(i), pc.get_Point(num2), pnt1) < 0.0;
                        if (flag6)
                        {
                            num--;
                        }
                    }
                }
            }
            bool flag7 = num == 0;
            return !flag7;
        }

        internal static void PointInFences(IPointCollection pc, IPointCollection comPC)
        {
            int num = pc.PointCount;
            comPC.RemovePoints(4, 1);
            for (int i = 0; i < num; i++)
            {
                bool flag = !PointInFences(pc.get_Point(i), comPC);
                if (flag)
                {
                    pc.RemovePoints(i, 1);
                    i--;
                    num--;
                }
            }
        }

        internal static IList<double> GeometryToZbs(IGeometry geometry)
        {
            IList<double> list = new List<double>();
            IPointCollection pointCollection = geometry as IPointCollection;
            for (int i = 0; i < pointCollection.PointCount; i++)
            {
                IPoint point = pointCollection.get_Point(i);
                list.Add(Math.Round(point.X, 2));
                list.Add(Math.Round(point.Y, 2));
            }
            return list;
        }

        internal static IPointCollection RemoveZbs(IPointCollection pc, IList<double> comZbs)
        {
            int pointCount = pc.PointCount;
            int i = 0;
            IList<int> list = new List<int>();
            while (i < pointCount)
            {
                IPoint point = pc.get_Point(i);
                double item = Math.Round(point.X, 2);
                double item2 = Math.Round(point.Y, 2);
                bool flag = comZbs.Contains(item) || comZbs.Contains(item2);
                if (flag)
                {
                    list.Add(i);
                    RemoveZbs(pc, list, i + 1, pointCount, comZbs);
                    break;
                }
                i++;
            }
            return RemoveZbs(pc, list);
        }

        private static IPointCollection RemoveZbs(IPointCollection pc, IList<int> indexs)
        {
            int count = indexs.Count;
            bool flag = count == 0;
            IPointCollection result;
            if (flag)
            {
                result = pc;
            }
            else
            {
                int i;
                for (i = 0; i < count - 1; i++)
                {
                    bool flag2 = indexs[i] + 1 < indexs[i + 1];
                    if (flag2)
                    {
                        break;
                    }
                }
                bool flag3 = i == count - 1;
                if (flag3)
                {
                    pc = Regroup(pc, indexs[count - 1], indexs[0]);
                }
                else
                {
                    pc = Regroup(pc, indexs[i], indexs[i + 1]);
                }
                result = pc;
            }
            return result;
        }

        private static IPointCollection Regroup(IPointCollection pc, int start, int end)
        {
            IPointCollection pointCollection = new PolygonClass();
            bool flag = end > start;
            if (flag)
            {
                for (int i = start; i <= end; i++)
                {
                    IPointCollection arg_2E_0 = pointCollection;
                    IPoint arg_2E_1 = pc.get_Point(i);
                    object missing = Type.Missing;
                    object missing2 = Type.Missing;
                    arg_2E_0.AddPoint(arg_2E_1, ref missing, ref missing2);
                }
            }
            else
            {
                for (int j = start; j < pc.PointCount; j++)
                {
                    IPointCollection arg_6A_0 = pointCollection;
                    IPoint arg_6A_1 = pc.get_Point(j);
                    object missing2 = Type.Missing;
                    object missing = Type.Missing;
                    arg_6A_0.AddPoint(arg_6A_1, ref missing2, ref missing);
                }
                for (int k = 1; k <= end; k++)
                {
                    IPointCollection arg_A7_0 = pointCollection;
                    IPoint arg_A7_1 = pc.get_Point(k);
                    object missing = Type.Missing;
                    object missing2 = Type.Missing;
                    arg_A7_0.AddPoint(arg_A7_1, ref missing, ref missing2);
                }
            }
            return pointCollection;
        }

        private static void RemoveZbs(IPointCollection pc, IList<int> indexs, int start, int end, IList<double> comZbs)
        {
            while (start < end)
            {
                IPoint point = pc.get_Point(start);
                double item = Math.Round(point.X, 2);
                double item2 = Math.Round(point.Y, 2);
                bool flag = comZbs.Contains(item) || comZbs.Contains(item2);
                if (flag)
                {
                    indexs.Add(start);
                }
                start++;
            }
        }

        public static ITextSymbol CreateTextSymbol(int size = 12, string font = "宋体")
        {
            Font font2 = new StdFontClass();
            font2.Name = font;
            return new TextSymbolClass
            {
                Font = font2 as IFontDisp,
                Size = (double)size
            };
        }

        internal static void AddScale(IPoint point, IPoint core, double scale)
        {
            double x = core.X;
            double y = core.Y;
            IPoint point2 = new PointClass();
            double x2 = point.X;
            double y2 = point.Y;
            bool flag = x2 > x;
            if (flag)
            {
                point.X = x2 + scale * 1.5;
            }
            else
            {
                point.X = x2 - scale * 1.5;
            }
            bool flag2 = y2 > y;
            if (flag2)
            {
                point.Y = y2 + scale;
            }
            else
            {
                point.Y = y2 - scale;
            }
        }

        /// <summary>
        /// 设置 数据框区域 结果未保存
        /// </summary>
        /// <param name="element"></param>
        /// <param name="weight">增减量</param>
        /// <param name="Height">增减量</param>
        public static void SetAddArea(IElement element, double weight, double Height)
        {
            IGeometry geometry = element.Geometry;
            IPointCollection pc = geometry as IPointCollection;
            IPoint pt1 = pc.Point[0];
            IPoint pt2 = pc.Point[1];
            IPoint pt3 = pc.Point[2];
            IPoint pt4 = pc.Point[3];
            IPoint pt5 = pc.Point[4];


            pc = new PolygonClass();
            PointClass pt = new PointClass();
            pt.PutCoords(pt1.X, pt1.Y - Height);
            pc.AddPoint(pt);
            pt = new PointClass();
            pt.PutCoords(pt2.X, pt2.Y);
            pc.AddPoint(pt);
            pt = new PointClass();
            pt.PutCoords(pt3.X, pt3.Y);
            pc.AddPoint(pt);
            pt = new PointClass();
            pt.PutCoords(pt4.X, pt4.Y - Height);
            pc.AddPoint(pt);
            pt = new PointClass();
            pt.PutCoords(pt5.X, pt5.Y - Height);
            pc.AddPoint(pt);
            element.Geometry = CreatePolygon(pc);

        }

        /// <summary>
        /// 封装Element
        /// </summary>
        /// <param name="axPageLayoutControl"></param>
        /// <returns></returns>
        public static ElementCustom GetElementCustom(AxPageLayoutControl axPageLayoutControl)
        {
            IList<IElement> list = new List<IElement>();
            var graphicsContainer = axPageLayoutControl.GraphicsContainer;
            IElement element = graphicsContainer.Next();
            while (element != null)
            {

                list.Add(element);
                element = graphicsContainer.Next();
            }
            return new ElementCustom(graphicsContainer, list);
        }
        /// <summary>
        /// 替换布局中的文字
        /// </summary>
        /// <param name="strReplace"></param>
        /// <param name="pageControl"></param>
        public static void ReplaceTextElement(Dictionary<string, string> strReplace, ElementCustom elementCustom)
        {
            IGraphicsContainer graphicsContainer = elementCustom.GraphicsContainer;
            foreach (IElement element in elementCustom.Elements)
            {
                ITextElement textElement = element as ITextElement;
                bool flag = textElement != null;
                if (flag)
                {
                    string text;
                    bool flag2 = strReplace.TryGetValue(textElement.Text, out text);
                    if (flag2)
                    {
                        textElement.Text = text;
                        graphicsContainer.UpdateElement((IElement)textElement);
                    }
                }
            }
        }
        /// <summary>
        /// 导出PDF
        /// </summary>
        /// <param name="docActiveView"></param>
        /// <param name="docExport"></param>
        /// <param name="iOutputResolution"></param>
        /// <param name="lResampleRatio"></param>
        /// <param name="bClipToGraphicsExtent"></param>
        public static void ExportActiveViewParameterizedPDF(IActiveView docActiveView, string saveName, long iOutputResolution = 96, long lResampleRatio = 300, Boolean bClipToGraphicsExtent = true)
        {

            IExport docExport = new ExportPDFClass();
            docExport.ExportFileName = saveName;
            long iPrevOutputImageQuality;
            IOutputRasterSettings docOutputRasterSettings;
            IEnvelope PixelBoundsEnv;
            tagRECT exportRECT;
            tagRECT DisplayBounds;
            IDisplayTransformation docDisplayTransformation;
            IPageLayout docPageLayout;
            IEnvelope docMapExtEnv;
            long hdc;
            long iScreenResolution;
            IEnvelope docGraphicsExtentEnv;
            IUnitConverter pUnitConvertor;

            docOutputRasterSettings = docActiveView.ScreenDisplay.DisplayTransformation as IOutputRasterSettings;
            iPrevOutputImageQuality = docOutputRasterSettings.ResampleRatio;

            iScreenResolution = 88; //88 is the win32 const for Logical pixels/inch in X)
            docExport.Resolution = iOutputResolution;

            if (docActiveView is IPageLayout)
            {
                //get the bounds of the "exportframe" of the active view.
                DisplayBounds = docActiveView.ExportFrame;
                //set up pGraphicsExtent, used if clipping to graphics extent.
                docGraphicsExtentEnv = GetGraphicsExtent(docActiveView);
            }
            else
            {
                //Get the bounds of the deviceframe for the screen.
                docDisplayTransformation = docActiveView.ScreenDisplay.DisplayTransformation;
                DisplayBounds = docDisplayTransformation.get_DeviceFrame();
            }
            PixelBoundsEnv = new Envelope() as IEnvelope;
            if (bClipToGraphicsExtent && (docActiveView is IPageLayout))
            {
                docGraphicsExtentEnv = GetGraphicsExtent(docActiveView);
                docPageLayout = docActiveView as PageLayout;
                pUnitConvertor = new UnitConverter();
                //assign the x and y values representing the clipped area to the PixelBounds envelope
                PixelBoundsEnv.XMin = 0;
                PixelBoundsEnv.YMin = 0;
                PixelBoundsEnv.XMax = pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.XMax, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution - pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.XMin, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution;
                PixelBoundsEnv.YMax = pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.YMax, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution - pUnitConvertor.ConvertUnits(docGraphicsExtentEnv.YMin, docPageLayout.Page.Units, esriUnits.esriInches) * docExport.Resolution;
                //'assign the x and y values representing the clipped export extent to the exportRECT
                exportRECT.bottom = (int)(PixelBoundsEnv.YMax) + 1;
                exportRECT.left = (int)(PixelBoundsEnv.XMin);
                exportRECT.top = (int)(PixelBoundsEnv.YMin);
                exportRECT.right = (int)(PixelBoundsEnv.XMax) + 1;
                //since we're clipping to graphics extent, set the visible bounds.
                docMapExtEnv = docGraphicsExtentEnv;
            }
            else
            {
                double tempratio = iOutputResolution / iScreenResolution;
                double tempbottom = DisplayBounds.bottom * tempratio;
                double tempright = DisplayBounds.right * tempratio;
                //'The values in the exportRECT tagRECT correspond to the width
                //and height to export, measured in pixels with an origin in the top left corner.
                exportRECT.bottom = (int)Math.Truncate(tempbottom);
                exportRECT.left = 0;
                exportRECT.top = 0;
                exportRECT.right = (int)Math.Truncate(tempright);

                //populate the PixelBounds envelope with the values from exportRECT.
                // We need to do this because the exporter object requires an envelope object
                // instead of a tagRECT structure.
                PixelBoundsEnv.PutCoords(exportRECT.left, exportRECT.top, exportRECT.right, exportRECT.bottom);
                //since it's a page layout or an unclipped page layout we don't need docMapExtEnv.
                docMapExtEnv = null;
            }
            // Assign the envelope object to the exporter object's PixelBounds property.  The exporter object
            // will use these dimensions when allocating memory for the export file.
            docExport.PixelBounds = PixelBoundsEnv;
            // call the StartExporting method to tell docExport you're ready to start outputting.
            hdc = docExport.StartExporting();
            // Redraw the active view, rendering it to the exporter object device context instead of the app display.
            // We pass the following values:
            //  * hDC is the device context of the exporter object.
            //  * exportRECT is the tagRECT structure that describes the dimensions of the view that will be rendered.
            // The values in exportRECT should match those held in the exporter object's PixelBounds property.
            //  * docMapExtEnv is an envelope defining the section of the original image to draw into the export object.
            docActiveView.Output((int)hdc, (int)docExport.Resolution, ref exportRECT, docMapExtEnv, null);
            //finishexporting, then cleanup.
            docExport.FinishExporting();
            docExport.Cleanup();

        }

        private static IEnvelope GetGraphicsExtent(IActiveView docActiveView)
        {

            IEnvelope GraphicsBounds;
            IEnvelope GraphicsEnvelope;
            IGraphicsContainer oiqGraphicsContainer;
            IPageLayout docPageLayout;
            IDisplay GraphicsDisplay;
            IElement oiqElement;
            GraphicsBounds = new EnvelopeClass();
            GraphicsEnvelope = new EnvelopeClass();
            docPageLayout = docActiveView as IPageLayout;
            GraphicsDisplay = docActiveView.ScreenDisplay;
            oiqGraphicsContainer = docActiveView as IGraphicsContainer;
            oiqGraphicsContainer.Reset();
            oiqElement = oiqGraphicsContainer.Next();
            while (oiqElement != null)
            {
                oiqElement.QueryBounds(GraphicsDisplay, GraphicsEnvelope);
                GraphicsBounds.Union(GraphicsEnvelope);
                oiqElement = oiqGraphicsContainer.Next();
            }
            return GraphicsBounds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="point"></param>
        /// <param name="textSymbol"></param>
        /// <returns></returns>
        public static ITextElement CreateTextElement(string text, IPoint point, ITextSymbol textSymbol)
        {
            ITextElement textElement = new TextElementClass();
            IElement element = textElement as IElement;
            textElement.ScaleText = false;
            textElement.Symbol = textSymbol;
            element.Geometry = point;
            textElement.Text = text;
            return textElement;
        }

        /// <summary>
        /// 对象方法去替换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="elementCustom"></param>
        public static void ReplaceTextElement(object obj, ElementCustom elementCustom)
        {

        }
       /// <summary>
        /// 圆去选择
       /// </summary>
       /// <param name="point"></param>
       /// <param name="featureLayer"></param>
       /// <returns></returns>

        public static IList<IFeature> CircleSelect(IPoint point, IFeatureLayer featureLayer,double radius=0.001)
        {
            ICircularArc circularArc = CreateCArcFull(point, radius, true);
            object missing = Type.Missing;
            ISegmentCollection segmentCollection = new RingClass();
            segmentCollection.AddSegment((ISegment)circularArc, ref missing, ref missing);
            IRing ring = (IRing)segmentCollection;
            ring.Close();
            IGeometryCollection geometryCollection = new PolygonClass();
            geometryCollection.AddGeometry(ring, ref missing, ref missing);
            IGeometry pGeometry = geometryCollection as IGeometry;
            return GetSeartchFeatures(featureLayer, pGeometry, esriSpatialRelEnum.esriSpatialRelIntersects);
        }
        /// <summary>
        /// 创建圆
        /// </summary>
        /// <param name="pCPoint"></param>
        /// <param name="Radius"></param>
        /// <param name="IsCCW"></param>
        /// <returns></returns>
        private static ICircularArc CreateCArcFull(IPoint pCPoint, double Radius, bool IsCCW)
        {
            IConstructCircularArc constructCircularArc = new CircularArcClass();
            constructCircularArc.ConstructCircle(pCPoint, Radius, IsCCW);
            return (ICircularArc)constructCircularArc;
        }
        public static void PositionFlashElement(IGeometry pGeometry)
        {
            var type = esriGeometryType.esriGeometryPolygon;
            ICartographicLineSymbol ipCartographicLineSymbol;
            ISimpleFillSymbol ipSimpleFillSymbol;
            ISimpleMarkerSymbol ipSimpleMarkersymbol;
            ISymbol ipSymbol = null;
            IRgbColor ipColor;
            IPoint pPoint = new PointClass();
            pPoint.X = pGeometry.Envelope.LowerLeft.X + pGeometry.Envelope.Width / 2;
            pPoint.Y = pGeometry.Envelope.LowerLeft.Y + pGeometry.Envelope.Height / 2;
            axMapControl.CenterAt(pPoint);
            //pGeometry.Envelope.LowerLeft
            int Size;
            ipColor = new RgbColor();
            ipColor.Red = 255;
            Size = 10;
            if (type == esriGeometryType.esriGeometryPolyline)
            {
                ipCartographicLineSymbol = new CartographicLineSymbol();
                ipSymbol = (ISymbol)ipCartographicLineSymbol;
                ipSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                ipCartographicLineSymbol.Width = Size;
                ipCartographicLineSymbol.Color = ipColor;
            }
            else if (type == esriGeometryType.esriGeometryPolygon)
            {
                ipSimpleFillSymbol = new SimpleFillSymbol();
                ipSymbol = (ISymbol)ipSimpleFillSymbol;
                ipSimpleFillSymbol.Color = ipColor;
            }
            else if (type == esriGeometryType.esriGeometryPoint || type == esriGeometryType.esriGeometryMultipoint)
            {
                ipSimpleMarkersymbol = new SimpleMarkerSymbol();
                ipSymbol = (ISymbol)ipSimpleMarkersymbol;
                ipSymbol.ROP2 = esriRasterOpCode.esriROPWhite;
                ipSimpleMarkersymbol.Color = ipColor;
                ipSimpleMarkersymbol.Size = 20;
            }
            //axMapControl.FlashShape(pGeometry, 1, 100, ipSymbol);
        }
        /// <summary>
        /// 得到选中的地块
        /// </summary>
        /// <returns></returns>
        public IList<IFeature> GetSelectFeature()
        {
            IList<IFeature> pList = new List<IFeature>();//用于存储选中的要素
            IEnumFeature pEnumFeature = axMapControl.Map.FeatureSelection as IEnumFeature;
            IFeature pFeature = pEnumFeature.Next();
            while (pFeature != null)
            {
                pList.Add(pFeature);
                pFeature = pEnumFeature.Next();
            }
            return pList;
        }
        /// <summary>
        /// 转换成DataTable
        /// </summary>
        /// <param name="pLayer"></param>
        /// <returns></returns>
        private static DataTable GetTable(ILayer pLayer)
        {
            ITable pTable = (pLayer as IFeatureLayer).FeatureClass as ITable;

            DataTable pDataTable = new DataTable();

            try
            {
                IQueryFilter que = new QueryFilterClass();
                ICursor pCursor = pTable.Search(que, true);
                IRow pRow = pCursor.NextRow();

                if (pRow != null)
                {
                    for (int i = 0; i < pRow.Fields.FieldCount; i++)
                    {
                        pDataTable.Columns.Add(pRow.Fields.get_Field(i).Name);

                        //  pDataTable.Columns.Add(pRow.Fields.get_Field(i).AliasName);      //别名
                    }
                    while (pRow != null)
                    {
                        DataRow pDataRow = pDataTable.NewRow();
                        for (int j = 0; j < pCursor.Fields.FieldCount; j++)
                            pDataRow[j] = pRow.get_Value(j);
                        pDataTable.Rows.Add(pDataRow);
                        pRow = pCursor.NextRow();
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return pDataTable;
        }
    }
}
