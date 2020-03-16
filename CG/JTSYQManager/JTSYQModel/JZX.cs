using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JTSYQManager.JTSYQModel
{
    public class JZX
    {
        public IFeature Feature { get; set; }
        public string QLR { get; set; }
        public string BM { get; set; }
        public virtual int JTSYQOBJECTID { get; set; }
        public IPolyline Polyline { get; set; }
      
    }
}
