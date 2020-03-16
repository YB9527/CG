using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MyUtils;
using ReflectManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArcGisManager
{
    public class ArcGisService
    {
        

        /// <summary>
        /// 添加实体对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="geometry"></param>
        /// <param name="featureClass"></param>
        public static void AddFeature(object obj, IGeometry geometry, IFeatureClass featureClass)
        {
            Dictionary<int, Clazz> dic = ArcGisService.GetTitleClzz(obj, featureClass.Fields,false);
            IFeatureCursor featureCursor = featureClass.Insert(true);
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            featureBuffer.Shape = geometry;
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

        public static IList<T> FindEntitys<T>(string layerName,string sql,string configName)
        {   
            IFeatureCursor pCursor = ArcGisDao.FindDatabseCussor(layerName, sql);
            Dictionary<int, Clazz> clazzDic = ArcGisDao.GetTitleClzz<T>(pCursor.Fields, configName);
            return  ArcGisDao.GetEntity<T>(clazzDic,pCursor);
        }

        public static void ReplaceText(object obj, Dictionary<string, XMLObject> clazzDic, MapDocumentClass mapDocument)
        {
            ArcGisUtils.axPageLayoutControl.PageLayout = mapDocument.PageLayout;
            var graphicsContainer = ArcGisUtils.axPageLayoutControl.GraphicsContainer;
             XMLObject xmlObject;
            IElement element = graphicsContainer.Next();
            while (element != null)
            {
                if(element is ITextElement)
                {
                    ITextElement textElement = (ITextElement)element;
                    string text = textElement.Text;
                    int start = text.IndexOf("[");
                    if(start != -1 )
                    {
                        string key = text.Substring(start+1, text.IndexOf("]") - start - 1);
                        if(clazzDic.TryGetValue(key,out xmlObject))
                        {
                            object value = XMLRead.GetObjectMethodResult(xmlObject, obj);
                           if(value != null)
                            {
                                textElement.Text = text.Replace("[" + key + "]", value.ToString());
                            }
                            else
                            {
                                textElement.Text = text.Replace("[" + key + "]", "");
                            }
                            graphicsContainer.UpdateElement((IElement)textElement );
                        }
                    }
                }
               
                element = graphicsContainer.Next();
            }
          
        }

        /// <summary>
        /// feature 转换为 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="features"></param>
        /// <param name="titleDic"></param>
        /// <returns></returns>
        public static IList<T> ObjToFeature<T>(IList<IFeature> features, Dictionary<int, Clazz> titleDic)
        {
            
            return ArcGisDao.GetEntity<T>(titleDic, features);
        }
        /// <summary>
        /// 得到字段对应的 clazzDic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static Dictionary<int, Clazz> GetTitleClzz<T>(IFields fields, bool retainEditable = true)
        {
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
            
            Dictionary<String, int> filed = ArcGisDao.FeatureTitileToDic(fields, retainEditable);
            filed.Add("Feature", -1);
            Dictionary<int, Clazz> titleClazz = ArcGisDao.GetTitleClzz(clazzDic, filed);
           
            return titleClazz;
        }
        /// <summary>
        /// 得到字段对应的 clazzDic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static Dictionary<int, Clazz> GetTitleClzz(object obj, IFields fields,bool retainEditable=true)
        {
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(obj);

            Dictionary<String, int> filed = ArcGisDao.FeatureTitileToDic(fields,retainEditable);
            Dictionary<int, Clazz> titleClazz = ArcGisDao.GetTitleClzz(clazzDic, filed);
            return titleClazz;

        }

        public static IList<T> FeatureToObj<T>(IList<IFeature> fs,string configName)
        {
           
            if(fs == null || fs.Count ==0)
            {
                return null;
            }
            Dictionary<int, Clazz> clazzDic = ArcGisDao.GetTitleClzz<T>(fs[0].Fields, configName);
            return ArcGisDao.GetEntity<T>(clazzDic, fs);
        }
        public static void SetEntity(IFeature f, string configName, object obj)
        {

            if (f == null )
            {
                return ;
            }
            Dictionary<int, Clazz> clazzDic = ArcGisDao.GetTitleClzz(obj,f.Fields, configName);
            ArcGisDao.SetEntity(obj,clazzDic, f);
        }

        public static IList<IFeature> ObjToFeature<T>(IList<T> list, string configName)
        {
           


            if(list.Count >0)
            {
                
                Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
                IFeature feature = (IFeature)clazzDic["Feature"].GetMethodInfo.Invoke(list[0], null);
               
                IFields fs = feature.Fields;
                Dictionary<int, Clazz> dic = ArcGisDao.GetTitleClzz<T>(fs, configName);

                RemoveEditUnAble(dic, fs);
                return ObjToFeature(list, dic);
            }
            return null;
           
          
        }

        private static void RemoveEditUnAble(Dictionary<int, Clazz> dic, IFields fields)
        {
            int[] indexs = dic.Keys.ToArray<int>();
            foreach(int index in indexs)
            {
                if(index < 0)
                {
                    continue;
                }
                if(!fields.Field[index].Editable)
                {
                    dic.Remove(index);
                }
            }
        }
        /// <summary>
        /// 修改map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="dic"></param>
        public static void UpdateFeate<T>(IList<T> list, Dictionary<int, Clazz> dic)
        {
            if(Utils.CheckListExists(list))
            {
                IList<IFeature> features = ObjToFeature(list, dic);
                UpdateFeaures(features);
            }
           
        }
        public static IList<IFeature> ObjToFeature<T>(IList<T> list, Dictionary<int, Clazz> dic)
        {
            IList<IFeature> features = new List<IFeature>();
           
            foreach (T t in list)
            {
                IFeature feature = (IFeature)dic[-1].GetMethodInfo.Invoke(t, null);
                if(feature == null)
                {
                    continue;
                }
                features.Add(ArcGisDao.ObjToFeature<T>(t, dic));
            }
            return features;
        }

        private static Dictionary<string, IFeatureLayer> GetLayerDic(AxMapControl axMapControl)
        {
            Dictionary<string, IFeatureLayer> dic = new Dictionary<string, IFeatureLayer>();
            IEnumLayer layers = axMapControl.Map.Layers;
            ILayer layer = layers.Next();
            while (layer != null)
            {
                dic.Add(layer.Name, (IFeatureLayer)layer);
                layer = layers.Next();
            }
            return dic;
        }

        /// <summary>
        /// 保存修改了的图形
        /// </summary>
        /// <param name="features"></param>
        public static void UpdateFeaures(IList<IFeature> features)
        {
            //IWorkspaceEdit edit = ArcGisDao.StratEdit();
            foreach(IFeature feature in features)
            {
                feature.Store();
            }
            //ArcGisDao.EndEdit(edit);

        }
    }
}
