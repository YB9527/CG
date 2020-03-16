using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;
using ReflectManager.XMLPackage;
using WordManager;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.ZaiJiDiModel;
using Utils = MyUtils.Utils;
namespace ZaiJiDi.Dao
{
    public class ZJDDao : IZJDDao
    {

        public IList<string> CheckZJD(JSYD jsyd, ZJDDataSourceViewModel model)
        {
            IList<string> erros = new List<string>();
            ZJDXZDM xzdm = jsyd.ZJDXZDM;
            Zdinfo zdinfo = jsyd.Zdinfo;
            string error;
            if (zdinfo == null)
            {
                CheckZJDWriteError(jsyd.Row, model.JSYDTableErrorCellIndex, "未找到宗地表");
                erros.Add("未找到宗地表");
            }
            else
            {
                error = CheckJSYD_Zdinfo(jsyd, zdinfo, model.JSYDTableErrorCellIndex);
                if (error != "")
                {
                    erros.Add(error);
                }

            }
            IList<NF> nfs = jsyd.NFs;
            if (nfs == null)
            {
                CheckZJDWriteError(jsyd.Row, model.JSYDTableErrorCellIndex, "未找到农房表");
                erros.Add("未找到农房表");
            }
            else
            {
                error = CheckNF(nfs, model.NFTableErrorCellIndex);
                if (error != "")
                {
                    erros.Add(error);
                }


                IList<Floor> floors = jsyd.Floors;
                if (floors == null)
                {
                    CheckZJDWriteError(jsyd.Row, model.JSYDTableErrorCellIndex, "未找到分层表");
                    erros.Add("未找到分层表");
                }
                else
                {
                    if (Utils.IsStrNull(jsyd.YTDSYZSH) && nfs[0].YJTTDSYZ != "未确权颁证")
                    {
                        erros.Add("无土地证 农房表 填写的不是 未确权颁证");
                    }
                    error = CheckFloor(floors, model.FloorTableErrorCellIndex);
                    if (error != "")
                    {
                        erros.Add(error);
                    }
                    error = CheckNF_Floor(nfs, floors, model.FloorTableErrorCellIndex);
                    if (error != "")
                    {
                        erros.Add(error);
                    }

                }
            }
            IList<JTCY> hzs = jsyd.HZs;
            CheckHZs(hzs, model.JTCYTableErrorCellIndex);
            IList<QZB> qzbs = jsyd.QZBs;
            IList<JZXInfo> jzxInfos = jsyd.JZXInfos;



            return erros;

        }
        /// <summary>
        /// 检查农房表、分层表
        /// </summary>
        /// <param name="nfs"></param>
        /// <param name="floors"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        private string CheckNF_Floor(IList<NF> nfs, IList<Floor> floors, int floorCellIndex)
        {
            NF nf = nfs[0];
            Floor floor = floors[0];
            StringBuilder sb = new StringBuilder();
            //检查总层数
            if (nf.ZCS != floor.ZCS)
            {
                CheckZJDWriteError(floor.Row, floorCellIndex, "农房表总层是：" + nf.ZCS);
                sb.Append("分层表总层数：" + floor.ZCS + "、农房表总层数：" + nf.ZCS);
            }
            //检查总面积
            double floorJZMJ = FloorCustom.GetJZMJTotal(floors);
            if (nf.JZMJ != floorJZMJ)
            {
                CheckZJDWriteError(floor.Row, floorCellIndex, " 与农房表建筑总面积" + nf.JZMJ + "不一致：");
                sb.Append("农房表总面积：" + nf.JZMJ + "、分层表建筑总面积：" + floorJZMJ);
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "农房表与分层表检查：");

                return sb.ToString();
            }
            return "";

        }

