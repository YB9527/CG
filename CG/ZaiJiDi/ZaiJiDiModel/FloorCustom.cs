using ExcelManager;
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
    public class FloorCustom
    {
        public static string XMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/Floor.Excel.hbm.xml";
        public static string ExcelWriteXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/Floor.WriteExcel.xml";

       



        /// <summary>
        /// excel 转换为jtcy 对象 
        /// </summary>
        /// <param name="path">Excel路径</param>
        /// <returns></returns>
        public static ObservableCollection<Floor> GetExcelToFloors(ISheet sheet)
        {

            ObservableCollection<Floor> list = ExcelUtils.GetExcelToObjectToObservableCollection<Floor>(sheet, XMLPath);
          
            return list;

        }

        /// <summary>
        /// 合计建筑总面积、 并保留两位
        /// </summary>
        /// <param name="floors"></param>
        /// <returns></returns>
        public static double GetJZMJTotal(IList<Floor> floors)
        {
            double area = 0;
            foreach(Floor floor in floors)
            {
                area += floor.CJZMJ;
            }
            return Math.Round(area, 2);
        }

        public static XMLTable GetWriteExcelXMLTable()
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(ExcelWriteXMLPath)[0];
            return xmlTable;
        }
    }
}
