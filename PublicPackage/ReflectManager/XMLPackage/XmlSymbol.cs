using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReflectManager.XMLPackage
{
    public class XMLSymbol
    {
        public XMLSymbol()
        {
            RepDic = new Dictionary<string, string>();
        }
        public Dictionary<string, string> RepDic;
        public  XMLObject XmlObject { get; set; }
        public string Result { get; set; }
    }
}
