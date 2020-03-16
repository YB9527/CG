
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFTemplate.Views
{
    public class FieldCustom
    {
       public FieldCustom()
        {
            Width = 100;
            Mode = BindingMode.TwoWay;
        }
        public int Index { get; set; }
        public string AliasName { get; set; }
        public string Name { get; set; }
        public bool Editable { get; set; }
        public int Width { get; set; }
        public BindingMode Mode { get; set; }
    }
}
