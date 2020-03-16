using System.Collections.Generic;

namespace ReflectManager.XMLPackage
{
    public class XMLTable
    {
        public XMLTable()
        {
            this.Index = 0;
        }

       public virtual int TableCount { get; set; }
       public virtual int TableIndex { get; set; }
       public virtual int RowStartIndex { get; set; }
        public virtual int RowEndIndex { get; set; }
        public virtual int Rows { get; set; }
        public virtual int ToRow { get; set; }
        public Dictionary<int,XMLObject>  CellDic{ get; set; }

       public virtual int Index { get; set; }
       public virtual int CellTotal { get; set; }
       public virtual IList<XMLGroup> XmlGroups { get; set; }	
       
        public virtual IList<XMLRow> XmlRows { get; set; }
    }
}