        /// <summary>
        /// 检查户主信息
        /// </summary>
        /// <param name="hzs"></param>
        private void CheckHZs(IList<JTCY> hzs, int cellIndex)
        {

            foreach (JTCY hz in hzs)
            {
                CheckHZ(hz, cellIndex);
            }
        }
        /// <summary>
        /// 检查单个户主
        /// </summary> 
        /// <param name="hz"></param>
        private void CheckHZ(JTCY hz, int cellIndex)
        {
            Dictionary<string, string> yhzgxDic = JTCYCustom.YHZGXDic;

            IList<JTCY> jtcys = hz.JTCies;
            foreach (JTCY jtcy in jtcys)
            {
                if (!Utils.CheckIDCard18(jtcy.GMSFHM))
                {
                    CheckZJDWriteError(jtcy.Row, cellIndex, "证件号码不符合规则");
                }
                //检查关系 是否符合规则
                string yhzgx = jtcy.YHZGX;
                if (yhzgx != null &&!yhzgxDic.ContainsKey(yhzgx))
                {
                    CheckZJDWriteError(jtcy.Row, cellIndex, yhzgx + ":关系不是标准");
                }
            }

        }

        private void CheckZJDWriteError(IRow row, int cellIndex, string error)
        {
            ICell cell = row.GetCell(cellIndex);
            error = error.Replace("：、", ":");
            if (error.StartsWith("、"))
            {
                error = error.Remove(0, 1);
            }
            if (cell == null)
            {
                cell = row.CreateCell(cellIndex);
                cell.SetCellType(CellType.String);
                string value = cell.StringCellValue;
                if (value == null || !value.Contains(error))
                {
                    cell.SetCellValue(error);
                }
            }
            else
            {
                cell.SetCellValue(cell.StringCellValue + "、" + error);
            }


        }

