using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeibernateManager.HibernateDao
{
    public class PropertPo
    {
        public PropertPo()
        {
            Values = new List<string>();
        }
        public IList<string> Values { get; set; }
        public string Forname { get; set; }
        public string methodName { get; set; }
        public string LogicalMethod { get; set; }
    }
}
