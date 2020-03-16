using JTSYQManager.JTSYQModel;
using MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSYQManager.XZDMManager
{
    public class ZDT
    {
        public string ZDTBLC { get; set; }
        public DateTime ZDTHTRQ { get; set; }
        public string ZDTHTY { get; set; }
        public DateTime ZDTSHRQ { get; set; }
        public string ZDTSHY { get; set; }
    }
    public class XZDM
    {
        public XZDM()
        {
            ZDT = new ZDT();
        }
        public ZDT ZDT { get; set; }
        public virtual int? OBJECTID { get; set; }
        public virtual string DJZQDM { get; set; }
        public virtual string DJZDQMC { get; set; }
        public virtual string Zu { get; set; }

        private  string cunzu;
        public virtual string CunZu
        {
            get
            {
                if (Utils.IsStrNull(cunzu))
                {
                    return Cun;
                }
                return cunzu;
            }
            set { cunzu = value; }
        }

        public virtual string Cun { get; set; }
        public virtual string XiangZheng { get; set; }
        public virtual string Shi { get; set; }


        /// <summary>
        /// 集体所有权编号
        /// </summary>
        public string JTSYQ_BH { get; set; }
        public DateTime JTSYQ_GSStartTime { get; set; }
        public DateTime JTSYQ_GSEndTime { get; set; }
        
        public int JTSYQ_DasTatal { get; set; }
        public string JTSYQ_SHR { get; set; }
        public DateTime JTSYQ_SHRQ{ get; set; }
        public string JTSYQ_ZFLXDH { get; set; }
        public string JTSYQ_DCY { get; set; }
        public DateTime JTSYQ_DCRQ { get; set; }
       
        public string ZuZhang { get; set; }
        public string ZhiWu { get; set; }
        public string ZuZhangSFZ { get; set; }
        public string ZuZhangLXDH { get; set; }
        /// <summary>
        /// 所有的多部件
        /// </summary>
        public virtual IList<JTSYQ> JTSYQS { get; set; }
        public virtual JTSYQ JTSYQ { get; set; }

        public string ZuToatal { get; set; }
        
            
        public void SetZuMiaoShu()
        {
            IList<XZDM> xzdms = XZDMCustom.GetXzdms(DJZQDM);
            string str = "1";
            for (int a = 2; a < xzdms.Count; a++)
            {
                str = str + "、" + a;
            }
            ZuToatal = str;
        }
       

}
}