        /// <summary>
        /// 检查分层表
        /// </summary>
        /// <param name="floors"></param>
        /// <returns></returns>
        private string CheckFloor(IList<Floor> floors, int cellIndex)
        {
            StringBuilder sb = new StringBuilder();
            int zcs = floors[0].ZCS;
            foreach (Floor floor in floors)
            {
                if (zcs < floor.SZC)
                {
                    sb.Append("、所在层 大于 总层 的数字");
                    CheckZJDWriteError(floor.Row, cellIndex, "、所在层 大于 总层 的数字");
                }
                if (floors[0].ZCS != floor.ZCS)
                {
                    sb.Append("、总层数不一致");
                    CheckZJDWriteError(floor.Row, cellIndex, "、总层数不一致");
                }
            }


            if (sb.Length > 0)
            {
                sb.Insert(0, "分层表自查：");

                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// 检查农房表
        /// </summary>
        /// <param name="jsyd"></param>
        /// <param name="nfs"></param>
        private string CheckNF(IList<NF> nfs, int cellIndex)
        {
            StringBuilder sb = new StringBuilder();
            NF nf = nfs[0];
            double tem = 0;
            //检查层数 
            int csInt;
            foreach (NF nf1 in nfs)
            {
                tem += nf1.CJZMJ;
                if(nf1.SZC == null)
                {
                    continue;
                }
                string[] csArray = nf1.SZC.Split('、');
                foreach (string csStr in csArray)
                {
                    if (int.TryParse(csStr, out csInt))
                    {
                        if (nf.ZCS < csInt)
                        {
                            sb.Append("、所在层中有 大于 总层 的数字");
                            CheckZJDWriteError(nf1.Row, cellIndex, "、所在层中有 大于 总层 的数字");
                        }

                    }
                    else
                    {
                        sb.Append("、所在层中填写有不是数字");
                        CheckZJDWriteError(nf1.Row, cellIndex, "、所在层中填写有不是数字");
                    }
                }
                if (nf1.CZTJG + "结构" != nf1.JG)
                {
                    sb.Append("、层主体结构 与 结构一致");
                    CheckZJDWriteError(nf1.Row, cellIndex, "、层主体结构一致");
                }
            }
            if (Math.Abs(nf.JZMJ - tem) > 0.001)
            {
                sb.Append("、层建筑面积 与 建筑面积不相等");
                CheckZJDWriteError(nf.Row, cellIndex, "、层建筑面积 与 建筑面积不相等字");
            }
            if (nf.JZMJ < nf.FZMJ)
            {
                sb.Append("、层建筑面积 小于 建筑面积");
                CheckZJDWriteError(nf.Row, cellIndex, "、层建筑面积 小于 建筑面积");
            }
            for (int a = 1; a < nfs.Count; a++)
            {
                NF temNF = nfs[a];
                if (nf.SYQZH != temNF.SYQZH)
                {
                    sb.Append("、所有权号不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、所有权号不一致");
                }
                if (nf.QLRMC != temNF.QLRMC)
                {
                    sb.Append("、权利人名称不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、权利人名称不一致");
                }
                if (nf.ZJHM != temNF.ZJHM)
                {
                    sb.Append("、证件号码不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、证件号码不一致");
                }
                if (nf.ZCS != temNF.ZCS)
                {
                    sb.Append("、总层数不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、总层数不一致");
                }

                if (nf.JZMJ != temNF.JZMJ)
                {
                    sb.Append("、建筑面积不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、建筑面积不一致");
                }
                if (nf.ZCS != temNF.CS)
                {
                    sb.Append("、总层数 与 层数不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、总层数 与 层数不一致");
                }
                if (nf.FWLY != temNF.FWLY)
                {
                    sb.Append("、房屋来源 不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、房屋来源 不一致");
                }
                if (nf.YJTTDSYZ != temNF.YJTTDSYZ)
                {
                    sb.Append("、土地权属来源（是否有原件）不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、房屋来源 不一致");
                }
                if (nf.YFWSYQZ != temNF.YFWSYQZ)
                {
                    sb.Append("、房屋权属来源（是否有原件）不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、房屋权属来源（是否有原件）不一致");
                }
                if (nf.JGRQ != temNF.JGRQ)
                {
                    sb.Append("、竣工日期不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、竣工日期不一致");
                }
                if (nf.ZL != temNF.ZL)
                {
                    sb.Append("、坐落不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、坐落不一致");
                }
                if (nf.ZL != temNF.ZL)
                {
                    sb.Append("、产权来源不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、产权来源不一致");
                }
                if (nf.ZL != temNF.ZL)
                {
                    sb.Append("、产权来源不一致");
                    CheckZJDWriteError(temNF.Row, cellIndex, "、产权来源不一致");
                }
            }
            //(nf.YJTTDSYZ == "已提交原件" || nf.YFWSYQZ == "已提交原件") || (
            if (nf.YJTTDSYZ == "未确权颁证" && nf.YFWSYQZ == "未确权颁证")
            {
                if (nf.FWLY != "其他情形")
                {
                    sb.Append("、房屋来源不是其他情形");
                    CheckZJDWriteError(nf.Row, cellIndex, "、产权来源不一致");
                }
                if (nf.CQLY != "自建")
                {
                    sb.Append("、产权来源不是自建");
                    CheckZJDWriteError(nf.Row, cellIndex, "、产权来源不是自建");
                }

            }
            else
            {
                if (!(nf.FWLY == "改建扩建" || nf.FWLY == "其他情形"))
                {
                    sb.Append("、房屋来源不是改建扩建");
                    CheckZJDWriteError(nf.Row, cellIndex, "、房屋来源不是改建扩建、其他情形");
                }
                if (!(nf.CQLY == "翻建" || nf.CQLY == "自建"))
                {
                    sb.Append("、产权来源不是翻建");
                    CheckZJDWriteError(nf.Row, cellIndex, "、产权来源不是翻建、自建");
                }
            }
            if (Utils.IsStrNull(nf.SYQZH))
            {
                if (nf.YFWSYQZ != "未确权颁证")
                {
                    sb.Append("、无证号，但填写的是 确权颁证");
                    CheckZJDWriteError(nf.Row, cellIndex, "、无证号，但填写的是 确权颁证");
                }

            }
            else
            {
                if (nf.YFWSYQZ == "未确权颁证")
                {
                    sb.Append("、有证号，但填写的是 未确权颁证");
                    CheckZJDWriteError(nf.Row, cellIndex, "、有证号，但填写的是 未确权颁证");
                }

            }

            if (sb.Length > 0)
            {
                sb.Insert(0, "农房表自查：");

                return sb.ToString();
            }
            return "";
        }

        /// <summary>
        /// 建设用地和宗地图对比
        /// </summary>
        /// <param name="jsyd"></param>
        /// <param name="zdinfo"></param>
        /// <returns></returns>
        private string CheckJSYD_Zdinfo(JSYD jsyd, Zdinfo zdinfo, int cellIndex)
        {
            StringBuilder sb = new StringBuilder();

            if (!Utils.StrEquals(jsyd.TFH, zdinfo.TUFU))
            {
                //sb.Append("、图幅号是：" + zdinfo.TUFU);
            }
            if (jsyd.ZDMJ != zdinfo.AREA)
            {
                sb.Append("、宗地面积是：" + zdinfo.AREA);
            }
            if (jsyd.ZDMJ < jsyd.SYQMJ)
            {
                sb.Append("、宗地面积小于使用权面积");
            }
            double dt = jsyd.ZDMJ - jsyd.SYQMJ - jsyd.CZMJ;
            if (Math.Abs(dt) > 0.001)
            {
                sb.Append("、宗地面积 减 使用权 减 超占 面积 = " + dt);
            }
            if (jsyd.JZZDZMJ != zdinfo.ZDMJ)
            {
                sb.Append("、占地面积是：" + zdinfo.ZDMJ);
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "宗地表和地籍表MDB对比：");
                CheckZJDWriteError(jsyd.Row, cellIndex, sb.ToString());
                return sb.ToString();
            }

            return "";
        }


        /// <summary>
        /// 检查重复的身份证
        /// </summary>
        /// <param name="hzs"></param>
        /// <returns></returns>
        public IList<string> ChecSFZ(IList<JTCY> hzs, ZJDDataSourceViewModel model)
        {
            IList<string> errors = new List<string>();
            IList<JTCY> jtcys = JTCYCustom.GetAllJTCY(hzs);
            Dictionary<string, IList<JTCY>> sfzDic = Utils.GetGroupDicToList("GMSFHM", jtcys);
            foreach (string sfz in sfzDic.Keys)
            {

                IList<JTCY> sfzList = sfzDic[sfz];
                if (sfzList.Count > 1)
                {
                    foreach (JTCY jtcy in sfzList)
                    {
                        if (sfz == "")
                        {
                            errors.Add(sfz + ",证件号码为空");
                        }
                        else
                        {
                            CheckZJDWriteError(jtcy.Row, model.JTCYTableErrorCellIndex, "证件号码重复");
                        }
                    }
                }
                else
                {
                    if (!Utils.CheckIDCard18(sfz))
                    {
                        if (sfz == "")
                        {
                            CheckZJDWriteError(sfzList[0].Row, model.JTCYTableErrorCellIndex, "证件号码为空");
                        }
                        else
                        {
                            CheckZJDWriteError(sfzList[0].Row, model.JTCYTableErrorCellIndex, "证件号码不符合规则");
                        }

                    }

                }
            }
            return errors;
        }
        /// <summary>
        /// 得到宅基地文件保存的名称
        /// </summary>
        /// <param name="jsyd"></param>
        /// <param name="saveDir"></param>
        /// <param name="docTempletePath"></param>
        /// <returns></returns>
        public static string GetZJDSaveFileName(JSYD jsyd, string saveDir, string docTempletePath)
        {
            IList<JTCY> hzs = jsyd.HZs;
            string hzmcTotal = hzs[0].XM;
            for (int a = 1; a < hzs.Count; a++)
            {
                hzmcTotal = hzmcTotal + "_" + hzs[a].XM;
            }
            return saveDir + "\\" + jsyd.ZDNUM + "(" + hzmcTotal + ")\\" + Path.GetFileName(docTempletePath);
        }
        public void ExportZJD_DangAnDai(JSYD jsyd, string saveDir)
        {

            
            string docTempletePath = JSYDCustom.DanAnDaiDocPath;
    
            XWPFDocument doc = WordRead.Read(docTempletePath);
            Dictionary<string, XMLObject> jsydXML = XMLRead.XmlToObjects(JSYDCustom.JSYDXMLPath);
            WordWrite.ReplaceText(doc, jsydXML, jsyd);

            Dictionary<string, XMLObject> xzdmXML = XMLRead.XmlToObjects(JSYDCustom.ZJDXZDMMLPath);
            WordWrite.ReplaceText(doc, xzdmXML, jsyd.ZJDXZDM);

            Dictionary<string, XMLObject> jtcyXML = XMLRead.XmlToObjects(JSYDCustom.JTCYXMLPath);
            WordWrite.ReplaceText(doc, jtcyXML, jsyd.HZs[0]);


        


            Dictionary<string, IList<RunCustomer>> docDic = WordRead.GetDocxDic(doc);
            IList<RunCustomer> runCustomers;
            NF nf = jsyd.NFs[0];
            //土地证位置是第一个
            if (nf.YJTTDSYZ.Equals("遗失/灭失"))
            {
                if (docDic.TryGetValue("遗失声明", out runCustomers))
                {
                    WordWrite.ReplaceSmpbol(runCustomers[0]);
                }
                if (docDic.TryGetValue("土地登记档案", out runCustomers))
                {
                    WordWrite.ReplaceSmpbol(runCustomers[0]);
                }
            }
            else
            {
                if (docDic.TryGetValue(nf.YJTTDSYZ, out runCustomers))
                {
                    WordWrite.ReplaceSmpbol(runCustomers[0]);
                }
            }
            //房屋来源
            if (nf.YFWSYQZ.Equals("遗失/灭失"))
            {
                if (docDic.TryGetValue("遗失声明", out runCustomers))
                {
                    WordWrite.ReplaceSmpbol(runCustomers[1]);
                }
                if (docDic.TryGetValue("房屋登记档案", out runCustomers))
                {
                    WordWrite.ReplaceSmpbol(runCustomers[0]);
                }

            }
            else
            {
                if (docDic.TryGetValue(nf.YFWSYQZ, out runCustomers))
                {
                    WordWrite.ReplaceSmpbol(runCustomers[1]);
                }
            }
            //房屋来源
            if (docDic.TryGetValue(nf.FWLY, out runCustomers))
            {
                WordWrite.ReplaceSmpbol(runCustomers[0]);
            }


            string saveName = GetZJDSaveFileName(jsyd, saveDir, docTempletePath);
            WordWrite.SaveToFile(doc, saveName);
        }

        public void ExportZJD_SPB(JSYD jsyd, string saveDir)
        {
            string docTempletePath = JSYDCustom.SPBDocPath;
            XWPFDocument doc = WordRead.Read(docTempletePath);

            Dictionary<string, XMLObject> jsydXML = XMLRead.XmlToObjects(JSYDCustom.JSYDXMLPath);
            WordWrite.ReplaceText(doc, jsydXML, jsyd);

            Dictionary<string, XMLObject> xzdmXML = XMLRead.XmlToObjects(JSYDCustom.ZJDXZDMMLPath);
            WordWrite.ReplaceText(doc, xzdmXML, jsyd.ZJDXZDM);

            Dictionary<string, XMLObject> jtcyXML = XMLRead.XmlToObjects(JSYDCustom.JTCYXMLPath);
            WordWrite.ReplaceText(doc, jtcyXML, jsyd.HZs[0]);

            Dictionary<string, XMLObject> nfXML = XMLRead.XmlToObjects(JSYDCustom.NFXMLPath);
            WordWrite.ReplaceText(doc, nfXML, jsyd.NFs[0]);


            Dictionary<string, string> replaceDic = new Dictionary<string, string>();
            replaceDic.Add("JTCYCount", jsyd.HZs[0].JTCies.Count + "");
            replaceDic.Add("JTCYs", JTCYCustom.ToStringJTCYs(jsyd.HZs[0]));
            WordWrite.ReplaceText(doc, replaceDic);

            //替换符号
            Dictionary<String, IList<RunCustomer>> docDic = WordRead.GetDocxDic(doc);
            IList<RunCustomer> customers;
            //房屋来源
            if (docDic.TryGetValue(jsyd.NFs[0].FWLY, out customers))
            {
                WordWrite.ReplaceSmpbol(customers[0]);
            }

            //土地证位置是第一个

            if (docDic.TryGetValue(jsyd.NFs[0].YJTTDSYZ, out customers))
            {
                WordWrite.ReplaceSmpbol(customers[0]);
            }
            //房产证位置是第二个
            if (docDic.TryGetValue(jsyd.NFs[0].YFWSYQZ, out customers))
            {
                WordWrite.ReplaceSmpbol(customers[1]);
            }
            if (jsyd.GYFS == "共同共有")
            {
                if (docDic.TryGetValue("是", out customers))
                {

                    WordWrite.ReplaceSmpbol(customers[1]);
                }
                if (docDic.TryGetValue("共同共有", out customers))
                {

                    WordWrite.ReplaceSmpbol(customers[0]);
                }
            }
            else
            {
                if (docDic.TryGetValue("否", out customers))
                {

                    WordWrite.ReplaceSmpbol(customers[1]);
                }

            }
           



            string saveName = GetZJDSaveFileName(jsyd, saveDir, docTempletePath);

            WordWrite.SaveToFile(doc, saveName);

        }


        /// <summary>
        /// 户主必须是第一个人
        /// </summary>
        /// <param name="jsyd"></param>
        /// <param name="saveDir"></param>
        public void ExportZJD_WTS(JSYD jsyd, string saveDir)
        {
            string docTempletePath = JSYDCustom.WTSDocPath;
            XWPFDocument doc = WordRead.Read(docTempletePath);

            //Dictionary<string, XMLObject> jtcyXML = XMLRead.XmlToObjects(JSYDCustom.JTCYXMLPath);
            //WordWrite.ReplaceText(doc, jtcyXML, jsyd.HZs[0]);

            IList<JTCY> oneJtcys = jsyd.HZs[0].JTCies;
            IList<XWPFParagraph> ps = doc.Paragraphs;
            StringBuilder sb = new StringBuilder();
            WordWrite.ReplaceText(ps[3].Runs[0], "孙", "");
            //第二段加入户主
            WordWrite.SetRun(ps[4].Runs[0], jsyd.HZs[0].XM + "(" + jsyd.HZs[0].GMSFHM + ")", ps[2].Runs[0]);

            for (int a = 0; a < oneJtcys.Count; a++)
            {
                JTCY jtcy = oneJtcys[a];
                sb.Append(jtcy.XM + "(" + jtcy.GMSFHM + ")");
                if (a < oneJtcys.Count - 1)
                {
                    sb.Append("、");
                }
                if (a % 2 != 0 && a != 0)
                {
                    if (a < oneJtcys.Count - 1)
                    {
                        sb.Append("\n\t");
                    }
                   
                    if (a != 1)
                    {
                        sb.Insert(0, " ");
                    }
                    //两个人就为一行数据
                    WordWrite.SetRun(ps[3].Runs[0], sb.ToString(), ps[2].Runs[0]);
                    sb.Remove(0, sb.Length);
                }

            }
            if (sb.Length > 0)
            {
                sb.Append("\n\t");
                if (oneJtcys.Count > 2)
                {
                    sb.Insert(0, " ");
                }
                //第三段加入家庭成员
                WordWrite.SetRun(ps[3].Runs[0], sb.ToString(), ps[2].Runs[0]);
                sb.Remove(0, sb.Length);
            }



            string saveName = GetZJDSaveFileName(jsyd, saveDir, docTempletePath);
            WordWrite.SaveToFile(doc, saveName);
        }

        public void ExportZJD_SMS(JSYD jsyd, string saveDir)
        {
            string docTempletePath = JSYDCustom.SMSDocPath;
            XWPFDocument doc = WordRead.Read(docTempletePath);
            Dictionary<string, XMLObject> jsydXML = XMLRead.XmlToObjects(JSYDCustom.JSYDXMLPath);
            WordWrite.ReplaceText(doc, jsydXML, jsyd);

            Dictionary<string, XMLObject> xzdmXML = XMLRead.XmlToObjects(JSYDCustom.ZJDXZDMMLPath);
            WordWrite.ReplaceText(doc, xzdmXML, jsyd.ZJDXZDM);

            Dictionary<string, XMLObject> jtcyXML = XMLRead.XmlToObjects(JSYDCustom.JTCYXMLPath);
            WordWrite.ReplaceText(doc, jtcyXML, jsyd.HZs[0]);

            IList<JTCY> jTCies = jsyd.HZs[0].JTCies;
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a <jTCies.Count; a++)
            {
                JTCY jtcy = jTCies[a];
                if (a %2 == 0)
                {
                    sb.Append(jtcy.XM + "(" + jtcy.GMSFHM + ")、");
                }
                else
                {
                    sb.Append(jtcy.XM + "(" + jtcy.GMSFHM + ")、\r\n");
                }

            }
            sb.Remove(sb.Length - 1, 1);
            Dictionary<string, string> replaceDic = new Dictionary<string, string>();
            replaceDic.Add("JTCYCount", jsyd.HZs[0].JTCies.Count + "");
            replaceDic.Add("Jtcy", sb.ToString());
            WordWrite.ReplaceText(doc, replaceDic);

            string saveName = GetZJDSaveFileName(jsyd, saveDir, docTempletePath);
            WordWrite.SaveToFile(doc, saveName);
        }

