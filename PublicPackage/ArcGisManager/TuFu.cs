using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using MyUtils;

namespace ArcGisManager
{
    public class TuFu
    {
        public IFeature Feautre { get; set; }
        public string TuFuHao { get; set; }
        /// <summary>
        /// 得到图幅号
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        public static void CreateTuFuFeautre(IEnvelope ev, IFeatureClass featureClass)
        {
            IPoint pt1 = ev.UpperLeft;
            if (pt1.IsEmpty)
            {
                return;
            }
            IPoint pt2 = ev.UpperRight;
            IPoint pt4 = ev.LowerRight;
            IList<string> list = new List<string>();
            double y1 = pt1.Y;
            double y4 = pt4.Y;
            
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            IFeatureCursor cursor = featureClass.Insert(true);
            int index = cursor.FindField("TuFu");

            while (y1 > y4)
            {
                double x1 = pt1.X;
                double x2 = pt2.X;
                while (x1 < x2)
                {
                    string tufu = GetTuFu(x1, y1);
                    list.Add(tufu);
                    IPointCollection pc = GeTuPointCollection(x1, y1);
                    IPolygon polygon =ArcGisUtils.CreatePolygon(pc);

                    featureBuffer.Shape = polygon;
                    featureBuffer.set_Value(3, tufu);
                    cursor.InsertFeature(featureBuffer);

                    x1 += 500;
                }
                y1 -= 500;
            }
       

        }
        /// <summary>
        /// 得到图幅点集合
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        private static IPointCollection GeTuPointCollection(double x1, double y1)
        {

            double x = Math.Round((double)((int)(((x1) / 500))) * 500, 2);
            double y = Math.Round((double)((int)(((y1 + 500) / 500))) * 500, 2);
            IPointCollection pc = new Polygon();

            IPoint pt1 = new PointClass();
            pt1.PutCoords(x, y);

            IPoint pt2 = new PointClass();
            pt2.PutCoords(x + 500, y);
            IPoint pt3 = new PointClass();
            pt3.PutCoords(x + 500, y - 500);
            IPoint pt4 = new PointClass();
            pt4.PutCoords(x, y - 500);

            IPoint pt5 = new PointClass();
            pt5.PutCoords(x, y);

            pc.AddPoint(pt1);
            pc.AddPoint(pt2);
            pc.AddPoint(pt3);
            pc.AddPoint(pt4);
            pc.AddPoint(pt4);
            pc.AddPoint(pt5);
            return pc;
        }
        private static string GetTuFu(double x, double y)
        {
            // double num = (double)((int)((x - 35000000.0) / 500.0)) * 0.5;
            double num = (double)((int)(x / 500.0)) * 0.5;
            return ((double)((int)(y / 500.0)) * 0.5).ToString("f2") + "-" + num.ToString("f2");
        }
        /// <summary>
        /// 设置图幅号
        /// </summary>
        /// <param name="features"></param>
        public static void SetTuFu( IFeatureLayer tufulayer,IList<IFeature> features, string filedName = "TuFu")
        {
            int index;
          
            if (features.Count != 0)
            {
                index = features[0].Fields.FindField(filedName);
            }
            else
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (IFeature feature in features)
            {
                IList<IFeature> tufus = ArcGisUtils.GetSeartchFeatures(tufulayer,feature.Shape,esriSpatialRelEnum.esriSpatialRelIntersects);

                TuFuOderByTuFuHao(tufus);

                for (int a = 0; a < tufus.Count; a++)
                {

                    if (a != 0)
                    {
                        sb.Append("、");
                    }
                    sb.Append(tufus[a].get_Value(3));
    
                }
                feature.Value[index] = sb.ToString();
                feature.Store();
                sb.Remove(0, sb.Length);

                
            }
        }

        private static void TuFuOderByTuFuHao(IList<IFeature> tufus)
        {
            int len = tufus.Count;


            for (int i = 0; i < len - 1; i++) /* 外循环为排序趟数，len个数进行len-1趟 */
                for (int j = 0; j < len - 1 - i; j++)
                { /* 内循环为每趟比较的次数，第i趟比较len-i次 */
                    if (tufus[j].get_Value(3).ToString().CompareTo(tufus[j + 1].get_Value(3).ToString()) > 0)
                    { /* 相邻元素比较，若逆序则交换（升序为左大于右，降序反之） */
                        IFeature temp = tufus[j];
                        tufus[j] = tufus[j + 1];
                        tufus[j + 1] = temp;
                    }
                }
        }
    }
}
