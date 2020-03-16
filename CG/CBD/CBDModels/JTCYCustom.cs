using ExcelManager;
using NPOI.SS.UserModel;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utils = MyUtils.Utils;
namespace CBD.CBDModels
{
    public class JTCYCustom
    {
        public static string ReadJTCYExcelXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "CBDModels/JTCY.ReadExcel.xml";
        public static string ReadHZExcelXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "CBDModels/HZ.ReadExcel.xml";
        public static IList<JTCY> ReadJTCYExcel(string path)
        {
            ObservableCollection<JTCY> jTCies = ExcelUtils.GetExcelToObjectToObservableCollection<JTCY>(path, ReadJTCYExcelXMLPath);
            ObservableCollection<JTCY> hzs = ExcelUtils.GetExcelToObjectToObservableCollection<JTCY>(path, ReadHZExcelXMLPath);

            ObservableCollection<JTCY> list = new ObservableCollection<JTCY>();
            foreach (JTCY hz in hzs)
            {
                hz.YHZGX = "02";
             
                bool flag = true;
                for(int a =list.Count-1; a >=0; a-- )
                {
                    if(list[a].CBFBM == hz.CBFBM)
                    {
                        flag = false;
                        break;
                    }
                    
                }
                if (flag)
                {
                   
                    list.Add(hz);
                }
            }
            //去除家庭成员 没有名字的情况
            foreach(JTCY jtcy in jTCies)
            {
                if(!Utils.IsStrNull(jtcy.XM))
                {
                    list.Add(jtcy);
                }
               
            }
            return list;
        }

        internal static IList<JTCY> ExtractHZs(IList<JTCY> jTCies)
        {
            IList<JTCY> hzs = new List<JTCY>();
            Dictionary<string, IList<JTCY>> bmDic = Utils.GetGroupDicToList("CBFBM", jTCies);
            foreach(string bm in bmDic.Keys)
            {
                try
                {
                    JTCY hz = ExtractHZ(bmDic[bm]);
                    hzs.Add(hz);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    return null;
                }
            }
            return hzs;
        }

       
        private static JTCY ExtractHZ(IList<JTCY> list)
        {
            JTCY hz = null;
            foreach(JTCY jtcy in list)
            {
                if(jtcy.YHZGX == "02" || jtcy.YHZGX == "户主")
                {
                    hz = jtcy;
                    break;
                }
            }
            if(hz == null)
            {
                throw new Exception("家庭成员没有户主:"+list[0].XM);
            }else
            {
                hz.jTCies = list;
                return hz;
            }
        }