        public void ExportZJD_QJDCB(JSYD jsyd, string saveDir)
        {
            string docTempletePath = JSYDCustom.QJDCBDocPath;
            XWPFDocument doc = WordRead.Read(docTempletePath);

            //写入界址签章表
            IList<QZB> qzbs = jsyd.QZBs;
            WordWrite.SetTableRowObj(doc, qzbs, JZXInfoCustom.qzbReflect);

            //写入标示表
            IList<JZXInfo> jzxinfos = jsyd.JZXInfos;
            JZXInfoCustom.CreateBSB(doc, jzxinfos);

          

            Dictionary<string, XMLObject> jsydXML = XMLRead.XmlToObjects(JSYDCustom.JSYDXMLPath);
            WordWrite.ReplaceText(doc, jsydXML, jsyd);

            Dictionary<string, XMLObject> xzdmXML = XMLRead.XmlToObjects(JSYDCustom.ZJDXZDMMLPath);
            WordWrite.ReplaceText(doc, xzdmXML, jsyd.ZJDXZDM);

            Dictionary<string, XMLObject> jtcyXML = XMLRead.XmlToObjects(JSYDCustom.JTCYXMLPath);
            WordWrite.ReplaceText(doc, jtcyXML, jsyd.HZs[0]);

            Dictionary<string, XMLObject> nfXML = XMLRead.XmlToObjects(JSYDCustom.NFXMLPath);
            WordWrite.ReplaceText(doc, nfXML, jsyd.NFs[0]);

            Dictionary<string, string> replaceDic = new Dictionary<string, string>();
            replaceDic.Add("Company", "四川旭普信息产业发展有限公司");
            string zdnum = jsyd.ZDNUM;
            replaceDic.Add("Qjbh", zdnum.Substring(6, 3) + "-" + zdnum.Substring(9, 3) + "-" + zdnum.Substring(12, 2) + "-" + zdnum.Substring(14, 5));



            //本宗地已取得原土地使用证: 简镇清集用（1990）字第0304 - 37号; 原房屋所有权证: 简农房权字第1002010383号;
            StringBuilder sb = new StringBuilder();
            string str = jsyd.YTDSYZSH;
            if (!Utils.IsStrNull(str))
            {
                sb.Append("原土地使用证:" + str + "；");
            }
            str = jsyd.NFs[0].SYQZH;
            if (!Utils.IsStrNull(str))
            {
                sb.Append("原房屋所有权证:" + str + "；");
            }
            if (sb.Length > 0)
            {
                sb.Insert(0, "本宗地已取得");

            }
            replaceDic.Add("Tag0", sb.ToString());

            WordWrite.ReplaceText(doc, replaceDic);

            string saveName = GetZJDSaveFileName(jsyd, saveDir, docTempletePath);
            WordWrite.SaveToFile(doc, saveName);
        }

