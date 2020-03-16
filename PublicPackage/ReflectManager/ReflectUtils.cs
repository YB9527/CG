using MyUtils;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


using System.Reflection;

namespace ReflectManager
{
    public class ReflectUtils
    {
       
       
        private static Dictionary<Type, Dictionary<string, Clazz>> UnFlagClazzDic = new Dictionary<Type, Dictionary<string, Clazz>>();

        /// <summary>
        /// 对象bool 设置相反的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodStartName">方法前缀必须要包含的文字</param>
        /// <param name="flag"></param>
        public static void UnFlag(object obj, string methodStartName, bool flag)
        {
            if(obj == null)
            {
                return;
            }
            Dictionary<string, Clazz> clazzDic;
            Type type = obj.GetType();
            if(!UnFlagClazzDic.TryGetValue(type, out clazzDic))
            {
                clazzDic = MethodToFunction(obj);
            }
            object[] paramters = new object[]
            {
                flag
            };
            foreach (string key in clazzDic.Keys)
            {
                if(key.StartsWith(methodStartName))
                {
                    clazzDic[key].SetMethodInfo.Invoke(obj, paramters);
                } 
            }
        }

       
       

        public Dictionary<string, int> getTitleList(IRow row)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            bool flag = row == null;
            Dictionary<string, int> result;
            if (flag)
            {
                result = null;
            }
            else
            {
                foreach (ICell cell in row)
                {
                    cell.SetCellType(CellType.String);
                    string stringCellValue = cell.StringCellValue;
                    bool flag2 = stringCellValue != null && !stringCellValue.Equals("");
                    if (flag2)
                    {
                        dictionary.Add(stringCellValue, cell.ColumnIndex);
                    }
                }
                result = dictionary;
            }
            return result;
        }

        public static Dictionary<string, Clazz> ReplaceString(Dictionary<string, string> xmlDic, Dictionary<string, Clazz> clazzDic)
        {
            Dictionary<string, Clazz> dic = new Dictionary<string, Clazz>();
            string value;
            Clazz clazz;
            foreach(string key in xmlDic.Keys)
            {
                
                if(clazzDic.TryGetValue(key, out clazz))
                {
                    dic.Add(xmlDic[key], clazz);
                }
            }
            return dic;
        }

        //private  ExcelToObject ExcelToObject = new ExcelToObject();
        private static Dictionary<string, Dictionary<String, Clazz>> ReflectDicCache = new Dictionary<string, Dictionary<string, Clazz>>();

        /// <summary>
        /// 列数,函数名映射
        /// </summary>
        /// <param name="functionMap"></param>
        /// <param name="classForName"></param>
        /// <returns></returns>
        public static Dictionary<int, Clazz> GetExcelToClazzMap(Dictionary<int, String> functionMap, String classForName)
        {

            functionMap.Add(-1, "objectid");
            //映射 函数名字 对应 函数方法
            Dictionary<String, Clazz> methodMap = MethodToLowerFunction(classForName);
            //映射 列号 对应 函数方法 
            Dictionary<int, Clazz> cellMap = MapToMapKeyToValue(functionMap, methodMap);

            return cellMap;
        }
        private static Dictionary<int, Clazz> MapToMapKeyToValue(Dictionary<int, string> functionMap, Dictionary<string, Clazz> methodMap)
        {
            Dictionary<int, Clazz> resultMap = new Dictionary<int, Clazz>();
            Dictionary<int, string>.KeyCollection keys = functionMap.Keys;
            Clazz clazz;
            String value;
            foreach (int key in keys)
            {
                functionMap.TryGetValue(key, out value);
                if (methodMap.TryGetValue(value.ToLower(), out clazz))
                {
                    resultMap.Add(key, clazz);
                }
            }
            /**
            Dictionary<String, int> map = KeyToValue(functionMap);
            int resultKey;
            Clazz resultValue;
            Dictionary<String, int>.KeyCollection keyCol = map.Keys;
            foreach (String key in keyCol)
            {
                if (methodMap.TryGetValue(key, out resultValue))
                {
                    map.TryGetValue(key, out resultKey);
                    resultMap.Add(resultKey, resultValue);
                }
            }
    */
            return resultMap;
        }
        //key 与 value 反转
        private Dictionary<String, int> KeyToValue(Dictionary<int, String> dic)
        {
            Dictionary<String, int> resultMap = new Dictionary<String, int>();
            Dictionary<int, String>.KeyCollection keyCol = dic.Keys;
            String resultValue;
            foreach (int key in keyCol)
            {
                if (dic.TryGetValue(key, out resultValue))
                {

                    resultMap.Add(resultValue, key);

                }
            }
            return resultMap;
        }



