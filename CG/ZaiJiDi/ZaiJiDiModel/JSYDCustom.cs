using ExcelManager;
using ExcelManager.Model;
using NPOI.SS.UserModel;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.Pages.ZJDPage.ExportData;
using Utils = MyUtils.Utils;
namespace ZaiJiDi.ZaiJiDiModel
{
    public class JSYDCustom
    {
       
        public static string XMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/JSYD.Excel.hbm.xml";
        public static string GYRXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/GYR.Excel.hbm.xml";
        public static string ExcelWriteXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/JSYD.WriteExcel.xml";

        public static string staticFile = System.AppDomain.CurrentDomain.BaseDirectory + "StaticFile\\ZaiJiDi\\";
        public static string DanAnDaiDocPath = staticFile + "0_档案袋.doc";
        public static string SPBDocPath = staticFile + "1_确权登记申请审批表.doc";
        public static string WTSDocPath = staticFile + "2_户代表委托书.doc";
        public static string SMSDocPath = staticFile + "3_户代表声明书.doc";
        public static string QJDCBDocPath = staticFile + "6_宅基地权籍调查表.doc";
        public static string CHBGDocPath = staticFile + "8_房产测绘报告.doc";

        public static string JSYDXMLPath = staticFile + "建设用地表替换.xml";
        public static string JTCYXMLPath = staticFile + "家庭成员表替换.xml";
        public static string NFXMLPath = staticFile + "农房表替换.xml";
        public static string FloorXMLPath = staticFile + "分层表替换.xml"; 
        public static string ZJDXZDMMLPath = staticFile + "行政代码表替换.xml";


        /// <summary>
        /// 深度克隆
        /// </summary>
        /// <param name="jsyd"></param>
        /// <returns></returns>
        public static JSYD Clone(JSYD jsyd)
        {
            JSYD clone = jsyd.Clone();

            IList<Floor> floors = ReflectManager.ReflectUtils.CloneList(jsyd.Floors);
            if(floors != null)
            {
                foreach (Floor floor in floors)
                {
                    floor.JSYD = clone;
                }
                clone.Floors = floors;
            }

            IList<NF> nfs = ReflectManager.ReflectUtils.CloneList(jsyd.NFs);
            if(nfs != null )
            {
                foreach (NF nf in nfs)
                {
                    nf.JSYD = clone;
                }
                clone.NFs = nfs;
            }

            ObservableCollection<JTCY> hzs = ReflectManager.ReflectUtils.CloneList(jsyd.HZs);
            if(hzs != null)
            {
                foreach (JTCY hz in hzs)
                {
                    IList<JTCY> jTCies = ReflectManager.ReflectUtils.CloneList(hz.JTCies);

                    ////设置户主 是同一个引用
                    for (int a = 0; a < jTCies.Count; a++)
                    {
                        if (jTCies[a].XM == hz.XM)
                        {

                            jTCies[a] = hz;
                            hz.JTCies = jTCies;
                            break;
                        }
                    }
                }

                clone.HZs = hzs;
            }

            ObservableCollection<JZXInfo> jZXInfos = ReflectManager.ReflectUtils.CloneList(jsyd.JZXInfos);
            clone.JZXInfos = jZXInfos;

            ObservableCollection<QZB> qzbs = ReflectManager.ReflectUtils.CloneList(jsyd.QZBs);
            clone.QZBs = qzbs;
            return clone;
        }

        public static ObservableCollection<JSYD> GetExcelToJSYDS(string path)
        {
            ObservableCollection<JSYD> list = ExcelUtils.GetExcelToObjectToObservableCollection<JSYD>(path, XMLPath);

            return list;
        }

