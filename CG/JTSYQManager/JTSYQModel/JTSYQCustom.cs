using ArcGisManager;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using HeibernateManager.Model;
using JTSYQManager.XZDMManager;
using MyUtils;
using ProgressTask;
using Public;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace JTSYQManager.JTSYQModel
{
    public class JTSYQCustom : JTSYQ
    {
        public JTSYQCustom(JTSYQ JTSYQ)
        {
            this.JTSYQ = JTSYQ;
        }
        public JTSYQ JTSYQ { get; set; }
        public static string XZDM_Reflect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\行政代码表替换.xml";
        public static string JTSYQLayerName = "B标段集体土地";
        public static string DLTBLayerName = "高新东区地类图斑";
        public static string DLTB_Reflect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQModel\\DLTB_Gis.xml";
        public static string arcgisConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQModel\\JTSYQ_Gis.xml";
        public static string JZDExcelTemplete = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\界址点成果表模板.xls";
        public static string JZDXMLRelfect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\导出界址点成果表.xml";
        public static string CunDangAnDaiTemplete = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\00公示公告档案袋.docx";
        public static string FanKuiYiJianShu = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地所有权公告反馈意见书.docx";

       

        public static string FanKuiYiJianShuXMLRelfect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地所有权公告反馈意见书.xml";
        public static string FanKuiYiJianShu_GongShi = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地所有权公示反馈意见书.docx";
        public static string FanKuiYiJianShuXMLRelfect_GongShi = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地所有权公示反馈意见书.xml";
        public static string JieGuo_GongGao = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地所有权确权结果公告.docx";
        public static string JieGuo_GongShi = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地所有权调查结果公示.docx";
        public static string Cun_GongGao = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\村公告表.xls";
        public static string Cun_GongGao_Reflect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\村公告表.Excel.xml";
        public static string Cun_GongShi = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\村公示表.xls";
        public static string Zu_DangAnDai = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\组档案袋.docx";
        public static string JTSYQ_Reflect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体所有权表替换.xml";
        public static string Zu_TuDiQuanShuLaiYuanZhengMing = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\土地权属来源证明.docx";
        public static string FaRenDaiBiaoShenFengZhengMing = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\法人代表身份证明.docx";
        public static string FaRenDaiBiaoWeiTuoShu = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\法人代表授权委托书.docx";
        public static string ZhiJieRenZhengMing = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\指界人身份证明.docx";
        public static string ShenPiaoBiao = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\审批表.docx";
        public static string JiTiTuDiDiaoChaBiao = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地调查表.xls";
        public static string JTSYQZDTMxd = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\所有权宗地图.mxd";
        public static string JTSYQZDTMdb = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\所有权宗地图数据库.mdb";
        public static string JiTiTuDiDiaoChaBiao_Reflect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地调查表.Excel.xml";
        public static string QuanJiDiaoChaBiao = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\权籍调查表.docx";
        public static string DangZhongDi = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\单宗地表.xls";
        public static string DangZhongDi_Reflect = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\单宗地表.Excel.xml";

        public static string JTSYQGSTMxd = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地公示图.mxd";
        public static string JTSYQGSTMDB = System.AppDomain.CurrentDomain.BaseDirectory + "JTSYQExcelTemplete\\集体土地公示图数据库.mdb";

        private static IFeatureLayer JTSYQFeatureLayer { get; set; }
        public static IFeatureLayer GetLayer()
        {
            return ArcGisUtils.GetFeatureLayer2(JTSYQCustom.JTSYQLayerName);

        }
        /// <summary>
        /// 使用要素类描述对象创建最简单的要素类，只包含2个必要字段“SHAPE”和“OBJECTID”，无空间参考
        /// </summary>
        /// <param name="featureWorkspace">目标工作空间</param>
        public static IFeatureClass CreatSimpleFeatureClass(IFeatureWorkspace featureWorkspace, string strShapeFile)
        {


            MapDocumentClass mapDocument = new MapDocumentClass();
            mapDocument.Open(ArcGisUtils.MXDPath, "");
            AxMapControl mapControl = ArcGisUtils.axMapControl;
            IMap map = mapDocument.Map[0];
            mapControl.Map = map;
            IFeatureClass featureClass = null;
            try
            {
                featureClass = featureWorkspace.OpenFeatureClass(strShapeFile);
                if (featureClass != null)
                {

                    IFeatureCursor featureCursor = featureClass.Search(new QueryFilterClass(), false);
                    IFeature feature = featureCursor.NextFeature();
                    while (feature != null)
                    {
                        feature.Delete();
                        feature = featureCursor.NextFeature();
                    }
                }
            }
            catch
            {
                IFields pFields = new FieldsClass();
                IFieldsEdit pFieldsEdit = (IFieldsEdit)pFields;

                IField pField = new FieldClass();
                IFieldEdit pFieldEdit = (IFieldEdit)pField;

                pFieldEdit.Name_2 = "SHAPE";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

                IGeometryDefEdit pGeoDef = new GeometryDefClass();
                IGeometryDefEdit pGeoDefEdit = (IGeometryDefEdit)pGeoDef;
                pGeoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                pGeoDefEdit.SpatialReference_2 = map.SpatialReference; //new UnknownCoordinateSystemClass();
                pFieldEdit.GeometryDef_2 = pGeoDef;
                pFieldsEdit.AddField(pField);

                /* pField = new FieldClass();
                 pFieldEdit = (IFieldEdit)pField;
                 pFieldEdit.Name_2 = "经度";
                 pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                 pFieldsEdit.AddField(pField);
                 */

                //创建shp
                featureClass = featureWorkspace.CreateFeatureClass(strShapeFile, pFields, null, null, esriFeatureType.esriFTSimple, "SHAPE", "");
                IFeatureLayer pFeaturelayer = new FeatureLayerClass();
                pFeaturelayer.FeatureClass = featureClass;
                pFeaturelayer.Name = strShapeFile;
                map.AddLayer(pFeaturelayer);
                //mapDocument.Save(); 
            }
            return featureClass;

        }

        public static IList<IPoint> CheckAngle(JTSYQ jtsyqGroup, double angle)
        {
            List<IPoint> pc = new List<IPoint>();
            foreach (JTSYQ jtsyq in jtsyqGroup.GroupJTSYQ)
            {
                IList<IPoint> points = ArcGisUtils.CheckAngle(jtsyq.Feature.Shape, angle);
                pc.AddRange(points);
            }

            return pc;
        }
        public static IList<MyAction> CheckAngle(IList<JTSYQ> jtsyqs, double angleValue)
        {
          
            IList<MyAction> actions = new List<MyAction>();
            List<IPoint> pc = new List<IPoint>();
           
            
              
                if (Utils.CheckListExists(jtsyqs))
                {
                    foreach (JTSYQ jtsyq in jtsyqs)
                    {
                        Action action = new Action(() =>
                        {

                            pc.AddRange(CheckAngle(jtsyq, angleValue));
                        });
                        MyAction myAction = new MyAction(action, jtsyq.QLR + "：：检查图形");
                        actions.Add(myAction);
                    }
                }
            
            Action action1 = new Action(() =>
            {
                if (pc.Count > 0)
                {
                    System.Windows.Forms.MessageBox.Show("共检查出：" + pc.Count + " 个点有问题");
                    IFeatureClass pFeatureClass = ArcGisUtils.GetFeatureLayer(JZDCustom.JZDLayer).FeatureClass;
                    IDataset dataset = (IDataset)pFeatureClass;
                    IWorkspace workspace = dataset.Workspace;
                    //添加图层
                    IFeatureClass featureClass = CreatSimpleFeatureClass(workspace as IFeatureWorkspace, "错误");
                    IFeatureCursor featureCursor = featureClass.Insert(true);
                    IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
                    //加入点
                    foreach (IPoint pt in pc)
                    {
                        IPoint point = new PointClass();
                        point.PutCoords(pt.X, pt.Y);
                        featureBuffer.Shape = point;
                        featureCursor.InsertFeature(featureBuffer);
                    }
                }
               
            });
            
            MyAction myAction1 = new MyAction(action1, "增加点");
            actions.Add(myAction1);
            return actions;
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="groupJTSYQ"></param>
        public static void SaveJTSYQBZMap(IMap map, IList<JTSYQ> jtsyqs)
        {
            ITextSymbol textSymbol = ArcGisUtils.CreateTextSymbol(14, "宋体");
            IGraphicsContainer pGraph = map as IGraphicsContainer;
           
         
                foreach (JTSYQ jtsyq in jtsyqs)
                {
                    string bz = jtsyq.XZDM.CunZu + "农民集体" + "\n" + jtsyq.BM;
                    IPointCollection pc = jtsyq.Feature.Shape as IPointCollection;
                    IPoint pt = ArcGisUtils.GetPolyCore(pc);
                    var textELment = ArcGisUtils.CreateTextElement(bz, pt, textSymbol);
                    pGraph.AddElement(textELment as IElement, 0);
                }

            
        }
        /// <summary>
        /// 检查数据
        /// </summary>
        public static IList<MyAction> CheckDataActions(IList<JTSYQ> entitys)
        {
            IList<MyAction> actions = new List<MyAction>();
           foreach(JTSYQ group in entitys)
            {
                MyAction action = new MyAction(new Action(() =>
                {
                    IList<string> list = JTSYQCustom.Check(group.GroupJTSYQ);
                    if (MyUtils.Utils.CheckListExists(list))
                    {
                        MessageBox.Show("有问题，请先修改：" + MyUtils.Utils.ListToString(list, "、"));
                    }
                  
                }), group.QLR+":基础检查数据", "");
                actions.Add(action);
            }
          

            return actions;
        }

        /// <summary>
        /// 检查数据的合理性
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static IList<string> Check(IList<JTSYQ> entitys)
        {
            string bm;
            string qlr;
            string error;
            IList<string> list = new List<string>();
            foreach (JTSYQ jtsyq in entitys)
            {
                bm = jtsyq.BM;
                if (bm == null)
                {
                    error = "集体所有权有编码为";
                    if (!list.Contains(error))
                    {
                        list.Add(error);
                    }
                }
                else if (bm.Length != 19)
                {
                    list.Add("集体所有权编码不是19位：" + bm);
                }
                qlr = jtsyq.QLR;
                if (qlr == null)
                {
                    error = "权利人名字有为空的";
                    if (!list.Contains(error))
                    {
                        list.Add(error);
                    }
                }
                else if (qlr.Length < 2)
                {
                    list.Add("集体所有权权利人名字少于2个字符串：" + qlr);
                }
            }
            return list;
        }

        public static void SaveMap(IList<JTSYQ> jtsyqs)
        {

            IFeatureClass featureClass = GetLayer().FeatureClass;
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                ArcGisService.AddFeature(jtsyq, jtsyq.Feature.ShapeCopy, featureClass);
            }
        }
        /// <summary>
        /// 得到多个图形的集合
        /// </summary>
        /// <param name="jtsyqs"></param>
        /// <returns></returns>
        public static IEnvelope SetExtent(IList<JTSYQ> jtsyqs,double scale=1.1)
        {
            IPointCollection pts = new PolygonClass();
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                IEnvelope extent = jtsyq.Feature.Shape.Envelope;
                pts.AddPointCollection(ArcGisUtils.GetEnvelopePc(extent));

            }
            IEnvelope dv = ArcGisUtils.ScaleEnvelope(ArcGisUtils.CreatePolygon(ArcGisUtils.CreatePolyline(pts)).Envelope, scale).Envelope;
            return dv;
        }

        public static void OrderByBM(IList<JTSYQ> jtsyqs)
        {
            if (jtsyqs == null)
            {
                return;
            }
            int len = jtsyqs.Count;
            for (int i = 0; i < len - 1; i++) /* 外循环为排序趟数，len个数进行len-1趟 */
            {
                for (int j = 0; j < len - 1 - i; j++)
                { /* 内循环为每趟比较的次数，第i趟比较len-i次 */

                    if (jtsyqs[j].BM.CompareTo((jtsyqs[j + 1].BM)) > 0)
                    { /* 相邻元素比较，若逆序则交换（升序为左大于右，降序反之） */
                        JTSYQ temp = jtsyqs[j];
                        jtsyqs[j] = jtsyqs[j + 1];
                        jtsyqs[j + 1] = temp;
                    }


                }
            }
        }



        public static IList<JTSYQ> FeaturesToJTSYQ(IList<IFeature> features)
        {
            IList<JTSYQ> list = ArcGisService.FeatureToObj<JTSYQ>(features, arcgisConfigPath);
            
            if (list != null)
            {
                foreach (JTSYQ jtsyq in list)
                {
                    string bm = jtsyq.BM;
 
                    if (bm.Length != 19)
                    {
                        throw new Exception("ArcGis图上，有不是19的编号");
                    }
                    if (bm.EndsWith("JA00999"))
                    {
                        bm = bm.Replace("JA00999", "");

                    }else
                    if (bm.EndsWith("JA00998"))
                    {
                        bm = bm.Replace("JA00998", "");

                    }
                    else if (bm.EndsWith("JA00000"))
                    {
                        bm = bm.Replace("JA00000", "");
                    }
                    else
                    {
                        bm = bm.Replace("JA000", "");
                    }
                    XZDM xzdm = XZDMCustom.GetXzdm(bm);
                    if (xzdm == null)
                    {
                        throw new Exception("此编码还没有在行政代码EXCEL中加入：" + bm);
                    }
                    jtsyq.XZDM = xzdm;
                }
            }
            return list;
        }
        public static JTSYQ FeaturesToJTSYQ(IFeature feature)
        {
            if(feature != null )
            {
                IList<IFeature> list = new List<IFeature>();
                list.Add(feature);
                return FeaturesToJTSYQ(list)[0];
            }
            return null;
        }

        private static IList<JTSYQ> saveList = new List<JTSYQ>();



        /// <summary>
        /// 得到四所涉及的行政代码 包括自身的行政代码
        /// </summary>
        /// <param name="jtsyq"></param>
        /// <returns></returns>
        public static IList<XZDM> GetSZInXZDM(JTSYQ jtsyq)
        {
            IList<XZDM> list = new List<XZDM>();
            list.Add(jtsyq.XZDM);
            string bm = jtsyq.BM;

            if (Utils.IsStrNull(jtsyq.SZD)  || Utils.IsStrNull(jtsyq.SZN) || Utils.IsStrNull(jtsyq.SZX) || Utils.IsStrNull(jtsyq.SZB))
            {
                MessageBox.Show(jtsyq.QLR + "：四至有为空的！！！");
                return list;
            }
            string szAll = jtsyq.SZD + "、" + jtsyq.SZN + "、" + jtsyq.SZX + "、" + jtsyq.SZB;

            string[] szArray = szAll.Split('、');
            IList<string> szlist = new List<string>();
            for(int a =0;a < szArray.Length;a++)
            {
                if(!szlist.Contains(szArray[a]))
                {
                    szlist.Add(szArray[a]);
                }
               
            }
            foreach (string sz in szlist)
            {
                XZDM xzdm = XZDMCustom.GetXzdmByAddress(sz);
                if (xzdm != null)
                {
                    list.Add(xzdm);
                }else
                {
                    MessageBox.Show("指界人，无法识别的四至："+ sz);
                }
            }
            return list;
        }

        public void ArcGisSave()
        {

            saveList.Add(this.JTSYQ);
            //IList<IFeature> features = new List<IFeature>();
            //features.Add(this.Feature);
            //var v = ArcGisService.FeatureToObj<JTSYQ>(features, arcgisConfigPath);
            ArcGisService.ObjToFeature<JTSYQ>(saveList, arcgisConfigPath);
            this.JTSYQ.Feature.Store();
            saveList.Remove(this.JTSYQ);
            this.JTSYQ.MapTabDictoryCustom.Dic = MapTabDictoryCustom.GetTabDictory(this.JTSYQ.Feature);
        }

        public static IList<JTSYQ> GetMapJTSYQ(string sql = "")
        {

            IFeatureLayer jtLayer = ArcGisUtils.GetFeatureLayer(JTSYQCustom.JTSYQLayerName);
            IFeatureCursor cur = ArcGisUtils.GetEntitys(sql, jtLayer);
            IList<IFeature> features = ArcGisUtils.CursorToList(cur);
            return FeaturesToJTSYQ(features);
        }


        public static IList<JTSYQ> GroupByZu(IList<JTSYQ> jtsyqs)
        {


            IList<JTSYQ> result = new List<JTSYQ>();
            if (jtsyqs == null)
            {
                return result;
            }
            Dictionary<string, IList<JTSYQ>> jtsyqDic = Utils.GetGroupDicToList("BM", jtsyqs);
            foreach (string bm in jtsyqDic.Keys)
            {

                IList<JTSYQ> list = jtsyqDic[bm];
                JTSYQ jtsyq = new JTSYQ();
                ReflectManager.ReflectUtils.ClassCopy(list[0], jtsyq);
                jtsyq.Area = Decimal.ToSingle(decimal.Round(new decimal(jtsyq.Shape_Area), 2));
                for (int a = 1; a < list.Count; a++)
                {
                    jtsyq.Area += Decimal.ToSingle(decimal.Round(new decimal(list[a].Shape_Area), 2));
                    if (list[a].SZB != null)
                    {
                        if (jtsyq.SZB == null || !jtsyq.SZB.Contains(list[a].SZB))
                        {
                            jtsyq.SZB = jtsyq.SZB + "、" + list[a].SZB;
                        }
                    }
                    if (list[a].SZD != null)
                    {

                        if (jtsyq.SZD == null || !jtsyq.SZD.Contains(list[a].SZD))
                        {
                            jtsyq.SZD = jtsyq.SZD + "、" + list[a].SZD;
                        }
                    }
                    if (list[a].SZN != null)
                    {

                        if (jtsyq.SZN == null || !jtsyq.SZN.Contains(list[a].SZN))
                        {
                            jtsyq.SZN = jtsyq.SZN + "、" + list[a].SZN;
                        }

                    }
                    if (list[a].SZX != null)
                    {
                        if (jtsyq.SZX == null || !jtsyq.SZX.Contains(list[a].SZX))
                        {
                            jtsyq.SZX = jtsyq.SZX + "、" + list[a].SZX;
                        }
                    }

                    if (list[a].TuFu != null)
                    {
                        if (jtsyq.TuFu == null || !jtsyq.TuFu.Contains(list[a].TuFu))
                        {
                            jtsyq.TuFu = jtsyq.TuFu + "、" + list[a].TuFu;
                        }
                    }

                }
                RepaireStr(jtsyq,"SZB");
                RepaireStr(jtsyq, "SZD");
                RepaireStr(jtsyq, "SZN");
                RepaireStr(jtsyq, "SZX");
                RepaireStr(jtsyq, "TuFu");
                jtsyq.Area = Math.Round(jtsyq.Area / 10000, 4);
                jtsyq.GroupJTSYQ = list;
                result.Add(jtsyq);
            }
            return result;
        }

        private static void RepaireStr(JTSYQ jtsyq, string methodName)
        {
            MethodInfo m = jtsyq.GetType().GetMethod("get_"+methodName);
            string value = m.Invoke(jtsyq,null) as string; 
            if(Utils.IsStrNull(value))
            {
                return;
            }
            string str ="";
            if(value .Contains("、"))
            {
                string[] array = value.Split('、');
                for(int a =0; a <array.Length;a++)
                {
                    if(!str.Contains(array[a]))
                    {
                        
                        str = str + "、" + array[a];
                    }
                   
                }
                if (str.StartsWith("、"))
                {
                    str = str.Remove(0, 1);
                }
                object[] paramters = new object[]
                {
                    str
                };
                MethodInfo m2 = jtsyq.GetType().GetMethod("set_" + methodName);
                m2.Invoke(jtsyq, paramters);
            }
          
            

        }

        /// <summary>
        /// 设置四至
        /// </summary>
        /// <param name="key"></param>
        /// <param name="jtsyq"></param>
        /// <param name="over"></param>
        public void SetSiZi(PreviewKeyDownEventArgs key, JTSYQ jtsyq, bool over = true)
        {
            string sz;
            string bm = this.JTSYQ.XZDM.DJZQDM;
            if (bm.Length <= 9)
            {
                return;
            }
            XZDM xzdm2 = jtsyq.XZDM;
            string bm2 = xzdm2.DJZQDM;
            if (bm2.Length <= 9)
            {
                return;
            }
            if (bm.Substring(0, 9).Equals(bm2.Substring(0, 9)))
            {
                sz = xzdm2.Cun + xzdm2.Zu;
            }
            else
            {
                sz = xzdm2.XiangZheng + xzdm2.Cun + xzdm2.Zu;
            }

            switch (key.KeyData)
            {
                case Keys.Up:
                    if (over)
                    {
                        JTSYQ.SZB = sz;
                    }
                    else
                    {
                        JTSYQ.SZB = JTSYQ.SZB + "、" + sz;
                    }
                    break;
                case Keys.Right:

                    if (over)
                    {
                        JTSYQ.SZD = sz;
                    }
                    else
                    {
                        JTSYQ.SZD = JTSYQ.SZD + "、" + sz;
                    }
                    break;
                case Keys.Down:
                    if (over)
                    {
                        JTSYQ.SZN = sz;
                    }
                    else
                    {
                        JTSYQ.SZN = JTSYQ.SZN + "、" + sz;
                    }

                    break;
                case Keys.Left:
                    if (over)
                    {
                        JTSYQ.SZX = sz;
                    }
                    else
                    {
                        JTSYQ.SZX = JTSYQ.SZX + "、" + sz;
                    }
                    break;
            }
            this.ArcGisSave();
        }

        /// <summary>
        /// 设置四至
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sizis"></param>
        public void SetSiZi(PreviewKeyDownEventArgs key, IList<JTSYQ> sizis)
        {
            //移除重复的
            for (int a = 0; a < sizis.Count; a++)
            {
                JTSYQ jtsyq = sizis[a];
                string bm = jtsyq.BM;
                for (int b = a + 1; b < sizis.Count; b++)
                {
                    if (bm.Equals(sizis[b].BM))
                    {
                        sizis.RemoveAt(b);
                        b--;
                    }
                }
            }
            //四至排序
            OrderByBM(sizis);

            for (int a = 0; a < sizis.Count; a++)
            {
                if (a == 0)
                {
                    SetSiZi(key, sizis[a]);
                }
                else
                {
                    SetSiZi(key, sizis[a], false);
                }

            }
        }

        public static IList<JTSYQ> GetSelect(ItemCollection items)
        {
            IList<JTSYQ> result = new List<JTSYQ>();
            Dictionary<string, IList<JTSYQ>> jtsyqsDic = MyUtils.Utils.GetGroupDicToList("BM", GetMapJTSYQ());

            Dictionary<string, IList<JZD>> jzdDic = MyUtils.Utils.GetGroupDicToList("JTSYQOBJECTID", JZDCustom.GetMapJZD());

            IList<JTSYQ> list;
            IList<JZD> jzds;

            foreach (PropertyNodeItem item in PropertyNodeItem.FindSelect(items))
            {

                if (item.IsSelected.Value)
                {
                    string zdnum = item.Name;


                    if (jtsyqsDic.TryGetValue(zdnum, out list))
                    {
                        foreach (JTSYQ jtsyq in list)
                        {
                            if (jzdDic.TryGetValue(jtsyq.OBJECTID + "", out jzds))
                            {
                                jtsyq.JZDS = jzds;
                            }
                        }
                        list[0].GroupJTSYQ = list;
                        result.Add(list[0]);
                    }
                }

            }
            return result;
        }

        /// <summary>
        /// 设置相交 、包含的的土地 、林地、建设用地等等各种面积
        /// </summary>
        /// <param name="jtsyqGroup"></param>
        public static void SetContainsFeatureArea(JTSYQ jtsyqGroup)
        {
            IFeatureLayer featureLayer = ArcGisUtils.GetFeatureLayer(DLTBLayerName);
            IFeature temp;
            jtsyqGroup.NYDMJ = 0;
            jtsyqGroup.GDMJ = 0;
            jtsyqGroup.LDMJ = 0;
            jtsyqGroup.CDMJ = 0;
            jtsyqGroup.YDMJ = 0;
            jtsyqGroup.QTNYDMJ = 0;
            jtsyqGroup.JSYDMJ = 0;
            jtsyqGroup.WLYDMJ = 0;
            foreach (JTSYQ jtsyq in jtsyqGroup.GroupJTSYQ)
            {
                IFeature searchFeature = jtsyq.Feature;
                IGeometry searchGeometry = searchFeature.Shape;
                IList<IFeature> containsFeature = ArcGisUtils.GetSeartchFeatures(featureLayer, searchGeometry, esriSpatialRelEnum.esriSpatialRelContains);

                IList<IFeature> IntersectFeature = ArcGisUtils.GetSeartchFeatures(featureLayer, searchGeometry, esriSpatialRelEnum.esriSpatialRelIntersects);
                Dictionary<string, IFeature> interFeatureDic = Utils.GetGroupDic("OID", IntersectFeature);
                if(containsFeature != null)
                {
                    foreach (IFeature feature in containsFeature)
                    {
                        if (interFeatureDic.TryGetValue(feature.OID + "", out temp))
                        {
                            IntersectFeature.Remove(temp);
                        }
                    }
                }
               
                //相交的面
                IList<DLTB> dltbs = ArcGisService.FeatureToObj<DLTB>(IntersectFeature, DLTB_Reflect);
                if(dltbs != null)
                {
                    foreach (DLTB dltb in dltbs)
                    {
                        IFeature feature = dltb.Feature;
                        IGeometry geometry = ArcGisUtils.GeometryClip(searchGeometry, feature.Shape);
                        SetQTMJ(jtsyqGroup, dltb, geometry as IArea);

                    }
                }
              
                //包含的面
                IList<DLTB> dltbs2 = ArcGisService.FeatureToObj<DLTB>(containsFeature, DLTB_Reflect);
                if (dltbs2 != null)
                {
                    foreach (DLTB dltb in dltbs2)
                    {
                        SetQTMJ(jtsyqGroup, dltb, dltb.Feature.Shape as IArea);
                    }
                }

            }

            jtsyqGroup.SetNYDMJ();
            SetMJ0Null(jtsyqGroup);
        }
        /// <summary>
        /// 创建裁剪线
        /// </summary>
        /// <param name="groupJTSYQ"></param>
        public static IList<IFeature> CreateCaiJianXian(IList<JTSYQ> jtsyqs)
        {
            IList<IFeature> list = new List<IFeature>();
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                IGeometry geometry = jtsyq.Feature.Shape;
                //相交的面
                IList<IFeature> intersectFeature = ArcGisUtils.GetSeartchFeatures(GetLayer(), geometry.Envelope, esriSpatialRelEnum.esriSpatialRelIntersects);
                foreach (IFeature feature in intersectFeature)
                {
                    if (feature.OID != jtsyq.Feature.OID)
                    {
                        list.Add(feature);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 如果面积是0 设为null;
        /// </summary>
        /// <param name="jtsyqGroup"></param>
        private static void SetMJ0Null(JTSYQ jtsyqGroup)
        {

            if (jtsyqGroup.NYDMJ == 0)
            {
                jtsyqGroup.NYDMJ = null;
            }
            if (jtsyqGroup.GDMJ == 0)
            {
                jtsyqGroup.GDMJ = null;
            }
            if (jtsyqGroup.LDMJ == 0)
            {
                jtsyqGroup.LDMJ = null;
            }
            if (jtsyqGroup.CDMJ == 0)
            {
                jtsyqGroup.CDMJ = null;
            }
            if (jtsyqGroup.YDMJ == 0)
            {
                jtsyqGroup.YDMJ = null;
            }
            if (jtsyqGroup.QTNYDMJ == 0)
            {
                jtsyqGroup.QTNYDMJ = null;
            }
            if (jtsyqGroup.JSYDMJ == 0)
            {
                jtsyqGroup.JSYDMJ = null;
            }
            if (jtsyqGroup.WLYDMJ == 0)
            {
                jtsyqGroup.WLYDMJ = null;
            }
        }
        private static void SetQTMJ(JTSYQ jtsyqGroup, DLTB dltb, IArea areaGeometry)
        {

            double area = Math.Round(areaGeometry.Area / 10000, 4);
            switch (dltb.DLBM)
            {
                case "011":
                case "012":
                case "013":
                    jtsyqGroup.GDMJ += area;
                    break;
                case "031":
                case "032":
                case "033":
                    jtsyqGroup.LDMJ += area;
                    break;
                case "041":
                case "042":
                case "043":
                    jtsyqGroup.CDMJ += area;
                    break;
                case "021":
                case "022":
                case "023":
                    jtsyqGroup.YDMJ += area;
                    break;
                case "104":
                case "114":
                case "117":
                case "122":
                case "123":
                    jtsyqGroup.QTNYDMJ += area;
                    break;
                case "051":
                case "052":
                case "053":
                case "054":
                case "061":
                case "062":
                case "063":
                case "071":
                case "072":
                case "081":
                case "082":
                case "083":
                case "084":
                case "085":
                case "086":
                case "087":
                case "088":
                case "091":
                case "092":
                case "093":
                case "094":
                case "095":
                case "101":
                case "102":
                case "103":
                case "105":
                case "106":
                case "107":
                case "113":
                case "118":
                case "121":
                case "202":
                case "203":
                case "204":
                case "205":
                    jtsyqGroup.JSYDMJ += area;
                    break;
                case "111":
                case "112":
                case "115":
                case "116":
                case "119":
                case "124":
                case "125":
                case "126":
                case "127":

                    jtsyqGroup.WLYDMJ += area;
                    break;
                default:
                    System.Windows.MessageBox.Show("算面积时，此编码无法识别：" + dltb.DLBM);
                    break;

            }
        }
        /// <summary>
        /// 给集体所有权所有 界址点 
        /// </summary>
        /// <param name="jtsyqs"></param>
        public static void SetJZD(IList<JTSYQ> jtsyqs)
        {
            Dictionary<string, IList<JZD>> jzdDic = MyUtils.Utils.GetGroupDicToList("JTSYQOBJECTID", JZDCustom.GetMapJZD());
            IList<JZD> jzds;
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                if (jzdDic.TryGetValue(jtsyq.OBJECTID + "", out jzds))
                {
                    OrderByJZDH(jzds);
                    jtsyq.JZDS = jzds;
                }
            }

        }
        /// <summary>
        /// 设置签章表（界址点顺序已经排好的情况下）
        /// </summary>
        /// <param name="jtsyqs"></param>
        public static void SetQZB(List<JTSYQ> jtsyqs)
        {
            IFeatureLayer jtLayer = ArcGisUtils.GetFeatureLayer(JTSYQCustom.JTSYQLayerName);
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                IList<JZD> jzds = jtsyq.JZDS;
                IList<IList<IFeature>> qzbs = new List<IList<IFeature>>();
                if (!Utils.CheckListExists(jzds))
                {
                  MessageBox.Show("宗地还没有界址点成果表：" + jtsyq.BM);
                    return;
                }
                IList<JZD> searchQZB = new List<JZD>();
                IPointCollection pc = jtsyq.Feature.Shape as IPointCollection;
                
                for (int a = 0; a < jzds.Count; a++)
                {
                    IList<IFeature> searchJtsyqs = ArcGisUtils.CircleSelect(jzds[a].Feature.Shape as IPoint, jtLayer);
                    qzbs.Add(searchJtsyqs);
                }
                IList<QZB> qzbsList = SearchJTSYQToQZB(jtsyq.Feature.OID, qzbs);
                if (qzbsList != null)
                {
                    jtsyq.QZBS = qzbsList;
                    foreach (QZB qzb in qzbsList)
                    {
                        int start = qzb.StartDH;
                        int end = qzb.EndDH;
                        qzb.StartDH = jzds[start - 1].JZDH;
                        qzb.EndDH = jzds[end - 1].JZDH;
                    }
                }

            }

        }

        /// <summary>
        /// 查询到的签章表转换成签章表
        /// </summary>
        /// <param name="qzbs"></param>
        /// <returns></returns>
        private static IList<QZB> SearchJTSYQToQZB(int selfOID, IList<IList<IFeature>> features)
        {
            //减少本身，以及重复的Feature
            IList<IFeature> result;
            int jtsyqCount = features.Count;
            for (int a = 0; a < jtsyqCount; a++)
            {
                result = features[a];
                if (result == null || result.Count == 1)
                {
                    //如果像U图形那样，中间空洞，就不能移除
                    //features.RemoveAt(a);
                    //jtsyqCount--;
                    //a--;
                    features[a] = null;
                }
                else
                {
                    int resultCount = result.Count;
                    for (int b = 0; b < resultCount; b++)
                    {
                        if (result[b].OID == selfOID)
                        {
                            result.RemoveAt(b);
                            resultCount--;
                            b--;
                        }
                    }
                }
            }
            return SearchJTSYQToQZB(features);

        }

        private static IList<QZB> SearchJTSYQToQZB(IList<IList<IFeature>> features)
        {

            IList<QZB> qzbs = new List<QZB>();
            int jtsyqCount = features.Count;
            for (int a = 0; a < jtsyqCount - 1;)
            {

                IList<IFeature> featuresSelf = features[a];
                a++;
                QZB qzb = GetQZB(features, featuresSelf, ref a);
                if (qzb != null)
                {
                    qzbs.Add(qzb);
                }


            }
            if (qzbs.Count == 0)
            {
                qzbs.Add(new QZB(1, jtsyqCount, ""));
            }
            //最后一个回头添加
            IList<IFeature> first = features[0];
            IList<IFeature> last = features[jtsyqCount - 1];
            if (last == null || last.Count == 0)
            {
                qzbs.Add(new QZB(jtsyqCount, 1, ""));
            }
            else if (last.Count == 1)
            {
                if (Utils.CheckListExists(first))
                {
                    IList<IFeature> xiangtong = CheckQZBXiangTong(last, first);
                    if (xiangtong.Count == 1)
                    {
                        qzbs.Add(JTSYQToQZB(xiangtong, jtsyqCount, 1));
                    }
                    else
                    {
                        qzbs.Add(new QZB(jtsyqCount, 1, ""));
                    }
                }
                else
                {
                    qzbs.Add(new QZB(jtsyqCount, 1, ""));
                }
            }
            else
            {
                if (Utils.CheckListExists(first))
                {
                    IList<IFeature> xiangtong = CheckQZBXiangTong(last, first);
                    if (xiangtong.Count == 1)
                    {
                        qzbs.Add((JTSYQToQZB(xiangtong, jtsyqCount, 1)));
                    }
                    else
                    {
                        qzbs.Add(new QZB(jtsyqCount, 1, ""));
                    }
                }
                else
                {
                    qzbs.Add(new QZB(1, jtsyqCount, ""));
                }
            }
            return qzbs;
        }

        /// <summary>
        /// 第一个不能来三角面
        /// </summary>
        /// <param name="features"></param>
        /// <param name="featuresSelf"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static QZB GetQZB(IList<IList<IFeature>> features, IList<IFeature> featuresSelf, ref int index)
        {



            //第一个是null 的时候
            if (!Utils.CheckListExists(featuresSelf))
            {
                //找下一个有实体的面
                while (index < features.Count)
                {
                    IList<IFeature> featuresAfter = features[index];
                    if (Utils.CheckListExists(featuresAfter))
                    {
                        return new QZB(features.IndexOf(featuresSelf) + 1, index + 1, "");
                    }
                    index++;
                }
            }
            else if (featuresSelf.Count == 1)//只有一个面
            {
                while (index < features.Count)
                {
                    IList<IFeature> featuresAfter = features[index];
                    if (!Utils.CheckListExists(featuresAfter))
                    {
                        //1、如果下一个没有
                        return JTSYQToQZB(featuresSelf, features.IndexOf(featuresSelf) + 1, index);
                    }
                    else if (featuresAfter.Count == 1)
                    {
                        //2、如果下一个 是1个不相同
                        if (featuresSelf[0].OID != featuresAfter[0].OID)
                        {
                            return JTSYQToQZB(featuresSelf, features.IndexOf(featuresSelf) + 1, index);
                        }
                    }
                    else if (featuresAfter.Count > 1)
                    {
                        //3、如果下一个 是1个以上，查找不相同的
                        foreach (IFeature feature in featuresAfter)
                        {
                            if (featuresSelf[0].OID != feature.OID)
                            {
                                return JTSYQToQZB(featuresSelf, features.IndexOf(featuresSelf) + 1, index + 1);
                            }
                        }
                    }

                    index++;
                }

            }
            else
            {
                //第一个点是三角面
                int first = index - 2;
                IList<IFeature> brefore;
                if (first == -1)
                {
                    brefore = features[features.Count - 1];
                }
                else
                {
                    brefore = features[index - 2];
                }

                IList<IFeature> butong = CheckQZBBuXiangTong(featuresSelf, brefore);
                IList<IFeature> featuresAfter = features[index];
                IList<IFeature> xiangtong = CheckQZBXiangTong(featuresSelf, featuresAfter);
                //只可能有一个面,用此面找到下一个里面没有的
                IList<IFeature> list = CheckQZBXiangTong(butong, xiangtong);
                while (index < features.Count - 1)
                {
                    index++;
                    featuresAfter = features[index];
                    if (featuresAfter == null || featuresAfter.Count == 0 || featuresAfter.Count == 2)
                    {
                        return JTSYQToQZB(list, features.IndexOf(featuresSelf) + 1, index);
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 只能来一个Feature
        /// </summary>
        /// <param name="features"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static QZB JTSYQToQZB(IList<IFeature> features, int start, int end)
        {
            if (!Utils.CheckListExists(features))
            {
                return new QZB(start, end, "");
            }
            XZDM xzdm = XZDMCustom.GetXzdm(XZDMCustom.JTSYQBianMaToXZDM(FeaturesToJTSYQ(features)[0].BM));
            QZB qzb = new QZB(start, end, xzdm.DJZDQMC);
            return qzb;
        }
        /// <summary>
        /// 1中只有一个在2中没有找到，就不相同,返回不相同的一个
        /// </summary>
        /// <param name="features1"></param>
        /// <param name="features2"></param>
        /// <returns></returns>
        private static IList<IFeature> CheckQZBBuXiangTong(IList<IFeature> features1, IList<IFeature> features2)
        {
            List<IFeature> result = new List<IFeature>();
            if (Utils.CheckListExists(features1))
            {
                if (!Utils.CheckListExists(features2))
                {
                    result.AddRange(features1);

                }
                else
                {
                    Dictionary<string, IFeature> feature2Dic = Utils.GetGroupDic("OID", features2);

                    //两个面时找不同的
                    foreach (IFeature feature1 in features1)
                    {
                        if (!feature2Dic.ContainsKey(feature1.OID + ""))
                        {
                            result.Add(feature1);
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 1中只有一个在2中找到，就相同,返回相同的一个
        /// </summary>
        /// <param name="features1"></param>
        /// <param name="features2"></param>
        /// <returns></returns>
        private static IList<IFeature> CheckQZBXiangTong(IList<IFeature> features1, IList<IFeature> features2)
        {
            List<IFeature> result = new List<IFeature>();
            if (Utils.CheckListExists(features1))
            {
                if (!Utils.CheckListExists(features2))
                {
                    result.AddRange(features1);

                }
                else
                {
                    Dictionary<string, IFeature> feature2Dic = Utils.GetGroupDic("OID", features2);

                    //两个面时找不同的
                    foreach (IFeature feature1 in features1)
                    {
                        if (feature2Dic.ContainsKey(feature1.OID + ""))
                        {
                            result.Add(feature1);
                        }
                    }
                }
            }
            else
            {
                return null;

            }

            return result;
        }

        /// <summary>
        /// 多部件界址点排序
        /// </summary>
        public static void OrderByJZDH(JTSYQ jt)
        {
            IList<JTSYQ> jtsyqs = jt.GroupJTSYQ;

            foreach (JTSYQ jtsyq in jtsyqs)
            {
                OrderByJZDH(jtsyq.JZDS);
            }
            int len = jtsyqs.Count;
            for (int i = 0; i < len - 1; i++) /* 外循环为排序趟数，len个数进行len-1趟 */

                for (int j = 0; j < len - 1 - i; j++)
                { /* 内循环为每趟比较的次数，第i趟比较len-i次 */
                    if (Utils.CheckListExists((jtsyqs[j].JZDS)) && Utils.CheckListExists(jtsyqs[j + 1].JZDS))
                    {
                        if (jtsyqs[j].JZDS[0].JZDH.CompareTo(jtsyqs[j + 1].JZDS[0].JZDH) > 0)
                        { /* 相邻元素比较，若逆序则交换（升序为左大于右，降序反之） */
                            JTSYQ temp = jtsyqs[j];
                            jtsyqs[j] = jtsyqs[j + 1];
                            jtsyqs[j + 1] = temp;
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(jt.QLR + ": 组内没有界址点号，无法排序");
                    }

                }
        }
        /// <summary>
        /// 界址点排序
        /// </summary>
        /// <param name="jzds"></param>
        public static void OrderByJZDH(IList<JZD> jzds)
        {
            if (jzds == null)
            {
                return;
            }
            int len = jzds.Count;
            for (int i = 0; i < len - 1; i++) /* 外循环为排序趟数，len个数进行len-1趟 */
            {
                for (int j = 0; j < len - 1 - i; j++)
                { /* 内循环为每趟比较的次数，第i趟比较len-i次 */

                    if (jzds[j].JZDH > (jzds[j + 1].JZDH))
                    { /* 相邻元素比较，若逆序则交换（升序为左大于右，降序反之） */
                        JZD temp = jzds[j];
                        jzds[j] = jzds[j + 1];
                        jzds[j + 1] = temp;
                    }


                }
            }

        }

        /// <summary>
        /// 刷新行政代码表
        /// </summary>
        public static IList<PropertyNodeItem> GetXZDMTree()
        {
            

            IFeatureLayer jtLayer = ArcGisUtils.GetFeatureLayer(JTSYQCustom.JTSYQLayerName);
            if(jtLayer == null)
            {
              
                return null;
            }
            //IFeatureClass featureClass = jtLayer.FeatureClass;
          

            IFeatureCursor cur = ArcGisUtils.GetEntitys("", jtLayer);
            IList<IFeature> features = ArcGisUtils.CursorToList(cur);

            IList<JTSYQ> jtsyqs = JTSYQCustom.FeaturesToJTSYQ(features);
            IList<XZDM> xzdms = XZDMCustom.JTSYQToXZDM(jtsyqs);
            IList<PropertyNodeItem> items = XZDMCustom.XZDMToItem(xzdms);
            if (items.Count == 0)
            {
                return items;
            }
         
            string[][] expandedAndSelected = PropertyNodeItemCustom.GetHibernateDM();
            foreach (PropertyNodeItem item in items[0].FindChildAll())
            {
                string dm = item.Name;
                if (Array.IndexOf(expandedAndSelected[0], dm) != -1)
                {
                    item.IsExpanded = true;
                }
                if (Array.IndexOf(expandedAndSelected[1], dm) != -1)
                {
                    item.IsSelected = true;
                }
            }
            return items;
        }


        /// <summary>
        /// 刷新集体所有权信息
        /// </summary>
        /// <param name="oldXZDMS"></param>
        public static void ReflushJTSYQ(IList<XZDM> oldXZDMS)
        {

            

            IFeatureLayer jtLayer = ArcGisUtils.GetFeatureLayer(JTSYQCustom.JTSYQLayerName);
            IFeatureCursor cur = ArcGisUtils.GetEntitys("", jtLayer);
            IList<IFeature> features = ArcGisUtils.CursorToList(cur);

            IList<JTSYQ> jtsyqs = JTSYQCustom.FeaturesToJTSYQ(features);
         
            IList<XZDM> xzdms = XZDMCustom.JTSYQToXZDM(jtsyqs);
            Dictionary<string, XZDM> xzdmDic = Utils.GetGroupDic("DJZQDM", xzdms);

            IList<XZDM> newXZDM = new List<XZDM>();
            XZDM xzdm;
            for(int a =0; a < oldXZDMS.Count;a++)
            {
                if(xzdmDic.TryGetValue(oldXZDMS[a].DJZQDM,out xzdm))
                {
                    oldXZDMS[a].JTSYQ = xzdm.JTSYQ;
                    oldXZDMS[a].JTSYQS = xzdm.JTSYQS;
                }
            }
        }
        public static IList<JTSYQ> GetSelectJTSYQ(IList<PropertyNodeItem> selecteds)
        {
            if (!MyUtils.Utils.CheckListExists(selecteds))
            {
                return null;
            }
            IList<JTSYQ> result = new List<JTSYQ>();
            Dictionary<string, IList<JTSYQ>> jtsyqsDic = MyUtils.Utils.GetGroupDicToList("BM", GetMapJTSYQ());

            Dictionary<string, IList<JZD>> jzdDic = MyUtils.Utils.GetGroupDicToList("JTSYQOBJECTID", JZDCustom.GetMapJZD());

            IList<JTSYQ> list;
            IList<JZD> jzds;
            foreach (PropertyNodeItem item in selecteds)
            {
                string zdnum = item.Name;
                if (zdnum.Length > 12)
                {
                    if(zdnum.EndsWith("98")|| zdnum.EndsWith("97")|| zdnum.EndsWith("96")|| zdnum.EndsWith("95") || zdnum.EndsWith("94") || zdnum.EndsWith("93") || zdnum.EndsWith("92") || zdnum.EndsWith("91") )
                    {
                        zdnum = zdnum.Insert(12, "JA009");
                    }else
                    {
                        zdnum = zdnum.Insert(12, "JA000");
                    }
                }

                if (jtsyqsDic.TryGetValue(zdnum, out list))
                {
                    foreach (JTSYQ jtsyq in list)
                    {
                        if (jzdDic.TryGetValue(jtsyq.OBJECTID + "", out jzds))
                        {

                            jtsyq.JZDS = jzds;
                        }
                    }
                    list[0].GroupJTSYQ = list;
                    result.Add(list[0]);
                }
            }
            return result;
        }

    }
}
