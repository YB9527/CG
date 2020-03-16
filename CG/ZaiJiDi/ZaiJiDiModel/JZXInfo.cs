using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class JZXInfo
    {
        public JZXInfo Clone()
        {
            return this.MemberwiseClone() as JZXInfo;
        }
        public virtual int? Objectid { get; set; }
        public virtual String BZDH { get; set; }
        public virtual String LZDH { get; set; }

        public virtual String QDH { get; set; }
        public virtual String ZDH { get; set; }

        private double tsbc;
        public virtual Double TSBC
        {
            get
            {
                return tsbc;
            }
            set
            {
                tsbc = Math.Round(value,2);
            }
        }


        public virtual Double KZBC { get; set; }

        public virtual String JXXZ { get; set; }

        public virtual String JZXLB { get; set; }

        public virtual String JZXWZ { get; set; }

        public virtual String BZDZJR { get; set; }

        public virtual String BZDZJRQ { get; set; }

        public virtual String LZDZJR { get; set; }
        public virtual String LZDZJRQ { get; set; }

        public virtual String QDZB { get; set; }

        public virtual String ZDZB { get; set; }

        public virtual String Cbfbm { get; set; }

    }
}
