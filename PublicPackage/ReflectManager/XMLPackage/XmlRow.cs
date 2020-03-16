using System.Collections.Generic;

namespace ReflectManager.XMLPackage
{
    public class XMLRow
    {
        public virtual int RowStartIndex { get; set; }
        public virtual int Rows { get; set; }
        public Dictionary<int, XMLObject> cellDic { get; set; }

        public virtual int Index { get; set; }
        public virtual int CellTotal { get; set; }
        public virtual IList<XMLGroup> XmlGroups { get; set; }
        public virtual int RowStep { get; set; }
        public virtual int PageCount { get; set; }
        public virtual XMLTable XmlTable { get; set; }
    }
}
