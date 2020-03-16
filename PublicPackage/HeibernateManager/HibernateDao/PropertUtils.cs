using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeibernateManager.HibernateDao
{
    public class PropertUtils
    {
        
        public static Dictionary<string,string> PropertMethodAlisaName()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("包含", "Like");
            dic.Add("相等","Eq");
            dic.Add("大于", "Gt");
            dic.Add("大于等于", "Ge");          
            dic.Add("值里面", "In");
            dic.Add("值不在里面", "NotIn");
            dic.Add("小于", "Lt");
            dic.Add("小于等于", "Le");                     
            return dic; 
        }

        public static void fun()
        {
            AbstractCriterion ab = ReflectGetValue("Eq", "DMDM",new string[] { "abc"});
        }
            ///
        public static Dictionary<string, MethodInfo> ClazzDic<T>(Dictionary<string,Type[]> typeParameters)
        {
            Dictionary<string, MethodInfo> dic = new Dictionary<string, MethodInfo>();
            Type type = typeof(T);          
            foreach(string methodName in typeParameters.Keys)
            {
                MethodInfo methodinfo=   type.GetMethod(methodName, typeParameters[methodName]);               
                dic.Add(methodName,methodinfo);
            }
            return dic;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="forName"></param>
        /// <param name="values">参数</param>
        /// <returns></returns>
        public static AbstractCriterion ReflectGetValue(string methodName, string forName,object[] values)
        {
            Type type = typeof(Property);           
            Type[] typeParameters = null;
            int parameterLen = values.Length;
            if (values != null && parameterLen > 0)
            {
                typeParameters = new Type[parameterLen];
                for(int a =0; a < parameterLen;a++)
                {
                    typeParameters[a] = values[a].GetType();
                }
            }
            MethodInfo methodinfo = type.GetMethod(methodName, typeParameters);
           return (AbstractCriterion)methodinfo.Invoke(Property.ForName(forName), values);
        }

    }
}
