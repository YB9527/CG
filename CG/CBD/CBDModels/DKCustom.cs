using ExcelManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBD.CBDModels
{
   public class DKCustom
    {
        public static string ReadExcelXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "CBDModels/DK.ReadExcel.xml";
        public static IList<DK> ReadDKExcel(string path)
        {
            ObservableCollection<DK> dks = ExcelUtils.GetExcelToObjectToObservableCollection<DK>(path, ReadExcelXMLPath);
            return dks;
        }
    }
}