        public void ExportZJD_CHBG(JSYD jsyd, string saveDir)
        {
            string docTempletePath = JSYDCustom.CHBGDocPath;
            XWPFDocument doc = WordRead.Read(docTempletePath);

            Dictionary<string, XMLObject> xzdmXML = XMLRead.XmlToObjects(JSYDCustom.ZJDXZDMMLPath);
            WordWrite.ReplaceText(doc, xzdmXML, jsyd.ZJDXZDM);

            Dictionary<string, XMLObject> jsydXML = XMLRead.XmlToObjects(JSYDCustom.JSYDXMLPath);
            WordWrite.ReplaceText(doc, jsydXML, jsyd);

            Dictionary<string, XMLObject> jtcyXML = XMLRead.XmlToObjects(JSYDCustom.JTCYXMLPath);
            WordWrite.ReplaceText(doc, jtcyXML, jsyd.HZs[0]);

            Dictionary<string, XMLObject> nfXML = XMLRead.XmlToObjects(JSYDCustom.NFXMLPath);
            WordWrite.ReplaceText(doc, nfXML, jsyd.NFs[0]);

            Dictionary<string, string> replaceDic = new Dictionary<string, string>();
            replaceDic.Add("Company", "四川旭普信息产业发展有限公司");
            string zdnum = jsyd.ZDNUM;
            replaceDic.Add("Qjbh", zdnum.Substring(6, 3) + "-" + zdnum.Substring(9, 3) + "-" + zdnum.Substring(12, 2) + "-" + zdnum.Substring(14, 5));
            foreach (NF nf in jsyd.NFs)
            {
                if (replaceDic.ContainsKey("JZJG"))
                {

                    replaceDic["JZJG"] = replaceDic["JZJG"] + "、" + nf.JG;
                }
                else
                {

                    replaceDic.Add("JZJG", nf.JG);
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (Floor f in jsyd.Floors)
            {
                sb.Append("S" + f.SZC + " = " + f.CJZMJ + "  平方米    ");
            }
            replaceDic.Add("FCMJ", sb.ToString());

            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(JSYDCustom.FloorXMLPath)[0];
            XWPFTable table = WordWrite.ReplaceTable_Font<Floor>(doc, xmlTable, jsyd.Floors);
            int rowX = xmlTable.RowStartIndex + jsyd.Floors.Count;
            while (table.Rows.Count - rowX > 1)
            {
                table.RemoveRow(rowX);
            }


            WordWrite.ReplaceText(doc, replaceDic);
            string saveName = GetZJDSaveFileName(jsyd, saveDir, docTempletePath);
            WordWrite.SaveToFile(doc, saveName);
        }

    }
}
