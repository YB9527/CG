using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGisManager;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ExcelManager;
using ExcelManager.Model;
using FileManager;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;
using MyUtils;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;
using ReflectManager;
using ReflectManager.XMLPackage;
using WordManager;
using System.Windows;
using System.Runtime.InteropServices;

namespace JTSYQManager.Dao
{
    public class JTSYQDao : IJTSQYDao
    {

        private static ArcGisUtils arcGisUtils;
        public JTSYQDao()
        {
            arcGisUtils = ArcGisManager.ArcGisUtils.GetInstance();
        }
        public void ExportCun_GongShiTuActions(XZDM cunXZDM, string saveDir)
        {
            //IList<JTSYQ> jtsyqs = null;
            //得到所有界址短线
            IList<JZX> jzxs = JZXCustom.SetJZX(cunXZDM.JTSYQS);
            //从中删除本村的界址短线
            for(int a =0; a < jzxs.Count; a++)
            {
                JZX jzx = jzxs[a];
                string str = jzx.BM.Substring(0, 12);
                if (str == cunXZDM.DJZQDM)
                {
                    jzxs.RemoveAt(a);
                    a--;
                }
            }

            string mxdPath = saveDir + "\\" + System.IO.Path.GetFileName(JTSYQCustom.JTSYQGSTMxd);
            string mdbPath = saveDir + "\\" + System.IO.Path.GetFileName(JTSYQCustom.JTSYQGSTMDB);
            //1、复制maxd
            FileUtils.CopyFile(JTSYQCustom.JTSYQGSTMxd, mxdPath);
            //2、复制数据库
            FileUtils.CopyFile(JTSYQCustom.JTSYQZDTMdb, mdbPath);

            //打开工作文件
            MapDocumentClass mapDocument = new MapDocumentClass();
            mapDocument.Open(mxdPath, "");
            AxMapControl mapControl = ArcGisUtils.axMapControl;
            IMap map = mapDocument.Map[0];
            mapControl.Map = map;
            //3、设置数据源,数据源是相对路径，不需要设置
            //4、复制 本村的界址线shap
            JTSYQCustom.SaveMap(cunXZDM.JTSYQS);
            //设置本村的界址点标注
            JTSYQCustom.SaveJTSYQBZMap(map, cunXZDM.JTSYQS);

          
            JZXCustom.SaveJZXMap(jzxs);
            JZXCustom.SaveJZXBZMap(map, jzxs);

            double scale = 100.0;
            // mapControl.Extent = JTSYQCustom.SetExtent(cunXZDM.JTSYQS);
            ArcGisUtils.axMapControl.Extent = JTSYQCustom.GetLayer().AreaOfInterest;
            double num = (double)((int)((map.MapScale + 30.0) / scale) * scale + scale);
            //mapControl.MapScale = 329;
            ArcGisUtils.ChangeMapScale(mapDocument, num);
            //5、替换文字内容,   
            Dictionary<string, XMLObject> xmlObjectDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            XMLObject xmlObject = new XMLObject { Column = "BLC", Deafult = "1:" + num };
            xmlObjectDic.Add("BLC", xmlObject);
            ArcGisService.ReplaceText(cunXZDM, xmlObjectDic, mapDocument);
            //使用的xml缓存，必须移除
            xmlObjectDic.Remove("BLC");
            //6、工程文件保留
            mapDocument.Save();
            mapDocument.Close();
            ArcGisUtils.Refresh();
        }
        public void ExportZu_ZhongDiTu(JTSYQ jtsyqGroup, string saveDir)
        {

            IList<JZX> jzxs = JZXCustom.SetJZX(jtsyqGroup.GroupJTSYQ);
            string mxdPath = saveDir + "\\" + System.IO.Path.GetFileName(JTSYQCustom.JTSYQZDTMxd);
            string mdbPath = saveDir + "\\" + System.IO.Path.GetFileName(JTSYQCustom.JTSYQZDTMdb);
            //1、复制maxd
            FileUtils.CopyFile(JTSYQCustom.JTSYQZDTMxd, mxdPath);
            //2、复制数据库
            FileUtils.CopyFile(JTSYQCustom.JTSYQZDTMdb, mdbPath);
            //打开工作文件
            MapDocumentClass mapDocument = new MapDocumentClass();
            mapDocument.Open(mxdPath, "");
            AxMapControl mapControl = ArcGisUtils.axMapControl;
            IMap map = mapDocument.Map[0];
            mapControl.Map = map;
            //3、设置数据源,数据源是相对路径，不需要设置
            //4、复制shap

            JTSYQCustom.SaveMap(jtsyqGroup.GroupJTSYQ);
            JTSYQCustom.SaveJTSYQBZMap(map, jtsyqGroup.GroupJTSYQ);
            IList<JZD> jzds = JZDCustom.SaveMap(jtsyqGroup);        
            JZXCustom.SaveJZXMap(jzxs);
            JZXCustom.SaveJZXBZMap(map, jzxs);
            JZDCustom.SaveJZDBZMap(map, jzds);

            double scale = 100.0;
            mapControl.Extent = JTSYQCustom.SetExtent(jtsyqGroup.GroupJTSYQ);
           
            double num = (double)((int)((map.MapScale + 30.0) / scale) * scale + scale);
            //mapControl.MapScale = 329;
            ArcGisUtils.ChangeMapScale(mapDocument, num);
            //5、替换文字内容,
            int index = 0;

            for(int a =0;a >-1;a++)
            {
                string tufu = jtsyqGroup.TuFu;
                if(Utils.IsStrNull(tufu))
                {
                    MessageBox.Show("你还没有生成图幅号！！！："+jtsyqGroup.QLR);
                    ArcGisUtils.Refresh();
                    return;
                }
                index = tufu.IndexOf("、", index+1);
                if(index ==-1)
                {
                    break;
                }
                if(a % 2 != 0)
                {
                    jtsyqGroup.TuFu = tufu.Insert(index+1, "\n");
                }
            }
            Dictionary<string, XMLObject> xmlObjectDic = XMLRead.XmlToObjects(JTSYQCustom.JTSYQ_Reflect, true);
            XMLObject xmlObject = new XMLObject { Column = "BLC", Deafult = "1:"+num };
            xmlObjectDic.Add("BLC", xmlObject);
            ArcGisService.ReplaceText(jtsyqGroup, xmlObjectDic, mapDocument);
            //使用的xml缓存，必须移除
            xmlObjectDic.Remove("BLC");
            //6、工程文件保留
            mapDocument.Save();
            mapDocument.Close();
            ArcGisUtils.Refresh();
        }

