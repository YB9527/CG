using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using System.IO;
using System.Data;

using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using FileManager;
using MyUtils;
using Newtonsoft.Json.Linq;
using ExcelManager.Model;
using ReflectManager;
using System.Reflection;
using ReflectManager.XMLPackage;
using ExcelManager.Pages;
using Spire.Xls;
using System.Windows;
using ExcelManager.Pages.Replace;

namespace ExcelManager
{
    public class ExcelWrite
    {
        /// <summary>
        ///  保存excel
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Save(IWorkbook workbook, string path)
        {
            if (workbook == null)
            {
                return false;
            }
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileStream filestream = new FileStream(path, FileMode.Create);
            workbook.Write(filestream);
            filestream.Close();
            return true;
        }





        public static void SetValue(IRow row, int cellIndex, string value)
        {
            if (row != null)
            {
                ICell cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    cell = row.CreateCell(cellIndex);
                }
                cell.SetCellValue(value);
            }
        }

        public static void Save(IRow row, string path)
        {
            Save(row.Sheet.Workbook, path);
        }

        /// <summary>
        /// sheet合并到一个Excel中
        /// </summary>
        /// <param name="model"></param>
        public static void MergeExcel(ExcelMergeModel model)
        {
            string saveName = model.SaveName;
            if (saveName == null)
            {
                MessageBox.Show("保存路径没有填写！！！");

                return;
            }
            Workbook newbook = new Workbook();
         
            //newbook.SaveToFile("d:/123.xls", ExcelVersion.Version2013);

            Spire.Xls.Workbook tempbook = null;
            //创建一个新的workbook对象
            newbook.Version = Spire.Xls.ExcelVersion.Version2013;

           
            //删除文档中的工作表（新创建的文档默认包含3张工作表）
            newbook.Worksheets.Clear();
           if(newbook.Worksheets.Count == 0)
            {
                newbook.CreateEmptySheet();
            }
            newbook.SaveToFile(saveName, ExcelVersion.Version2013);
            IList<FileNameCustom> fileNameCustoms = model.Files;
            if (model.MergeModel == MergeSytle.MoreSheet)
            {
                tempbook = new Workbook();
                foreach (FileNameCustom custom in fileNameCustoms)
                {

                    tempbook.LoadFromFile(custom.FilePath);

                    //使用AddCopy方法，将文档中的所有工作表添加到新的workbook
                    foreach (Worksheet sheet in tempbook.Worksheets)
                    {
                        newbook.Worksheets.AddCopy(sheet);
                    }
                }
                newbook.SaveToFile(saveName, ExcelVersion.Version2013);
            }
            else if (model.MergeModel == MergeSytle.OneSheet)
            {
                //实例化一个Workbook类，加载Excel文档

                for (int index = 0; index < fileNameCustoms.Count; index++)
                {

                    FileNameCustom fileNameCustom = fileNameCustoms[index];
                    tempbook = new Workbook();
                    try
                    {
                        tempbook.LoadFromFile(fileNameCustom.FilePath);
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("文件有问题，可能有批注："+fileNameCustom.FilePath);
                        continue;
                    }
               
                    for (int a = 0; a < tempbook.Worksheets.Count; a++)
                    {
                        //获取第1、2张工作表
                        Worksheet sheet1 = tempbook.Worksheets[a];
                        if (newbook.Worksheets.Count <= a)
                        {
                            newbook.CreateEmptySheet(sheet1.Name);
                        }
                        Worksheet newsheet = newbook.Worksheets[a];

                        //复制第2张工作表内容到第1张工作表的指定区1域中
                        CellRange range1 = sheet1.AllocatedRange;
                        if (range1.RowCount > model.ReduceStartRowCount + model.ReduceEndRowCount+1)
                        {
                            //CellRange range = sheet1.Range[model.ReduceStartRowCount + 1,  range1.CellsCount, range1.RowCount - model.ReduceEndRowCount, range1.CellsCount];
                            int cellCount = range1.CellsCount;
                            if(cellCount > 256)
                            {
                                cellCount = 254;
                            }
                            int rowCount = range1.RowCount;
                            if(rowCount > 20000)
                            {
                                rowCount = 20000;
                            }
                            CellRange range = sheet1.Range[model.ReduceStartRowCount + 1, 1, rowCount - model.ReduceEndRowCount, cellCount];
                            if (newsheet.LastRow == -1)
                            {
                                range.Copy(newsheet.Range[newsheet.LastRow + 2, 1]);
                                try
                                {
                                   
                                }
                                catch(Exception e)
                                {
                                    MessageBox.Show("文件有问题：" + fileNameCustom.FilePath);
                                }
                               
                            }
                            else
                            {
                                range.Copy(newsheet.Range[newsheet.LastRow + 1, 1]);
                                try
                                {
                                  
                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("非常严重问题：" + fileNameCustoms[index-1].FilePath);
                                    return;
                                }
                              
                            }
                           
                        }
                    }
                    //break;
                }
                //保存并运行文档
                newbook.SaveToFile(saveName, ExcelVersion.Version2013);
            }

        }

