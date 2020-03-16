using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using MyUtils;

namespace ArcGisManager
{
   public class ElementCustom
    {
       public IElement TCElement { get; set; }
       public ElementCustom(IGraphicsContainer graphicsContainer, IList<IElement> Elements)
       {
           
           string name;
           this.GraphicsContainer = graphicsContainer;
           this.Elements = Elements;
           IList<IElement> list;
           ElementNameDic = new Dictionary<string, IList<IElement>>();
           foreach (IElement element in Elements)
           {
                IElementProperties pElmentProperties = element as IElementProperties;
                if (pElmentProperties != null)
                {

                    name = pElmentProperties.Name;
                    if (!Utils.IsStrNull(name))
                    {
                        if (name.Equals("图层"))
                        {
                            TCElement = element;
                            continue;
                        }
                        if (ElementNameDic.TryGetValue(name, out list))
                        {
                            list.Add(element);
                        }
                        else
                        {
                            list = new List<IElement>();
                            list.Add(element);
                            ElementNameDic.Add(name, list);
                        }
                    }
                }
           }
           
       }
       public IGraphicsContainer GraphicsContainer { get; set; }
       public IList<IElement> Elements { get; set; }
       public Dictionary<string, IList<IElement>> ElementNameDic { get; set; }
      
       
    }
}
