using MyUtils;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelManager.Model
{
    public class ExcelJsonModel
    {
        public ISheet Sheet { get; set; }
        public Dictionary<int, string> TitleDic { get; set; }
        
        public int StartRowIndex { get; set; }
        public int TitelRowIndex { get; set; }
        public int LastRowIndex { get; set; }


        public ExcelJsonModel(string excelPath, int TitelRowIndex=0, int StartRowIndex=1,int LastRowIndex=-1,int sheetIndex =0)
        {
            if (!Utils.CheckFileExists(excelPath))
            {
                throw new Exception("文件不存在：" + excelPath);
            }

            Sheet = ExcelRead.ReadExcelSheet(excelPath, sheetIndex);
            if (LastRowIndex == -1)
            {
                this.LastRowIndex = Sheet.LastRowNum;
            }
            else
            {
                this.LastRowIndex = LastRowIndex;
            }
            this.TitelRowIndex = TitelRowIndex;
            this.StartRowIndex = StartRowIndex;
        }
        public IList<JObject> GetJObjects()
        {
            Dictionary<int, string> titleDic = ExcelRead.GetRowDic(Sheet.GetRow(TitelRowIndex));
            return  ExcelUtils.RowsToJObjects(titleDic, ExcelRead.GetRows(Sheet, StartRowIndex, LastRowIndex));
        }
    }
}
