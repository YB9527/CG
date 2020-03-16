using ExcelManager;
using FirstFloor.ModernUI.Presentation;
using NPOI.SS.UserModel;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class NF:NotifyPropertyChanged
    {
        public NF Clone()
        {
            return this.MemberwiseClone() as NF;
        }

        public  IRow Row { get; set; }
        public  int? OBJECTID { get; set; }
        public  int ZRZH { get; set; }
        public  int HH { get; set; }
        public  string ZDNUM { get; set; }
        public  string SJH { get; set; }
        public  string CJH { get; set; }
        public  string HJH { get; set; }

       
        private string syqzh;

        private static bool syqzhUpdateFlag = true;
        public string SYQZH
        {
            get
            {
                return syqzh;
            }
            set
            {
                if (syqzh != value)
                {
                    if (syqzhUpdateFlag)
                    {

                        if (JSYD != null)
                        {
                            syqzhUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {
                                nf.SYQZH = value;
                            }
                            syqzhUpdateFlag = true;
                        }

                    }
                    syqzh = value;
                    OnPropertyChanged("syqzh");
                }
            }
        }
        public  string YFCDAH { get; set; }
        public  string QLRMC { get; set; }
        public  string ZJHM { get; set; }
        public  string YQLR { get; set; }
        public  int ZCS { get; set; }
        public  string SZC { get; set; }
        public string cztjg;
        public  string CZTJG
        {
            get
            {
                return cztjg;
            }
            set
            {
                if(cztjg != value)
                {
                    cztjg = value;
                    OnPropertyChanged("cztjg");
                }
            }
        }
        private double cjzmj;
        public  Double CJZMJ
        {
            get
            {
                return cjzmj;
            }
            set
            {
                if (cjzmj != value)
                {
                    cjzmj = value;
                    if (JSYD != null)
                    {
                        double totalJZMJ = 0;
                        foreach (NF nf in JSYD.NFs)
                        {
                            totalJZMJ += nf.cjzmj;
                        }
                        JZMJ = Math.Round(totalJZMJ, 2);
                    }
                   
                    OnPropertyChanged("cjzmj");
                }
            }
        }
        public  Double CZYJZMJ { get; set; }
        public  Double CFTMJ { get; set; }
        public  string CFWYT { get; set; }
        public  string HX { get; set; }
        public  string HXJG { get; set; }
        public  string TDYT { get; set; }
        public  string TDDJ { get; set; }
        public  string FWLX { get; set; }
        public  string FWXZ { get; set; }
        private string jg;
        public  string JG
        {
            get
            {
                return cztjg+"结构";
            }
            set
            {
                jg = value;
            }
        }
        private double jzmj;
        private static bool jzmjUpdateFlag = true;
        public  Double JZMJ
        {
            get
            {
                return jzmj;
            }
            set
            {
                if (jzmj != value)
                {
                    if (jzmjUpdateFlag)
                    {

                        if (JSYD != null)
                        {
                            jzmjUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {

                                nf.JZMJ = value;

                            }
                            jzmjUpdateFlag = true;
                        }
                    }
                    jzmj = value;
                    OnPropertyChanged("jzmj");
                }
            }
        }
        public  int CS { get; set; }
        private double fzmj;
        public  Double FZMJ
        {
            get
            {
                return fzmj;
            }
            set
            {
                if(fzmj != value)
                {
                    fzmj = value;
                    OnPropertyChanged("fzmj");
                }
            }
        }
        public  string SCMJ { get; set; }
        public  string FWYT { get; set; }
        public  string GHYT { get; set; }
        public  string DJZL { get; set; }
        public  string CB { get; set; }
        public  string SQDJSY { get; set; }

        private string fwly;
        
        private static bool fwlyUpdateFlag = true;
        public  string FWLY
        {
            get
            {
                return fwly;
            }
            set
            {
                if(fwly != value)
                {
                    if(fwlyUpdateFlag)
                    {
                       
                        if(JSYD != null)
                        {
                            fwlyUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {

                                nf.FWLY = value;
                                
                            }
                            fwlyUpdateFlag = true;
                        }
                      
                    }
                    fwly = value;
                    OnPropertyChanged("fwly");
                }
            }
        }
      
        private string yjttdsyz;

        private static bool yjttdsyzUpdateFlag = true;
        public string YJTTDSYZ
        {
            get
            {
                return yjttdsyz;
            }
            set
            {
                if (yjttdsyz != value)
                {
                    if (yjttdsyzUpdateFlag)
                    {
                        if (JSYD != null)
                        {
                            yjttdsyzUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {
                                nf.YJTTDSYZ = value;
                            }
                            yjttdsyzUpdateFlag = true;
                        }
                    }
                    yjttdsyz = value;
                    OnPropertyChanged("yjttdsyz");
                }
            }
        }
       
        private string yfwsyqz;

        private static bool yfwsyqzUpdateFlag = true;
        public string YFWSYQZ
        {
            get
            {
                return yfwsyqz;
            }
            set
            {
                if (yfwsyqz != value)
                {
                    if (yfwsyqzUpdateFlag)
                    {

                        if (JSYD != null)
                        {
                            yfwsyqzUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {
                                nf.YFWSYQZ = value;
                            }
                            yfwsyqzUpdateFlag = true;
                        }

                    }
                    yfwsyqz = value;
                    OnPropertyChanged("yfwsyqz");
                }
            }
        }
        public  string YDSPCL { get; set; }
        public  Double SCMJTN { get; set; }
        public  Double SCQTJZMJ { get; set; }
        public  Double SCFTMJ { get; set; }
     
        private int jgrq;
        private static bool jgrqUpdateFlag = true;
        public int JGRQ
        {
            get
            {
                return jgrq;
            }
            set
            {
                if (jgrq != value)
                {
                    if (jgrqUpdateFlag)
                    {

                        if (JSYD != null)
                        {
                            jgrqUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {
                                nf.JGRQ = value;
                            }
                            jgrqUpdateFlag = true;
                        }

                    }
                    jgrq = value;
                    OnPropertyChanged("jgrq");
                }
            }
        }
        public  string ZL { get; set; }
       
        private string cqly;
        private static bool cqlyUpdateFlag = true;
        public string CQLY
        {
            get
            {
                return cqly;
            }
            set
            {
                if (cqly != value)
                {
                    if (cqlyUpdateFlag)
                    {

                        if (JSYD != null)
                        {
                            cqlyUpdateFlag = false;
                            foreach (NF nf in JSYD.NFs)
                            {
                                nf.CQLY = value;
                            }
                            cqlyUpdateFlag = true;
                        }

                    }
                    cqly = value;
                    OnPropertyChanged("cqly");
                }
            }
        }
        public  string QTGSD { get; set; }
        public  string QTGSN { get; set; }
        public  string QTGSX { get; set; }
        public  string QTGSB { get; set; }
        public  string DCY { get; set; }
        public  DateTime DCRQ { get; set; }
        public  string DCJS { get; set; }
        public  string SHY { get; set; }
        public  DateTime SHRQ { get; set; }
        public  string SHYJ { get; set; }
        public  string BZ { get; set; }

        public JSYD JSYD { get; set; }

        /// <summary>
        /// 导出农房表
        /// </summary> 
        /// <param name="nfs"></param>
        /// <param name="saveDirPath"></param>
        public static void ExportNFTable(IList<NF> nfs, string saveDirPath)
        {
            List<NF> allNF = new List<NF>();
            List<Floor> allFloor = new List<Floor>();
            foreach (NF nf in nfs)
            {
                //allNF.AddRange(nf.NFS);
                //allFloor.AddRange(nf.Floors);
            }
            Dictionary<int, XMLTable> xmlJtcyDkDic = XMLRead.GetXmlToXMLTabl("Model/NF.Excel.hbm.xml");
            XMLTable xmlTable = xmlJtcyDkDic[0];
            IWorkbook workbook = ExcelRead.ReadExcel("ChengDuGaoXingZaiJiDiModel/农房表模板.xls");
            ISheet sheet = workbook.GetSheetAt(0);
            ExcelWrite.WriteObjects(sheet, xmlTable, allNF);
            //导出分层表
            ExcelWrite.WriteObjects(workbook.GetSheetAt(1), XMLRead.GetXmlToXMLTabl("Model/Floor.Excel.hbm.xml")[0], allFloor);

            ExcelWrite.Save(workbook, saveDirPath + "\\农房表.xls");
        }

    }
}
