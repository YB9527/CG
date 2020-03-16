using ESRI.ArcGIS.Geodatabase;
using FirstFloor.ModernUI.Presentation;
using Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = System.Object;

namespace ArcGisManager
{
    public class MapTabDictoryCustom : DictoryCustom
    {
        public static Dictionary<string, object> GetTabDictory(IFeature feature)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            IFields fields = feature.Fields;
            for (int a =0; a <fields.FieldCount;a++)
            {
                dic.Add(fields.Field[a].AliasName, feature.Value[a]);
            }
            return dic;
        }
    }
}
