using ReflectManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WordManager;

namespace ZaiJiDi.ZaiJiDiModel
{
    class QZBCustom
    {
        public static string QZBTableName= "QZB";
        public static ObservableCollection<QZB> GetMDBToQZB(string mdbPath)
        {
            ObservableCollection<QZB> list = MDBUtils.ReadAllDataToObservableCollection<QZB>(QZBTableName, mdbPath);
         
            return list;
        }

        public static IList<QZB> DocToDaAnDaiQZB(string docPath, string zdnum)
        {

            return null;
        }
        
    }
}

