using ExcelManager;
using ExcelManager.Model;
using NPOI.SS.UserModel;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    class NFCustom
    {
        public static string XMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/NF.Excel.hbm.xml";

        public static string ExcelWriteXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/NF.WriteExcel.xml";

        public static ObservableCollection<NF> GetExcelToNFS(ISheet sheet)
        {

            ObservableCollection<NF> list = ExcelUtils.GetExcelToObjectToObservableCollection<NF>(sheet, XMLPath);

            return list;
        }

        public static XMLTable GetWriteExcelXMLTable()
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(ExcelWriteXMLPath)[0];
            return xmlTable;
        }

        /// <summary>
        /// 根据户主 更新农房表信息
        /// </summary>
        /// <param name="nf"></param>
        /// <param name="hz"></param>
        internal static void UpdateNF_HZ(NF nf, JTCY hz)
        {
            if(hz == null)
            {
                return;
            }
            nf.QLRMC = hz.XM;
            nf.ZJHM = hz.GMSFHM;
        }
    }
}
