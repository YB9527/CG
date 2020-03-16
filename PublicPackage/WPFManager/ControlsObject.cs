using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFManager
{
   public class ControlsObject
    {
        public string Disname { get; set; }
        public object DataContext { get; set; }

        public static IList<ControlsObject> DicToControlsObject(Dictionary<string, string> dic)
        {
            IList<ControlsObject> list = new List<ControlsObject>();
            if(dic != null)
            {
                foreach(string text in dic.Keys)
                {
                    ControlsObject obj = new ControlsObject();
                    obj.Disname = text;
                    obj.DataContext = dic[text];
                    list.Add(obj);
                }
            }
            return list;
        }
    }
}