        internal static void SetHZs(IList<JSYD> jsyds, IList<JSYD> gyrs, IList<JTCY> hzs)
        {
            Dictionary<string, JTCY> hzZJHMDic = null;
            try
            {

                hzZJHMDic = MyUtils.Utils.GetGroupDic("GMSFHM", hzs);
            }
            catch (Exception e)
            {
                MessageBox.Show("家庭成员表：证件号码：" + e.Message);
                return;
            }
            Dictionary<string, IList<JSYD>> gyrDic = Utils.GetGroupDicToList("ZDNUM", gyrs);
            JTCY hz;
            IList<JSYD> gyrTem;
            foreach (JSYD jsyd in jsyds)
            {
                string zjhm = jsyd.ZJHM;
                if (!Utils.IsStrNull(zjhm) && hzZJHMDic.TryGetValue(zjhm, out hz))
                {

                    jsyd.HZs.Add(hz);
                }
                //
                string zdnum = jsyd.ZDNUM;
                if (gyrDic.TryGetValue(zdnum, out gyrTem))
                {
                    foreach (JSYD gyr in gyrTem)
                    {
                        zjhm = gyr.ZJHM;
                        if (!Utils.IsStrNull(zjhm) && hzZJHMDic.TryGetValue(zjhm, out hz))
                        {

                            jsyd.HZs.Add(hz);
                        }
                    }
                }
            }
        }

       

        internal static void SetZJDXZDM(Dictionary<string, JSYD> jsydDic, IList<ZJDXZDM> zjdXZDM)
        {
            Dictionary<string, ZJDXZDM> xzdmDic = Utils.GetGroupDic("DJZQDM", zjdXZDM);
            ZJDXZDM xzdm;
            foreach (string zdnum in jsydDic.Keys)
            {

                if (zdnum.Length >= 14 && xzdmDic.TryGetValue(zdnum.Replace("JB", "").Replace("JC", "").Substring(0, 14), out xzdm))
                {
                    jsydDic[zdnum].ZJDXZDM = xzdm;
                }
            }
        }

        public static ObservableCollection<JSYD> GetExcelToGYRS(string path)
        {
            ObservableCollection<JSYD> list = ExcelUtils.GetExcelToObjectToObservableCollection<JSYD>(path, GYRXMLPath);

            return list;
        }


        internal static void SetZdinfo(Dictionary<string, JSYD> jsydDic, IList<Zdinfo> zdinfos)
        {
            Dictionary<string, Zdinfo> zdinfoDic = Utils.GetGroupDic("ZDNUM", zdinfos);
            JSYD jsyd;
            foreach (string zdnum in zdinfoDic.Keys)
            {
                if (jsydDic.TryGetValue(zdnum, out jsyd))
                {
                    jsyd.Zdinfo = zdinfoDic[zdnum];
                }
            }
        }

        internal static void SetNF(Dictionary<string, JSYD> jsydDic, IList<NF> nfs)
        {
            Dictionary<string, IList<NF>> dic = null;
            try
            {
                dic = Utils.GetGroupDicToList("ZDNUM", nfs);
            }
            catch (Exception e)
            {
                MessageBox.Show("农房表：宗地编码：" + e.Message);
                return;
            }


            JSYD jsyd;
            foreach (string zdnum in dic.Keys)
            {
                if (jsydDic.TryGetValue(zdnum, out jsyd))
                {
                    jsyd.NFs = dic[zdnum];
                    foreach(NF nf in jsyd.NFs)
                    {
                        nf.JSYD = jsyd;
                    }
                }
            }
        }

        internal static void SetFloor(Dictionary<string, JSYD> jsydDic, IList<Floor> floors)
        {
            Dictionary<string, IList<Floor>> dic = null;
            try
            {
                dic = Utils.GetGroupDicToList("ZDNUM", floors);
            }
            catch (Exception e)
            {
                MessageBox.Show("分层表：宗地编码：" + e.Message);
                return;
            }

            JSYD jsyd;
            foreach (string zdnum in dic.Keys)
            {
                if (jsydDic.TryGetValue(zdnum, out jsyd))
                {
                    jsyd.Floors = dic[zdnum];
                    foreach(Floor floor in jsyd.Floors)
                    {
                        floor.JSYD = jsyd;
                    }
                }
            }
        }

