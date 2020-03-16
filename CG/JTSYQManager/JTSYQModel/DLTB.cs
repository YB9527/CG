using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSYQManager.JTSYQModel
{
    public class DLTB
    {
        public IFeature Feature { get; set; }
        public int OBJECTID { get; set; }
        public string DLBM { get; set; }
        public string DLMC { get; set; }
    }
}
