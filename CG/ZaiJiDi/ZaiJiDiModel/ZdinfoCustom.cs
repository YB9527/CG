using ReflectManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using Utils = MyUtils.Utils;
namespace ZaiJiDi.ZaiJiDiModel
{
    public class ZdinfoCustom
    {
        

        public static ObservableCollection<Zdinfo> GetMDBToZdinfos(string mdbPath)
        {

            //IList<Zdinfo> list = ExcelUtils.GetExcelToObject<NF>(path, XMLPath);
            ObservableCollection<Zdinfo> list =Utils.ListToObservableCollection( MDBUtils.ReadAllData<Zdinfo>("Zdinfo", mdbPath));
            return list;
        }


        

    }
}
