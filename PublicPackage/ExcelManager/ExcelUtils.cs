using ExcelManager.Model;
using MyUtils;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using ReflectManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ExcelManager
{
    public class ExcelUtils
    {
        /// <summary>
        /// Excel转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelPath"></param>
        /// <param name="XMLPath"></param>
        /// <returns></returns>
        public static IList<T> GetExcelToObject<T>(string excelPath, string XMLPath)
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(XMLPath)[0];
            ExcelReflectModel<T> reflectModel = new ExcelReflectModel<T>(excelPath, xmlTable);
            IList<T> list = ExcelUtils.SheetToObjectsByXMLTable<T>(reflectModel);
            return list;
        }

        public static IList<T> GetExcelToObject<T>(ISheet sheet, string XMLPath)
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(XMLPath)[0];
            ExcelReflectModel<T> reflectModel = new ExcelReflectModel<T>(sheet, xmlTable);
            IList<T> list = ExcelUtils.SheetToObjectsByXMLTable<T>(reflectModel);
            return list;
        }


        public static IList<T> SheetToObjects<T>(ExcelReflectModel<T> model)
        {
            return ToObject<T>(model.Sheet, model.StartRowIndex, model.LastRowIndex, model.TitleDic);
        }

        public static IList<JObject> RowsToJObjects(Dictionary<int, string> titleDic, IList<IRow> rows)
        {
            IList<JObject> list = new List<JObject>();

            foreach (IRow row in rows)
            {

                list.Add(JObject.Parse(RowToJsonString(titleDic, row)));
            }
            return list;
        }

        public static string RowToJsonString(Dictionary<int, string> titleDic, IRow row)
        {
            StringBuilder sb = new StringBuilder("{");
            foreach (int index in titleDic.Keys)
            {

                ICell cell = row.GetCell(index);
                if (cell == null)
                {
                    continue;
                }
                cell.SetCellType(CellType.String);
                string value = cell.StringCellValue;
                if (Utils.IsStrNull(value))
                {
                    continue;
                }
                if (value.Contains("\""))
                {
                    value = value.Replace("\"", "\\\"");
                }
                else if (value.Contains("\\"))
                {
                    value = value.Replace("\\", "\\\\");
                }
                sb.Append("\"" + titleDic[index] + "\":" + "\"" + value + "\"" + ",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }



        /// <summary>
        /// excel列数转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet">需要转换的sheet</param>
        /// <param name="startRow">开始转换的行数</param>
        /// <param name="lastRow">最后一行</param>
        /// <param name="clazzName">类的fullName</param>
        /// <param name="reflectFileName">列数映射</param>
        /// <returns></returns>
        public static IList<T> ToObject<T>(ISheet sheet, int startRow, int lastRow, String clazzName, String reflectFileName)
        {


            Dictionary<int, Clazz> clazzMap = ReflectUtils.GetExcelToClazzMap(ExcelRead.ReadExcelToDic(reflectFileName), clazzName);
            return ToObject<T>(sheet, startRow, lastRow, clazzMap);
        }
        public static IList<T> SheetToObjectsByXMLTable<T>(ExcelReflectModel<T> model)
        {
            List<T> list = new List<T>();
            MethodInfo m = typeof(T).GetMethod("set_Row");
            bool hasRowMethod = false;
            if (m != null)
            {
                hasRowMethod = true;
            }
            ISheet sheet = model.Sheet;
            int startRow = model.StartRowIndex;
            Dictionary<int, XMLObject> xmlObjectDic = model.XMLObjectDic;
            T obj;
            int rowCount = model.LastRowIndex - model.EndRowIndex;
            while (startRow <= rowCount)
            {
                IRow row = sheet.GetRow(startRow);
                if (row == null)
                {
                    continue;
                }
                obj = RowToObject<T>(row, xmlObjectDic);
                if (obj != null)
                {
                    if (hasRowMethod)
                    {
                        m.Invoke(obj, new object[] { row });
                    }

                    list.Add(obj);
                }


                startRow++;
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sheet"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="clazzMap"></param>
        /// <param name="setRow">是否把行对象设置进T 方法名必须是 ROW</param>
        /// <returns></returns>
        public static List<T> ToObject<T>(ISheet sheet, int startIndex, int endIndex, Dictionary<int, Clazz> clazzMap, bool setRow = false)
        {

            IRow row;
            List<T> list = new List<T>();
            dynamic obj = null;
            if (endIndex > sheet.LastRowNum + 1)
            {
                endIndex = sheet.LastRowNum + 1;
            }

            for (int a = startIndex; a <= endIndex; a++)
            {
                row = sheet.GetRow(a);
                if (row == null)
                {
                    continue;
                }
                //如果第一个是合计就此行就跳过
                ICell cell0 = row.GetCell(0);
                if (cell0 != null && cell0.CellType == CellType.String)
                {
                    if (cell0.StringCellValue.Trim().Equals("合计"))
                    {
                        break;
                    }
                }
                //循环行，存入对象
                obj = RowToObject(row, clazzMap);

                if (obj != null)
                {
                    if (setRow)
                    {
                        obj.Row = row;
                    }
                    list.Add((T)obj);
                }

                //每行读完 清空 对象obj
                obj = null;
            }
            return list;
        }
        private static object RowToObject(IRow row, Dictionary<int, Clazz> clazzMap)
        {

            Clazz clazz;
            object obj = null;
            Object[] paramters = new Object[1];
            foreach (int cellIndex in clazzMap.Keys)
            {

                ICell cell = row.GetCell(cellIndex);
                if (cell == null || cell.CellType == CellType.Blank)
                {
                    continue;
                }
                //得到对应的函数名字                    
                if (clazzMap.TryGetValue(cellIndex, out clazz))
                {
                    if (obj == null)
                    {
                        Type type = Type.GetType(clazz.getFullClassName());
                        obj = Activator.CreateInstance(type);
                        //设置行数为objecitd
                        Clazz objectidClazz;
                        clazzMap.TryGetValue(-1, out objectidClazz);
                        if (objectidClazz != null)
                        {
                            objectidClazz.GetMethodInfo.Invoke(obj, new object[] { row.RowNum });
                        }

                    }
                    //得到方法
                    MethodInfo m = clazz.SetMethodInfo;
                    //得到参数，判断其类型
                    String parameterName = clazz.getParamterType().Name;

                    switch (parameterName)
                    {

                        case "String":

                            paramters[0] = GetCellTrueType(cell, parameterName);
                            m.Invoke(obj, paramters);
                            break;
                        case "DateTime":
                            paramters[0] = GetCellTrueType(cell, parameterName);
                            m.Invoke(obj, paramters);
                            break;
                        case "Int32":
                        case "Int64":
                            paramters[0] = GetCellTrueType(cell, parameterName);
                            m.Invoke(obj, paramters);
                            break;

                        case "Single":
                        case "Double":
                            paramters[0] = GetCellTrueType(cell, parameterName);
                            m.Invoke(obj, paramters);
                            break;
                        default:
                            //有无法识别的类型
                            break;
                    }

                }
            }
            return obj;
        }
        public static T RowToObject<T>(IRow row, Dictionary<int, XMLObject> xmlObjectDic)
        {


            XMLObject xmlObject;
            T obj = ReflectUtils.CreateObject<T>();
            Object[] paramters = new Object[1];
            object value;
            bool rowHasValue = false;
            foreach (int cellIndex in xmlObjectDic.Keys)
            {
                ICell cell = row.GetCell(cellIndex);
                if (cell == null || cell.CellType == CellType.Blank)
                {
                    continue;
                }

                //得到对应的函数名字   
                xmlObject = xmlObjectDic[cellIndex];
                MethodInfo m = xmlObject.MethodInfos[0];
                if (m.Name.StartsWith("set_"))
                {
                    value = GetCellTrueType(cell, m.GetParameters()[0].ParameterType.ToString());
                    if (value != null)
                    {
                        rowHasValue = true;
                        paramters[0] = value;
                        m.Invoke(obj, paramters);
                    }
                }
                else
                {
                    value = GetCellTrueType(cell, xmlObject.MethodInfos[1].GetParameters()[0].ParameterType.ToString());
                    if (value != null)
                    {
                        rowHasValue = true;
                        paramters[0] = value;
                        xmlObject.MethodInfos[1].Invoke(m.Invoke(obj, null), paramters);
                    }

                }

            }
            if (rowHasValue)
            {
                return obj;
            }
            else
            {
                return default(T);
            }

        }
        public static Object GetCellTrueType(ICell cell, String reultType)
        {
            Object ob = null;
            String cellValue;
            reultType = reultType.Replace("System.", "");
            if (!cell.CellType.ToString().Equals(reultType))
            {

                switch (reultType)
                {
                    case "String":

                        cell.SetCellType(CellType.String);
                        cellValue = cell.StringCellValue;
                        if (!cellValue.Equals(""))
                        {
                            ob = cellValue;
                        }

                        break;
                    case "DateTime":

                        if (cell.CellType == CellType.Numeric)
                        {
                            ob = cell.DateCellValue;
                        }
                        else
                        {
                            cell.SetCellType(CellType.String);
                            cellValue = cell.StringCellValue;
                            if (!cellValue.Equals(""))
                            {

                                //日期格式化  - - - 
                                try
                                {
                                    DateTime dt = Utils.FormatDate(cellValue);
                                    ob = dt;
                                }
                                catch
                                {
                                    //报出错误
                                    // throw new Exception(cell.Sheet.SheetName + "数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                                }
                            }

                        }


                        break;
                    case "Int32":
                        cell.SetCellType(CellType.String);
                        cellValue = cell.StringCellValue;
                        if (!cellValue.Equals(""))
                        {
                            if (Regex.Match(cellValue, @"^-?\d+$").Success)
                            {
                                ob = int.Parse(cellValue);
                            }
                            else
                            {
                                //报出错误
                                //throw new Exception("数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                            }
                        }

                        break;
                    case "Single":
                        cell.SetCellType(CellType.String);
                        cellValue = cell.StringCellValue;
                        if (!cellValue.Equals(""))
                        {

                            if (Regex.Match(cellValue, @"^(-?\d+)(\.\d+)?$").Success)
                            {
                                ob = float.Parse(cellValue);
                            }
                            else
                            {
                                if (cellValue.StartsWith("."))
                                {
                                    ob = float.Parse("0" + cellValue);
                                }
                                else
                                {
                                    //报出错误
                                    // throw new Exception("数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                                }

                            }
                        }
                        break;
                    case "Double":
                        cell.SetCellType(CellType.String);
                        cellValue = cell.StringCellValue;
                        if (!cellValue.Equals(""))
                        {
                            if (Regex.Match(cellValue, @"^(-?\d+)(\.\d+)?$").Success)
                            {
                                ob = double.Parse(cellValue);
                            }
                            else
                            {
                                //报出错误
                                // throw new Exception("数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                            }
                        }
                        break;

                    default:
                        //有无法识别的类型
                        break;
                }
            }
            else
            {

                switch (cell.CellType.ToString())
                {
                    case "String":
                        ob = cell.StringCellValue;
                        break;
                    case "DateTime":

                        ob = cell.DateCellValue;
                        break;
                    case "Int32":

                        ob = (int)cell.NumericCellValue;
                        break;
                    case "Double":

                        ob = (double)cell.NumericCellValue;
                        break;

                    default:
                        //有无法识别的类型
                        break;
                }
            }
            return ob;
        }

        public static ObservableCollection<T> GetExcelToObjectToObservableCollection<T>(string path, string xmlPath)
        {

            IList<T> list = GetExcelToObject<T>(path, xmlPath);
            ObservableCollection<T> oc = Utils.ListToObservableCollection(list);

            return oc;
        }
        public static ObservableCollection<T> GetExcelToObjectToObservableCollection<T>(ISheet sheet, string XMLPath)
        {
            IList<T> list = GetExcelToObject<T>(sheet, XMLPath);
            ObservableCollection<T> oc = Utils.ListToObservableCollection(list);

            return oc;
        }
    }
}
