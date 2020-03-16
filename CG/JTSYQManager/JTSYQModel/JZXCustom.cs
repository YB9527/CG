using System;
using System.Collections.Generic;
using ArcGisManager;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace JTSYQManager.JTSYQModel
{


    public class JZXCustom
    {
        public static string JZXLayerName = "裁剪线";



       
        
        /// <summary>
        /// 设置周围的界址线
        /// </summary>
        /// <param name="jtsyqs"></param>
        public static List<JZX>  SetJZX(IList<JTSYQ> jtsyqs)
        {

            List<JZX> list = new List<JZX>();
            IList<JTSYQ> otherJTSYQS = new List<JTSYQ>();
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                IGeometry geometry = jtsyq.Feature.Shape;
                //相交的面
                IList<IFeature> intersectFeature = ArcGisUtils.GetSeartchFeatures(JTSYQCustom.GetLayer(), geometry.Envelope, esriSpatialRelEnum.esriSpatialRelIntersects);
                IList<JTSYQ> interJTSYQS = JTSYQCustom.FeaturesToJTSYQ(intersectFeature);
                IList<JZX> jzxs = new List<JZX>();
                foreach (JTSYQ inter in interJTSYQS)
                {
                    if (JTSYQHasObject(inter.OBJECTID, jtsyqs))
                    {
                        continue;
                    }
                    JZX jzx = new JZX();
                    jzx.BM = inter.BM;
                    jzx.QLR = inter.QLR;
                    jzx.JTSYQOBJECTID = inter.OBJECTID;
                    IEnvelope envelope = jtsyq.Feature.Shape.Envelope;
                  
                    IPolyline pl2 = ArcGisUtils.PolygonToPolyline(inter.Feature.Shape as IPolygon);
                    IGeometry clipGeometry = ArcGisUtils.GeometryClip(pl2, ArcGisUtils.ScaleEnvelope(envelope, 1.2));
                    jzx.Polyline = clipGeometry as IPolyline;
                    jzxs.Add(jzx);
                    inter.SelfJZX = jzx;
                    otherJTSYQS.Add(inter);
                }
                jtsyq.JZXS = jzxs;
            }
            
             //移除重复的地块
             int count = otherJTSYQS.Count;
            /* for (int a =0; a < count; a++)
             {
                 JTSYQ j1 = otherJTSYQS[a];

                 for (int b= a+1; b < count;b++)
                 {
                     JTSYQ j2 = otherJTSYQS[b];
                     if(j1.OBJECTID == j2.OBJECTID)
                     {
                         otherJTSYQS.RemoveAt(b);
                         b--;
                         count--;
                     }
                 }

             }*/

            //移除被大地块所包含的
            for (int a = 0; a < count; a++)
             {
                 JTSYQ j1 = otherJTSYQS[a];
                 IList<IFeature> containsFeature = ArcGisUtils.GetSeartchFeatures(JTSYQCustom.GetLayer(), ArcGisUtils.ScaleEnvelope(j1.Feature.Shape.Envelope, 0.9) , esriSpatialRelEnum.esriSpatialRelContains);
                 IList<JTSYQ> containsJTSYQS = JTSYQCustom.FeaturesToJTSYQ(containsFeature);
                 if(containsJTSYQS != null)
                 {
                     for (int c = 0; c < containsJTSYQS.Count; c++)
                     {
                         for (int b = 0; b < count; b++)
                         {
                             if (a == b)
                             { //除自己
                                 continue;
                             }
                             JTSYQ j2 = otherJTSYQS[b];
                             if (containsJTSYQS[c].OBJECTID == j2.OBJECTID)
                             {
                                 otherJTSYQS.RemoveAt(b);
                                 b--;
                                 count--;
                             }
                         }
                     }
                 }
                
             }
            List<JZX> lastJZXS = new List<JZX>();
            foreach(JTSYQ jtsyq in otherJTSYQS)
            {
                lastJZXS.Add(jtsyq.SelfJZX);
            }
            //groupJTSYQ.JZXS = lastJZXS;
            return lastJZXS;
           
          
        }

        private static bool EnvelopeHasPoint(IPoint point, IEnvelope envelope)
        {
            double x = point.X;
            double y = point.Y;
            if(x == envelope.LowerLeft.X || x == envelope.LowerRight.X || x== envelope.UpperLeft.X || x == envelope.UpperRight.X)
            {
                return true;
            }else if (y == envelope.LowerLeft.Y || y == envelope.LowerRight.Y || y == envelope.UpperLeft.Y || y == envelope.UpperRight.Y)
            {
                return true;
            }
            return false;
        }

        private static bool JTSYQHasObject(int objectid, IList<JTSYQ> jtsyqs)
        {
          
            foreach(JTSYQ jtsyq in jtsyqs)
            {
                if(jtsyq.OBJECTID == objectid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 设置界址线标注 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="jzxs"></param>
        public static void SaveJZXBZMap(IMap map, IList<JZX> jzxs)
        {
            ITextSymbol textSymbol = ArcGisUtils.CreateTextSymbol(10, "宋体");
            IGraphicsContainer pGraph = map as IGraphicsContainer;
            foreach (JZX jzx in jzxs)
            {
                string bz = jzx.QLR + "\n" + jzx.BM;
                if(bz != null)
                {
                    bz = bz.Replace("成都高新区", "");
                    IPointCollection pc = jzx.Polyline as IPointCollection;
                    IPoint pt = ArcGisUtils.GetPolyCore(pc);
                    var textELment =  ArcGisUtils.CreateTextElement(bz, pt, textSymbol);
                    pGraph.AddElement(textELment as IElement,0);
                }
               
            }
        }

        /// <summary>
        /// 保存进入map
        /// </summary>
        /// <param name="jzxs"></param>
        public static void SaveJZXMap(IList<JZX> jzxs)
        {
            IFeatureLayer layer = ArcGisManager.ArcGisUtils.GetFeatureLayer(JZXLayerName);
            foreach(JZX jzx in jzxs)
            {
                ArcGisService.AddFeature(jzx, jzx.Polyline, layer.FeatureClass);
            }
        }
    }
}
