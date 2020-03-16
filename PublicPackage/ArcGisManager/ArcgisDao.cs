
using ESRI.ArcGIS.Geodatabase;
using ExcelManager;
using MyUtils;
using ReflectManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;


namespace ArcGisManager
{
    public class ArcGisDao
    {
        private static Dictionary<string, Dictionary<int, Clazz>> fieldDicCache = new Dictionary<string, Dictionary<int, Clazz>>();
        public static  IFeatureCursor FindDatabseCussor(string layerName, string sql)
        {
            IFeatureWorkspace space = ArcGisController.GetIFeatureWorkspace();
            IFeatureClass pFeatureClass = space.OpenFeatureClass(layerName);           
            IQueryFilter queryFilter = new QueryFilterClass();
            if (Utils.IsStrNull(sql))
            {
                queryFilter = null;
            }
            else
            {
                
                queryFilter.WhereClause = sql;
            }


            IFeatureCursor pCursor = pFeatureClass.Search(queryFilter, false);
            return pCursor;
        }

        public static Dictionary<int, Clazz> GetTitleClzz(Dictionary<string, Clazz> clazzDic, Dictionary<string, int> filedDic)
        {
            Dictionary<int, Clazz> dic = new Dictionary<int, Clazz>();
            Clazz clazz;
            if (clazzDic.TryGetValue("IFeature",out clazz))
            {
                dic.Add(-1, clazz);
            }
            foreach(string title in filedDic.Keys)
            {
                if(clazzDic.TryGetValue(title,out clazz))
                {
                    dic.Add(filedDic[title], clazz);
                }
            }
            return dic;
        }

        public static IList<T> GetEntity<T>(Dictionary<int, Clazz> dic, IList<IFeature> fs)
        {

            IList<T> list = new List<T>();
            if(fs == null || fs.Count ==0)
            {
                return list;
            }
            Clazz clazz;
           
            System.Object obj;
            System.Object[] paramers = new System.Object[1];
            IList<int> keys = dic.Keys.ToList<int>();
            keys.Remove(-1);
            MethodInfo featureMethod = typeof(T).GetMethod("set_Feature");
            foreach (IFeature feature in fs)
            {
               
                obj = ReflectUtils.CreateObject<T>();
                paramers[0] = feature;
                featureMethod.Invoke(obj, paramers);
                //创建对象去接收
                foreach (int index in keys)
                {
                    clazz = dic[index];
                    paramers[0] = feature.get_Value(index);
                    if (!(paramers[0] is System.DBNull))
                    {
                        clazz.SetMethodInfo.Invoke(obj, paramers);
                    }
                }
                list.Add((T)obj);             
            }
            return list;

        }



        internal static void SetEntity(object obj,Dictionary<int, Clazz> dic, IFeature feature)
        {
            Clazz clazz;
            System.Object[] paramers = new System.Object[1];
            IList<int> keys = dic.Keys.ToList<int>();
            keys.Remove(-1);        
            //-1是feature
            if (dic.TryGetValue(-1, out clazz))
            {
                paramers[0] = feature;
                clazz.SetMethodInfo.Invoke(obj, paramers);
            }
            //创建对象去接收
            foreach (int index in keys)
            {
                clazz = dic[index];
                paramers[0] = feature.get_Value(index);
                if (!(paramers[0] is System.DBNull))
                {
                    clazz.SetMethodInfo.Invoke(obj, paramers);
                }
            }
           
        }