        public void  ExportZu_ZhongDiBiao(JTSYQ jtsyqGroup, string saveDir)
        {
         //   JTSYQCustom.SetContainsFeatureArea(jtsyqGroup);

            Dictionary<int, XMLTable> ClazzDic = XMLRead.GetXmlToXMLTabl(JTSYQCustom.DangZhongDi_Reflect);


            XMLTable xmlTable = ClazzDic[1];
            IWorkbook workbook = ExcelRead.ReadExcel(JTSYQCustom.DangZhongDi);
            ISheet sheet = workbook.GetSheetAt(0);

            //行数据插入
            IList<JTSYQ> list = new List<JTSYQ>();
            list.Add(jtsyqGroup);
            double area = jtsyqGroup.Area;
            jtsyqGroup.Area = jtsyqGroup.Area * 10000;
            ExcelWrite.WriteObjects(sheet, xmlTable, list);
            ExcelWrite.Save(workbook, saveDir + "//" + jtsyqGroup.BM+".xls");
            jtsyqGroup.Area = area;

        }
        public void ExportZu_QuanJiDiaoChaBiao(JTSYQ jtsyqGroup, string saveDir)
        {
            var doc = WordRead.Read(JTSYQCustom.QuanJiDiaoChaBiao);
            List<JZD> jzds = new List<JZD>();
            int jzdCount = 0;
            foreach (JTSYQ jtsyq in jtsyqGroup.GroupJTSYQ)
            {
                IList<JZD> temp = jtsyq.JZDS;
                if (Utils.CheckListExists(temp))
                {
                    jzdCount += temp.Count;

                    jzds.AddRange(temp);
                    jzds.Add(temp[0]);
                    JZD jzd = new JZD();
                    jzds.Add(jzd);

                }
                else
                {
                    MessageBox.Show(jtsyq.QLR + ",图上还没有界址点！！！");
                    return;
                }
            }
            jtsyqGroup.JZDCount = jzdCount;
            //最后一个空白不要
            jzds.RemoveAt(jzds.Count - 1);
            int jzdEndTableIndex = QuanJiDiaoChaBiaoWriteJZDS(doc, jzds);
            List<QZB> qzbs = new List<QZB>();
            foreach (JTSYQ jtsyq in jtsyqGroup.GroupJTSYQ)
            {
                qzbs.AddRange(jtsyq.QZBS);
            }
            QuanJiDiaoChaBiaoWriteQZBS(doc, jzdEndTableIndex + 1, qzbs);

           // JTSYQCustom.SetContainsFeatureArea(jtsyqGroup);
            XZDM xzdm = jtsyqGroup.XZDM;
            Dictionary<string, XMLObject> xzdmDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            WordWrite.ReplaceText(doc, xzdmDic, xzdm);
            Dictionary<string, XMLObject> jtsyqDic = XMLRead.XmlToObjects(JTSYQCustom.JTSYQ_Reflect, true);
            WordWrite.ReplaceText(doc, jtsyqDic, jtsyqGroup);
            WordWrite.SaveToFile(doc, saveDir + "/" + System.IO.Path.GetFileName(JTSYQCustom.QuanJiDiaoChaBiao));


        }

