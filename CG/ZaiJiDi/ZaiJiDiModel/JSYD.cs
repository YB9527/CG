using ExcelManager;
using FirstFloor.ModernUI.Presentation;
using NPOI.SS.UserModel;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ZaiJiDi.ZaiJiDiModel
{
   
    public class JSYD:NotifyPropertyChanged
    {
        
        public JSYD Clone()
        {
            JSYD clone = this.MemberwiseClone() as JSYD;
           
         
            return clone;
        }
        public  IRow Row { get; set; }
       
        public JSYD()
        {
            
            this.HZs = new ObservableCollection<JTCY>();
        }
        public  int? OBJECTID { get; set; }

        public  int XH { get; set; }
        private string zdnum;
        public  string ZDNUM
        {
            get
            {
                return zdnum;
            }
            set
            {
                zdnum = value;
                OnPropertyChanged("zdnum");
            }
        }

        public  int ZH { get; set; }
        private string qlrmc;
        public  string QLRMC
        {
            get
            {
                return qlrmc;
            }
            set
            {
                qlrmc = value;
                OnPropertyChanged("qlrmc");
            }
        }
        public  string ZJHM { get; set; }
        public  string ZJLX { get; set; }
        public  string TDZL { get; set; }
        public  string YTDSYZSH { get; set; }
        public  string TDYT { get; set; }
        public  string TDDJ { get; set; }
        public  string QSRQ { get; set; }
        public  string ZZRQ { get; set; }
        public  string SYNX { get; set; }
        public  string TDJG { get; set; }
        public  string TDYT2 { get; set; }
        public  string TDDJ2 { get; set; }
        public  string QSRQ2 { get; set; }
        public  string ZZRQ2 { get; set; }
        public  string SYNX2 { get; set; }

        public  string TDJG2 { get; set; }
        public  string TDYT3 { get; set; }
        public  string TDDJ3 { get; set; }
        public  string QSRQ3 { get; set; }
        public  string ZZRQ3 { get; set; }
        public  string SYNX3 { get; set; }
        public  string TDJJG3 { get; set; }
        public  string QLLX { get; set; }
        public  string QLXZ { get; set; }
        public  string TFH { get; set; }
        public  string ZJR { get; set; }

        private double zdmj;
        public  double ZDMJ
        {
            get
            {
                return zdmj;
            }
            set
            {
                zdmj = value;
                CZMJ = zdmj - SYQMJ;
                OnPropertyChanged("zdmj");
            }
        }
        private double syqmj;
        public  double SYQMJ {
            get
            {
                return syqmj;
            }
            set
            {
                syqmj = value;
                CZMJ = ZDMJ - syqmj;
                OnPropertyChanged("syqmj");
            }
         }
        public  double DYMJ { get; set; }
        public  double FTMJ { get; set; }

        private double czmj;

        

        public  double CZMJ {
            get
            {
                return czmj;
            }
            set
            {
                czmj = value;
                OnPropertyChanged("czmj");
            }
        }
        public  double WFMJ { get; set; }
        public  double PZMJ { get; set; }
        public  double JZZDZMJ { get; set; }
        public  string SZDZ { get; set; }
        public  string SSNZ { get; set; }
        public  string SZXZ { get; set; }
        public  string SZBZ { get; set; }
        public  string PZYT { get; set; }
        public  string SJYT { get; set; }
        public  string PZRQ { get; set; }
        public  string SYQX { get; set; }
        public  string ZZRQ4 { get; set; }
        public  string QDJG { get; set; }
        public  double JZRJl { get; set; }
        public  double JZMD { get; set; }
        public  double JZXG { get; set; }
        public  double JZWZDM { get; set; }
        public  string JZWLX { get; set; }
        public  string SBJZWQS { get; set; }
        public  string ZDSM { get; set; }
        public  string ZYJZDWSM { get; set; }
        public  string ZYQSJXZXSM { get; set; }
        public  string ZDXBXZGYHQTNMJTTDFGRSM{ get; set; }
       public  string QTSJ { get; set; }
        public  string DCY{ get; set; }
        public  DateTime DCRQ{ get; set; }
        public  string DCJS{ get; set; }
        public  string KZY{ get; set; }
        public  DateTime KZRQ{ get; set; }
        public  string KZJS{ get; set; }
        public  string SHY{ get; set; }
        public  DateTime SHRQ{ get; set; }
        public  string SHYJ{ get; set; }
        private string bz;
        public string BZ
        {
            get
            {
                return bz;
            }
            set
            {
                bz = value;
                OnPropertyChanged("bz");
            }
        }
        public static void ExportJSYDTable(IList<JSYD> jsyds, string saveDirPath)
        {
            string jsydExcelPath = "ChengDuGaoXingZaiJiDiModel/建设用地表共用宗模板.xls";
            Dictionary<int, XMLTable> xmlJtcyDkDic = XMLRead.GetXmlToXMLTabl("Model/JSYD.Excel.hbm.xml");
            IWorkbook workbook = ExcelRead.ReadExcel(jsydExcelPath);
            ISheet sheet = workbook.GetSheetAt(0);
            ExcelWrite.WriteObjects<JSYD>(sheet, xmlJtcyDkDic[0], jsyds);
            ExcelWrite.Save(workbook, saveDirPath+"\\建设用地表.xls");
        }
        public Zdinfo Zdinfo { get; set; }

      
        public IList<NF> NFs { get; set; }
        public IList<Floor> Floors { get; set; }
        public ZJDXZDM ZJDXZDM { get; set; }

        public IList<QZB> QZBs { get; set; }
        public IList<JTCY> HZs { get; set; }
        public IList<JZXInfo> JZXInfos { get; set; }
        private string gyfs;
        public string GYFS
        {
            get
            {
               
                if (HZs.Count >1)
                {
                    gyfs = "共同共有";
                    return gyfs;
                }
                foreach(JTCY hz in HZs)
                {
                    if(hz.JTCies.Count >1)
                    {
                        gyfs = "共同共有";
                        return gyfs;
                    }
                }
                gyfs = "单独所有";
                return gyfs;
            }
            set
            {
                gyfs = value;
            }
        }
    }
}
