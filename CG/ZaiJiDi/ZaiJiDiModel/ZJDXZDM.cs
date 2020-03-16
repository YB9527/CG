using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class ZJDXZDM
    {
        public virtual IRow Row { get; set; }
        public string DJZQDM { get; set; }
        public string DJZQMC { get; set; }
        public DateTime DCRQ { get; set; }
        public string DCY { get; set; }
        public DateTime SHRQ { get; set; }
        public string SHY { get; set; }
        public DateTime ZTRQ { get; set; }
        public string ZTY { get; set; }
        public DateTime ZTSHRQ { get; set; }
        public string ZTSHY { get; set; }
        public string ZJR { get; set; }
        public string TBRQ { get; set; }

        public string XiangZheng { get; set; }
        public string Cun { get; set; }

        public string Zu { get; set; }
    }
}