        public static void ReplaceText(RepalcePageViewModel model, IList<FileNameCustom> fileNameCustoms)
        {
            if (!Utils.CheckListExists(fileNameCustoms))
            {
                return;
            }
            IList<ReplaceViewModel> replaceModel = model.ReplaceViewModels;

            foreach (FileNameCustom fileNameCustom in fileNameCustoms)
            {
                string path = fileNameCustom.FilePath;

                if (Utils.CheckFileExists(path))
                {
                    Workbook workbook = new Spire.Xls.Workbook();
                    workbook.LoadFromFile(path);
                    //存在文件就替换
                    if (model.AllSheet)
                    {
                        //全部页面替换
                        ReplaceWorkbook(workbook, model.ReplaceViewModels);
                    }
                    else
                    {
                        int sheetIndex = model.ReplaceSheetIndex;
                        if (workbook.Worksheets.Count > sheetIndex)
                        {
                            //指定的sheet替换
                            ReplaceSheet(workbook.Worksheets[sheetIndex], model.ReplaceViewModels);
                        }
                        
                    }
                    workbook.Save();
                }
            }
        }

    

        /// <summary>
        /// workbook全部替换内容
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="replaceViewModels"></param>
        public static void ReplaceWorkbook(Workbook workbook, IList<ReplaceViewModel> replaceViewModels)
        {

            double oldnum;
            double newnum;
            CellRange[] ranges;
            foreach (ReplaceViewModel model in replaceViewModels)
            {
                
                string oldText = model.OldText;
                string newText = model.NewText;
                if (double.TryParse(oldText, out oldnum))
                {  //数字
                    ranges = workbook.FindAllNumber(oldnum, false);
                    if (ranges!= null &&　ranges.Length != 0)
                    {
                        if (double.TryParse(newText, out newnum))
                        {
                            foreach (CellRange range in ranges)
                            {
                                switch(model.StrRelation)
                                {
                                    case StrRelation.StartsWith:
                                        break;
                                    case StrRelation.Contains:
                                        break;
                                    case StrRelation.EndsWith:
                                        break;
                                    case StrRelation.Equals:
                                        break;
                                }
                                range.NumberValue = double.Parse(range.NumberText.Replace(oldText, newText));
                            }
                        }
                        else
                        {
                            foreach (CellRange range in ranges)
                            {
                                range.Value2 = range.NumberText.Replace(oldText, newText);
                            }
                        }
                    }
                }

                    //文字
                    ranges = workbook.FindAllString(oldText, false, false);
                    if (ranges != null && ranges.Length != 0)
                    {

                        foreach (CellRange range in ranges)
                        {
                            range.Value2 = range.Text.Replace(oldText, newText);
                        }

                    }
                
            }
        }

        /// <summary>
        /// 对象写入 row
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="row"></param>
        /// <param name="xmlDic"></param>
        public static void WriteRowObject(object obj,IRow row, Dictionary<int, XMLObject> xmlDic)
        {
            foreach(int index in xmlDic.Keys)
            {
                XMLObject xmlObject = xmlDic[index];
                object result = XMLRead.GetObjectMethodResult(xmlObject, obj);
                SetValue(row, index, result);
            }
        }

