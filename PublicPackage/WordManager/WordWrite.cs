using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using ExcelManager;
using MyUtils;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using ReflectManager;
using ReflectManager.XMLPackage;

namespace WordManager
{
    public class WordWrite
    {


        /// <summary>
        /// 创建行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static XWPFTableRow CreateRow(XWPFTable table)
        {
            CT_Row m_NewRow = new CT_Row();
            XWPFTableRow m_row = new XWPFTableRow(m_NewRow, table);
            m_row.GetCTRow().AddNewTrPr().AddNewTrHeight().val = (ulong)426;
            table.AddRow(m_row);

            return m_row;
        }
        public static void ReplaceText(IList<XWPFParagraph> ps, Dictionary<string, XMLObject> xmlObjectDic, object obj)
        {
            if(!Utils.CheckListExists(ps))
            {
                return;
            }
            XMLObject xmlObject;
            bool pictureFlag = false ;
            foreach (XWPFParagraph p in ps)
            {
                if(p.Text== null|| !p.Text.Contains("["))
                {
                    continue;
                }
                IList<XWPFRun> runs = p.Runs;
                int count = runs.Count;
                for (int a = 0; a < runs.Count; a++)
                {
                    if(a == 14)
                    {

                    }
                    int index;
                    int endIndex;
                    string text;
                    XWPFRun run = runs[a];
                    //得到第一段 与 最后一段
                    if(run.GetText(0) == null)
                    {
                        continue;
                    }
                    index = run.GetText(0).IndexOf("[");
                    
                    if (index != -1)
                    {
                        text = run.GetText(0);
                        int b = a;
                        while (a < runs.Count)
                        {
                          
                            run = runs[a];
                            if (index == -1)
                            {
                                b++;
                                index = run.GetText(0).IndexOf("[");
                                if (index == -1)
                                {
                                    break;
                                }
                            }
                            if( a == b)
                            {
                                endIndex = run.GetText(0).IndexOf("]", index);
                            }else
                            {
                                endIndex = run.GetText(0).IndexOf("]");
                            }
                           

                            if (a != 0 && !text.Contains(run.GetText(0)))
                            {
                                text = text + run.GetText(0);
                            }
                            if (endIndex != -1)
                            {

                                if(text.EndsWith("_P]"))
                                {
                                    pictureFlag = true;
                                    text = text.Substring(index + 1, text.IndexOf("]", index) - index - 3);
                                }else
                                {

                                    text = text.Substring(index + 1, text.IndexOf("]", index) - index - 1);

                                }
                               
                                if (xmlObjectDic.TryGetValue(text, out xmlObject))
                                {
                                    //移除多余fun
                                    for (int c = b + 1; c <= a; c++)
                                    {
                                        if(c == a)
                                        {
                                            string s = runs[b + 1].GetText(0).Substring(endIndex+1);
                                            runs[b + 1].SetText(s, 0);
                                        }
                                        else
                                        {
                                            p.RemoveRun(b + 1);
                                        }
                                        
                                    }
                                   
                                    object value = XMLRead.GetObjectMethodResult(xmlObject, obj);
                                    if (value == null)
                                    {
                                        //pictureFlag = true;
                                        ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index), "");
                                        a = b;
                                    }
                                    else
                                    {
                                        endIndex = p.Runs[b].GetText(0).IndexOf("]", index);
                                        string runValue = value.ToString();
                                        if (endIndex != -1 )
                                        {
                                            if(pictureFlag)
                                            {
                                                pictureFlag = false;
                                                ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index, endIndex - index + 1), "");
                                               try
                                                {
                                                    InsertImage_QuanJiDiaoBiao(p.Runs[b], runValue);
                                                }
                                                catch(Exception e)
                                                {
                                                    MessageBox.Show(e.Message);
                                                    return;
                                                }
                                               
                                                a = b;
                                            }
                                            else
                                            {
                                               
                                                ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index, endIndex - index + 1), runValue);
                                                a = b;
                                            }
                                           
                                        }
                                        else
                                        {
                                            if (pictureFlag)
                                            {
                                                pictureFlag = false;
                                                ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index), "");
                                                try
                                                {
                                                    InsertImage_QuanJiDiaoBiao(p.Runs[b], runValue);
                                                }
                                                catch (Exception e)
                                                {
                                                    MessageBox.Show(e.Message);
                                                    return;
                                                }
                                               
                                                a = b + 1;

                                            }
                                            else
                                            {
                                               
                                                ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index), runValue);
                                                a = b + 1;
                                            }
                                            
                                        }
                                       

                                    }
                                    a--;
                                    break;
                                }else
                                {
                                   
                                    index = run.GetText(0).IndexOf("[", index+1);
                                    if(index != -1)
                                    {
                                        text = run.GetText(0);
                                        a--;
                                    }else
                                    {
                                        text = "";
                                    }
                                }
                            }
                            a++;
                        }

                    }
                }
            }
        }
        /// <summary>
        /// 权籍照片插入
        /// </summary>
        /// <param name="run"></param>
        /// <param name="runValue"></param>
        private static void InsertImage_QuanJiDiaoBiao(XWPFRun run, string runValue)
        {
            if(runValue.Trim().Equals(""))
            {
                return;
            }
            string path;
            if (PictureDic.TryGetValue(runValue, out path))
            {
                if (Utils.CheckFileExists(path))
                {
                    InsertImage(path, run, runValue.Length);
                }
                else
                {
                    throw new Exception("配置的照片，路径不存在:" + path);
                }

            }
            else
            {
                throw new Exception("人员没有配置照片：" + runValue);
            }
        }

        public static Dictionary<string, string> picturedic;
        public static Dictionary<string,string> PictureDic
        {

            get
            { if (picturedic == null)
                {
                    Config.JTSYQ.FileConfigPageViewModel config = Config.JTSYQ.FileConfigPageViewModel.GetRedis();

                    string path = config.PhotoExcelPath;
                    if(path =="")
                    {
                        MessageBox.Show("照片没有配置");
                    }else if(!Utils.CheckFileExists(path))
                    {
                        MessageBox.Show("照片路径不存在："+path);
                    }else
                    {
                        picturedic = ExcelRead.SheetToDic(ExcelRead.ReadExcelSheet(config.PhotoExcelPath, 0));
                    }
                    return picturedic;
                }else
                {
                    return picturedic;
                }
            }
        }
        
        /// <summary>
        /// 替换文字，不复制格式
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xmlObject"></param>
        /// <param name="obj"></param>
        public static void ReplaceText(XWPFDocument doc, Dictionary<string, XMLObject> xmlObjectDic, object obj)
        {
           
            foreach(XWPFTable table in doc.Tables)
            {
                foreach(XWPFTableRow row in table.Rows)
                {
                    
                    foreach(XWPFTableCell cell in row.GetTableCells())
                    {
                       
                      
                           ReplaceText(cell.Paragraphs, xmlObjectDic, obj);
                    }
                }
            }
            ReplaceText(doc.Paragraphs, xmlObjectDic, obj);
        }
        /// <summary>
        /// 替换文字 包括照片
        /// </summary>
        /// <param name="docDic"></param>
        /// <param name="replaceMap"></param>
        public static void ReplaceText(Dictionary<string, IList<RunCustomer>> docDic, Dictionary<string, string> replaceMap)
        {
            IList<RunCustomer> runCustomers;
            String name;
            String imagePath;
            foreach (String key in replaceMap.Keys)
            {
                if (docDic.TryGetValue(key, out runCustomers))
                {
                    if (key.EndsWith("P]"))
                    {
                        Dictionary<String, String> signName = null;//ExcelRead.FileConfigToDic("人员签名与文件名对照", 0, true, false, false);
                        //得到照片的名字                         
                        if (replaceMap.TryGetValue(key, out name))
                        {

                            if (signName.TryGetValue(name, out imagePath))
                            {
                                foreach (RunCustomer runCustomer in runCustomers)
                                {
                                    XWPFRun run = runCustomer.Run;
                                    run.SetText("", 0);
                                    InsertImage(imagePath, run, name.Length);
                                }
                            }
                            else
                            {
                                throw new Exception("名字还没有配照片：" + name);
                            }
                        }
                    }
                    else
                    {
                        foreach (RunCustomer runCustomer in runCustomers)
                        {
                            ReplaceText(runCustomer, key, replaceMap[key]);

                        }
                    }
                }
            }
        }



        /// <summary>
        /// 创建列
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static XWPFTableCell CreateCell(XWPFTableRow row)
        {
            XWPFTableCell cell = row.CreateCell();
            CT_Tc cttc = cell.GetCTTc();
            CT_TcPr ctpr = cttc.AddNewTcPr();

            cttc.GetPList()[0].AddNewPPr().AddNewJc().val = ST_Jc.center;//水平居中

            ctpr.AddNewVAlign().val = ST_VerticalJc.center;//垂直居中

            //ctpr.tcW = new CT_TblWidth();
            //ctpr.tcW.w = "1200";//默认列宽
            ctpr.tcW.type = ST_TblWidth.dxa;

            return cell;
        }
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="Span"></param>
        public static void SetColSpan(XWPFTableCell cell, int Span)
        {
            CT_Tc cttc = cell.GetCTTc();
            CT_TcPr ctpr = cttc.AddNewTcPr();
            ctpr.gridSpan = new CT_DecimalNumber();
            ctpr.gridSpan.val = Span.ToString();//合并单元格

            cttc.GetPList()[0].AddNewPPr().AddNewJc().val = ST_Jc.center;//水平居中

            ctpr.AddNewVAlign().val = ST_VerticalJc.center;//垂直居中
        }
        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="table">表格</param>
        /// <param name="ColIndex">列号。从0开始编号</param>
        /// <param name="Width">列宽</param>
        public static void SetColWith(XWPFTable table, int ColIndex, int Width)
        {
            CT_TcPr m_pr = table.GetRow(0).GetCell(ColIndex).GetCTTc().AddNewTcPr();
            m_pr.tcW = new CT_TblWidth();
            m_pr.tcW.w = Width.ToString();
            m_pr.tcW.type = ST_TblWidth.dxa;
        }
        /// <summary>
        /// 给定一个图片路径，得出合适的长宽插入文档中
        /// </summary>
        /// <param name="imgpath"></param>
        /// <param name="imgw"></param>
        /// <param name="imgh"></param>
        public static void CalcImgWH(string imgpath, ref int imgw, ref int imgh)
        {
            if (!File.Exists(imgpath)) return;
            Bitmap bmap = new Bitmap(imgpath);
            SizeF size = bmap.PhysicalDimension;
            imgw = (int)size.Width;
            imgh = (int)size.Height;
            decimal d = 70;//算出一厘米的大概像素,1CM=360000EMUS
            imgw = (int)(imgw * 360000 / d);
            imgh = (int)(imgh * 360000 / d);

        }
        /// <summary>
        /// 给定一个图片流，得出合适的长宽插入文档中
        /// </summary>
        /// <param name="imgpath"></param>
        /// <param name="imgw"></param>
        /// <param name="imgh"></param>
        public static void CalcImgWH(Stream stream, ref int imgw, ref int imgh)
        {
            Bitmap bmap = new Bitmap(stream);
            SizeF size = bmap.PhysicalDimension;
            imgw = (int)size.Width;
            imgh = (int)size.Height;
            decimal d = 70;//算出一厘米的大概像素,1CM=360000EMUS
            imgw = (int)(imgw * 360000 / d);
            imgh = (int)(imgh * 360000 / d);
        }
        private static void SaveToFile(MemoryStream ms, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();

                fs.Write(data, 0, data.Length);
                fs.Flush();
                data = null;
            }
        }

        public static void ReplaceTable<T>(XWPFDocument doc, XMLTable xmlTable, IList<T> objects)
        {
            int tblIndex = xmlTable.TableIndex;
            XWPFTable tbl = doc.Tables[tblIndex];
            int rowStart = xmlTable.RowStartIndex;
            int rows = xmlTable.Rows;
            int objectCount = objects.Count;
            Dictionary<int, XMLObject> cells = xmlTable.CellDic;
            XWPFTableRow row;
            Object obj;
            Object reuslut;
            XWPFTableCell cell;
            XMLObject xmlObject;
            int jtcyRowX = objectCount - rows;
            int cellTotal = xmlTable.CellTotal;

            XWPFTableRow rowModel = tbl.GetRow(rowStart);

            for (int a = 0; a < jtcyRowX; a++)
            {
                row = tbl.InsertNewTableRow(rowStart + 1);

                for (int b = cellTotal; b >= 0; b--)
                {
                    cell = row.CreateCell();
                }
                CopyCellStyle(rowModel, row);
            }

            for (int a = 0; a < objectCount; a++)
            {
                obj = objects[a];
                row = tbl.GetRow(a + rowStart);

                //循环行数据
                foreach (int b in cells.Keys)
                {
                    cell = row.GetCell(b);
                    xmlObject = cells[b];
                    reuslut = XMLRead.GetObjectMethodResult(xmlObject, obj);
                    XWPFRun run = SetCellValue(cell, reuslut);
                    CopyRunStyle(rowModel.GetCell(b).Paragraphs[0].Runs[0], run);
                }
            }
            //检查是否要序列号
            if (xmlTable.Index != -1)
            {
                for (int a = 0; a < objectCount; a++)
                {
                    row = tbl.GetRow(a + rowStart);
                    cell = row.GetCell(xmlTable.Index);
                    if (cell == null)
                    {
                        cell = row.CreateCell();
                    }
                    XWPFRun run = SetCellValue(cell, a + 1);
                    CopyRunStyle(rowModel.GetCell(xmlTable.Index).Paragraphs[0].Runs[0], run);
                }
            }
            //检查是否组编辑
            IList<XMLGroup> xmlGroups = xmlTable.XmlGroups;
            if (xmlGroups != null)
            {
                foreach (XMLGroup xmlGroup in xmlGroups)
                {
                    Dictionary<String, int> symbolDic = xmlGroup.GroupDic;
                    String symbol = xmlGroup.Result;
                    for (int a = 0; a < objectCount; a++)
                    {
                        obj = objects[a];
                        row = tbl.GetRow(a + rowStart);
                        reuslut = XMLRead.GetObjectMethodResult(xmlGroup.XmlObject, obj);
                        cell = row.GetCell(symbolDic[reuslut.ToString()]);
                        XWPFRun run = SetCellValue(cell, symbol);
                        CopyRunStyle(rowModel.GetCell(xmlTable.Index).Paragraphs[0].Runs[0], run);
                    }
                }
            }

        }

        public static void ReplaceSymbol(Dictionary<string, IList<RunCustomer>> docDic, Dictionary<String, XMLSymbol> symbolDic)
        {
            IList<RunCustomer> customers;
            foreach (string key in symbolDic.Keys)
            {
                customers = docDic[symbolDic[key].Result];
                ReplaceSmpbol(customers[0]);
            }
        }
        /// <summary>
        /// 不会以创建行 删除行，行中必须要有数据 ，没有对象的行会删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="xmlTable"></param>
        /// <param name="objects"></param>
        public static XWPFTable ReplaceTable_Font<T>(XWPFDocument doc, XMLTable xmlTable, IList<T> objects)
        {
            int tblIndex = xmlTable.TableIndex;
            XWPFTable tbl = doc.Tables[tblIndex];
            int rowStart = xmlTable.RowStartIndex;
            int rows = xmlTable.Rows;
            int objectCount = objects.Count;
            Dictionary<int, XMLObject> cells = xmlTable.CellDic;
            XWPFTableRow row;
            Object obj;
            Object reuslut;
            XWPFTableCell cell;
            XMLObject xmlObject;
            int cellTotal = xmlTable.CellTotal;

            XWPFTableRow rowModel = tbl.GetRow(rowStart);
            /*
            for (int a = objectCount; a < rows; a++)
            {
                row = tbl.GetRow(rowStart+a);
                foreach (int b in cells.Keys)
                {
                    cell = row.GetCell(b);
                    SetCellValue(cell, "");
                }
            }*/
            for (int a = 0; a < objectCount; a++)
            {
                obj = objects[a];
                row = tbl.GetRow(a + rowStart);
                //循环行数据
                foreach (int b in cells.Keys)
                {
                    cell = row.GetCell(b);
                    xmlObject = cells[b];
                    reuslut = XMLRead.GetObjectMethodResult(xmlObject, obj);
                    SetCellValue(cell, reuslut);
                }
            }
            //检查是否要序列号
            if (xmlTable.Index != -1)
            {
                for (int a = 0; a < objectCount; a++)
                {
                    row = tbl.GetRow(a + rowStart);
                    cell = row.GetCell(xmlTable.Index);
                    if (cell == null)
                    {
                        cell = row.CreateCell();
                    }
                    SetCellValue(cell, a + 1);
                }
            }
            //检查是否组编辑
            IList<XMLGroup> xmlGroups = xmlTable.XmlGroups;
            if (xmlGroups != null)
            {
                foreach (XMLGroup xmlGroup in xmlGroups)
                {
                    Dictionary<String, int> symbolDic = xmlGroup.GroupDic;
                    String symbol = xmlGroup.Result;
                    for (int a = 0; a < objectCount; a++)
                    {
                        obj = objects[a];
                        row = tbl.GetRow(a + rowStart);
                        reuslut = XMLRead.GetObjectMethodResult(xmlGroup.XmlObject, obj);
                        cell = row.GetCell(symbolDic[reuslut.ToString()]);
                        SetCellValue(cell, symbol);

                    }
                }
            }
            return tbl;
        }

        public static XWPFRun SetCellValue(XWPFTableCell cell, object reuslut)
        {
            //   cell.Paragraphs[0].Runs[0].SetText();
            XWPFParagraph p = cell.Paragraphs[0];
            int count = p.Runs.Count();
            XWPFRun run;
            if (count == 0)
            {
                run = p.CreateRun();
            }
            else
            {
                run = p.Runs[0];
            }
            run.SetText(reuslut.ToString(), 0);
            return run;
        }

        /// <summary>
        /// 复制行格式
        /// </summary>
        /// <param name="rowModel"></param>
        /// <param name="row"></param>
        public static void CopyCellStyle(XWPFTableRow rowModel, XWPFTableRow row)
        {
            IList<XWPFTableCell> cells = row.GetTableCells();
            IList<XWPFTableCell> cellsModel = rowModel.GetTableCells();
            row.SetHeight(rowModel.GetHeight());
            XWPFParagraph p;
            for (int a = 0; a < cellsModel.Count; a++)
            {
                XWPFTableCell cell = cells[a];
                XWPFTableCell cellModel = cellsModel[a];
                cell.GetCTTc().tcPr = cellModel.GetCTTc().tcPr;
                CopyParagraphStyle(cellModel.Paragraphs[0], cell.Paragraphs[0]);
            }
        }
        /// <summary>
        /// 复制段落格式
        /// </summary>
        /// <param name="pModel"></param>
        /// <param name="p"></param>
        public static void CopyParagraphStyle(XWPFParagraph pModel, XWPFParagraph p)
        {
            if (pModel == null)
            {
                return;
            }
            p.Alignment = pModel.Alignment;
            p.VerticalAlignment = pModel.VerticalAlignment;
            XWPFRun run = p.CreateRun();
        }
        /// <summary>
        /// 复制 run格式
        /// </summary>
        /// <param name="runModel"></param>
        /// <param name="run"></param>
        private static void CopyRunStyle(XWPFRun runModel, XWPFRun run)
        {
            int size = runModel.FontSize;
            if (size == -1)
            {
                size = 12;
            }
            run.FontSize = size;
            run.SetBold(runModel.IsBold);
            run.SetColor(runModel.GetColor());
            run.FontFamily = runModel.FontFamily;
            //run.FontFamily = "宋体";
            run.SetUnderline(runModel.Underline);

        }

        /// <summary>
        /// 根据模板段落段落设置内容、格式
        /// </summary>
        /// <param name="p"></param>
        /// <param name="runIndex"></param>
        /// <param name="value"></param>
        /// <param name="rModel"></param>
        public static void SetP(XWPFParagraph p, int runIndex, String value, XWPFRun rModel)
        {

            p.Alignment = rModel.Paragraph.Alignment;
            XWPFRun r;
            if (p.Runs.Count <= runIndex)
            {
                r = p.CreateRun();
            }
            else
            {
                r = p.Runs[runIndex];
            }
            SetRun(r, value, rModel);
        }
        /// <summary>
        /// run  设置内容，根据模板 设置格式，
        /// </summary>
        /// <param name="r"></param>
        /// <param name="value"></param>
        /// <param name="rModel"></param>
        public static void SetRun(XWPFRun r, String value, XWPFRun rModel)
        {
            if (Utils.IsStrNull(value))
            {
                return;
            }
            r.FontFamily = rModel.FontFamily;
            // r.FontFamily ="Microsoft Yahei";
            r.FontSize = rModel.FontSize;
            r.SetBold(rModel.IsBold);
            r.SetText(value);
        }
        /// <summary>
        /// 段落设置内容、格式
        /// </summary>
        /// <param name="p"></param>
        /// <param name="paragraphAlignment"></param>
        /// <param name="value"></param>
        /// <param name="fontSize">初号（0号）=84，小初=72，1号=52，2号=44，小2=36，3号=32，小3=30，4号=28，小4=24，5号=21，小5=18，6号=15，小6=13，7号=11，8号=10</param>
        /// <param name="fontFamily"></param>
        /// <param name="bold"></param>
        public static void SetP(XWPFParagraph p, ParagraphAlignment paragraphAlignment, String value, int fontSize, String fontFamily, Boolean bold)
        {
            p.Alignment = paragraphAlignment;
            // XWPFRun r = p.CreateRun();
            XWPFRun r = p.Runs[0];
            r.FontFamily = fontFamily;
            r.FontSize = fontSize;
            r.SetBold(bold);
            r.SetText(value);
        }
        /// <summary>
        /// docx文件保存至fileName路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="doc"></param>
        public static void SaveToFile(XWPFDocument doc, String fileName)
        {
            FileStream fs = null;

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }
                fs = new FileStream(fileName, FileMode.Create);
                doc.Write(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        public void SetP(IList<XWPFParagraph> ps, int index, ParagraphAlignment paragraphAlignment, String value, int fontSize, String fontFamily, Boolean bold)
        {
            XWPFParagraph p = ps[index];
            SetP(p, paragraphAlignment, value, fontSize, fontFamily, bold);
        }

        public static void ReplaceText(XWPFRun run, String oldString, string newString)
        {


            String text = run.GetText(0).Replace(oldString, newString);
            run.SetText(text, 0);

        }
        /// <summary>
        /// 只针对于 cell 里面有一个run 值的单元格
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="text"></param>
        public static void SetCellText(XWPFTableCell cell, String text)
        {
            cell.Paragraphs[0].Runs[0].SetText(text,0);

        }
        /// <summary>
        /// 指定的段落替换数据
        /// </summary>
        /// <param name="p"></param>
        /// <param name="dic"></param>
        public static void ReplaceText(XWPFParagraph p, Dictionary<string, string> dic)
        {

            string value;
            if (dic.TryGetValue(p.ParagraphText, out value))
            {
                CopyRunStyle(p.Runs[0], p.CreateRun());
                for (int a = 0; a < p.Runs.Count - 1;)
                {

                    p.RemoveRun(a);
                }
                p.Runs[0].SetText(value);
                return;
            }
            IList<XWPFRun> runs = p.Runs;
            String[] arrayValue;
            foreach (XWPFRun run in runs)
            {

                String text = run.GetText(0);
                //Console.WriteLine(text);
                if (text != null && dic.TryGetValue(text.Trim(), out value))
                {
                    if (value.Contains("\n"))
                    {
                        arrayValue = value.Split('\n');
                        for (int a = 0; a < arrayValue.Count(); a++)
                        {
                            if (arrayValue[a].Equals(""))
                            {
                                continue;
                            }
                            run.SetText(arrayValue[a] + "", a);
                            run.AddCarriageReturn();
                        }
                    }
                    else
                    {
                        run.SetText(value, 0);
                    }

                }

            }
        }




        /// <summary>
        /// 向文中复制 段落
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ps"></param>
        /// <param name="ModelP"></param>
        /// <param name="GoalP"></param>
        /// <returns></returns>
        public static XWPFParagraph ConpyP(XWPFDocument doc, int ModelP, int GoalP)
        {
            XWPFParagraph p = doc.Paragraphs[ModelP];
            doc.CreateParagraph();
            int length = doc.Paragraphs.Count;
            for (int a = length - 1; a > GoalP; a--)
            {
                doc.SetParagraph(doc.GetParagraphArray(a - 1), a);
                //doc.SetTable(15, tables[0]);
            }
            doc.SetParagraph(p, GoalP);


            return doc.Paragraphs[GoalP];
        }

   

        public static void ReplaceText(IList<XWPFTable> tables, Dictionary<string, string> dic)
        {
            if (tables == null)
            {
                return;
            }
            else
            {
                foreach (XWPFTable tbl in tables)
                {
                    ReplaceText(tbl, dic);
                }

            }



        }

        public static void ReplaceText(XWPFTable tbl, Dictionary<string, string> dic)
        {
            for (int a = 0; a < tbl.NumberOfRows; a++)
            {

                XWPFTableRow row = tbl.GetRow(a);
                ReplaceText(row, dic);

            }
        }

        private static void ReplaceText(XWPFTableRow row, Dictionary<string, string> dic)
        {

            IList<XWPFTableCell> cells = row.GetTableCells();
            foreach (XWPFTableCell cell in cells)
            {
                ReplaceText(cell, dic);

            }
        }

        private static void ReplaceText(XWPFTableCell cell, Dictionary<string, string> dic)
        {
            IList<XWPFParagraph> cellPs = cell.Paragraphs;
            foreach (XWPFParagraph cellP in cellPs)
            {
                ReplaceText(cellP, dic);
            }
        }

        /// <summary>
        /// 向段落中添加值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="doc"></param>
        /// <param name="pIndex"></param>
        /// <param name="wortFontSyle"></param>
        internal static void CreateRun(String value, XWPFDocument doc, int pIndex, WordFontSyle wortFontSyle)
        {
            XWPFParagraph p = doc.Paragraphs[pIndex];


            XWPFRun r = p.CreateRun();
            r.SetText(value);
            SetRunStyle(r, wortFontSyle);
        }
        /// <summary>
        /// 段落区域替换
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dkReplaceMap"></param>
        internal static void ReplaceText(IList<XWPFParagraph> ps, int start, int end, Dictionary<string, string> dkReplaceMap)
        {
            while (start < end)
            {
                ReplaceText(ps[start], dkReplaceMap);
                start++;
            }
        }

        /// <summary>
        /// 给段落设置格式
        /// </summary>
        /// <param name="r"></param>
        /// <param name="wortFontSyle"></param>
        private static void SetRunStyle(XWPFRun r, WordFontSyle wordFontSyle)
        {
            r.SetBold(wordFontSyle.Bold);
            r.SetColor(wordFontSyle.Color);
            r.FontFamily = wordFontSyle.FontFamily;
            r.FontSize = wordFontSyle.FontSize;
            r.SetUnderline(wordFontSyle.Underline);
        }

        internal static void SetRun(XWPFRun r, string value, int fontSize, string fontFamily, Boolean bold)
        {
            r.FontFamily = fontFamily;
            if (-1 != fontSize)
            {
                r.FontSize = fontSize;
            }

            r.SetBold(bold);
            r.SetText(value);
        }
       
        public static void ReplaceSmpbol(IList<RunCustomer> customers)
        {
           if(Utils.CheckListExists(customers))
            {
                foreach(RunCustomer customer in customers)
                {
                    ReplaceSmpbol(customer);
                }
            }
        }
        public static void ReplaceSmpbol(RunCustomer customer)
        {
            
                XWPFRun run = customer.Run;
                XWPFParagraph p = run.Paragraph;
                IList<XWPFRun> runs = p.Runs;
                int index = runs.IndexOf(customer.Run) - 1;
                int fontSize = runs[index].FontSize;
                p.RemoveRun(index);
                SetRun(p.InsertNewRun(index), "R", fontSize, "Wingdings 2", run.IsBold);
            
        }
        public static void ReplaceSmpbol(XWPFRun run)
        {
            XWPFParagraph p = run.Paragraph;
            IList<XWPFRun> runs = p.Runs;
            int a = runs.IndexOf(run);
            int index = runs.IndexOf(run);
            int fontSize = runs[index].FontSize;
            p.RemoveRun(index);
            if (p.Runs.Count > 0)
            {
                SetRun(p.InsertNewRun(index-1), "R", fontSize, "Wingdings 2", run.IsBold);
            }
            else
            {
                SetRun(p.CreateRun(), "R", fontSize, "Wingdings 2", run.IsBold);
            }
        }

        public static void ReplaceText(RunCustomer runCustomer, string oldText, string newText)
        {
            XWPFRun run = runCustomer.Run;
            ReplaceText(run, oldText, newText);
        }
        
        //public static void InsertImage1(String imagePath, XWPFRun r2, int strLength)
        //{

        //    int heightEmus = 287909;

        //    int widthEmus = (int)(212466 * strLength);

        //    using (FileStream picData = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
        //    {
        //        r2.AddPicture(picData, (int)PictureType.JPEG, "image1", widthEmus, heightEmus);

        //    }
        //}
        public static void InsertImage(String imagePath, XWPFRun r2, int strLength)
        {
            double width;
            if (strLength <= 3)
            {
                width = 1.77;
            }
            else if (strLength <= 7)
            {
                width = 2.35;
            }
            else
            {
                width = strLength * 0.4;
            }
            InsertImage(imagePath, r2, 0.8, width);
        }
        public static void InsertImage(String imagePath, XWPFRun r2, double heiht, double width)
        {

            int heightEmus = (int)(358873 * heiht);

            int widthEmus = (int)(358873 * width);

            using (FileStream picData = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                r2.AddPicture(picData, (int)PictureType.JPEG, "image1", widthEmus, heightEmus);

            }
        }

        public static void SetTableRowObj<T>(XWPFDocument doc, IList<T> objs, string path)
        {
            Dictionary<String, String> configDic = ExcelRead.ReadExcelToDic(path, 0);
            Dictionary<String, Clazz> clazzMap = ReflectUtils.GetRealClazzDicValue(configDic);
            IList<String> errors = new List<String>();
            //插入行数据
            string tableIndex;
            if (!configDic.TryGetValue("tableIndex", out tableIndex))
            {
                errors.Add(path + "文件中没有配置是第几张表");
            }
            string startRow;
            if (!configDic.TryGetValue("startRow", out startRow))
            {
                errors.Add(path + "文件中没有配置从第几行开始");
            }
            string cellTotal;
            if (!configDic.TryGetValue("cellTotal", out cellTotal))
            {
                errors.Add(path + "文件中没有配置一共有几行");
            }


            IList<XWPFTable> tables = doc.Tables;
            int tableIndexInt = int.Parse(tableIndex);
            if (tables.Count <= tableIndexInt)
            {
                errors.Add(path + "文件中没有配置表格坐标大于总共的 表格数了");
            }



            XWPFTable table = tables[tableIndexInt];

            int startRowIndex = int.Parse(startRow);
            IList<XWPFTableRow> rows = table.Rows;
            int rowsLength = rows.Count;
            if (rowsLength <= startRowIndex + objs.Count)
            {
                errors.Add(path + "文件中没有配置表格要写入的行数大于总共的 行数了");
            }
            int cellTotalInt = int.Parse(cellTotal);
            IList<XWPFTableCell> cells = rows[startRowIndex].GetTableCells();
            if (cells.Count < cellTotalInt)
            {
                errors.Add(path + "文件中没有配置表格列数坐标大于总共的 列数了");
            }

            Clazz clazz;
            T obj;
            String value;
            Object reulstObj;
            XWPFTableCell cell;
            XWPFParagraph p;
            XWPFRun run;
            for (int a = 0; a < objs.Count; a++)
            {
                obj = objs[a];
                XWPFTableRow row = rows[a + startRowIndex];
                for (int b = 0; b < cellTotalInt; b++)
                {
                    cell = row.GetCell(b);
                    p = cell.Paragraphs[0];
                    run = p.Runs[0];
                    if (clazzMap.TryGetValue(b + "", out clazz))
                    {
                        reulstObj = clazz.GetMethodInfo.Invoke(obj, null);
                        if (reulstObj != null)
                        {
                            run.SetText(reulstObj.ToString());
                        }

                    }
                    else if (configDic.TryGetValue(b + "", out value))
                    {
                        run.SetText(value);
                    }
                }
            }
        }

        public static void ReplaceTable2<T>(XWPFDocument doc, XMLTable xmlTable, IList<T> objects)
        {

            int tblIndex = xmlTable.TableIndex;
            XWPFTable tbl = doc.Tables[tblIndex];
            int rows = xmlTable.Rows;
            int objectCount = objects.Count;
            IList<XMLRow> xmlRows = xmlTable.XmlRows;
            int pageCount = 0;
            foreach (XMLRow xmlRow in xmlRows)
            {
                int pageRow = xmlRow.PageCount;
                pageCount = (objectCount + 1) / pageRow + 1;
                for (int a = 0; a < pageCount; a++)
                {
                    tbl = doc.Tables[a + tblIndex];
                    if (objectCount < pageRow)
                    {
                        ReplaceTable2_RowCell(tbl, xmlRow, objects, 0, objectCount);
                        ReplaceTable2_RowGroup(tbl, xmlRow, objects, 0, objectCount);
                        ReplaceTable2_Index<T>(tbl, xmlRow, objects, 0, objectCount);
                    }
                    else
                    {
                        int start = a * (pageRow - 1);
                        int end = (a + 1) * (pageRow - 1);
                        if (end > objectCount)
                        {
                            end = objectCount;
                        }
                        ReplaceTable2_RowCell(tbl, xmlRow, objects, start, end);
                        ReplaceTable2_RowGroup(tbl, xmlRow, objects, start, end);
                        ReplaceTable2_Index<T>(tbl, xmlRow, objects, start, end);
                    }
                }
            }
            //移除不要的
            int index = doc.BodyElements.IndexOf(tbl);
            //移除没有用的表格
            for (int a = pageCount; a < xmlTable.TableCount; a++)
            {
                doc.RemoveBodyElement(index + (pageCount * 2));
                doc.RemoveBodyElement(index + (pageCount * 2));
            }
        }
        private static int ReplaceTable2_RowCell<T>(XWPFTable tbl, XMLRow xmlRow, IList<T> objects, int start, int end)
        {
            XWPFTableRow row;
            Object obj;
            Object reuslut;
            XWPFTableCell cell;
            int rowStart = xmlRow.RowStartIndex;
            int rowStep = xmlRow.RowStep;
            Dictionary<int, XMLObject> cellDic = xmlRow.cellDic;
            //设置值
            if (cellDic != null && cellDic.Count != 0)
            {
                for (int a = start; a < end; a++)
                {
                    obj = objects[a];
                    row = tbl.GetRow(rowStart);
                    foreach (int cellIndex in cellDic.Keys)
                    {
                        cell = row.GetCell(cellIndex);
                        reuslut = XMLRead.GetObjectMethodResult(cellDic[cellIndex], obj);
                        SetCellValue(cell, reuslut);
                    }
                    rowStart += rowStep;
                }
            }
            return rowStart;

        }
        private static void ReplaceTable2_Index<T>(XWPFTable tbl, XMLRow xmlRow, IList<T> objects, int start, int end)
        {

            XWPFTableRow row;
            XWPFTableCell cell;
            //设置序列号
            if (xmlRow.XmlTable.Index != -1)
            {
                int rowStart = xmlRow.RowStartIndex;
                int rowStep = xmlRow.RowStep;
                rowStart = xmlRow.RowStartIndex;
                rowStep = xmlRow.RowStep;
                int xulie = xmlRow.Index;
                for (int a = start; a < end; a++)
                {
                    row = tbl.GetRow(rowStart);
                    cell = row.GetCell(xulie);
                    SetCellValue(cell, a + 1);
                    rowStart += rowStep;
                }
            }
        }
        private static void ReplaceTable2_RowGroup<T>(XWPFTable tbl, XMLRow xmlRow, IList<T> objects, int start, int end)
        {
            XWPFTableRow row;
            Object obj;
            Object reuslut;
            XWPFTableCell cell;
            int rowStart = xmlRow.RowStartIndex;
            int rowStep = xmlRow.RowStep;
            //设置分组数据               
            IList<XMLGroup> xmlGroups = xmlRow.XmlGroups;
            if (xmlGroups != null)
            {
                rowStart = xmlRow.RowStartIndex;
                rowStep = xmlRow.RowStep;
                for (int a = start; a < end; a++)
                {
                    obj = objects[a];
                    row = tbl.GetRow(rowStart);
                    foreach (XMLGroup xmlGroup in xmlGroups)
                    {
                        Dictionary<String, int> symbolDic = xmlGroup.GroupDic;
                        String symbol = xmlGroup.Result;
                        reuslut = XMLRead.GetObjectMethodResult(xmlGroup.XmlObject, obj);
                        cell = row.GetCell(symbolDic[reuslut.ToString()]);
                        SetCellValue(cell, symbol);
                    }
                    rowStart += rowStep;
                }
            }
        }
        /// <summary>
        /// 替换文字，不复制格式
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="xmlObject"></param>
        /// <param name="obj"></param>
        public static void ReplaceText(XWPFDocument doc, Dictionary<string, string> replaceDic)
        {

            foreach (XWPFTable table in doc.Tables)
            {
                foreach (XWPFTableRow row in table.Rows)
                {

                    foreach (XWPFTableCell cell in row.GetTableCells())
                    {


                        ReplaceText(cell.Paragraphs, replaceDic);
                    }
                }
            }
            ReplaceText(doc.Paragraphs, replaceDic);
        }
        public static void ReplaceText(IList<XWPFParagraph> ps, Dictionary<string, string> replaceDic)
        {
            if (!Utils.CheckListExists(ps))
            {
                return;
            }
            foreach (XWPFParagraph p in ps)
            {
                //Console.WriteLine(p.Text);
                if (p.Text == null || !p.Text.Contains("["))
                {
                    continue;
                }
                IList<XWPFRun> runs = p.Runs;
                int count = runs.Count;
                for (int a = 0; a < runs.Count; a++)
                {

                    int index;
                    int endIndex;
                    string text;
                    XWPFRun run = runs[a];
                    //得到第一段 与 最后一段
                    if (run.GetText(0) == null)
                    {
                        continue;
                    }
                    index = run.GetText(0).IndexOf("[");

                    if (index != -1)
                    {
                        text = run.GetText(0);
                        int b = a;
                        while (a < runs.Count)
                        {

                            run = runs[a];
                            if (index == -1)
                            {
                                b++;
                                index = run.GetText(0).IndexOf("[");
                                if (index == -1)
                                {
                                    break;
                                }
                            }
                            if (a == b)
                            {
                                endIndex = run.GetText(0).IndexOf("]", index);
                            }
                            else
                            {
                                endIndex = run.GetText(0).IndexOf("]");
                            }


                            if (a != 0 && !text.Contains(run.GetText(0)))
                            {
                                text = text + run.GetText(0);
                            }
                            if (endIndex != -1)
                            {
                               text = text.Substring(index + 1, text.IndexOf("]", index) - index - 1);

                                
                                string strValueTmp;
                                if (replaceDic.TryGetValue(text, out strValueTmp))
                                {
                                    //移除多余fun
                                    for (int c = b + 1; c <= a; c++)
                                    {
                                        if (c == a)
                                        {
                                            string s = runs[b + 1].GetText(0).Substring(endIndex + 1);
                                            runs[b + 1].SetText(s, 0);
                                        }
                                        else
                                        {
                                            p.RemoveRun(b + 1);
                                        }

                                    }
                                    if (strValueTmp == null)
                                    {
                                      
                                        ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index), "");
                                        a = b;
                                    }
                                    else
                                    {
                                        endIndex = p.Runs[b].GetText(0).IndexOf("]", index);
                                       
                                        if (endIndex != -1)
                                        {
                                                ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index, endIndex - index + 1), strValueTmp);
                                                a = b;
                                        }
                                        else
                                        {
                                                ReplaceText(p.Runs[b], p.Runs[b].GetText(0).Substring(index), strValueTmp);
                                                a = b + 1;
                                        }


                                    }
                                    a--;
                                    break;
                                }
                                else
                                {

                                    index = run.GetText(0).IndexOf("[", index + 1);
                                    if (index != -1)
                                    {
                                        text = run.GetText(0);
                                        a--;
                                    }
                                    else
                                    {
                                        text = "";
                                    }
                                }
                            }
                            a++;
                        }

                    }
                }
            }
        }

    }
}