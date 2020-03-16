using ExcelManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    class ZJDXZDMCustom
    {
        public static string XMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/ZJDXZDM.Excel.hbm.xml";
        public static ObservableCollection<ZJDXZDM> GetExcelToZJDXZDM(string path)
        {
            ObservableCollection<ZJDXZDM> list = ExcelUtils.GetExcelToObjectToObservableCollection<ZJDXZDM>(path, XMLPath);

            return list;
        }
    }
}
