using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyUtils;
using Newtonsoft.Json.Linq;
using ReflectManager.XMLPackage;

namespace ReflectManager
{
    public class JObejctReflect
    {
        public static T ToObejct<T>(string str)
        {
            return JToken.Parse(str).ToObject<T>();
        }
        public static string ObjectToStr(object obj)
        {

            return ToJToken(obj).ToString();
        }

        public static JToken ToJToken(object obj)
        {

            return JObject.FromObject(obj);
        }

      
        public static Object GetTrueValueType(JToken jToken,string reultType)
        {

            reultType = reultType.Replace("System.", "");
            object value = null;
            string jsonValue = jToken.ToString();
            return Utils.GetValueTrueType(jsonValue, reultType);
            
        }
        public static IList<string> GetJTokenKeys(JToken jToken)
        {
            JObject jObject = jToken as JObject;
            
            IList<string> list = new List<string>();
          
            foreach(var v in jObject.Properties())
            {
                list.Add(v.Name);
            }
           
            return list;
        }
        public static T ToObject<T>(JObject jObject, Dictionary<string, Clazz> dic)
        {
            T t = ReflectUtils.CreateObject<T>();
            object[] paramters = new object[1];
            object value;
            Clazz clazz;
            foreach(string key in dic.Keys)
            {
                clazz = dic[key];
                value = clazz.GetTrueValueType(jObject[key]);
                if(value != null)
                {
                    paramters[0] = value;
                    clazz.SetMethodInfo.Invoke(t, paramters);
                }
               
            }
            return t;
        }
        public static IList<T> ToObject<T>(IList<JObject> jObjects, string xmlPath)
        {
            IList<T> list = new List<T>();
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
            /*Dictionary<string, string> xmlDic = XMLRead.GetConfigXmlDic(xmlPath, "property", "name", "column");
           
            Dictionary<string, Clazz> dic = ReflectUtils.ReplaceString(xmlDic, clazzDic);
            foreach(JObject jObject in jObjects)
            {
                list.Add(ToObject<T>(jObject,dic));
            }*/
            Dictionary<string, XMLObject> xmlObject = XMLRead.XmlToObjects(xmlPath);
            foreach (JObject jObject in jObjects)
            {
                list.Add(ToObject<T>(jObject, xmlObject));
            }
            return list;
        }

        public static T ToObject<T>(JObject jObject, Dictionary<string, XMLObject> xmlObjectDic)
        {
            T t = ReflectUtils.CreateObject<T>();
            object[] paramters = new object[1];
            object value = null;
            XMLObject xmlObject;
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
            Clazz clazz;
            JToken jToken;
            string column;
            foreach (string key in xmlObjectDic.Keys)
            {
                xmlObject = xmlObjectDic[key];
                clazz = clazzDic[key];
                if (!Utils.IsStrNull(xmlObject.Deafult))
                {
                    value = clazz.GetTrueValueType(xmlObject.Deafult);
                    if (value != null)
                    {
                        paramters[0] = value;
                        clazz.SetMethodInfo.Invoke(t, paramters);
                    }
                }
                else
                {
                    column = xmlObject.Column;
                    if (!Utils.IsStrNull(column))
                    {
                        if (jObject.TryGetValue(column, out jToken))
                        {


                            value = clazz.GetTrueValueType(jToken);
                            if (value != null)
                            {
                                paramters[0] = value;
                                clazz.SetMethodInfo.Invoke(t, paramters);
                            }
                        }

                    }
                }
            }
            return t;
        }
    }
}