        /// <summary>
        /// 替换内容
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="replaceViewModels"></param>
        public static void ReplaceSheet(Worksheet worksheet, IList<ReplaceViewModel> replaceViewModels)
        {
            
            double oldnum;
            double newnum;
            CellRange[] ranges;
            foreach (ReplaceViewModel model in replaceViewModels)
            {
                string oldText = model.OldText;
                string newText = model.NewText;
                if(double.TryParse(oldText,out oldnum))
                {  //数字
                    ranges = worksheet.FindAllNumber(oldnum, false);
                    if(ranges.Length != 0)
                    {
                        if(double.TryParse(newText, out newnum))
                        {
                            foreach (CellRange range in ranges)
                            {
                              range.NumberValue = double.Parse(range.NumberText.Replace(oldText, newText));
                            }
                        }else
                        {
                            foreach (CellRange range in ranges)
                            {
                                range.Value2 = range.NumberText.Replace(oldText, newText);
                            }
                        }
                    }
                }else
                {
                    //文字
                    ranges = worksheet.FindAllString(oldText, false,false);
                    if (ranges.Length != 0)
                    {
                        
                        foreach (CellRange range in ranges)
                        {
                            range.Value2 = range.Text.Replace(oldText, newText);
                        }
                        
                    }
                }
            }
        }
        public static void SetRowValue<T>(object obj, ExcelReflectModel<T> reflectModel, IRow row)
        {
            object value;
            Dictionary<int, Clazz> clazzDic = reflectModel.TitleDic;
            foreach (int index in clazzDic.Keys)
            {
                ICell cell = row.GetCell(index);
                if (cell == null)
                {
                    cell = row.CreateCell(index);

                }
                value = clazzDic[index].GetMethodInfo.Invoke(obj, null);
                if (value != null)
                {
                    cell.SetCellValue(value.ToString());
                }

            }
        }



        public static void CreateRows(ISheet sheet, int modelIndex, int cellTotal, int start, int rowsCount)
        {
            
            sheet.ShiftRows(start + 1, sheet.LastRowNum + 1, rowsCount);
            IRow rowModel = sheet.GetRow(start);
            IRow row;
            ICell cell;
            for (int a = 0; a < rowsCount; a++)
            {

                row = sheet.CreateRow(start + 1 + a);
                for (int b = 0; b < cellTotal; b++)
                {

                    cell = row.CreateCell(b);
                    ICell cellModel = rowModel.GetCell(b);
                    if (cellModel != null)
                    {
                        cell.CellStyle = cellModel.CellStyle;
                        //cell.SetCellValue(cellModel.StringCellValue);
                    }
                }
            }

        }
        /// <summary>
        /// 行格式复制 
        /// </summary>
        /// <param name="rowModel"></param>
        /// <param name="row"></param>
        public static void CopyRowStyle(IRow rowModel, IRow row)
        {
            ICell cellModel;
            ICell cell;
            for (int a =  0; a <= rowModel.LastCellNum;a++)
            {
               
                 cellModel = rowModel.GetCell(a);

                if (cellModel != null)
                {
                    cell = row.GetCell(a);
                    if (cell == null)
                    {
                        cell = row.CreateCell(a);
                    }
                    cell.CellStyle = cellModel.CellStyle;

                }
                   
            }
        }

        public static int CreateTitleByDataRow(IRow row, DataColumnCollection columns)
        {
            if (columns == null)
            {
                return -1;
            }
            int count = columns.Count;
            if (count == 0)
            {
                return -1;
            }
            for (int a = 0; a < count; a++)
            {
                DataColumn dataColumn = columns[a];
                row.CreateCell(a).SetCellValue(dataColumn.ColumnName);
            }
            return count;

        }

