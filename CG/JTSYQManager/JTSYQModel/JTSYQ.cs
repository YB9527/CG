using ESRI.ArcGIS.Geodatabase;
using System.Collections.Generic;
using FirstFloor.ModernUI.Presentation;
using MyUtils;
using System;
using JTSYQManager.XZDMManager;
using ArcGisManager;

namespace JTSYQManager.JTSYQModel
{
    public class JTSYQ : NotifyPropertyChanged
    {

        public MapTabDictoryCustom MapTabDictoryCustom { get; set; }
        public XZDM XZDM { get; set; }

        
        public IList<JZX> JZXS { get; set; }
        public JZX SelfJZX { get; set; }
        public IFeature Feature { get; set; }
     
        public int OBJECTID { get; set; }

        public string ZhiBiao { get; set; }
        public string ShenHei { get; set; }
        /// <summary>
        /// 使用面积
        /// </summary>
        public double Area { get; set; }
        public string CZ { get; set; }
        public string QLR { get; set; }
        public string BM { get; set; }
        public string BZ { get; set; }

        private string tufu;
        public string TuFu
        {
            get { return this.tufu; }
            set
            {
                if (this.tufu != value)
                {
                    this.tufu = value;
                    OnPropertyChanged("tufu");
                }
            }
        }
        /// <summary>
        /// 图上面积
        /// </summary>
        public double Shape_Area { get; set; }
        private string szb;
        public IList<JZD> JZDS { get; set; }
        public int JZDCount { get; set; }
        public IList<QZB> QZBS { get; set; }
        public string SZB
        {
            get { return this.szb; }
            set
            {
                if (this.szb != value)
                {
                    this.szb = value;
                    OnPropertyChanged("szb");
                }
            }
        }
        private string szd;
        public string SZD

        {
            get { return this.szd; }
            set
            {
                if (this.szd != value)
                {
                    this.szd = value;
                    OnPropertyChanged("szd");
                }
            }
        }
        private string szn;
        public string SZN
        {
            get { return this.szn; }
            set
            {
                if (this.szn != value)
                {
                    this.szn = value;
                    OnPropertyChanged("szn");
                }
            }
        }


        private string szx;

        public string SZX
        {
            get { return this.szx; }
            set
            {
                if (this.szx != value)
                {
                    this.szx = value;
                    OnPropertyChanged("szx");
                }
            }
        }

        public IList<JTSYQ> GroupJTSYQ { get; set; }


        public double? ZDMJ { get; set; }
        public double? NYDMJ { get; set; }

        public double? GDMJ { get; set; }
        public double? LDMJ { get; set; }
        public double? CDMJ { get; set; }
        public double? YDMJ { get; set; }
        public double? QTNYDMJ { get; set; }
        public double? JSYDMJ { get; set; }
        public double? WLYDMJ { get; set; }

        public void SetNYDMJ()
        {

            NYDMJ = Math.Round(GDMJ.Value + LDMJ.Value + CDMJ.Value + YDMJ.Value + QTNYDMJ.Value, 4);
            /* double area = 0;
             foreach(JTSYQ jtsyq in GroupJTSYQ)
             {
                 area += jtsyq.Shape_Area;
                 Math.Round(area / 10000, 4)
             }*/
            double d = Area  - NYDMJ.Value - JSYDMJ.Value - WLYDMJ.Value;
            if (GDMJ != 0)
            {
                GDMJ = Math.Round(GDMJ.Value + d, 4);
            }
            else if (LDMJ != 0)
            {
                LDMJ = Math.Round(LDMJ.Value + d, 4);
            }
            else if (JSYDMJ != 0)
            {
                JSYDMJ = Math.Round(JSYDMJ.Value + d, 4);
            }
            NYDMJ = Math.Round(GDMJ.Value + LDMJ.Value + CDMJ.Value + YDMJ.Value + QTNYDMJ.Value, 4);
            //d = Decimal.Round(Area, 4) - NYDMJ.Value - JSYDMJ.Value - WLYDMJ.Value;
        }
       
    }

}