        private Dictionary<int, string> MapToMapKeyToValue(Dictionary<string, int> title, Dictionary<string, string> map)
        {
            Dictionary<int, String> resultMap = new Dictionary<int, string>();
            int resultKey;
            String resultValue;

            Dictionary<string, int>.KeyCollection keyCol = title.Keys;
            foreach (String key in keyCol)
            {
                if (map.TryGetValue(key, out resultValue))
                {
                    title.TryGetValue(key, out resultKey);
                    resultMap.Add(resultKey, resultValue);

                }
            }
            return resultMap;
        }


        public static Dictionary<string, Clazz> MethodToLowerFunction(object obj)
        {
            return MethodToLowerFunction(obj.GetType().FullName);
        }
        /// <summary>
        /// 必须自带方法 Clone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static ObservableCollection<T> CloneList<T>(IList<T> objects)
        {
            if(objects == null)
            {
                return null;
            }
            ObservableCollection<T>  list= new ObservableCollection<T>();
       
            if(objects.Count ==0)
            {
                return list;
            }
            MethodInfo clone = typeof(T).GetMethod("Clone");
            foreach (T obj in objects)
            {
                list.Add((T)clone.Invoke(obj, null));
            }
            return list;
        }

        /// <summary>
        /// 对象深度克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByReflect<T>(T obj)
        {
            //如果是字符串或值类型则直接返回
            if (obj is string || obj.GetType().IsValueType) return obj;
            object retval = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                catch { }
            }
            return (T)retval;
        }

        public static void ClassCopy(object src, object desc, Dictionary<Clazz, Clazz> xmlDic)
        {
            Clazz srcClazz;
            object[] parematers = new object[1];
            foreach (Clazz descClazz in xmlDic.Keys)
            {
                srcClazz = xmlDic[descClazz];
                object result = srcClazz.GetMethodInfo.Invoke(src, null);
                if (result != null)
                {
                    parematers[0] = result;
                    descClazz.SetMethodInfo.Invoke(desc, parematers);
                }
            }
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>()
        {
            string classForName =  typeof(T).AssemblyQualifiedName;
            return CreateObject<T>(classForName);
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classForName"></param>
        /// <returns></returns>
        public static T CreateObject<T>(string classForName)
        {
           
            Type type = Type.GetType(classForName);
            object obj = Activator.CreateInstance(type);
            return (T)obj;

        }
        public static Dictionary<string, Clazz> MethodToFunction(object obj)
        {
            return MethodToFunction(obj.GetType().AssemblyQualifiedName);
        }
        /// <summary>
        /// 空间名称得到方法的clazz Dic
        /// </summary>
        /// <param name="classForName"></param>
        /// <returns></returns>
        public static Dictionary<string, Clazz> MethodToLowerFunction(string classForName)
        {
            //classForName = "YanBo_CG.Zjd.ZjdPo.Jtcy2";
            Dictionary<string, Clazz> dic = new Dictionary<string, Clazz>();
            Type type = Type.GetType(classForName);
            dynamic obj = type.Assembly.CreateInstance(classForName);
            MethodInfo[] ms = type.GetMethods();

            Clazz clazz;
            foreach (MethodInfo m in ms)
            {

                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length == 0)
                {
                    continue;
                }
                //得到方法的名字
                String methodName = m.Name.Replace("set_", "");
                //去掉set，第一个字母大写
                methodName = methodName.ToLower();
                //都只有一个参数，得到参数类型
                Type pt = ps[0].ParameterType;
                clazz = new Clazz();
                clazz.setFunction(methodName);
                clazz.setParamterType(pt);
                //List<MethodInfo> list = new List<MethodInfo>();
                //list.Add(m);
                // clazz.setMethodInfos(list);
                clazz.SetMethodInfo = m;
                clazz.setFullClassName(classForName);

                clazz.SetMethodInfo = m;
                dic.Add(methodName, clazz);
            }
            foreach (MethodInfo m in ms)
            {
                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length != 0)
                {
                    continue;
                }
                //得到方法的名字
                String methodName = m.Name.Replace("get_", "");
                //去掉set，第一个字母大写
                methodName = methodName.ToLower();
                if (dic.TryGetValue(methodName, out clazz))
                {
                    // clazz.getMethodInfos().Add(m);
                    clazz.GetMethodInfo = m;
                }
            }

            return dic;
        }


        public static Dictionary<string, Clazz> MethodToFunction(string classForName)
        {
            Dictionary<string, Clazz> dic;
            if (ReflectDicCache.TryGetValue(classForName, out dic))
            {
                return dic;
            }
            dic = new Dictionary<string, Clazz>();
            Type type = Type.GetType(classForName);
            dynamic obj = type.Assembly.CreateInstance(classForName);
            MethodInfo[] ms = type.GetMethods();
            Clazz clazz;
            foreach (MethodInfo m in ms)
            {

                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length == 0)
                {
                    continue;
                }
                //得到方法的名字
                String methodName = m.Name.Replace("set_", "");
                if(methodName.Equals("Equals"))
                {
                    continue;
                }
                //都只有一个参数，得到参数类型
                Type pt = ps[0].ParameterType;
                clazz = new Clazz();
                clazz.setFunction(methodName);
                clazz.setParamterType(pt);
                /* List<MethodInfo> list = new List<MethodInfo>();
                 list.Add(m);
                 clazz.setMethodInfos(list);
                 */
                clazz.SetMethodInfo = m;
                clazz.setFullClassName(classForName);
                if(!dic.Keys.Contains(methodName))
                {
                    dic.Add(methodName, clazz);
                }
                
            }
            foreach (MethodInfo m in ms)
            {
                ParameterInfo[] ps = m.GetParameters();
                if (ps.Length != 0)
                {
                    continue;
                }
                //得到方法的名字
                String methodName = m.Name.Replace("get_", "");

                if (methodName.Equals("Equals"))
                {
                    continue;
                }
                if (dic.TryGetValue(methodName, out clazz))
                {
                    //clazz.getMethodInfos().Add(m);
                    clazz.GetMethodInfo = m;
                }
            }
            ReflectDicCache.Add(classForName, dic);
            return dic;
        }




        public String GetFullName(Dictionary<String, Clazz> clazzDic)
        {
            Dictionary<String, Clazz>.KeyCollection functions = clazzDic.Keys;
            Clazz clazz;
            String fullName = null;
            foreach (String function in functions)
            {
                clazzDic.TryGetValue(function, out clazz);
                fullName = clazz.getFullClassName();
                return fullName;
            }
            return null;
        }
        /// <summary>
        /// 根据方法得到 对象值
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetValue<T>(string methodName, object obj)
        {
            if(Utils.IsStrNull(methodName))
            {
                  return default(T);
            }
            Dictionary<string, Clazz> clazzDic = MethodToFunction(obj);
            Clazz clazz;
            if(clazzDic.TryGetValue(methodName, out clazz))
            {
              return   (T)clazz.GetMethodInfo.Invoke(obj, null);
            }
            return default(T);
        }

        public static Dictionary<string, Clazz> MethodToFunction<T>()
        {
            
            return MethodToFunction(typeof(T).AssemblyQualifiedName);
        }
        /// <summary>
        /// 根据方法名 向对象里设置值
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetValue(string methodName, object obj, object value)
        {
           return  SetValue(methodName, obj, new object[] { value });
        }
        /// <summary>
        /// 根据方法名 向对象里设置值
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <param name="paramters"></param>
        /// <returns></returns>
        public static bool SetValue(string methodName, object obj, object[] paramters)
        {
            if (Utils.IsStrNull(methodName))
            {
                return false;
            }
            Dictionary<string, Clazz> clazzDic = MethodToFunction(obj);
            Clazz clazz;
            if (clazzDic.TryGetValue(methodName, out clazz))
            {
                 clazz.SetMethodInfo.Invoke(obj, paramters);
                return true;
            }
            return false;
            
        }
        public static  Dictionary<int, Clazz> GetClazzDic(Dictionary<int, string> rowDic, Dictionary<string, string> propertDic, Dictionary<string, Clazz> clazzDic)
        {
            string value;
            string tempValue;
            Clazz clazz;
            Dictionary<int, Clazz> resultDic = new Dictionary<int, Clazz>();
            foreach (int i in rowDic.Keys)
            {
                value = rowDic[i];
                if (propertDic.TryGetValue(value, out tempValue))
                {
                    if (clazzDic.TryGetValue(tempValue, out clazz))
                    {
                        resultDic.Add(i, clazz);
                    }
                }
            }
            return resultDic;
        }
        public static void ClassCopy(object srcObj, object descObj)
        {
            if (srcObj == null)
            {
                return;
            }
            Dictionary<string, Clazz> srcClazz = MethodToFunction(srcObj);
            Dictionary<string, Clazz> descClazz = MethodToFunction(descObj);
            Clazz clazz;
            object[] ps = new object[1];
            foreach (string srcM in srcClazz.Keys)
            {

                if (descClazz.TryGetValue(srcM, out clazz))
                {
                    MethodInfo m = srcClazz[srcM].GetMethodInfo;
                    
                    if (m != null && m.Name.StartsWith("get_"))
                    {
                        ps[0] = m.Invoke(srcObj, null);
                        clazz.SetMethodInfo.Invoke(descObj, ps);
                    }

                }

            }
        }
        /// <summary>
        /// 统计 方法的 所有对象的面积
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">方法名字</param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double GetTotal_double<T>(string key ,IList<T> list)
        {
            Dictionary<string, Clazz> clazzDic = MethodToFunction<T>();
            Decimal total = 0;
            foreach(T t in list)
            {
                //double tem = (double)clazzDic[key].GetMethodInfo.Invoke(t, null);
                decimal tem = new decimal((double)clazzDic[key].GetMethodInfo.Invoke(t, null));
                total  = Decimal.Add(total, tem);
                
            }
            return Decimal.ToDouble(total);
        }

      
        /// <summary>
        /// 映射列数 和clazz
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<String, Clazz> GetRealClazzDicValue(Dictionary<String, String> dic)
        {
            String fullName;
            Dictionary<String, Clazz> calzzDic = null;
            Clazz clazz = new Clazz();
            if (dic.TryGetValue("类的全名称", out fullName))
            {
                calzzDic = MethodToLowerFunction(fullName);

            }
            //转换成小写的值
            dic = Utils.ValueToLower(dic);
            Dictionary<String, String>.KeyCollection keys = dic.Keys;
            Dictionary<String, Clazz> newCalzzDic = ChangeValue<String, Clazz>(dic, calzzDic);
            clazz.setFullClassName(fullName);
            newCalzzDic.Add("类的全名称", clazz);
            return newCalzzDic;
        }
        public static Dictionary<T1, T2> ChangeValue<T1, T2>(Dictionary<T1, T1> dic, Dictionary<T1, T2> calzzDic)
        {
            Dictionary<T1, T2> newCalzzDic = new Dictionary<T1, T2>();
            Dictionary<T1, T1>.KeyCollection keys = dic.Keys;
            T1 value;
            T2 clazz;
            foreach (T1 key in keys)
            {

                dic.TryGetValue(key, out value);
                if (calzzDic.TryGetValue(value, out clazz))
                {
                    newCalzzDic.Add(key, clazz);
                }
            }

            return newCalzzDic;
        }

    }
}
