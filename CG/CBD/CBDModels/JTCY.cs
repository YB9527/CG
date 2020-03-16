using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBD.CBDModels
{
    public class JTCY
    {
        public string CBFBM { get; set; }
        public string CBFLX { get; set; }
        public string XM { get; set; }
        public string XB { get; set; }
        public string YHZGX { get; set; }
        public string ZJLX { get; set; }
        public string ZJH { get; set; }
        public string LXDH { get; set; }
        public string DZ { get; set; }
        public string YZBM { get; set; }
        public string YHTBH { get; set; }
        public string YDKS { get; set; }
        public double YHTMJ { get; set; }
        public string CBFS { get; set; }
        public string CBQXQ { get; set; }
        public string CBQXZ { get; set; }
        public string SFGYR { get; set; }
        public string BZ { get; set; }
        public string DCY { get; set; }
        public string DCJS { get; set; }
        public double DCSJ { get; set; }
        public string SHR { get; set; }
        public string SHYJ { get; set; }
        public double SHSJ { get; set; }
        public IList<JTCY> jTCies { get; set; }

        private int gyrCount;
        public int GYRCount
        {
            get
            {
                int count = 0;
                if(jTCies != null)
                {
                    foreach(JTCY jtcy in jTCies)
                    {
                        if(jtcy.SFGYR == "是" || jtcy.SFGYR == "1")
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
            set
            {

            }
        }
        public IList<DK> DKs { get; set; }

        public int DKCount
        {
            get
            {
                if(DKs != null)
                {
                    return DKs.Count;
                }
                else
                {
                    return 0;
                }

            }
            set
            {

            }
        }
        private double mjhj;
        public  double MJHJ
        {
            get
            {
                mjhj = 0;
                if(DKs == null)
                {
                    return 0;
                }
                else
                {
                    foreach(DK dk in DKs)
                    {
                        double area =Math.Round( double.Parse(dk.SCMJ),2);
                        mjhj = Math.Round(area + mjhj, 2);
                        
                    }
                    return mjhj;
                }
            }
            set
            {

            }
        }


        public static string[] AddPeopleKey = new string[]
        {
            "迁入","新生"
        };
        public static string[] ReducePeopleKey = new string[]
        {
            "死亡","迁出",
        };

        private string addPeopleCount;
        public string AddPeopleCount
        {
            get
            {
                int count = 0;
                if (jTCies != null)
                {
                    foreach (JTCY jtcy in jTCies)
                    {
                        string bz = jtcy.BZ;
                        if(!MyUtils.Utils.IsStrNull(bz))
                        {
                            foreach (string key in AddPeopleKey)
                            {
                                if (bz.Contains(key))
                                {
                                    count++;
                                }
                        }
                        }
                       
                        
                    }
                }
                if (count == 0)
                {
                    return "";
                }
                else
                {
                    return count + "";
                }
            }
            set
            {

            }
        }

        private string reducePeopleCount;
        public string ReducePeopleCount
        {
            get
            {
                int count = 0;
                if (jTCies != null)
                {
                    foreach (JTCY jtcy in jTCies)
                    {
                        string bz = jtcy.BZ;
                        if (!MyUtils.Utils.IsStrNull(bz))
                        {
                            foreach (string key in ReducePeopleKey)
                            {
                                if (bz.Contains(key))
                                {
                                    count++;
                                }
                            }
                        }


                    }
                }
                if(count ==0)
                {
                    return"";
                }else
                {
                    return count+"";
                }
               
            }
            set
            {

            }
        }
    }
}
