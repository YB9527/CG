using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZaiJiDi.ZaiJiDiModel;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.Service;
using NPOI.SS.UserModel;
using Utils = MyUtils.Utils;
using ZaiJiDi.Pages.ZJDPage;

namespace ZaiJiDi.Controller
{
    public class ZJDController
    {
        private IZJDService zjdService = new ZJDService();

        public IList<string> CheckZJD(ZJDDataSourceViewModel model)
        {
            IList<string> errors;

            IList<JSYD> jsyds =  zjdService.GetJSYDByZJDDataSourceViewModel(model);
            if(MyUtils.Utils.CheckListExists(jsyds))
            {
                errors = zjdService.CheckZJD(jsyds, model);
                foreach(JSYD jsyd in jsyds)
                {
                    if(SaveZJDWorkbook(jsyd,model))
                    {
                        return errors;
                       
                    }
                }               
            }
            return null;
        }
        public IList<JSYD> GetJSYD(ZJDDataSourceViewModel model)
        {
           
            return zjdService.GetJSYDByZJDDataSourceViewModel(model);
        }

        private bool SaveZJDWorkbook(JSYD jsyd, ZJDDataSourceViewModel model)
        {
            bool flag = false;
            IList<NF> nfs = jsyd.NFs;
            IList<JTCY> jtcys = jsyd.HZs;
            IList<Floor> floors = jsyd.Floors;

            if(Utils.CheckListExists(nfs) && Utils.CheckListExists(jtcys) && Utils.CheckListExists(floors))
            {
                flag = true;
                ExcelManager.ExcelWrite.Save(jsyd.Row, model.JSYDTablePath);
                ExcelManager.ExcelWrite.Save(nfs[0].Row,model.NFTablePath);
                ExcelManager.ExcelWrite.Save(jtcys[0].Row, model.JTCYTablePath);
              
            }
            return flag;
        }

        public void ExportZJD(IList<JSYD> jsyds,ZJDExportDataViewModel model)
        {

            zjdService.ExportZJD(jsyds, model);
        }
    }
}
