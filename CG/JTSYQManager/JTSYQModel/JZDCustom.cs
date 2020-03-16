using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcGisManager;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MyUtils;
using NPOI.SS.UserModel;
using ReflectManager;

namespace JTSYQManager.JTSYQModel
{
    public class JZDCustom : JZD
    {
        public static string JZDLayer = "所有权界址点";

        public static IList<JZD> GetMapJZD(string sql="")
        {

            IFeatureLayer jtLayer = ArcGisUtils.GetFeatureLayer(JZDCustom.JZDLayer);
            IFeatureCursor cur = ArcGisUtils.GetEntitys(sql, jtLayer);
            IList<IFeature> features = ArcGisUtils.CursorToList(cur);
            return FeatureToList(features) ;
        }

        public JZD JZD { get; private set; }
        public JZDCustom(JZD JZD)
        {
            this.JZD = JZD;
        }
        private  static IFeatureLayer JZDFeatureLayer { get; set; }
        public static IFeatureLayer GetJZDLayer()
        {
           
                return ArcGisUtils.GetFeatureLayer2(JZDLayer);
           
        }
        public static IList<JZD> FeatureToList(IList<IFeature> features)
        {
            if (Utils.CheckListExists(features))
            {
                return ArcGisService.ObjToFeature<JZD>(features, ArcGisService.GetTitleClzz<JZD>(features[0].Fields));
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 保存所有多部件集体所有权界址点
        /// </summary>
        /// <param name="jtsyqGroup"></param>
        public static IList<JZD> SaveMap(JTSYQ jtsyqGroup)
        {
            List<JZD> lists = new List<JZD>();
            foreach(JTSYQ jtsyq in jtsyqGroup.GroupJTSYQ)
            {
                var jzds = jtsyq.JZDS;
                if (Utils.CheckListExists(jzds))
                {
                    lists.AddRange(jzds);
                }
            }
            SaveMap(lists);
            return lists;
        }
        /// <summary>
        /// 增加界址点标注
        /// </summary>
        /// <param name="groupJTSYQ"></param>
        public static void SaveJZDBZMap(IMap map, IList<JZD> jzds)
        {
            ITextSymbol textSymbol = ArcGisUtils.CreateTextSymbol(10, "宋体");
            IGraphicsContainer pGraph = map as IGraphicsContainer;

            IList<IFeatureLayer> layers = new List<IFeatureLayer>();
            for (int a =0; a < map.LayerCount;a++)
            {
                layers.Add(map.Layer[a] as IFeatureLayer);
              
            }
            foreach (JZD jzd in jzds)
            {
                string bz = "J"+jzd.JZDH;
                if (bz != null)
                {
                    IPoint pt =GetBZPoint(jzd, layers);
                    ITextElement textELment = ArcGisUtils.CreateTextElement(bz, pt, textSymbol);
                  
                    pGraph.AddElement(textELment as IElement, 0);
                }

            }
        }

        private static IPoint GetBZPoint(JZD jzd, IList<IFeatureLayer> layers)
        {
            IPoint pt1 = jzd.Feature.Shape as IPoint;
            IPoint pt2 = jzd.Feature.Shape as IPoint;
            double radius = 30;
            int basic = 10;
            for (int a =1; a <7;a++)
            {
                pt1.X += basic * a;
                if(CheckBZPoint(pt1, layers, radius))
                {
                    return pt1;
                }
                pt1.Y += basic * a;
                if (CheckBZPoint(pt1, layers, radius))
                {
                    return pt1;
                }
                pt2.X -= basic * a;
                if (CheckBZPoint(pt2, layers, radius))
                {
                    return pt2;
                }
                pt2.Y -= basic * a;
                if (CheckBZPoint(pt2, layers, radius))
                {
                    return pt2;
                }

            }

            return jzd.Feature.Shape as IPoint;

        }

        private static bool CheckBZPoint(IPoint pt1, IList<IFeatureLayer> layers, double radius)
        {
        
            foreach (IFeatureLayer layer in layers)
            {
                if (ArcGisUtils.CircleSelect(pt1, layer, radius).Count != 0)
                {
                    return  false;
                }
            }
            return true;
        }

        /// <summary>
        /// jzd保存进入map
        /// </summary>
        /// <param name="jzd"></param>
        public static void SaveMap(IList<JZD> jzds)
        {
            
            IFeatureClass featureClass = GetJZDLayer().FeatureClass;
            Dictionary<int, Clazz> dic = ArcGisService.GetTitleClzz<JZD>( featureClass.Fields,false);
            dic.Remove(-1);
            IFeatureCursor featureCursor = featureClass.Insert(true);
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            foreach (JZD jzd in jzds)
            {
                if(jzd.Point == null)
                {
                    featureBuffer.Shape = jzd.Feature.ShapeCopy;
                }
                else
                {
                    IPoint point = new PointClass();
                    point.PutCoords(jzd.Point.X, jzd.Point.Y);
                    featureBuffer.Shape = point;
                    
                   
                }
                SetFeatureBufferValue(featureBuffer, dic, jzd);
                featureCursor.InsertFeature(featureBuffer);
            }
        }
        /// <summary>
        /// jzd保存进入map
        /// </summary>
        /// <param name="jzd"></param>
        public static void SaveMap(JZD jzd)
        {

                AddJzdFeature(jzd, jzd.Point,GetJZDLayer().FeatureClass);
        }
        /// <summary>
        /// 修改界址点
        /// </summary>
        /// <param name="jzd"></param>
        public static void Update(IList<JZD> jzds)
        {
            ArcGisService.UpdateFeate(jzds, ArcGisService.GetTitleClzz<JZD>(GetJZDLayer().FeatureClass.Fields,false));
        }
        public static void AddJzdFeature(object obj, IPoint geometry, IFeatureClass featureClass)
        {
            Dictionary<int, Clazz> dic = ArcGisService.GetTitleClzz(obj, featureClass.Fields);
            IFeatureCursor featureCursor = featureClass.Insert(true);
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            IPoint point = new PointClass();
            point.PutCoords(geometry.X, geometry.Y);
            featureBuffer.Shape = point;
            SetFeatureBufferValue(featureBuffer, dic, obj);
            featureCursor.InsertFeature(featureBuffer);
        }
        private static void SetFeatureBufferValue(IFeatureBuffer featureBuffer, Dictionary<int, Clazz> dic, object obj)
        {
            Dictionary<int, Clazz>.KeyCollection keys = dic.Keys;
            foreach (int current in keys)
            {
                Clazz clazz = dic[current];
                object value = clazz.GetMethodInfo.Invoke(obj, null);
                if (value != null)
                {
                   featureBuffer.set_Value(current, value);
                }

            }
        }

        public static IList<JZD> SearchOrCreateJZD(JTSYQ jtsyq, IFeatureLayer featureLayer)
        {
            IList<JZD> list = new List<JZD>();
            string bm = jtsyq.BM;
            IPolygon polygon = jtsyq.Feature.Shape as IPolygon;
            IPointCollection pc = polygon as IPointCollection;
            //减少一个收尾的 因闭合多一个点
            int max = pc.PointCount-1;
            
            for (int a = 0; a < max; a++)
            {
                IPoint pt1 = pc.Point[a];

                IList<JZD> oldJZDS = JZDCustom.FeatureToList(ArcGisUtils.CircleSelect(pc.get_Point(a), featureLayer));

                if (!Utils.CheckListExists(oldJZDS))
                {
                    continue;
                }else  if(oldJZDS.Count >= 2)
                {
                    RemoveMoreJZD(oldJZDS);
                }
                bool flag = true;
                foreach (JZD jzd in oldJZDS)
                {
                    flag = false;
                    string jzdbm = jzd.ZDNUM;
                    //如果有空编码 或者 本宗地 直接添加                   
                    if (Utils.IsStrNull(jzdbm) || jzdbm.Equals(bm))
                    {
                        jzd.JZDIndex = a;
                        jzd.ZDNUM = bm;
                        bool hasJzd = true;
                        //检查重复是不有重复点
                        foreach(JZD jzd1 in list)
                        {
                            double dx =Math.Abs( pc.get_Point(a).X - (jzd1.Feature.Shape as IPoint).X);
                            if(dx < 0.01)
                            {
                                double dy = Math.Abs(pc.get_Point(a).Y - (jzd1.Feature.Shape as IPoint).Y);
                                hasJzd = false;
                                break;
                            }
                        }
                        if(hasJzd)
                        {
                            list.Add(jzd);
                        }
                        
                       
                        break;
                    }
                }
               if(flag)
                {
                    IPoint JTSYQJZD = pc.get_Point(a);
                    //检查完才能确定，是否有其他宗地编码，则创建
                    JZD jzd = new JZD();
                    Point point = new PointClass();
                    point.PutCoords(JTSYQJZD.X, JTSYQJZD.Y);
                    jzd.ZDNUM = bm;
                    jzd.JZDIndex = a;
                    jzd.Point = point;
                    SaveMap(jzd);
                    list.Add(jzd);
                }
            }


            return list;

        }

      

        /// <summary>
        /// 移除重复的界址点
        /// </summary>
        /// <param name="oldJZDS"></param>
        public static void RemoveMoreJZD(IList<JZD> oldJZDS)
        {
            int count = oldJZDS.Count;
            for (int a =0; a < count-1; a++)
            {

                
                for(int b = a+1; b< count;b++)
                {
                    if(oldJZDS[a].ZDNUM.Equals( oldJZDS[b].ZDNUM))
                    {
                        oldJZDS[b].Feature.Delete();
                        oldJZDS.RemoveAt(b);
                        b--;
                        count--;
                       
                    }
                }
            }
        }

        /// <summary>
        /// 界址点近顺序编码
        /// </summary>
        /// <param name="jtsyq"></param>
        /// <param name="jzds"></param>
        /// <param name="featureLayer"></param>
        public static void SetBM(int startBH, JTSYQ jtsyq, IList<JZD> jzds, IFeatureLayer featureLayer)
        {
            if(jzds == null)
            {
                return;
            }
            //找到西北角第一点
            IPolygon pl = jtsyq.Feature.Shape as IPolygon;
            int[] fourPoint = ArcGisUtils.GetFourPointsIndex(pl);
            int index = PointDiMin(fourPoint[0], jzds);
            int startIndex = index;
            int id = jtsyq.OBJECTID;
            if(jzds.Count == 0)
            {
                MessageBox.Show("你选择的图中有没有界址点的，宗地是：" + jtsyq.QLR + ", 编码是：" + jtsyq.BM);
                return;
            }
            while (startIndex < jzds.Count)
            {
                JZD jzd = jzds[startIndex];
                jzd.JTSYQOBJECTID = id;
                jzd.JZDH = startBH++ ;
                startIndex++;
            }
            int min = 0;
            while (min < index)
            {
                JZD jzd = jzds[min];
                jzd.JTSYQOBJECTID = id;
                jzd.JZDH = startBH++ ;
                min++;
            }
            //修改
            Update(jzds);
            
        }

        private static int PointDiMin(int startIndex, IList<JZD> jzds)
        {
            int dx = -1;
            int dx2 = -1;
            int max = jzds.Count - 1;
            for (int a =0; a < max; a++)
            {
                JZD jzd = jzds[a];
                dx = Math.Abs(jzd.JZDIndex- startIndex);
                if(dx == 0)
                {
                    return a;
                }
                dx2 = Math.Abs(jzds[a+1].JZDIndex - startIndex);
                if (dx < dx2)
                {
                    return a;
                }
            }
            return max;
        }
        /// <summary>
        /// 创建，写入 jzd 表格
        /// </summary>
        /// <param name="jzds"></param>
        /// <returns></returns>
        public static ISheet CreateJZDSheet(List<JZD> jzds)
        {
            IWorkbook work = ExcelManager.ExcelRead.ReadExcel(JTSYQCustom.JZDExcelTemplete);
            ISheet sheet = work.GetSheetAt(0);
            int jzdCount = jzds.Count;
            int page = (jzdCount / 37)+1;//37个一页
            NPOI.SS.UserModel.IRow row;
            int jzdIndex = 0;
            for(int a =0; a < page;a++)
            {
                sheet.GetRow(a * 83 + 1).GetCell(4).SetCellValue("共 "+ page + " 页");
                for(int b =0; b < 71;b+=2)
                {
                    int rowIndex = (a) * 83 + b+9;
                   
                    row = sheet.GetRow(rowIndex);
                    if(jzdIndex == 107)
                    {

                    }
                    Console.WriteLine(jzdIndex);
                    if(jzdIndex < jzdCount)
                    {
                        row.GetCell(0).SetCellValue(jzdIndex + 1);
                        SetJZDRowValue(row,jzds[jzdIndex]);
                        jzdIndex++;
                    }
                    else
                    {

                       ExcelManager.ExcelWrite.DeleteRow(sheet,page*83-1,sheet.LastRowNum);
                        return sheet;
                    }
                }
            }
          
            return sheet;

        }
    
        private static void SetJZDRowValue(NPOI.SS.UserModel.IRow row, JZD jzd)
        {
            IPoint point = jzd.Feature.Shape as IPoint;
            
            row.GetCell(1).SetCellValue("J"+jzd.JZDH);
            row.GetCell(2).SetCellValue(point.X);
            row.GetCell(3).SetCellValue(point.Y);
        }
    }
}