        /// <summary>
        /// 设置每一个户主的 地块 
        /// </summary>
        /// <param name="hzs"></param>
        /// <param name="dks"></param>
        internal static void SetJTCY_DK(IList<JTCY> hzs, IList<DK> dks)
        {
            Dictionary<string, IList<DK>> cbfbmDKs = Utils.GetGroupDicToList("CBFBM", dks);
            IList<DK> dkTemp;
            foreach (JTCY hz in hzs)
            {
                string cbfbm = hz.CBFBM;
                if(cbfbmDKs.TryGetValue(cbfbm,out dkTemp))
                {
                    hz.DKs = dkTemp;
                }
            }
        }

        
        /// <summary>
        /// 导出 农村土地家庭承包农户基本情况信息表(一)
        /// </summary>
        /// <param name="hzs"></param>
        public static bool ExportBsicInformationExcel(IList<JTCY> hzs,string saveDir)
        {
            Dictionary<string, string> dklbDic = new Dictionary<string, string>();
            dklbDic.Add("10","承包地");
            dklbDic.Add("21", "自留地");
            dklbDic.Add("22", "机动地");
            dklbDic.Add("23", "开荒地");
            dklbDic.Add("99", "其他集体土地");
            

            if (Utils.IsStrNull(saveDir))
            {
                return false;
            }
            string basicInformationExxcelTemplete = System.AppDomain.CurrentDomain.BaseDirectory + "CBDTemplete\\农村土地家庭承包农户基本情况信息表(一)模板.xls";
            Dictionary<int, XMLTable> ClazzDic = XMLRead.GetXmlToXMLTabl(System.AppDomain.CurrentDomain.BaseDirectory+ "CBDTemplete\\WriteJTCYBaicInformationExcel.xml");
            Dictionary<string, XMLObject> xmlObjectDic = XMLRead.XmlToObjects(System.AppDomain.CurrentDomain.BaseDirectory + "CBDTemplete\\WriteJTCYBaicInformationExcel.xml");



            string basicInformationExxcelTemplete2 = System.AppDomain.CurrentDomain.BaseDirectory + "CBDTemplete\\农村土地承包经营权(家庭承包)台帐(二)模板.xls";
            Dictionary<int, XMLTable> ClazzDic2 = XMLRead.GetXmlToXMLTabl(System.AppDomain.CurrentDomain.BaseDirectory + "CBDTemplete\\WriteDKBaicInformationExcel.xml");
            Dictionary<string, XMLObject> xmlObjectDic2 = XMLRead.XmlToObjects(System.AppDomain.CurrentDomain.BaseDirectory + "CBDTemplete\\WriteDKBaicInformationExcel.xml");


            string cbfbm;
            if (Utils.CheckListExists(hzs))
            {
                DMToText(hzs);
                foreach(JTCY hz in hzs)
                {
                    string cbfdz = hz.DZ;
                    if (!cbfdz.EndsWith("组"))
                    {
                        cbfdz = cbfdz.Substring(0, cbfdz.LastIndexOf("组")) + "组";
                    }
                    int index = cbfdz.IndexOf("镇");
                    if (index == -1)
                    {
                        index = cbfdz.IndexOf("乡");
                    }
                    hz.DZ = cbfdz.Substring(index + 1, cbfdz.Length - index - 1);
                    XMLTable xmlTable = ClazzDic[0];
                    IWorkbook workbook = ExcelRead.ReadExcel(basicInformationExxcelTemplete);
                    ISheet sheet = workbook.GetSheetAt(0);
                    //家庭成员表行数据插入
                    ExcelWrite.WriteObjects(sheet, xmlTable, hz.jTCies);
                    //家庭成员表 文字替换
                    ExcelWrite.ReplaceTextByXMLObject(sheet, hz, xmlObjectDic);
                    ExcelWrite.Save(workbook,  saveDir+ "//TZ-" +  hz.CBFBM.Substring(0,14)+" " +cbfdz+ "\\TZ4 -" + hz.CBFBM+ "-1农村土地家庭承包农户基本情况信息表(一).xls");

                    //写地块表
                    Dictionary<string, string> tdlylxDic = ExcelRead.ReadExcelToDic(System.AppDomain.CurrentDomain.BaseDirectory + "CBDTemplete\\土地利用类型映射表.xlsx",0);
                    IList <DK> dks = hz.DKs;
                    if(dks == null)
                    {
                        continue;
                    }
                    foreach(DK dk in dks)
                    {
                       
                        if (dk.DKLB == "21")
                        {
                            dk.ZLDMJ = Math.Round(dk.HTMJ, 2).ToString("f2");
                        }
                        else
                        {
                            dk.ELCBMJ = Math.Round(dk.HTMJ, 2).ToString("f2");
                        }
                        
                        
                        string dklb = dk.DKLB;
                        if(dklb != null)
                        {
                            if(dklbDic.TryGetValue(dklb,out dklb))
                            {
                                //地块类别无法识别的情况
                                dk.DKLB = dklb;
                            }

                        }
                        string tdlylx = dk.TDLYLX;
                        if(tdlylx != null)
                        {
                            if(tdlylxDic.TryGetValue(tdlylx,out tdlylx))
                            {
                                dk.TDLYLX = tdlylx;
                            }
                        }

                    }
                    IWorkbook workbook2 = ExcelRead.ReadExcel(basicInformationExxcelTemplete2);
                    ISheet sheet2 = workbook2.GetSheetAt(0);
                    ExcelWrite.ReplaceTextByXMLObject(sheet2, hz, xmlObjectDic);
                    ExcelWrite.WriteObjects(sheet2, ClazzDic2[0], hz.DKs);
                    

                    for (int a =0; a < dks.Count ;a++)
                    {
                        IRow row = sheet.GetRow(a + 5);
                    }

                    ExcelWrite.Save(workbook2, saveDir + "//TZ-" + hz.CBFBM.Substring(0, 14) + " " + cbfdz + "\\TZ4 -" + hz.CBFBM + "-2农村土地承包经营权(家庭承包)台帐(二).xls");

                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 代码关系转换 为文字关系
        /// </summary>
        /// <param name="hzs"></param>
        private static void DMToText(IList<JTCY> hzs)
        {
            int value;
            Dictionary<string, string> yhzgxDic = ExcelRead.ReadExcelToDic(System.AppDomain.CurrentDomain.BaseDirectory+ "家庭关系代码.xls", 0);
            foreach(JTCY hz in hzs)
            {
                foreach(JTCY jtcy in hz.jTCies)
                {
                    string sex = jtcy.XB;
                    if(sex != null )
                    {
                        if(int.TryParse(sex,out value))
                        {
                            if(value % 2 == 0)
                            {
                                jtcy.XB = "男";
                            }else
                            {
                                jtcy.XB = "女";
                            }
                        }
                    }
                    string yhzgx = jtcy.YHZGX;
                    
                    if(!Utils.IsStrNull(yhzgx) && yhzgxDic.TryGetValue(yhzgx,out yhzgx))
                    {
                        jtcy.YHZGX = yhzgx;
                    }
                    string sfgyr = jtcy.SFGYR;
                    if(sfgyr == "1" || sfgyr == "是")
                    {
                        jtcy.SFGYR = "是";
                    }else
                    {
                        jtcy.SFGYR = "否";
                    }
                    
                }
            }
        }
    }
}
