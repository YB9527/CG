using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSYQManager.JTSYQModel
{
    public class QZB
    {
        public QZB(int StartDH,int EndDH,string MS)
        {
            this.StartDH = StartDH;
            this.EndDH = EndDH;
            this.MS = MS;
        }
        public int StartDH { get; set; }
        public int EndDH { get; set; }
        public string MS { get; set; }
    }
}
