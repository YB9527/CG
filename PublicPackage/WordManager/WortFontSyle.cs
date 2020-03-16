using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.XWPF.UserModel;

namespace WordManager
{
    public class WordFontSyle
    {
        public virtual bool AddBreak { get; set; }
        public virtual UnderlinePatterns Underline { get; set; }
        public virtual String Color { get; set; }
        public virtual bool Bold { get; set; }
        public virtual int FontSize { get; set; }
        public virtual String FontFamily { get; set; }
       
    }
}