        private void QuanJiDiaoChaBiaoWriteQZBS(XWPFDocument doc, int startTableIndex, IList<QZB> list)
        {

            if (list == null)
            {
                return;
            }
            int pageMax = 25;
            IList<XWPFTable> tbls = doc.Tables;

            int endIndex = doc.BodyElements.IndexOf(tbls[pageMax + startTableIndex - 1]);

            int onePageCount = 16;
            int pageCount = (int)(list.Count / (onePageCount + 0.01));
            int startIndex = doc.BodyElements.IndexOf(tbls[startTableIndex + pageCount]);
            for (int a = startIndex + 2; a <= endIndex + 1; a++)
            {
                doc.RemoveBodyElement(startIndex + 2);
            }
            for (int a = 0; a <= pageCount; a++)
            {
                WriteQZBPage(tbls[a + startTableIndex], list, a * 16, (a + 1) * 16);
            }

        }
        /// <summary>
        /// 写入签章表
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void WriteQZBPage(XWPFTable tbl, IList<QZB> qzbs, int start, int end)
        {
            if (end >= qzbs.Count)
            {
                end = qzbs.Count;
            }
            int rowIndex = 3;
            XWPFTableRow row;
            while (start < end)
            {
                QZB qzb = qzbs[start];
                row = tbl.GetRow(rowIndex++);
                WordWrite.SetCellText(row.GetCell(0), "J" + qzb.StartDH);
                WordWrite.SetCellText(row.GetCell(1), "J" + qzb.EndDH);
                WordWrite.SetCellText(row.GetCell(2), qzb.MS);
                start++;
            }
        }

