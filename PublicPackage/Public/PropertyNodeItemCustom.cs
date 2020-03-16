using HeibernateManager.HibernateDao;
using HeibernateManager.Model;
using MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Public
{
    public class PropertyNodeItemCustom
    {
        public void SaveItem()
        {
            
        }
        /// <summary>
        ///选中的行政代码
        /// </summary>
        /// <param name="items"></param>
        public static void SaveDMToHibernate(IList<PropertyNodeItem> items)
        {
            IList<string> expanded = new List<string>();
            IList<string> selected = new List<string>();
            foreach (PropertyNodeItem item in items)
            {
                foreach (PropertyNodeItem child in item.FindChildAll())
                {
                    if (child.IsExpanded)
                    {
                        expanded.Add(child.Name);
                    }
                    if (child.IsSelected != null && child.IsSelected.Value)
                    {
                        selected.Add(child.Name);
                    }
                }
            }
            HibernateUtils hibernate = HibernateUtils.GetInstance();
            string ex = Utils.ListToString(expanded, "、");
            hibernate.SaveEntity(new SoftwareConfig("xzdmExpanded", ex));

            string sel = Utils.ListToString(selected, "、");
            hibernate.SaveEntity(new SoftwareConfig("xzdmSelected", sel));
        }
        public static string[][] GetHibernateDM()
        {
            HibernateUtils hibernate = HibernateUtils.GetInstance();
            string[][] array = new string[2][];
            SoftwareConfig expanded = hibernate.FindEntityById<SoftwareConfig>("xzdmExpanded");
            SoftwareConfig selected = hibernate.FindEntityById<SoftwareConfig>("xzdmSelected");
            if(expanded == null)
            {
                array[0] = new string[0];
            }else
            {
                array[0] = expanded.Value.Split('、');
                //删除数据库中的，因为每次关闭软件会重新保存
                hibernate.DeleteEntity(expanded);
            }
            if(selected == null)
            {
                array[1] = new string[0];
            }else
            {
                array[1] = selected.Value.Split('、');
                hibernate.DeleteEntity(selected);
            }
            
            return array;

        }

    }
}