        public static void SetValueByDataRow(IRow row, DataRow dataRow)
        {

            object[] itemArray = dataRow.ItemArray;
            object value;
            for (int a = 0; a < itemArray.Length; a++)
            {
                value = itemArray[a];
                if (value != null)
                {
                    row.CreateCell(a).SetCellValue(value.ToString());
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="cellStart"></param>
        /// <param name="jsonStr"></param>
        /// <returns></returns>

        public static void SetValueByJsonValue(IRow row, int cellStart, string jsonStr)
        {
            JObject jo = JObject.Parse(jsonStr);
            object value;
            foreach (var pair in jo)
            {
                value = pair.Value;
                if (value != null)
                {
                    row.CreateCell(cellStart++).SetCellValue(value.ToString());
                }

            }

        }

        public static IWorkbook CreateWorkbookByExtension(string extensionName)
        {
            if (extensionName.Contains("xlsx"))
            {
                return new XSSFWorkbook();
            }
            return new HSSFWorkbook();
        }
        public static void CopyRow(IWorkbook workbook, ISheet worksheet, int sourceRowNum, int destinationRowNum)
        {
            // Get the source / new row
            IRow newRow = worksheet.GetRow(destinationRowNum);
            IRow sourceRow = worksheet.GetRow(sourceRowNum);

            // If the row exist in destination, push down all rows by 1 else create a new row
            if (newRow != null)
            {
                worksheet.ShiftRows(destinationRowNum, worksheet.LastRowNum, 1);
            }
            else
            {
                newRow = worksheet.CreateRow(destinationRowNum);
            }

            // Loop through source columns to add to new row
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                ICell oldCell = sourceRow.GetCell(i);
                ICell newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                // Copy style from old cell and apply to new cell
                ICellStyle newCellStyle = workbook.CreateCellStyle();
                newCellStyle.CloneStyleFrom(oldCell.CellStyle); ;
                newCell.CellStyle = newCellStyle;

                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case CellType.Blank:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case CellType.Error:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case CellType.Formula:
                        newCell.SetCellFormula(oldCell.CellFormula);
                        break;
                    case CellType.Numeric:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case CellType.String:
                        newCell.SetCellValue(oldCell.RichStringCellValue);
                        break;
                    case CellType.Unknown:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
            }

            // If there are are any merged regions in the source row, copy to new row
            for (int i = 0; i < worksheet.NumMergedRegions; i++)
            {
                CellRangeAddress cellRangeAddress = worksheet.GetMergedRegion(i);
                if (cellRangeAddress.FirstRow == sourceRow.RowNum)
                {
                    CellRangeAddress newCellRangeAddress = new CellRangeAddress(newRow.RowNum,
                                                                                (newRow.RowNum +
                                                                                 (cellRangeAddress.FirstRow -
                                                                                  cellRangeAddress.LastRow)),
                                                                                cellRangeAddress.FirstColumn,
                                                                                cellRangeAddress.LastColumn);
                    worksheet.AddMergedRegion(newCellRangeAddress);
                }
            }
        }

        /// <summary>
        /// 写入行数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldDic"></param>
        /// <param name="row"></param>
        public static void SetRowValue(object obj, Dictionary<int, Clazz> fieldDic, IRow row)
        {

            foreach (int key in fieldDic.Keys)
            {
                Clazz clazz = fieldDic[key];
                object value = clazz.GetMethodInfo.Invoke(obj, null);
                SetValue(row, key, value);
            }
        }

        public static void SetValue(IRow row, int cellIndex, object value)
        {
            if (value == null)
            {
                return;
            }
            ICell cell = row.GetCell(cellIndex);

            if (cell == null)
            {
                cell = row.CreateCell(cellIndex);
                switch (value.GetType().Name)
                {
                    case "DateTime":
                    case "String":
                        cell.SetCellType(CellType.String);
                        break;
                    case "Single":
                    case "Double":
                    case "Int32":
                    case "Int64":
                        cell.SetCellType(CellType.Numeric);
                        break;
                    default:
                        //有无法识别的类型
                        break;
                }
            }
            SetValue(cell, value);

        }

        private static void SetValue(ICell cell, object value)
        {
            switch (value.GetType().Name)
            {
                case "DateTime":
                case "String":

                    cell.SetCellValue(value as string);
                    break;
                case "Single":
                case "Double":
                    cell.SetCellValue((double)value);
                    break;
                case "Int32":
                case "Int64":
                    cell.SetCellValue((int)value);
                    break;
                default:
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(value as string);
                    break;
            }
        }



        /// <summary>
        /// 替换文字内容
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="groupJTSYQ"></param>
        /// <param name="xmlobjectDic"></param>
        public static void ReplaceTextByXMLObject(ISheet sheet, object obj, Dictionary<string, XMLObject> xmlobjectDic)
        {
            foreach (IRow row in sheet)
            {
                foreach (ICell cell in row)
                {
                    if (cell.CellType == CellType.String)
                    {
                        string cellValue = cell.StringCellValue;
                        if (cellValue.Contains("["))
                        {

                            foreach (string method in xmlobjectDic.Keys)
                            {
                                if (cellValue.Contains(method))
                                {
                                    Object value = XMLRead.GetObjectMethodResult(xmlobjectDic[method], obj);
                                    if (value != null)
                                    {
                                        cellValue = cellValue.Replace("[" + method + "]", value.ToString());

                                    }
                                    else
                                    {
                                        cellValue = cellValue.Replace("[" + method + "]", "");
                                    }
                                    cell.SetCellValue(cellValue);
                                }
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// 真正删除行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startRow"></param>
        /// <param name="endRow"></param>
        public static void DeleteRow(ISheet sheet, int startRow, int endRow)
        {

            while (startRow < endRow)
            {
                sheet.RemoveRow(sheet.GetRow(endRow));
                sheet.ShiftRows(startRow + 1, endRow, -1);
                endRow--;
            }

        }
        public static void CopyRow(IRow sourceRow, IRow newRow)
        {
            var style = sourceRow.RowStyle;
            if(style != null)
            {
                newRow.RowStyle = style;
            }
           
            // Loop through source columns to add to new row
            for (int i = 0; i < sourceRow.LastCellNum; i++)
            {
                // Grab a copy of the old/new cell
                ICell oldCell = sourceRow.GetCell(i);
                ICell newCell = newRow.CreateCell(i);

                // If the old cell is null jump to next cell
                if (oldCell == null)
                {
                    newCell = null;
                    continue;
                }

                // Copy style from old cell and apply to new cell


                // If there is a cell comment, copy
                if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

                // If there is a cell hyperlink, copy
                if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

                // Set the cell data type
                newCell.SetCellType(oldCell.CellType);

                // Set the cell data value
                switch (oldCell.CellType)
                {
                    case CellType.Blank:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.Boolean:
                        newCell.SetCellValue(oldCell.BooleanCellValue);
                        break;
                    case CellType.Error:
                        newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                        break;
                    case CellType.Formula:
                        newCell.SetCellFormula(oldCell.CellFormula);
                        break;
                    case CellType.Numeric:
                        newCell.SetCellValue(oldCell.NumericCellValue);
                        break;
                    case CellType.String:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                    case CellType.Unknown:
                        newCell.SetCellValue(oldCell.StringCellValue);
                        break;
                }
            }
        }
        public static void WriteObjects<T>(ISheet sheet, XMLTable xmlTable, IList<T> objects)
        {
            IList<string> errors = new List<string>();
            int rowStart = xmlTable.RowStartIndex;
            int rows = xmlTable.Rows;
            int objectCount = objects.Count;
            Dictionary<int, XMLObject> cells = xmlTable.CellDic;
            Object obj;
            Object reuslut;
            XMLObject xmlObject;
            int objectRowX = objectCount - rows;
            int cellTotal = xmlTable.CellTotal;

            IRow row;
            IRow rowModel = sheet.GetRow(rowStart);
            ICell cell;
            if (objectRowX > 0)
            {
                sheet.ShiftRows(rowStart + 1, sheet.LastRowNum, objectRowX);
                for (int a = 0; a < objectRowX; a++)
                {
                    row = sheet.CreateRow(rowStart + 1 + a);
                    ExcelWrite.CopyRow(rowModel, row);
                    for (int b = 0; b < cellTotal; b++)
                    {
                        cell = row.GetCell(b);
                        ICell cellModel = rowModel.GetCell(b);
                        if (cellModel != null)
                        {
                            cell.CellStyle = cellModel.CellStyle;
                        }

                    }
                }
            }
            for (int a = 0; a < objectCount; a++)
            {
                row = sheet.GetRow(a + rowStart);
                obj = objects[a];
                //循环行数据
                foreach (int b in cells.Keys)
                {
                    cell = row.GetCell(b);
                    xmlObject = cells[b];
                    reuslut = XMLRead.GetObjectMethodResult(xmlObject, obj);
                    cell.SetCellValue(reuslut.ToString());

                }

            }
            //检查是否组编辑
            IList<XMLGroup> xmlGroups = xmlTable.XmlGroups;
            int rs;
            if (xmlGroups != null)
            {
                foreach (XMLGroup xmlGroup in xmlGroups)
                {
                    Dictionary<String, int> symbolDic = xmlGroup.GroupDic;
                    String symbol = xmlGroup.Result;
                    for (int a = 0; a < objectCount; a++)
                    {
                        obj = objects[a];
                        row = sheet.GetRow(a + rowStart);
                        reuslut = XMLRead.GetObjectMethodResult(xmlGroup.XmlObject, obj);
                        if (symbolDic.TryGetValue(reuslut.ToString(), out rs))
                        {
                            cell = row.GetCell(rs);
                            cell.SetCellValue(symbol);
                        }
                        else
                        {
                            errors.Add("xml配置中：" + reuslut.ToString() + "没有这个数据的分组");
                        }

                    }
                }

            }
        }

        public static void ReplaceText(IRow row, object obj)
        {
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(obj);
            Clazz clazz;
            foreach (ICell cell in row)
            {
                cell.SetCellType(CellType.String);
                string value = cell.StringCellValue;
                if (clazzDic.TryGetValue(value, out clazz))
                {
                    object result = clazz.GetMethodInfo.Invoke(obj, null);
                    if (result != null)
                    {
                        cell.SetCellValue(result.ToString());
                    }
                }
            }
        }
    }
}
