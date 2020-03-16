using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeibernateManager.Model
{
    public class Setting
    {
        public Setting()
        {
            this.ThemeIndex = 0;
            this.FontSizesIndex = 0;
            this.AccentColorsIndex = 0;
            this.PalettesIndex = 0;
        }
        public virtual Int64? OBJECTID { get; set; }
        public virtual int ThemeIndex { get; set; }
        public virtual int FontSizesIndex { get; set; }
        public virtual int AccentColorsIndex { get; set; }
        public virtual int PalettesIndex { get; set; }


    }
}
