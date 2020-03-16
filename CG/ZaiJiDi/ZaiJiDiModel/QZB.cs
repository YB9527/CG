using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class QZB:NotifyPropertyChanged

    {
        public QZB Clone()
        {
            return this.MemberwiseClone() as QZB;
        }
        public virtual int? Objectid { get; set; }
        private string bzdh;
        public virtual String BZDH
        {
            get
            {
                return bzdh;
            }
            set
            {
                bzdh = value;
                OnPropertyChanged("bzdh");
            }
        }
        private string lzdh;
        public virtual String LZDH
        {
            get
            {
                return lzdh;
            }
            set
            {
                lzdh = value;
                OnPropertyChanged("lzdh");
            }
        }
       
        private string qdh;
        public virtual String QDH
        {
            get
            {
                return qdh;
            }
            set
            {
                qdh = value;
                OnPropertyChanged("qdh");
            }
        }
        private string zdh;
        public virtual String ZDH
        {
            get
            {
                return zdh;
            }
            set
            {
                zdh = value;
                OnPropertyChanged("zdh");
            }
        }
        private string lzdzjr;
        public virtual String LZDZJR
        {
            get
            {
                return lzdzjr;
            }
            set
            {
                lzdzjr = value;
                OnPropertyChanged("lzdzjr");
            }
        }
        public virtual String Cbfbm { get; set; }
    }
}
