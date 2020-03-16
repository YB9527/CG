using MyUtils;
using NPOI.SS.UserModel;
using ReflectManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelManager.Model
{
    public class ExcelReflectModel<T>
    {

        public ISheet Sheet { get; set; }
        public Dictionary<int, Clazz> TitleDic { get; set; }
        public Dictionary<string, Clazz> ClazzDic { get; set; }
        public Dictionary<string ,string> PropertDic { get; set; }
        public Dictionary<int, XMLObject> XMLObjectDic { get; set; }
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }

        public ExcelReflectModel(string excelPath, XMLTable xmlTable)
        {

            Sheet = ExcelRead.ReadExcelSheet(excelPath, xmlTable.TableIndex);
            Init(Sheet,xmlTable);
        }
        public ExcelReflectModel(string excelPath,string sheetName, XMLTable xmlTable)
        {

            Sheet = ExcelRead.ReadExcelSheet(excelPath, sheetName);
            Init(Sheet, xmlTable);
        }
        public ExcelReflectModel(ISheet sheet, XMLTable xmlTable)
        {
            this.Sheet = sheet;
            Init(sheet, xmlTable);
        }
        public void Init(ISheet sheet, XMLTable xmlTable)
        {
            
            Sheet = sheet;
            //this.TitelRowIndex = TitelRowIndex;
            this.ClazzDic = ReflectUtils.MethodToFunction<T>();
            //this.TitleDic = GetTitleDic(Sheet.GetRow(TitelRowIndex), this.PropertDic, this.ClazzDic);
            this.LastRowIndex = Sheet.LastRowNum - xmlTable.ToRow;
            this.StartRowIndex = xmlTable.RowStartIndex;
            this.EndRowIndex = xmlTable.RowEndIndex;
            this.XMLObjectDic = xmlTable.CellDic;
        }
     



        public ExcelReflectModel(ISheet Sheet, string xmlPath, int TitelRowIndex = 0, int StartRowIndex = 1, int EndRowIndex = -1)
        {
          
            if (!Utils.CheckFileExists(xmlPath))
            {
                throw new Exception("文件不存在：" + xmlPath);
            }

            this.Sheet = Sheet;
            this.PropertDic = XMLRead.GetConfigXmlDic(xmlPath, "property", "name", "column");
            //this.TitelRowIndex = TitelRowIndex;
            this.ClazzDic = ReflectUtils.MethodToFunction<T>();
            this.TitleDic = GetTitleDic(Sheet.GetRow(TitelRowIndex), this.PropertDic, this.ClazzDic);
            if (EndRowIndex == -1)
            {
                this.LastRowIndex = Sheet.LastRowNum;
            }
            else
            {
                this.LastRowIndex = EndRowIndex;
            }

            this.StartRowIndex = StartRowIndex;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="TitelRowIndex"></param>
        /// <param name="EndRowIndex">此行要读取，默认是最后一行</param>
        public ExcelReflectModel(string excelPath , string path,int sheetIndex = 0,int TitelRowIndex = 0, int StartRowIndex = 1,int EndRowIndex =-1)
        {
            if (!Utils.CheckFileExists(excelPath) )
            {
                throw new Exception("文件不存在："+ excelPath);
            }
            if (!Utils.CheckFileExists(path))
            {
                throw new Exception("文件不存在：" + path);
            }

            Sheet = ExcelRead.ReadExcelSheet(excelPath, sheetIndex);
            this.PropertDic = XMLRead.GetConfigXmlDic(path, "property", "name", "column");
            //this.TitelRowIndex = TitelRowIndex;
            this.ClazzDic = ReflectUtils.MethodToFunction<T>();
            this.TitleDic = GetTitleDic(Sheet.GetRow(TitelRowIndex), this.PropertDic, this.ClazzDic); 
            if(EndRowIndex == -1)
            {
                this.LastRowIndex = Sheet.LastRowNum;
            }
            else
            {
                this.LastRowIndex = EndRowIndex;
            }
         
            this.StartRowIndex = StartRowIndex;


        }

      

        private Dictionary<int, Clazz> GetTitleDic(IRow row, Dictionary<string, string> propertDic, Dictionary<string, Clazz> clazzDic)
        {
            TitleDic = new Dictionary<int, Clazz>();
            string value;
           
            Dictionary< int, string> rowDic = new Dictionary<int,string>();
            foreach (ICell cell in row)
            {
                if(cell == null)
                {
                    continue;
                }
                cell.SetCellType(CellType.String);
                value = cell.StringCellValue;
                if(!Utils.IsStrNull(value))
                {
                    rowDic.Add(cell.ColumnIndex, value);
                    
                }
            }

            return GetTitleDic(rowDic, propertDic, clazzDic); ;
        }

        private Dictionary<int, Clazz> GetTitleDic(Dictionary<int, string> rowDic, Dictionary<string, string> propertDic, Dictionary<string, Clazz> clazzDic)
        {
            string value;
            string tempValue;
            Clazz clazz;
            foreach (int i in rowDic.Keys)
            {
                value = rowDic[i];
                if (PropertDic.TryGetValue(value, out tempValue))
                {
                    if (clazzDic.TryGetValue(tempValue, out clazz))
                    {
                        TitleDic.Add(i, clazz);
                    }
                }
            }
            return TitleDic;
        }

        public int TitelRowIndex { get; set; }
        public int LastRowIndex { get; set; }
    }
}
