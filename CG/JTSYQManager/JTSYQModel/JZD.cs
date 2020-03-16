using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace JTSYQManager.JTSYQModel
{
    public class JZD
    {

            public virtual int? OBJECTID { get; set; }
            public virtual int JTSYQOBJECTID { get; set; }
        private string zdnum;
        public virtual string ZDNUM
        {
            get
            {
                if(zdnum == null)
                {
                    zdnum = "";
                }
                return zdnum;
            }
            set
            {
                zdnum = value;
            }
        }
            public virtual int JZDH { get; set; }
            public virtual IFeature Feature { get; set; }
            public virtual IPoint Point { get; set; }
            public  virtual int JZDIndex { get; set; }
    }
}
