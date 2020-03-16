using ExcelManager;
using MyUtils;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using ReflectManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class JTCY
    {
        
        public JTCY Clone()
        {
            return this.MemberwiseClone() as JTCY;
        }
        public virtual IRow Row { get; set; }
        public virtual int? OBJECTID { get; set; }
        public virtual string ZH { get; set; }
        public virtual string SFGYR { get; set; }
        public virtual string CBFLX { get; set; }
        public virtual string HH { get; set; }
        
        public virtual string XM { get; set; }
        public virtual string CYZJLX { get; set; }
       
        public virtual string GMSFHM { get; set; }
    
        public virtual string LXDH { get; set; }
        private string sex;
        /// <summary>
        /// 首先使用身份证来判断
        /// </summary>
        public virtual string Sex { get
            {
                if(GMSFHM != null && GMSFHM.Length == 18)
                {
                    int tem = 0;
                    if(int.TryParse(GMSFHM.Substring(16, 1),out tem))
                    {
                        sex = tem % 2 == 0 ? "女" : "男";
                    }
                     
                }
                return sex;
            }
            set
            {
                sex = value;
            }
        }
        public virtual string MZ { get; set; }
        public virtual DateTime Birthday { get; set; }
        public virtual int NL { get; set; }
        public virtual string Address { get; set; }
        public virtual string BZ { get; set; }
        public virtual string CBFBM { get; set; }
        public virtual string YHZGX { get; set; }
        public virtual string QLRLX { get; set; }
        public virtual string GZDW { get; set; }
        public virtual string Email { get; set; }
        public virtual string TXDZ { get; set; }
        public virtual string YZBM { get; set; }
        public virtual string DCY { get; set; }
        public virtual DateTime DCRQ { get; set; }
        public virtual string DCJS { get; set; }
        public virtual string SHR { get; set; }
        public virtual DateTime SHRQ { get; set; }
        public virtual string SHYJ { get; set; }
        public virtual IList<JTCY> JTCies { get; set; }

        public static void ExportJTCYTable(IList<JTCY> hzs, string saveDirPath)
        {
            Dictionary<int, XMLTable> xmlJtcyDkDic = XMLRead.GetXmlToXMLTabl("Model/JTCY.Excel.hbm.xml");
            XMLTable xmlTable = xmlJtcyDkDic[0];
            IWorkbook workbook = ExcelRead.ReadExcel("ChengDuGaoXingZaiJiDiModel/家庭成员表模板.xls");
            ISheet sheet = workbook.GetSheetAt(0);
            List<JTCY> jtcys = new List<JTCY>();
            foreach(JTCY hz in hzs)
            {
                jtcys.AddRange(hz.JTCies);
            }
            ExcelWrite.WriteObjects(sheet, xmlTable, jtcys);
            //合并户主的单元格
            int rowIndex = xmlTable.RowStartIndex;
            foreach (JTCY hz in hzs)
            {
                ExcelWrite.SetValue(sheet.GetRow(rowIndex), 0, rowIndex - 4 +"");
                ExcelWrite.SetValue(sheet.GetRow(rowIndex), 1, hz.XM);
                int lastRow = rowIndex+hz.JTCies.Count;
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, lastRow-1, 0, 0));
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, lastRow-1, 1, 1));
                rowIndex = lastRow;
            }

            ExcelWrite.Save(workbook, saveDirPath + "\\家庭成员表.xls");

        }

        
    }
}