        /// <summary>
        /// 写入界址点成果表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="jzds"></param>
        /// <returns>返回写入的界址点的最后一页的 tablIndex 下标</returns>
        private int QuanJiDiaoChaBiaoWriteJZDS(XWPFDocument doc, List<JZD> jzds)
        {
            int initPage = 2;
            int pageMax = 25;
            double countPage = 16.001;
            int pageCount = (int)(jzds.Count / countPage);

            if (pageCount > pageMax)
            {
                throw new Exception("权籍调查表写入界址点时超出了" + pageMax + "页，请改代码");
            }
            //移除不要的页面
            IList<XWPFTable> tbls = doc.Tables;
            int startIndex = doc.BodyElements.IndexOf(tbls[pageCount + initPage]);
            int endIndex = doc.BodyElements.IndexOf(tbls[pageMax - 1 + initPage]);

            for (int a = startIndex; a < endIndex; a++)
            {
                doc.RemoveBodyElement(startIndex);
            }
            //写入界址点

            for (int a = 0; a <= pageCount; a++)
            {
                WriteJZDSPage(tbls[a + initPage], jzds, a * 16, (a + 1) * 16);
            }
            return pageCount + initPage;


        }
        /// <summary>
        /// 写入界址
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="jzds"></param>
        /// <param name="start">界址点下标</param>
        /// <param name="end"></param>
        private void WriteJZDSPage(XWPFTable tbl, List<JZD> jzds, int start, int end)
        {
            bool isNotLast = true;
            if (end >= jzds.Count)
            {
                isNotLast = false;
                end = jzds.Count;
            }
            int rowIndex = 3;
            XWPFTableRow row;
            XWPFTableCell cell;
            while (start < end)
            {
                JZD jzd = jzds[start];
                row = tbl.GetRow(rowIndex++);
                cell = row.GetCell(0);
                if(jzd.JZDH !=0)
                {
                    WordWrite.SetCellText(cell, "J" + jzd.JZDH);
                    cell = row.GetCell(4);
                    WordWrite.SetCellText(cell, "√");
                }
                
                if (start != 0 && jzd.JZDH< jzds[start-1].JZDH)
                {
                    rowIndex++;
                }
                else
                {
                   
                    cell = row.GetCell(4);
                    WordWrite.SetCellText(cell, "√");
                    if (isNotLast || start != jzds.Count - 1)
                    {
                        row = tbl.GetRow(rowIndex++);
                        cell = row.GetCell(7);
                        WordWrite.SetCellText(cell, "√");
                        cell = row.GetCell(16);
                        WordWrite.SetCellText(cell, "√");
                    }
                }
                start++;
            }
        }

        public void ExportCun_JiTiTuDiDiaoChaBiao(IList<JTSYQ> jtsyqs, string saveDir)
        {
            Dictionary<int, XMLTable> ClazzDic = XMLRead.GetXmlToXMLTabl(JTSYQCustom.JiTiTuDiDiaoChaBiao_Reflect);
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                jtsyq.XZDM = XZDMCustom.GetXzdm(XZDMCustom.JTSYQBianMaToXZDM(jtsyq.BM));
               // JTSYQCustom.SetContainsFeatureArea(jtsyq);
            }

            XMLTable xmlTable = ClazzDic[1];
            IWorkbook workbook = ExcelRead.ReadExcel(JTSYQCustom.JiTiTuDiDiaoChaBiao);
            ISheet sheet = workbook.GetSheetAt(0);