        /// <summary>
        /// 设置签章表
        /// </summary>
        /// <param name="jsydDic"></param>
        /// <param name="qzbs"></param>
        internal static void SetQZBs(ObservableCollection<JSYD> jsyds, ObservableCollection<QZB> qzbs)
        {
            Dictionary<string, ObservableCollection<QZB>> dic = null;
            try
            {
                dic = Utils.GetGroupDicToList("BZDH", qzbs);
            }
            catch (Exception e)
            {
                MessageBox.Show("界址线表：宗地编码：" + e.Message);
                return;
            }


            ObservableCollection<QZB> list;
            foreach (JSYD jsyd in jsyds)
            {
                if (!Utils.IsStrNull(jsyd.ZDNUM) && dic.TryGetValue(jsyd.ZDNUM, out list))
                {
                    jsyd.QZBs = list;
                }
            }
        }

        /// <summary>
        /// 设置界址线表
        /// </summary>
        /// <param name="jsydDic"></param>
        /// <param name="jzxs"></param>
        internal static void SetJZXs(ObservableCollection<JSYD> jsyds, ObservableCollection<JZXInfo> jzxs)
        {
            Dictionary<string, ObservableCollection<JZXInfo>> dic = null;
            try
            {
                dic = Utils.GetGroupDicToList("BZDH", jzxs);
            }
            catch (Exception e)
            {
                MessageBox.Show("界址线表：宗地编码：" + e.Message);
                return;
            }


            ObservableCollection<JZXInfo> list;
            foreach (JSYD jsyd in jsyds)
            {
                if (!Utils.IsStrNull(jsyd.ZDNUM) && dic.TryGetValue(jsyd.ZDNUM, out list))
                {
                    jsyd.JZXInfos = list;
                }
            }
        }

        public static  XMLTable GetWriteExcelXMLTable()
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(ExcelWriteXMLPath)[0];
            return xmlTable;
        }

        
        public static void SavejSYDRow(JSYD jsyd, ZJDDataSourceViewModel dataSource)
        {
            if(jsyd == null)
            {
                return;
            }
       

            IList<JTCY> hzs = jsyd.HZs;
            if(hzs != null)
            {
                Dictionary<int, XMLObject> xmlObjectDic = JTCYCustom.GetWriteExcelXMLTable().CellDic;
                foreach (JTCY hz in hzs)
                {
                    foreach(JTCY jtcy in hz.JTCies)
                    {
                        //第三个户主的名称也要更改
                        ICell cell = jtcy.Row.GetCell(1);
                        if ( cell!=null &&cell.CellType == CellType.String)
                        {
                            cell.SetCellValue(hz.XM);
                        }
                        ExcelWrite.WriteRowObject(jtcy, jtcy.Row, xmlObjectDic);
                    }
                }
                UpdateJSYD_HZ(jsyd,hzs[0]);
                ExcelWrite.Save(hzs[0].Row.Sheet.Workbook,dataSource.JTCYTablePath);
            }
            ExcelWrite.WriteRowObject(jsyd, jsyd.Row, GetWriteExcelXMLTable().CellDic);

            ExcelWrite.Save(jsyd.Row.Sheet.Workbook, dataSource.JSYDTablePath);
            //农房表不用保存，分层表保存就行
            IList<NF> nfs = jsyd.NFs;
            if (nfs != null)
            {

                Dictionary<int, XMLObject> xmlObjectDic = NFCustom.GetWriteExcelXMLTable().CellDic;
                foreach (NF nf in nfs)
                {
                    NFCustom.UpdateNF_HZ(nf,hzs[0]);
                    ExcelWrite.WriteRowObject(nf, nf.Row, xmlObjectDic);
                }
                
            }

            IList<Floor> floors = jsyd.Floors;
            if (floors != null)
            {
                Dictionary<int, XMLObject> xmlObjectDic = FloorCustom.GetWriteExcelXMLTable().CellDic;
                foreach (Floor floor in floors)
                {
                    ExcelWrite.WriteRowObject(floor, floor.Row, xmlObjectDic);
                }
                ExcelWrite.Save(floors[0].Row.Sheet.Workbook, dataSource.NFTablePath);
            }
        }

        /// <summary>
        /// 根据家庭成员更新 建设用地信息
        /// </summary>
        /// <param name="jsyd"></param>
        /// <param name="jTCY"></param>
        private static void UpdateJSYD_HZ(JSYD jsyd, JTCY hz)
        {
            jsyd.QLRMC = hz.XM;
            jsyd.ZJHM = hz.GMSFHM;
        }
    }
}
