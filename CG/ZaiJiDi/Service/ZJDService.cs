using ExcelManager;
using NPOI.SS.UserModel;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaiJiDi.Dao;
using ZaiJiDi.Pages.ZJDPage;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.ZaiJiDiModel;
using Utils = MyUtils.Utils;

namespace ZaiJiDi.Service
{
    public class ZJDService:IZJDService
    {
        private  IZJDDao zjdDao = new ZJDDao();

        public IList<string> CheckZJD(IList<JSYD> jsyds, ZJDDataSourceViewModel model)
        {
            List<string> errors = new List<string>();
            foreach(JSYD jsyd in jsyds)
            {
             
                IList<string> error = zjdDao.CheckZJD(jsyd, model);
                foreach(string str in error)
                {
                    errors.Add("宗地编码是 "+ jsyd.ZDNUM+" :"+str);
                }
              
            }
            return errors;
        }

       

        public IList<JSYD> GetJSYDByZJDDataSourceViewModel(ZJDDataSourceViewModel model)
        {
           
            ObservableCollection<ZJDXZDM> zjdXZDM = ZJDXZDMCustom.GetExcelToZJDXZDM(model.ZJDXZDMTablePath);
            ObservableCollection<JTCY> hzs = JTCYCustom.GetExcelToHZS(model.JTCYTablePath);
            //户主必须要先检查
            zjdDao.ChecSFZ(hzs, model);

            ObservableCollection<JSYD> jsyds = JSYDCustom.GetExcelToJSYDS(model.JSYDTablePath);

            IWorkbook workbook = ExcelRead.ReadExcel(model.NFTablePath);
            ObservableCollection<NF> nfs = NFCustom.GetExcelToNFS(workbook.GetSheetAt(0));
            ObservableCollection<Floor> floors = FloorCustom.GetExcelToFloors(workbook.GetSheetAt(1));

            ObservableCollection<Zdinfo> zdinfos = ZdinfoCustom.GetMDBToZdinfos(model.ZdinfoMDBPath);
            Dictionary<string, JSYD> jsydDic = MyUtils.Utils.GetGroupDic("ZDNUM", jsyds);
            JSYDCustom.SetZdinfo(jsydDic, zdinfos);

            ObservableCollection<JSYD> gyrs = JSYDCustom.GetExcelToGYRS(model.JSYDTablePath);
            JSYDCustom.SetHZs(jsyds, gyrs, hzs);
            JSYDCustom.SetNF(jsydDic, nfs);
            JSYDCustom.SetFloor(jsydDic, floors);
            JSYDCustom.SetZJDXZDM(jsydDic, zjdXZDM);

            ObservableCollection<QZB> qzbs = QZBCustom.GetMDBToQZB(model.QZ_BSMDBPath);
            JSYDCustom.SetQZBs(jsyds, qzbs);
            ObservableCollection<JZXInfo> jzxs = JZXInfoCustom.GetMDBToJZX(model.QZ_BSMDBPath);
            JSYDCustom.SetJZXs(jsyds, jzxs);
            return jsyds;
        }

        public  void ExportZJD(IList<JSYD> jsyds, ZJDExportDataViewModel model)
        {

            foreach(JSYD jsyd in jsyds)
            {
                if(Utils.CheckListExists(jsyd.HZs) && Utils.CheckListExists(jsyd.NFs) && Utils.CheckListExists(jsyd.Floors))
                {
                    ExportZJD(jsyd, model);
                }
            }
        }

        private void ExportZJD(JSYD jsyd, ZJDExportDataViewModel model)
        {
            string saveDir = model.SaveDir;


          
            //档案袋
            if (model.IsDangAnDai)
            {
                //Type type = jsyd.JZXInfos.GetType();
                //Type typeA = Type.GetType(type.AssemblyQualifiedName);
                //Console.WriteLine(type.AssemblyQualifiedName);
                zjdDao.ExportZJD_DangAnDai(jsyd, saveDir);
            }
            //审批表
            if (model.IsSPB)
            {
                zjdDao.ExportZJD_SPB(jsyd, saveDir);
            }
            //委托书
            if (model.IsWTS)
            {
                zjdDao.ExportZJD_WTS(jsyd, saveDir);
            }
            //声明书
            if (model.IsSMS)
            {
                zjdDao.ExportZJD_SMS(jsyd, saveDir);
            }
            //权籍调查表
            if (model.IsQJDCB)
            {
                if(Utils.CheckListExists(jsyd.QZBs))
                {
                    if (Utils.CheckListExists(jsyd.JZXInfos))
                    {
                        zjdDao.ExportZJD_QJDCB(jsyd, saveDir);
                    }
                }
            }
            //测绘报告
            if (model.IsCHBG)
            {
                zjdDao.ExportZJD_CHBG(jsyd, saveDir);
            }
        }
    }
}
