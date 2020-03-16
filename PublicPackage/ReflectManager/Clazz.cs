using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using MyUtils;
using System.Text.RegularExpressions;

namespace ReflectManager
{
    public class Clazz
    {


        public String getFullClassName()
        {

            return fullClassName;
        }


        public void setFullClassName(String fullClassName)
        {
            this.fullClassName = fullClassName;
        }
        public String getFunction()
        {
            return function;
        }
        public void setFunction(String function)
        {
            int a = 1 + 1;
            this.function = function;
        }
        public Type getParamterType()
        {
            return paramterType;
        }
        public void setParamterType(Type paramterType)
        {
            this.paramterType = paramterType;
        }
        public Type getResultType()
        {
            return resultType;
        }
        public void setResultType(Type resultType)
        {
            this.resultType = resultType;
        }

        //类名
        private String fullClassName;
        //函数名
        private String function;
        //参数类型
        private Type paramterType;
        //返回值
        private Type resultType;
        public MethodInfo GetMethodInfo { get; set; }
        public MethodInfo SetMethodInfo { get; set; }

        public void SetValue(object obj, JToken jToken)
        {
            if (jToken == null)
            {
                return;
            }

            object[] paramters = new object[1];
            object value = GetTrueValueType(jToken);
            if (value != null && !value.Equals(""))
            {
                paramters[0] = value;
                this.SetMethodInfo.Invoke(obj, paramters);
            }
        }
        public Object GetTrueValueType(JToken jToken)
        {
           return JObejctReflect.GetTrueValueType(jToken, this.getParamterType().ToString());
            
        }

}
}