        internal static Dictionary<int, Clazz> GetTitleClzz<T>(IFields fields, string configName)
        {
            Dictionary<int, Clazz> resultDic;
            if(fieldDicCache.TryGetValue(configName,out resultDic))
            {
                return resultDic;
            }             
             Dictionary<String, int> filed = FeatureTitileToDic(fields);
            Dictionary<String, String> configDic = XMLRead.GetConfigXmlDic(configName, "property", "name", "column");
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
           
             resultDic = GetChangeDic(filed, configDic, clazzDic);
             fieldDicCache.Add(configName, resultDic);
             return resultDic;

        }
        internal static Dictionary<int, Clazz> GetTitleClzz(object obj ,IFields fields, string configName)
        {
            Dictionary<int, Clazz> resultDic;
            if (fieldDicCache.TryGetValue(configName, out resultDic))
            {
                return resultDic;
            }
            Dictionary<String, int> filed = FeatureTitileToDic(fields);
            Dictionary<String, String> configDic = XMLRead.GetConfigXmlDic(configName, "property", "key", "value");
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(obj);
            resultDic = GetChangeDic(filed, configDic, clazzDic);
            fieldDicCache.Add(configName, resultDic);
            return resultDic;

        }
        /// <summary>
        /// 前提是转过对象
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, Clazz> ConfigNameToClazz(string configName)
        {
            return fieldDicCache[configName];
        }
        public  static IList<T> GetEntity<T>(Dictionary<int, Clazz> dic, IFeatureCursor pCursor)
        {
            IList<T> list = new List<T>();
            Clazz clazz;
            string fullName = typeof(T).ToString();
            Type type = Type.GetType(fullName);
            System.Object obj;
            System.Object[] paramers = new System.Object[1];
            IList<int> keys = dic.Keys.ToList<int>();
            keys.Remove(-1);
            IFeature feature = pCursor.NextFeature();
            while (feature != null)
            {
                obj = type.Assembly.CreateInstance(fullName);
                //-1是feature
                if (dic.TryGetValue(-1, out clazz))
                {
                    paramers[0] = feature;
                    clazz.SetMethodInfo.Invoke(obj, paramers);
                }
                //创建对象去接收
                foreach (int index in keys)
                {
                    clazz = dic[index];
                    paramers[0] = feature.get_Value(index);
                    if (!(paramers[0] is System.DBNull))
                    {
                        clazz.SetMethodInfo.Invoke(obj, paramers);
                    }
                }
                list.Add((T)obj);
                feature = pCursor.NextFeature();
            }

            return list;
        }
        private static Dictionary<int, Clazz> GetChangeDic(Dictionary<string, int> titleDic, Dictionary<string, string> filedDic, Dictionary<string, Clazz> clazzMap)
        {
            IList<String> errors = new List<String>();
            Dictionary<int, Clazz> dic = new Dictionary<int, Clazz>();
            Dictionary<string, int>.KeyCollection keys = titleDic.Keys;
            string value;
            Clazz clazz;
            foreach (String key in keys)
            {
                if (filedDic.TryGetValue(key, out value))
                {
                    if (clazzMap.TryGetValue(value, out clazz))
                    {

                        dic.Add(titleDic[key], clazz);
                    }
                    else
                    {
                        errors.Add(key + ",没有配置目标字段！！！");
                    }

                }
            }

            if (clazzMap.TryGetValue("Feature", out clazz))
            {
                if (!dic.ContainsKey(-1))
                {
                    dic.Add(-1, clazz);
                }
            }
            
            return dic;
        }
        public static Dictionary<String, int> FeatureTitileToDic(IFields fields, bool retainEditable=true)
        {
            Dictionary<String, int> dic = new Dictionary<String, int>();
            IField field;
            for (int a = 0; a < fields.FieldCount; a++)
            {
                field = fields.Field[a];
                if (field.Editable || retainEditable)
                {
                    dic.Add(field.Name, a);
                }

            }
            return dic;
        }
        /// <summary>
        /// int 对应图形字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dk"></param>
        /// <param name="clazzDic"></param>
        /// <returns></returns>
        public static IFeature ObjToFeature<T>(T dk, Dictionary<int, Clazz> clazzDic)
        {
            IFeature feature= (IFeature)clazzDic[-1].GetMethodInfo.Invoke(dk, null);
            
            IList<int> indexs = clazzDic.Keys.ToList<int>();
            
            indexs.Remove(-1);
            foreach (int index in indexs)
            {
                feature.set_Value(index, clazzDic[index].GetMethodInfo.Invoke(dk, null));
            }
            return feature;
        }
        public static IWorkspaceEdit StratEdit()
        {

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)ArcGisController.GetIFeatureWorkspace();
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();
            return workspaceEdit;
        }
        public static void EndEdit(IWorkspaceEdit workspaceEdit)
        {
            //关闭要素编辑状态
            try
            {
                workspaceEdit.StopEditing(true);
                workspaceEdit.StopEditOperation();
            }
            catch (Exception e)
            {
                MessageBox.Show("编辑停止失败：" + e.ToString());
            }
        }



    }
}
