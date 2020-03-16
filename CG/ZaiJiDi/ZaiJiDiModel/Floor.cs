using FirstFloor.ModernUI.Presentation;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class Floor:NotifyPropertyChanged
    {
        public Floor Clone()
        {
            return this.MemberwiseClone() as Floor;
        }
        public virtual string ZDNUM { get; set; }
        private int szc;       
        public int SZC
        {
            get
            {
                return szc;
            }
            set
            {
                if (szc != value)
                {
                    
                    szc = value;
                    OnPropertyChanged("szc");
                }
            }
        }
      
        private int zcs;
        private static bool zcsUpdateFlag = true;
        public int ZCS
        {
            get
            {
                return zcs;
            }
            set
            {
                if (zcs != value)
                {
                    if (zcsUpdateFlag)
                    {

                        if (JSYD != null)
                        {
                            zcsUpdateFlag = false;
                            foreach (Floor floor in JSYD.Floors)
                            {
                                floor.ZCS = value;
                            }
                            zcsUpdateFlag = true;
                        }

                    }
                    zcs = value;
                    OnPropertyChanged("zcs");
                }
            }
        }
        private double cjzmj;
        public double CJZMJ
        {
            get
            {
                return cjzmj;
            }
            set
            {
                if (cjzmj != value)
                {

                    cjzmj = value;
                    OnPropertyChanged("cjzmj");
                }
            }
        }
        public virtual int DH { get; set; }
        public JSYD JSYD { get; set; }
        public virtual IRow Row { get; set; }
    }
}
