using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public
{
   public class DictoryCustom: NotifyPropertyChanged
    {
        private Dictionary<string, Object> dic;
        public Dictionary<string, Object> Dic
        {
            get { return this.dic; }
            set
            {
                this.dic = value;
                OnPropertyChanged("dic");
            }

        }        
    }
}
