using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTemplate.Views
{
    public class ComBoxFieldCustom:FieldCustom
    {
        // ItemsSource = Users, DisplayMember = "Name",IsTextEditable = false , ValueMember = "StrRelation"
       public System.Collections.IList Items { get; set; }
       public string DisplayMember { get; set; }
       public string ValueMember { get; set; }
    }
}
