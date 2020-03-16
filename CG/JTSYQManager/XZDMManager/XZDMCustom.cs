using ArcGisManager;
using Config.JTSYQ;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ExcelManager;
using ExcelManager.Model;
using JTSYQManager.JTSYQModel;
using MyUtils;
using NPOI.SS.UserModel;
using Public;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JTSYQManager.XZDMManager
{
    public class XZDMCustom
    {

        // private static string XZDMPath =  System.AppDomain.CurrentDomain.BaseDirectory + "XZDMManager/行政代码.xls";
        public static string XZDMXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "XZDMManager/XZDM.Excel.hbm.xml";

        public static string GetXZDMPath()
        {
            FileConfigPageViewModel model = FileConfigPageViewModel.GetRedis();
            return model.XZDMExcelPath;
        }

        private static Dictionary<string, XZDM> xzdmdic;
        public static Dictionary<string, XZDM> XZDMDic
        {
            set
            {
                xzdmdic = value;

            }
            get
            {
                if (xzdmdic == null)
                {
                    XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(XZDMXMLPath)[0];
                    string path = XZDMCustom.GetXZDMPath();
                    if (path == "")
                    {
                        return xzdmdic;
                    }
                    ExcelReflectModel<XZDM> reflectModel = new ExcelReflectModel<XZDM>(path, xmlTable);
                    IList<XZDM> xzdms = ExcelUtils.SheetToObjectsByXMLTable<XZDM>(reflectModel);
                    XZDMDic = Utils.GetGroupDic("DJZQDM", xzdms);
                    foreach (XZDM xzdm in xzdms)
                    {
                        //工作日期加15天
                        DateTime dateTime = xzdm.JTSYQ_GSStartTime;
                        if (dateTime.Year != 1)
                        {
                            DateTime dt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                            for (int a = 1; a <= 15; a++)
                            {
                                dt = dt.AddDays(1);
                                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    a--;
                                }
                            }
                            xzdm.JTSYQ_GSEndTime = dt;
                        }
                        SetJb(xzdm, XZDMDic);
                    }
                }
                return xzdmdic;
            }
        }

        /// <summary>
        /// 得到所有本村所有集体所有权
        /// </summary>
        /// <param name="xzdm">村</param>
        /// <returns></returns>
        public static IList<JTSYQ> GetCunZuAllJTSYQ(XZDM xzdm)
        {
            string bm = xzdm.DJZQDM;
            string sql = " BM LIKE  '*" + bm + "*'";

            IList<JTSYQ> jtsyqs = JTSYQCustom.GetMapJTSYQ(sql);
            JTSYQCustom.OrderByBM(jtsyqs);


            IList<JTSYQ> jtsyqGroup = JTSYQCustom.GroupByZu(jtsyqs);

            return jtsyqGroup;
        }
        /// <summary>
        /// 得到所有权 如果是村代码 只得到集体
        /// </summary>
        /// <param name="xzdm"></param>
        /// <returns></returns>
        public static IList<JTSYQ> GetSelectJTSYQ(XZDM xzdm)
        {
            string bm = xzdm.DJZQDM;
            string sql = "";
            switch (bm.Length)
            {
                case 6:
                    break;
                case 9:
                    sql = " BM LIKE  '*" + bm + "*' And BM LIKE '*998'";
                    break;
                case 12:
                    sql = " BM LIKE  '*" + bm + "*' And BM LIKE '*00'";
                    break;
                case 14:
                    sql = " BM LIKE  '*" + bm.Insert(12, "JA000") + "*' ";
                    break;
            }
            IList<JTSYQ> jtsyqs = JTSYQCustom.GetMapJTSYQ(sql);
            IList<JTSYQ> jtsyqGroup = JTSYQCustom.GroupByZu(jtsyqs);
            if (Utils.CheckListExists(jtsyqGroup))
            {
                foreach (JTSYQ jtsyq in jtsyqGroup)
                {
                    jtsyq.XZDM = xzdm;
                }
            }
            return jtsyqGroup;
        }
        /// <summary>
        /// 集体所有权编码转换成行政代码
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public static string JTSYQBianMaToXZDM(string bm)
        {
            if (bm != null && bm.Length > 5)
            {
                if (bm.EndsWith("998"))
                {
                    return bm.Substring(0, 6);
                }
                else if (bm.EndsWith("00"))
                {
                    return bm.Replace("JA000", "");
                }
                else
                {
                    return bm.Replace("JA000", "");
                }
            }
            return "";
        }
        /// <summary>
        /// 通过地址得到行政代码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static XZDM GetXzdmByAddress(string address)
        {
            address = address.Replace("农民集体", "");
            int index = address.LastIndexOf("乡");
            if (index == -1)
            {
                index = address.LastIndexOf("镇");
            }
            if (index != -1)
            {
                address = address.Substring(index + 1);
            }
            foreach (XZDM xzdm in XZDMDic.Values)
            {
                if (xzdm.DJZDQMC.Contains(address))
                {
                    return xzdm;
                }
            }
            //throw new Exception("通过地址无法找到行政代码，行政代码表没有添加这个地址：" + address);
            return null;
        }

        /// <summary>
        /// 只适用于只有一个根节点
        /// </summary>
        /// <param name="xzdms"></param>
        /// <returns></returns>
       /* public static IList<PropertyNodeItem> XZDMToItem(IList<XZDM> xzdms)
        {
            OrderByBM(xzdms);
            List<PropertyNodeItem> list = new List<PropertyNodeItem>();
            if (xzdms.Count == 0)
            {
                return list;
            }

            XZDM xzdm1 = xzdms[0];
            PropertyNodeItem item1 = GetNodeItem(xzdm1,6);
            list.Add(item1);
            Dictionary<string, PropertyNodeItem> dic = new Dictionary<string, PropertyNodeItem>();
            dic.Add(item1.Name, item1);
             PropertyNodeItem parent;
            for (int a = 0; a < xzdms.Count; a++)
            {
                XZDM xzdm = xzdms[a];
                string bm = xzdm.DJZQDM;
                PropertyNodeItem child = GetNodeItem(xzdm);
                if (bm.Length == 14)
                {
                   
                    if (dic.TryGetValue(bm.Substring(0, 12), out parent))
                    {
                        
                        if(!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                       
                    }
                }
                else
                if (bm.Length == 12)
                {
                    if (dic.TryGetValue(bm.Substring(0, 9), out parent))
                    {
                        
                        if (!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                    }
                }
                else
                 if (bm.Length == 9)
                {
                   if (dic.TryGetValue(bm.Substring(0, 6), out parent))
                    {
                        
                        if (!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                    }

                }
                else if (bm.Length == 6)
                {
                    if (dic.TryGetValue(bm.Substring(0, 6), out parent))
                    {

                       
                        if (!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                    }
                }
            }
            return list;
        }*/

        /// <summary>
        /// 只适用于只有一个根节点
        /// </summary>
        /// <param name="xzdms"></param>
        /// <returns></returns>
        public static IList<PropertyNodeItem> XZDMToItem(IList<XZDM> xzdms)
        {
            OrderByBM(xzdms);
            List<PropertyNodeItem> list = new List<PropertyNodeItem>();
            if (xzdms.Count == 0)
            {
                return list;
            }

            XZDM xzdm1 = xzdms[0];
            PropertyNodeItem item1 = GetNodeItem(xzdm1,6);
            list.Add(item1);
            Dictionary<string, PropertyNodeItem> dic = new Dictionary<string, PropertyNodeItem>();
            dic.Add(item1.Name, item1);
             PropertyNodeItem parent;
            for (int a = 0; a < xzdms.Count; a++)
            {
                XZDM xzdm = xzdms[a];
                string bm = xzdm.DJZQDM;
                PropertyNodeItem child = GetNodeItem(xzdm);
                if (bm.Length == 14)
                {
                   
                    if (dic.TryGetValue(bm.Substring(0, 12), out parent))
                    {
                        
                        if(!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                       
                    }
                }
                else
                if (bm.Length == 12)
                {
                    if (dic.TryGetValue(bm.Substring(0, 9), out parent))
                    {
                        
                        if (!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                    }
                }
                else
                 if (bm.Length == 9)
                {
                   if (dic.TryGetValue(bm.Substring(0, 6), out parent))
                    {
                        
                        if (!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                    }

                }
                else if (bm.Length == 6)
                {
                    if (dic.TryGetValue(bm.Substring(0, 6), out parent))
                    {

                       
                        if (!dic.ContainsKey(bm))
                        {
                            parent.AddChild(child);
                            dic.Add(bm, child);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 找到行政代码的parent
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private static PropertyNodeItem FintParentItem(string dm, IList<PropertyNodeItem> list)
        {

            foreach (PropertyNodeItem item in list)
            {
                IList<PropertyNodeItem> childs = item.FindChildAll();
                for (int a = childs.Count - 1; a >= 0; a--)
                {

                    if (dm.Contains(childs[a].Name))
                    {
                        return childs[a];
                    }
                }
                if (dm.Contains(item.Name))
                {
                    return item;
                }
            }
            return null;
        }

        private static PropertyNodeItem GetAllNodeItem(XZDM xzdm)
        {
            PropertyNodeItem root = GetNodeItem(xzdm, 6);
            PropertyNodeItem root1 = GetNodeItem(xzdm, 9);
            PropertyNodeItem root2 = GetNodeItem(xzdm, 12);
            PropertyNodeItem root3 = GetNodeItem(xzdm, 14);
            if (root3 != null)
            {
                root3.Parent = root2;

            }
            if (root2 != null)
            {
                root2.Parent = root1;

            }
            if (root1 != null)
            {
                root1.Parent = root;
            }

            return root;


        }

        private static PropertyNodeItem GetNodeItem(XZDM xzdm, int sub)
        {
            string bm = xzdm.DJZQDM;
            if (bm.Length >= sub)
            {
                try
                {
                    XZDM xzdm2 = GetXzdm(bm.Substring(0, sub));

                    return GetNodeItem(xzdm2);
                }
                catch (Exception e)
                {
                    return null;
                }

            }
            return null;
        }

        private static PropertyNodeItem GetNodeItem(XZDM xzdm)
        {
            PropertyNodeItem item = new PropertyNodeItem();
            if (xzdm.DJZQDM.Length == 14)
            {
                item.DisplayName = xzdm.Zu;
            }
            else if (xzdm.DJZQDM.Length == 12)
            {
                item.DisplayName = xzdm.Cun;
            }
            else if (xzdm.DJZQDM.Length == 9)
            {
                item.DisplayName = xzdm.XiangZheng;
            }
            else if (xzdm.DJZQDM.Length == 6)
            {
                item.DisplayName = xzdm.Shi;
            }

            item.data = xzdm;
            item.Name = xzdm.DJZQDM;
            item.data = xzdm;
            return item;
        }
        /// <summary>
        /// 得到选中的村
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IList<XZDM> GetSelectCun(ItemCollection items)
        {
            IList<XZDM> list = new List<XZDM>();
            foreach (PropertyNodeItem item in items)
            {
                foreach (PropertyNodeItem child in item.FindChildAll())
                {
                    if (child.Name.Length == 12)
                    {
                        if (child.IsSelected != null && child.IsSelected.Value)
                        {
                            list.Add(child.data as XZDM);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 得到选中的行政代码
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IList<XZDM> GetSelect(ItemCollection items)
        {
            


            IList<XZDM> list = new List<XZDM>();
            foreach (PropertyNodeItem item in items)
            {
                foreach (PropertyNodeItem child in item.FindChildAll())
                {
                    if (child.IsSelected != null && child.IsSelected.Value)
                    {
                        list.Add(child.data as XZDM);
                    }


                }
            }
            return list;
        }


        /// <summary>
        /// 行政代码排序
        /// </summary>
        /// <param name="xzdms"></param>
        private static void OrderByBM(IList<XZDM> xzdms)
        {
            int len = xzdms.Count;


            for (int i = 0; i < len - 1; i++) /* 外循环为排序趟数，len个数进行len-1趟 */
                for (int j = 0; j < len - 1 - i; j++)
                { /* 内循环为每趟比较的次数，第i趟比较len-i次 */
                    if (xzdms[j].DJZQDM.CompareTo(xzdms[j + 1].DJZQDM) > 0)
                    { /* 相邻元素比较，若逆序则交换（升序为左大于右，降序反之） */
                        XZDM temp = xzdms[j];
                        xzdms[j] = xzdms[j + 1];
                        xzdms[j + 1] = temp;
                    }
                }
        }
        public static IList<XZDM> GetExcelXZDM()
        {
            return XZDMDic.Values.ToList();
        }
        public static XZDM GetXzdm(string bm)
        {


            XZDM xzdm = null;
            bool flag2 = bm.Length >= 14;
            if (flag2)
            {
                if (!XZDMDic.TryGetValue(bm.Substring(0, 14), out xzdm))
                {
                    if (!XZDMDic.TryGetValue(bm.Substring(0, 12), out xzdm))
                    {
                        if (!XZDMDic.TryGetValue(bm.Substring(0, 9), out xzdm))
                        {
                            if (!XZDMDic.TryGetValue(bm.Substring(0, 6), out xzdm))
                            {

                            }
                        }
                    }
                }
               
            }
            else
            {
                bool flag3 = bm.Length >= 12;
                if (flag3)
                {
                    if (!XZDMDic.TryGetValue(bm.Substring(0, 12), out xzdm))
                    {
                        if (!XZDMDic.TryGetValue(bm.Substring(0, 9), out xzdm))
                        {
                            if (!XZDMDic.TryGetValue(bm.Substring(0, 6), out xzdm))
                            {

                            }
                        }
                    }
                }
                else
                {
                    bool flag4 = bm.Length >= 9;
                    if (flag4)
                    {
                        if (!XZDMDic.TryGetValue(bm.Substring(0, 9), out xzdm))
                        {
                            if (!XZDMDic.TryGetValue(bm.Substring(0, 6), out xzdm))
                            {

                            }
                        }
                    }
                    else
                    {
                        bool flag5 = bm.Length >= 6;
                        if (flag5)
                        {
                            XZDMDic.TryGetValue(bm.Substring(0, 6), out xzdm);
                        }
                    }
                }
            }
            bool flag6 = xzdm == null;
            if (flag6)
            {
                throw new Exception("行政代码表没有这个编码：" + bm);
            }
            return xzdm;
        }

        private static void SetJb(XZDM xzdm, Dictionary<string, XZDM> xzdmDic)
        {
            string dm = xzdm.DJZQDM;
            if (Utils.IsStrNull(dm))
            {
                return;
            }
            string[] array = SpliteAddress(xzdm.DJZQDM, xzdmDic);
            int num = array.Length;
            for (int i = 0; i < num; i++)
            {
                string text = array[i];
                switch (i)
                {
                    case 0:
                        xzdm.Shi = text;
                        break;
                    case 1:
                        xzdm.XiangZheng = text;
                        break;
                    case 2:
                        xzdm.Cun = text;
                        break;
                    case 3:
                        xzdm.Zu = text;
                        xzdm.CunZu = xzdm.Cun + text;
                        break;
                }
            }
        }

        /// <summary>
        /// 得到包含这个编码的所有行政代码
        /// </summary>
        /// <param name="xzdm"></param>
        /// <returns></returns>
        public static IList<XZDM> GetXzdms(string bm)
        {
            IList<XZDM> result = new List<XZDM>();

            foreach (XZDM x in XZDMDic.Values)
            {
                if (x.DJZQDM.Contains(bm))
                {
                    result.Add(x);
                }
            }
            return result;
        }

        private static string[] SpliteAddress(string dm, Dictionary<string, XZDM> xzdmDic)
        {
            bool flag = dm.Length > 14;
            if (flag)
            {
                dm = dm.Substring(0, 14);
            }
            string[] array = new string[4];
            bool flag2 = dm == null;
            if (flag2)
            {
                throw new Exception("传入的行政代码没有值");
            }
            bool flag3 = dm.Length >= 6;
            if (flag3)
            {
                array[0] = GetAddress(dm, 6, xzdmDic);
            }
            bool flag4 = dm.Length >= 9;
            if (flag4)
            {
                array[1] = GetAddress(dm, 9, xzdmDic);
            }
            bool flag5 = dm.Length >= 12;
            if (flag5)
            {
                array[2] = GetAddress(dm, 12, xzdmDic);
            }
            bool flag6 = dm.Length >= 14;
            if (flag6)
            {
                array[3] = GetAddress(dm, 14, xzdmDic);
            }
            Utils.ArrayReplaceAfter(array);
            return Utils.ArrayRemoveNull(array);
        }
        private static string GetAddress(string bm, int sub, Dictionary<string, XZDM> xzdmDic)
        {
            XZDM xzdm = GetXzdm(bm.Substring(0, sub), xzdmDic);
            return xzdm.DJZDQMC;
        }
        private static XZDM GetXzdm(string bm, Dictionary<string, XZDM> xzdmDic)
        {
            XZDM result;
            bool flag = xzdmDic.TryGetValue(bm, out result);
            if (flag)
            {
                return result;
            }
            throw new Exception("行政代码表没有这个代码：" + bm);
        }
        /// <summary>
        /// 集体所有权转 行政代码
        /// </summary>
        /// <param name="jtsyqs"></param>
        /// <returns></returns>
        public static IList<XZDM> JTSYQToXZDM(IList<JTSYQ> jtsyqs)
        {
            IList<XZDM> xzdms = XZDMCustom.GetExcelXZDM();
            IList<XZDM> list = new List<XZDM>();

            IList<JTSYQ> jtsqyGroups = JTSYQCustom.GroupByZu(jtsyqs);

            // Dictionary<string, IList<JTSYQ>> jtsyqDic = Utils.GetGroupDicToList("BM", jtsyqs);
            Dictionary<string, XZDM> xzdmsDic = Utils.GetGroupDic("DJZQDM", xzdms);
            XZDM xzdm;
            foreach (JTSYQ groupJTSYQ in jtsqyGroups)
            {

                string bm = groupJTSYQ.BM;
                if (bm.EndsWith("JA00998"))
                {

                }
                
                if (!xzdmsDic.TryGetValue(bm.Replace("JA000", ""), out xzdm))
                {
                    if (bm.Contains("99"))
                    {
                        bm = bm.Substring(0, 12);
                        if (!xzdmsDic.TryGetValue(bm, out xzdm))
                        {
                            if (!xzdmsDic.TryGetValue(bm.Substring(0, 9), out xzdm))
                            {
                                xzdm = null;
                            }

                        }
                    }
                  
                }
                if (xzdm != null)
                {
                    xzdm.JTSYQS = groupJTSYQ.GroupJTSYQ;
                    xzdm.JTSYQ = groupJTSYQ;
                    //list.Add(xzdm);
                }
            }

            return xzdmsDic.Values.ToList();
        }

    }
}
