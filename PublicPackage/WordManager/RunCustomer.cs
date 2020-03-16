using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.XWPF.UserModel;

namespace WordManager
{
   public class RunCustomer
    {
        
        public virtual XWPFTableCell Cell { get; set; }
        public virtual XWPFRun Run { get; set; }
    }
}
