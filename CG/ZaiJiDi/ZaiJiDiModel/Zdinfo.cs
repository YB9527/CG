using MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class Zdinfo
    {
        public string ZDNUM { get; set; }
        public string QH { get; set; }
        public string QUANLI { get; set; }
        public string DILEI { get; set; }
        private  double area;
        public double AREA {
            get
            {
                return area;
            }

            set
            {
                area = Math.Round(value,2);
            }
        }
        public double zdmj { get; set; }
        public double ZDMJ
        {
            get
            {
                return zdmj;
            }

            set
            {
                zdmj = Math.Round(value, 2);
            }
        }
        public float JZMJ { get; set; }
        public string DWXZ { get; set; }
        public string QSXZ { get; set; }
        public string SYQLX { get; set; }
        public string TDDJ { get; set; }
        public string ZGBM { get; set; }
        public string FRDB { get; set; }
        public string FRSFZ { get; set; }
        public string FRDH { get; set; }
        public string DLR { get; set; }
        public string DLSFZ { get; set; }
        public string DLDH { get; set; }
        public string TUFU { get; set; }
        public string TXDZ { get; set; }
        public string TDZL { get; set; }
        public string EAST { get; set; }
        public string SOUTH { get; set; }
        public string WEST { get; set; }
        public string NORTH { get; set; }
        public string QSLY { get; set; }
        public string PZYT { get; set; }
        public string TDSYZ { get; set; }
        public string JZWQS { get; set; }
        public string YBH { get; set; }
        public string TDZH { get; set; }
        public string SHRQ { get; set; }
        public string DJRQ { get; set; }

       
        public string ZZRQ { get; set; }

        public static void RemoveHu(IList<Zdinfo> zdinfos)
        {
           
            foreach(Zdinfo zdinfo in zdinfos)
            {
                string quanli = zdinfo.QUANLI;
                if(!Utils.IsStrNull(quanli))
                {
                    /* index = quanli.IndexOf("(");
                    if(index == -1)
                    {
                        index = quanli.IndexOf("（");
                    }
                    if(index != -1)
                    {
                        zdinfo.QUANLI = quanli.Substring(0,index);
                    }*/
                    zdinfo.QUANLI = quanli.Replace("(户)", "").Replace("（户）", "").Replace("(户）", "").Replace("（户)", "");


                }
            }
        }

        public float BDDJ { get; set; }
        public float SBDJ { get; set; }
        public string MPH { get; set; }
        public float JZMD { get; set; }
        public float RJL { get; set; }


      

    }
}
