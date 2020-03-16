using ExcelManager;
using NPOI.XWPF.UserModel;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WordManager;
using Document = Spire.Doc.Document;
using Spire.Doc;
using Spire.Doc.Collections;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace ZaiJiDi.ZaiJiDiModel
{
   
    class JZXInfoCustom
    {
        public static string JZXTableName = "jzxinfo";
        public static string staticFile = System.AppDomain.CurrentDomain.BaseDirectory + "StaticFile\\ZaiJiDi\\";
        public static string bsbReflect = staticFile + "权籍资料反射表_标示表.xls";
        public static string jzxZLRelfect = staticFile + "界址线种类.xls"; 
         public static string qzbReflect = staticFile + "权籍资料反射表_签章表.xls";
        

        public static ObservableCollection<JZXInfo> GetMDBToJZX(string mdbPath)
        {
            ObservableCollection<JZXInfo> list =MyUtils.Utils.ListToObservableCollection( MDBUtils.ReadAllData<JZXInfo>(JZXTableName, mdbPath));

            int len = list.Count;
            int a = 0;
            int b = 0;
            for (int i = 0; i < len - 1; i++) //外循环为排序趟数，len个数进行len-1趟
                for (int j = 0; j < len - 1 - i; j++)
                { //内循环为每趟比较的次数，第i趟比较len-i次 
                    string qdh1 = list[j].QDH;
                    string qdh2 = list[j + 1].QDH; 
                    if(qdh1 != null && qdh2 !=null)
                    {
                        if(int.TryParse(qdh1.Replace("J",""),out a) && int.TryParse(qdh2.Replace("J", ""), out b))
                        {
                            if (a > b)
                            { // 相邻元素比较，若逆序则交换（升序为左大于右，降序反之） 
                                var temp = list[j];
                                list[j] = list[j + 1];
                                list[j + 1] = temp;
                            }
                        }
                    }
                   
                }
            


            return list;
        }

        /// <summary>
        /// 写入界址标示表word
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="jzxinfos"></param>
        /// <param name="configName"></param>
        public static void CreateBSB(XWPFDocument doc, IList<JZXInfo> jzxinfos)
        {

            
           
            //得到配置数据
            Dictionary<String, String> jzxConfigDic = ExcelRead.ReadExcelToDic(bsbReflect, 0);
            Dictionary<String, String> zlDic = ExcelRead.ReadExcelToDic(jzxZLRelfect, 0);

           
           // Dictionary<String, Clazz> clazzMap = ReflectUtils.ConfigNameToClazzMapCache(configName);
            IList<String> errors = new List<String>();


            //每页显示的个数
            String pageRowStr;
            jzxConfigDic.TryGetValue("pageRow", out pageRowStr);
            int pageRow = int.Parse(pageRowStr);

            //从第几张表开始
            String tableIndexStr;
            jzxConfigDic.TryGetValue("tableIndex", out tableIndexStr);
            int tableIndex = int.Parse(tableIndexStr);
            IList<XWPFTable> tables = doc.Tables;
            XWPFTable table;

            //总共有几张标示表
            String bsTableTotalStr;
            jzxConfigDic.TryGetValue("tableTotal", out bsTableTotalStr);
            int bsTableTotal = int.Parse(bsTableTotalStr);
            //得到表格
            int jzxCount = jzxinfos.Count;

            String startRowStr;
            jzxConfigDic.TryGetValue("startRow", out startRowStr);

            int startRow = int.Parse(startRowStr);

            for (int a = 0; a < (jzxCount + 1) / pageRow + 1; a++)
            {
                if (a == bsTableTotal)
                {//到达最大的表格数就不再进行
                    MessageBox.Show("界址点个数超过了最大的个数了，宗地编码是" + jzxinfos[0].BZDH);
                    return;
                }
                table = tables[a + tableIndex];
                if (jzxCount < pageRow)
                {
                   JzxWriteBSB_Page(table, startRow, jzxinfos, 0, jzxCount, jzxConfigDic, zlDic);

                }
                else
                {
                   JzxWriteBSB_Page(table, startRow, jzxinfos, a * (pageRow - 1), (a + 1) * (pageRow - 1), jzxConfigDic, zlDic);

                }

            }
            //移除没有用的表格      
            int index = jzxCount / pageRow + 1;
            for (int a = index; a < 3; a++)
            {
                
                doc.RemoveBodyElement(19 + ((index) * 3));
                doc.RemoveBodyElement(19 + ((index) * 3));
                doc.RemoveBodyElement(19 + ((index ) * 3));

    
            }

        }
        public static void JzxWriteBSB_Page(XWPFTable table, int startRow, IList<JZXInfo> jzxinfos, int jzxStart, int jzxEnd, Dictionary<string, string> jzxConfigDic, Dictionary<string, string> zlDic)
        {
            IList<XWPFTableRow> rows = table.Rows;
            int rowLength = rows.Count;
            XWPFTableRow row;
            JZXInfo jzxinfo;
            String zl;
            String lb;
            String wz;
            String cellIndexStr;
            int cellIndex;

            if (jzxEnd > jzxinfos.Count)
            {
                jzxEnd = jzxinfos.Count;
            }
            for (int a = jzxStart; a < jzxEnd; a++)
            {
                //前一行
                row = rows[startRow++];
                jzxinfo = jzxinfos[a];
                jzxConfigDic.TryGetValue("界址点号", out cellIndexStr);
                row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText(jzxinfo.QDH);


                lb = jzxinfo.JZXLB;
                zlDic.TryGetValue(lb, out zl);
                jzxConfigDic.TryGetValue(zl, out cellIndexStr);
                row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText("√", 0);

                //后面的要多一行
                row = rows[startRow++];
                jzxConfigDic.TryGetValue(lb, out cellIndexStr);
                row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText("√", 0);

                wz = jzxinfo.JZXWZ;
                jzxConfigDic.TryGetValue(wz, out cellIndexStr);
                row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText("√", 0);

                jzxConfigDic.TryGetValue("界址距离", out cellIndexStr);
                row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText(jzxinfo.TSBC.ToString("f2"), 0);

            }

            if (jzxEnd == jzxinfos.Count)
            {
                jzxinfo = jzxinfos[0];

            }
            else
            {
                jzxinfo = jzxinfos[jzxEnd];
            }
            jzxConfigDic.TryGetValue("界址点号", out cellIndexStr);
            row = rows[startRow];
            row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText(jzxinfo.QDH, 0);
            lb = jzxinfo.JZXLB;
            zlDic.TryGetValue(lb, out zl);
            jzxConfigDic.TryGetValue(zl, out cellIndexStr);
            row.GetCell(int.Parse(cellIndexStr)).Paragraphs[0].Runs[0].SetText("√", 0);
        }

        public static IList<JZXInfo> ReadBSBByDangAnDai(Document doc, string zdnum)
        {

            return null;
        }

        private static void JZXLastSet(IList<JZXInfo> jzxs)
        {
            JZXInfo jzx = null;
            int last = jzxs.Count - 1;
            for (int a = 0; a < last; a++)
            {
                jzx = jzxs[a];
                jzx.BZDH = "宗地编码";
                jzx.ZDH = jzxs[a + 1].QDH;

            }
            for (int a = 0; a < jzxs.Count; a++)
            {
               if(jzxs[a].TSBC == 0)
                {
                    jzxs.RemoveAt(a);
                    a--;
                }

            }
            //jzxs.RemoveAt(last);
        }

        public static void SetJZXLBAndWeiZhi(JZXInfo jzx, TableRow row)
        {
            

        }
       
    }
}