            //行数据插入
            ExcelWrite.WriteObjects(sheet, xmlTable, jtsyqs);
            ExcelWrite.Save(workbook, saveDir + "//" + System.IO.Path.GetFileName(JTSYQCustom.JiTiTuDiDiaoChaBiao));


        }
        public void ExportZu_ShenPiBiao(JTSYQ jtsyq, string saveDir)
        {
            //JTSYQCustom.SetContainsFeatureArea(jtsyq);
            XZDM xzdm = jtsyq.XZDM;
            Dictionary<string, XMLObject> xzdmDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            var doc = WordRead.Read(JTSYQCustom.ShenPiaoBiao);
            WordWrite.ReplaceText(doc, xzdmDic, xzdm);
            Dictionary<string, XMLObject> jtsyqDic = XMLRead.XmlToObjects(JTSYQCustom.JTSYQ_Reflect, true);
            WordWrite.ReplaceText(doc, jtsyqDic, jtsyq);
            WordWrite.SaveToFile(doc, saveDir + "/05审批表"  +xzdm.Zu + ".doc");


        }
        public void ExportZu_ZhiJieRenShenFengZhengMing(JTSYQ jtsyq, string saveDir)
        {
            //得到四至涉及单位
            /*string str = "510185047009JA00008".Replace("510185","");
            var v = str.Insert(3, "-");
            var v1 = v.Insert(7, "-");
            var v2 = v1.Insert(10, "-");*/
            List<XZDM> xzdms = new List<XZDM>();
            foreach(JTSYQ tem in jtsyq.GroupJTSYQ)
            {
                xzdms.AddRange(JTSYQCustom.GetSZInXZDM(tem));
            }
           
            //得到行政代码表
            foreach (XZDM xzdm in xzdms)
            {
                Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
                var doc = WordRead.Read(JTSYQCustom.ZhiJieRenZhengMing);
                WordWrite.ReplaceText(doc, ClazzDic, xzdm);
                //查看是村不是组
                if (Utils.IsStrNull(xzdm.CunZu))
                {
                    WordWrite.SaveToFile(doc, saveDir + "/04指界人身份证明及指界委托书" + xzdm.Cun + ".doc");
                }
                else
                {
                    WordWrite.SaveToFile(doc, saveDir + "/04指界人身份证明及指界委托书/"+xzdm.CunZu+".doc" );
                }
            }


        }
        public void ExportZu_FaRenDaiBiaoWeiTuoShu(XZDM xzdm, string saveDir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            var doc = WordRead.Read(JTSYQCustom.FaRenDaiBiaoWeiTuoShu);
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            //查看是村不是组
            WordWrite.SaveToFile(doc, saveDir + "/03法人代表授权委托书" + xzdm.Zu + ".doc");

        }
        public void ExportZu_FaRenDaiBiaoShenFengZhengMing(XZDM xzdm, string saveDir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            var doc = WordRead.Read(JTSYQCustom.FaRenDaiBiaoShenFengZhengMing);
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            WordWrite.SaveToFile(doc, saveDir + "/02法人代表身份证明"+xzdm.Zu+".doc" );

        }
        public void ExportZu_TuDiQuanShuLaiYuanZhengMing(JTSYQ jtsyq, string saveDir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.JTSYQ_Reflect, true);
            var doc = WordRead.Read(JTSYQCustom.Zu_TuDiQuanShuLaiYuanZhengMing);
            WordWrite.ReplaceText(doc, ClazzDic, jtsyq);
            WordWrite.SaveToFile(doc, saveDir + "/01土地权属来源证明" + jtsyq.XZDM.XiangZheng+jtsyq.XZDM.CunZu+"("+jtsyq.XZDM.DJZQDM+").doc");


        }
        public void ExportZu_DangAnDai(JTSYQ jtsyq, string saveDir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            var doc = WordRead.Read(JTSYQCustom.Zu_DangAnDai);
            XZDM xzdm = jtsyq.XZDM;
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            Dictionary<string, XMLObject> ClazzDic2 = XMLRead.XmlToObjects(JTSYQCustom.JTSYQ_Reflect, true);
            WordWrite.ReplaceText(doc, ClazzDic2, jtsyq);
            WordWrite.SaveToFile(doc, saveDir + "/00" +xzdm.XiangZheng+xzdm.CunZu+"("+xzdm.DJZQDM+")_1档案袋.doc");


        }

        public void ExportCun_GongGao(IList<JTSYQ> jtsyqs, XZDM xzdm, string dir)
        {
            Dictionary<int, XMLTable> ClazzDic = XMLRead.GetXmlToXMLTabl(JTSYQCustom.Cun_GongGao_Reflect);
            XMLTable xmlTable = ClazzDic[1];
            IWorkbook workbook = ExcelRead.ReadExcel(JTSYQCustom.Cun_GongGao);
            ISheet sheet = workbook.GetSheetAt(0);
            //文字替换
            Dictionary<string, XMLObject> xmlobjectDic = XMLRead.XmlToObjects(JTSYQCustom.FanKuiYiJianShuXMLRelfect, false);
            xzdm.JTSYQ_DasTatal = (xzdm.JTSYQ_GSEndTime - xzdm.JTSYQ_GSStartTime).Days;
            ExcelWrite.ReplaceTextByXMLObject(sheet, xzdm, xmlobjectDic);

            //行数据插入
            ExcelWrite.WriteObjects(sheet, xmlTable, jtsyqs);

            ExcelWrite.Save(workbook, dir + "//" + System.IO.Path.GetFileName(JTSYQCustom.Cun_GongGao));
        }
        public void ExportCun_GongShi(IList<JTSYQ> jtsyqs, XZDM xzdm, string dir)
        {
            Dictionary<int, XMLTable> ClazzDic = XMLRead.GetXmlToXMLTabl(JTSYQCustom.Cun_GongGao_Reflect);
            XMLTable xmlTable = ClazzDic[1];
            IWorkbook workbook = ExcelRead.ReadExcel(JTSYQCustom.Cun_GongShi);
            ISheet sheet = workbook.GetSheetAt(0);
            //文字替换
            Dictionary<string, XMLObject> xmlobjectDic = XMLRead.XmlToObjects(JTSYQCustom.FanKuiYiJianShuXMLRelfect, false);
            xzdm.JTSYQ_DasTatal = (xzdm.JTSYQ_GSEndTime - xzdm.JTSYQ_GSStartTime).Days;
            ExcelWrite.ReplaceTextByXMLObject(sheet, xzdm, xmlobjectDic);

            //行数据插入
            ExcelWrite.WriteObjects(sheet, xmlTable, jtsyqs);

            ExcelWrite.Save(workbook, dir + "//" + System.IO.Path.GetFileName(JTSYQCustom.Cun_GongShi));
        }
        public void ExportCunJieGuo_GongGao(XZDM xzdm, string dir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.FanKuiYiJianShuXMLRelfect_GongShi, false);
            var doc = WordRead.Read(JTSYQCustom.JieGuo_GongGao);
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            WordWrite.SaveToFile(doc, dir + "//" + System.IO.Path.GetFileName(JTSYQCustom.JieGuo_GongGao));
        }
        public void ExportCunJieGuo_GongShi(XZDM xzdm, string dir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            var doc = WordRead.Read(JTSYQCustom.JieGuo_GongShi);
            xzdm.SetZuMiaoShu();
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            WordWrite.SaveToFile(doc, dir + "//" + System.IO.Path.GetFileName(JTSYQCustom.JieGuo_GongShi));
        }
        public void ExportCunYiJianFanKuaiShu_GongShi(XZDM xzdm, string dir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.FanKuiYiJianShuXMLRelfect_GongShi, false);
            var doc = WordRead.Read(JTSYQCustom.FanKuiYiJianShu_GongShi);
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            WordWrite.SaveToFile(doc, dir + "//" + System.IO.Path.GetFileName(JTSYQCustom.FanKuiYiJianShu_GongShi));
        }

        public void ExportCunYiJianFanKuaiShu(XZDM xzdm, string dir)
        {
            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects(JTSYQCustom.FanKuiYiJianShuXMLRelfect, false);
            var doc = WordRead.Read(JTSYQCustom.FanKuiYiJianShu);
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            WordWrite.SaveToFile(doc, dir + "//" + System.IO.Path.GetFileName(JTSYQCustom.FanKuiYiJianShu));
        }
        public void ExportCunDangAnDai(XZDM xzdm, string saveDir)
        {

            Dictionary<string, XMLObject> ClazzDic = XMLRead.XmlToObjects_get<XZDM>();
            var doc = WordRead.Read(JTSYQCustom.CunDangAnDaiTemplete);
            WordWrite.ReplaceText(doc, ClazzDic, xzdm);
            WordWrite.SaveToFile(doc, saveDir + "//00档案袋.doc" );
        }
        public IList<JZD> ExtractJZD_Intersectant(JTSYQ jtsyq, IList<JZD> jzds)
        {
            IFeature feature = jtsyq.Feature;
            IList<JZD> jzds1 = ExtractJzd(jtsyq, ArcGisUtils.GetFeatureLayer(JTSYQCustom.JTSYQLayerName), ArcGisUtils.GetFeatureLayer(JZDCustom.JZDLayer));

            foreach (JZD jzd in jzds1)
            {
                jzd.ZDNUM = jtsyq.BM;
                jzd.JTSYQOBJECTID = jtsyq.OBJECTID;
                //jzd.Feature.Shape = jzd.Point;
                //JZDCustom.SaveMap(jzd);
            }
            JZDCustom.SaveMap(jzds1);
            return null;
        }

        public Dictionary<string, IList<JZD>> GetMapJZDS()
        {
            Dictionary<string, IList<JZD>> dic = new Dictionary<string, IList<JZD>>();
            IList<IFeature> features = ArcGisUtils.GetEntitysList("", JZDCustom.JZDLayer);
            if (Utils.CheckListExists(features))
            {
                IList<JZD> jzds = JZDCustom.FeatureToList(features);
                return Utils.GetGroupDicToList("ZDNUM", jzds);

            }
            else
            {
                return dic;
            }
        }


        private IList<JZD> ExtractJzd(JTSYQ dk, IFeatureLayer dkLayer, IFeatureLayer jzdLayer)
        {

            IFeature feature = dk.Feature;
            IPolygon polygon = feature.Shape as IPolygon;
            IPointCollection pointCollection = polygon as IPointCollection;
            IList<JZD> list = new List<JZD>();

            int num = pointCollection.PointCount - 2;
            IPoint point = new PointClass();
            point.X = 0.0;
            point.Y = 0.0;
            IList<JTSYQ> list2 = JTSYQCustom.FeaturesToJTSYQ(ArcGisUtils.CircleSelect(pointCollection.get_Point(num), dkLayer));
            IList<JTSYQ> list3 = JTSYQCustom.FeaturesToJTSYQ(ArcGisUtils.CircleSelect(pointCollection.get_Point(0), dkLayer));
            IList<JTSYQ> entity = JTSYQCustom.FeaturesToJTSYQ(ArcGisUtils.CircleSelect(pointCollection.get_Point(1), dkLayer));
            for (int i = 0; i <= num; i++)
            {
                IPoint point2 = pointCollection.get_Point(i);
                int num2 = list3.Count<JTSYQ>();
                bool flag2 = num2 > list2.Count || num2 > entity.Count;
                if (flag2)
                {
                    JZD JZD = new JZD();
                    JZD.JZDIndex = i;
                    JZD.Point = point2;
                    list.Add(JZD);
                    point = point2;
                }
                else
                {
                    bool flag3 = num2 != 1 && this.DifferentDks(list3, list2);
                    if (flag3)
                    {
                        JZD JZD = new JZD();
                        JZD.JZDIndex = i;
                        JZD.Point = point2;
                        list.Add(JZD);
                        point = point2;
                    }
                    else
                    {
                        double num3 = ArcGisUtils.CalculateTwoPt(point2, point);                      
                        bool flag4 = i == 0;
                        double num4 = -1;
                       
                        if (flag4)
                        {
                            num4 = ArcGisUtils.Angle(point2, pointCollection.get_Point(num), pointCollection.get_Point(1));
                        }
                        else
                        {

                            bool flag5 = i == num;
                            if (flag5)
                            {
                                
                                    num4 = ArcGisUtils.Angle(point2, pointCollection.get_Point(i - 1), pointCollection.get_Point(0));
                                    double num5 = ArcGisUtils.CalculateTwoPt(point2, pointCollection.get_Point(0));
                                    bool flag6 = num5 < num3;
                                    if (flag6)
                                    {
                                        num3 = num5;
                                    }
                                
                                
                            }
                            else
                            {
                                    num4 = ArcGisUtils.Angle(point2, pointCollection.get_Point(i - 1), pointCollection.get_Point(i + 1));
                            }
                        }
   
                        bool flag7 = num4 < 150;
                        if (flag7)
                        {
                            bool flag8 = num4 < 130.0 || num3 > 2.0;
                            if ( flag8)
                            {
                                //检查与上一个
                                int count = list.Count - 1;
                                if(count >= 0 )
                                {
                                    double di = ArcGisUtils.CalculateTwoPt(point2, list[count].Point);
                                    if(di > 1)

                                    {
                                        //有上一个点的话，要大于50距离才增加
                                        JZD JZD = new JZD();
                                        JZD.JZDIndex = i;
                                        JZD.Point = point2;
                                        list.Add(JZD);
                                        point = point2;
                                    }
                                }
                                else
                                {
                                    JZD JZD = new JZD();
                                    JZD.JZDIndex = i;
                                    JZD.Point = point2;
                                    list.Add(JZD);
                                    point = point2;
                                }
                                
                                
                            }
                            else
                            {
                                IList<IFeature> featureCursor = ArcGisUtils.CircleSelect(point2, jzdLayer);

                                if (Utils.CheckListExists(featureCursor))
                                {
                                    JZD JZD = new JZD();
                                    JZD.JZDIndex = i;
                                    JZD.Point = point2;
                                    list.Add(JZD);
                                    point = point2;
                                }
                            }
                        }
                    }
                }
                list2 = list3;
                list3 = entity;
                bool flag10 = i != num;
                if (flag10)
                {
                    entity = JTSYQCustom.FeaturesToJTSYQ(ArcGisUtils.CircleSelect(pointCollection.get_Point(i + 2), dkLayer));
                }
                else
                {
                    entity = JTSYQCustom.FeaturesToJTSYQ(ArcGisUtils.CircleSelect(pointCollection.get_Point(0), dkLayer));
                }
            }

            return list;
        }
        private bool DifferentDks(IList<JTSYQ> JTSYQs, IList<JTSYQ> beforeJTSYQs)
        {
            int i = 0;
        IL_03:
            bool result;
            while (i < JTSYQs.Count)
            {
                JTSYQ JTSYQ = JTSYQs[i];
                string zdnum = JTSYQ.BM;
                foreach (JTSYQ current in beforeJTSYQs)
                {
                    bool flag = zdnum.Equals(current.BM);
                    if (flag)
                    {
                        i++;
                        goto IL_03;
                    }
                }
                result = true;
                return result;
            }
            result = false;
            return result;
        }

        public int JZDBM(int startBH, JTSYQ jtsyq, IFeatureLayer featureLayer)
        {

            IList<JZD> jzds = JZDCustom.SearchOrCreateJZD(jtsyq, featureLayer);
            //设置界址点号
            //jzds = JZDCustom.GetMapJZD(" JTSYQOBJECTID = " + jtsyq.OBJECTID);
            JZDCustom.SetBM(startBH, jtsyq, jzds, featureLayer);
            jtsyq.JZDS = jzds;

            return jzds.Count;
        }

        public void ExportJZDTable(JTSYQ GroupJTSYQ, string dir)
        {
            JTSYQCustom.OrderByJZDH(GroupJTSYQ);

            List<JZD> jzds = new List<JZD>();
            double pingFangArea=0;
       
            foreach (JTSYQ jtsyq in GroupJTSYQ.GroupJTSYQ)
            {
                pingFangArea += Decimal.ToSingle(decimal.Round(new decimal(jtsyq.Shape_Area), 2));
                IList<JZD> temp = jtsyq.JZDS;
                if(temp == null)
                {
                    return;
                }
                jzds.AddRange(temp);
            }
            double area = GroupJTSYQ.Area;
            GroupJTSYQ.Area = Math.Round(pingFangArea, 2);
            ISheet sheet = JZDCustom.CreateJZDSheet(jzds);
            sheet.Workbook.SetSheetName(0, GroupJTSYQ.BM);
           
            Dictionary<string, XMLObject> xmlobjectDic = XMLRead.XmlToObjects(JTSYQCustom.JZDXMLRelfect, false);
            ExcelWrite.ReplaceTextByXMLObject(sheet, GroupJTSYQ, xmlobjectDic);

            Dictionary<string, XMLObject> xmlobjectDic2 = XMLRead.XmlToObjects(JTSYQCustom.XZDM_Reflect, true);
            ExcelWrite.ReplaceTextByXMLObject(sheet, GroupJTSYQ.XZDM, xmlobjectDic2);

            ExcelWrite.Save(sheet.Workbook, dir + "\\06界址点成果表"+ GroupJTSYQ.XZDM.Zu+ ".xls");
            GroupJTSYQ.Area = area;

        }

        public void DeleteJZD(IList<JZD> jzds)
        {
           if(jzds !=null)
            {
                foreach(JZD jzd in jzds)
                {
                    IFeature feature = jzd.Feature;
                    if(feature != null)
                    {
                        feature.Delete();
                    }
                }
            }
        }

       
    }
}
