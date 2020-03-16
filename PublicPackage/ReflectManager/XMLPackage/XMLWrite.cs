using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ReflectManager.XMLPackage
{
    public class XMLWrite
    {
        public static void SetAttribute(String xmlPath, String tagName,int index , String valueName,String value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);
            XmlElement xe = (XmlElement)nodes[index];
            
            xe.SetAttribute("Text", value);
           
            xmlDoc.Save(xmlPath);
         //   xmlDoc.Save(xw);
         //写入文件

        }
        public static void SetTextAttribute(String xmlPath, String tagName, int index, String valueName, String value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);
            XmlElement xe = (XmlElement)nodes[index];
            xe.InnerText = value;
           

            xmlDoc.Save(xmlPath);
            //   xmlDoc.Save(xw);
            //写入文件

        }
    }
}
