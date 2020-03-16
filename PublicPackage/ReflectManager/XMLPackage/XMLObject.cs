using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReflectManager.XMLPackage
{
   public class XMLObject
    {
        public XMLObject()
        {
            Deafult = "";
            MethodInfos = new List<MethodInfo>();
            Parameters = new List<object[]>();
            Types = new List<Type[]>();
            MethoedName = new List<String>();
        }
        public String Key { get; set; }
        public IList<String> MethoedName { get; set; }

        public IList<MethodInfo> MethodInfos{ get; set; }
        public IList<object[]> Parameters { get; set; }
        public IList<Type[]> Types { get; set; }
        public String Deafult { get; set; }
        public IList<object> Entitys { get; set; }
        public XMLTable XmlTable { get; set; }
        public string Column { get;  set; }
    }
}
