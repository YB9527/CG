using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSYQManager.JTSYQModel
{
    public class ZuDangAnDai
    {
        public ZuDangAnDai()
        {
            ZJZMS = new List<string>();
        }
        public IList<string> ZJZMS { get; set; }
        public string FRDBSFZM { get; set; }
        public string FRDBSQS { get; set; }
        public string SPB { get; set; }
        public string TDQSLYZM { get; set; }
        public string ZDAD { get; set; }
        public string DZB { get; set; }
        public string JZDCGB { get; set; }
        public string QJDCB { get; set; }
        public string ZDTPDF { get; set; }
    }
}
