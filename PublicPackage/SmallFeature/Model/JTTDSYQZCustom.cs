using ExcelManager;
using ExcelManager.Model;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallFeature.Model
{
    public class JTTDSYQZCustom
    {
        public static string ExcelRelect = System.AppDomain.CurrentDomain.BaseDirectory + "SmallFeatureConfig\\集体土地所有权证ExcelReflect.xml";
        public static string DocxTemplete = System.AppDomain.CurrentDomain.BaseDirectory + "SmallFeatureConfig\\集体土地所有权证模板.docx";
        public static IList<JTTDSYQZ> GetJTTDSYQZS(string excelPath)
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(ExcelRelect)[0];
            ExcelReflectModel<JTTDSYQZ> model = new ExcelReflectModel<JTTDSYQZ>(excelPath, xmlTable);
            IList<JTTDSYQZ> list = ExcelUtils.SheetToObjectsByXMLTable<JTTDSYQZ>(model);
            return list;
        }
    }
}
