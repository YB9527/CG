using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.XWPF.UserModel;
using System.IO;
using MyUtils;

namespace WordManager
{
   public class WordRead
    {
        public static XWPFDocument Read(String path)
        {
            Stream stream = null;
            try
            {
                stream = File.OpenRead(path);
                XWPFDocument doc = new XWPFDocument(stream);
                return doc;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        /// <summary>
        /// 将word解析成文本，主要看清段落
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IList<string> GetDocxText(XWPFDocument doc)
        {
            List<String> list = new List<String>();
            IList<IBodyElement>  bodys = doc.BodyElements;
            foreach(IBodyElement ele in bodys)
            {
                if(ele.ElementType == BodyElementType.TABLE)
                {
                    XWPFTable tbl = ele as XWPFTable;
                    GetTableText(tbl, list);
                }
                else
                {
                    XWPFParagraph p = ele as XWPFParagraph;
                   GetParagrapHText(p,list);
                
                }
                   
            }
            return list;
        }

        private static void GetTableText(XWPFTable tbl, List<string> list)
        {
            foreach (XWPFTableRow row in tbl.Rows)
            {
                foreach (XWPFTableCell cell in row.GetTableCells())
                {
                    foreach (XWPFParagraph p in cell.Paragraphs)
                    {
                        GetParagrapHText(p, list);
                        
                    }
                }
            }
        }

        private static void GetParagrapHText(XWPFParagraph p, List<string> list)
        {
            IList<XWPFRun> runs = p.Runs;
            foreach (XWPFRun run in runs)
            {
                list.Add(run.GetText(0));
            }
            
        }

        /// <summary>
        /// 得到 文本 映射 CellCustomer
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<String, IList<RunCustomer>> GetDocxDic(XWPFDocument doc)
        {
            Dictionary<String, IList<RunCustomer>> dic = new Dictionary<String, IList<RunCustomer>>();
            String text;
            IList<RunCustomer> list;
            RunCustomer cellCustomer;
            foreach (XWPFParagraph p in doc.Paragraphs)
            {
                foreach (XWPFRun run in p.Runs)
                {
                    text = run.GetText(0);
                    if (Utils.IsStrNull(text))
                    {
                        continue;
                    }
                    text = text.Trim();
                    cellCustomer = new RunCustomer();
                    cellCustomer.Run = run;
                    if (dic.TryGetValue(text, out list))
                    {
                        list.Add(cellCustomer);
                    }
                    else
                    {
                        list = new List<RunCustomer>();
                        list.Add(cellCustomer);
                        dic.Add(text, list);
                    }
                }
            }
            foreach (XWPFTable tbl in doc.Tables)
            {

                foreach (XWPFTableRow row in tbl.Rows)
                {
                    foreach (XWPFTableCell cell in row.GetTableCells())
                    {
                        foreach (XWPFParagraph p in cell.Paragraphs)
                        {
                            foreach (XWPFRun run in p.Runs)
                            {
                                text = run.GetText(0);
                                if (Utils.IsStrNull(text))
                                {
                                    continue;
                                }
                                text = text.Trim();

                                cellCustomer = new RunCustomer();
                                cellCustomer.Cell = cell;
                                cellCustomer.Run = run;
                                if (dic.TryGetValue(text, out list))
                                {
                                   
                                    list.Add(cellCustomer);
                                }
                                else
                                {
                                    list = new List<RunCustomer>();
                                    list.Add(cellCustomer);
                                    dic.Add(text, list);
                                }
                            }
                        }
                    }
                }
            }
            return dic;
        }
    }
}
