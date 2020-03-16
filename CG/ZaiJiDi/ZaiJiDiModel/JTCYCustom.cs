using ExcelManager;
using ExcelManager.Model;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZaiJiDi.ZaiJiDiModel;
using Utils = MyUtils.Utils;
namespace ZaiJiDi.ZaiJiDiModel
{
    public class JTCYCustom
    {
        public static string YHZGXPath = System.AppDomain.CurrentDomain.BaseDirectory + "StaticFile/家庭关系代码.xls";
        public static string XMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/JTCY.Excel.hbm.xml";

        public static string ExcelWriteXMLPath = System.AppDomain.CurrentDomain.BaseDirectory + "ZaiJiDiModel/JTCY.WriteExcel.xml";



        private static Dictionary<string, string> yhzgxDic;

        /// <summary>
        /// 得到与户主关系 key 为文字  value 为数字
        /// </summary>
        public static Dictionary<string, string> YHZGXDic
        {
            get
            {
                if(yhzgxDic == null)
                {
                    yhzgxDic = Utils.DicReset(ExcelManager.ExcelRead.ReadExcelToDic(JTCYCustom.YHZGXPath, 0, 0));
                }
                return yhzgxDic;
            }
           
        }
      
        /// <summary>
        /// excel 转换为jtcy 对象 
        /// </summary>
        /// <param name="path">Excel路径</param>
        /// <returns></returns>
        public static ObservableCollection<JTCY> GetExcelToHZS(string path)
        {

            ObservableCollection<JTCY> list = ExcelUtils.GetExcelToObjectToObservableCollection<JTCY>(path, XMLPath);
            ObservableCollection<JTCY> hzs = GetJTCYSToHz(list);
            //户主放在第一位
            for(int b=0; b < hzs.Count;b++)
            {
                JTCY hz = hzs[b];
                if(Utils.IsStrNull(hz.GMSFHM))
                {
                    hzs.RemoveAt(b);
                    b--;
                    continue;
                }
                IList<JTCY> jTCies = hz.JTCies;
                for (int a=1;a<jTCies.Count;a++)
                {
                  
                    JTCY jTCY = jTCies[a];
                    if (jTCY.YHZGX == "户主")
                    {
                        jTCies.RemoveAt(a);
                        jTCies.Insert(0, jTCY);
                        break;
                    }
                }
                for (int a = 2; a < jTCies.Count; a++)
                {
                    JTCY jTCY = jTCies[a];
                    if (jTCY.YHZGX == "妻")
                    {
                        jTCies.RemoveAt(a);
                        jTCies.Insert(1, jTCY);
                        break;
                    }
                }

            }
            
            return hzs;

        }

        /// <summary>
        /// 所有家庭成员 设置到户主中 去
        /// </summary>
        /// <param name="jtcys"></param>
        /// <returns></returns>
        private static ObservableCollection<JTCY> GetJTCYSToHz(ObservableCollection<JTCY> jtcys)
        {
            if(jtcys == null)
            {
                return null;
            }
          
            string tem="";
            int index = 1;
            foreach(JTCY jtcy in jtcys)
            {
               
                if(!Utils.IsStrNull(jtcy.CBFBM))
                {
                    tem = (index++) + "";
                    jtcy.CBFBM = tem;
                  
                }else
                {
                    jtcy.CBFBM = tem;
                }
              
               
            }
            Dictionary<string, ObservableCollection<JTCY>> jtcyGroupDic = Utils.GetGroupDicToList("CBFBM", jtcys);
            ObservableCollection<JTCY> hzs = new ObservableCollection<JTCY>();
            foreach (string cbfbm in jtcyGroupDic.Keys)
            {
                IList<JTCY> oneJTCY = jtcyGroupDic[cbfbm];
                JTCY hz = FindOneJtcyHZ(oneJTCY);
                if(hz != null)
                {
                    hz.JTCies = oneJTCY;
                    hzs.Add(hz);
                }
            }
            return hzs;
        }
        /// <summary>
        /// 从一户中找到户主
        /// </summary>
        /// <param name="oneJTCY"></param>
        /// <returns></returns>
        private static JTCY FindOneJtcyHZ(IList<JTCY> oneJTCY)
        {
            int count = 0;
            JTCY hz = null;
            foreach(JTCY jtcy in oneJTCY)
            {
                if(jtcy.YHZGX == "户主")
                {
                    count++;
                    hz = jtcy;
                }
            }
            if(count ==0)
            {
                MessageBox.Show(oneJTCY[0].Address + ",姓名：" + oneJTCY[0].XM + ",家庭中没有找到户主");
               
            }else if(count > 1)
            {
                MessageBox.Show(hz.Address + ",姓名：" + hz.XM + ",家庭中找到户主有多个");
            }
            return hz;
        }
        /// <summary>
        /// 得到所有家庭成员
        /// </summary>
        /// <param name="hzs"></param>
        /// <returns></returns>

        public static IList<JTCY> GetAllJTCY(IList<JTCY> hzs)
        {
            List<JTCY> jtcys = new List<JTCY>();
            foreach(JTCY hz in hzs)
            {
                jtcys.AddRange(hz.JTCies);
            }
            return jtcys;
        }
        /// <summary>
        /// 得到所有户主
        /// </summary>
        /// <param name="jsyds"></param>
        /// <returns></returns>
        public static IList<JTCY> GetAllHzs(IList<JSYD> jsyds)
        {
            List<JTCY> hzs = new List<JTCY>();
            IList<JTCY> list;
            foreach (JSYD jsyd in jsyds)
            {
                list = jsyd.HZs;
                if (MyUtils.Utils.CheckListExists(list))
                {
                    hzs.AddRange(list);
                }

            }
            return hzs;
        }

        /// <summary>
        /// 输出所有家庭成员
        /// </summary>
        /// <param name="hz"></param>
        /// <returns></returns>
        public static string ToStringJTCYs(JTCY hz)
        {
            StringBuilder sb = new StringBuilder();
            IList<JTCY> jtcys = hz.JTCies;
            for(int a =0; a< jtcys.Count;a++)
            {
                JTCY jtcy = jtcys[a];
                if( a % 2 == 0 )
                {
                    sb.Append(jtcy.XM + " (" + jtcy.GMSFHM + "); ");
                }
                else
                {
                    sb.Append(jtcy.XM + " (" + jtcy.GMSFHM + ");        \r");
                 
                }
               
            }
            return sb.ToString();
        }

        public static XMLTable GetWriteExcelXMLTable()
        {
            XMLTable xmlTable = XMLRead.GetXmlToXMLTabl(ExcelWriteXMLPath)[0];
            return xmlTable;
        }
    }
}
