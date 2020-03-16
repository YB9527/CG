using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeibernateManager.HibernateDao
{
    public class LogcalRelation
    {
        public static Dictionary<string, string>  GetDic()
        {
            Dictionary<string, string> logicalDic = new Dictionary<string, string>();
            logicalDic.Add("并且", "And");
            logicalDic.Add("或者", "Or");
            return logicalDic;
        }
    }
}
