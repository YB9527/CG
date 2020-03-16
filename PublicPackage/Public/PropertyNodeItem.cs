using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Public
{
    public class PropertyNodeItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private bool isexpanded;
        public bool IsExpanded
        {
            get { return isexpanded; }
            set
            {
                isexpanded = value;
            }
        }
        private bool? isSelected = false;
       
        
        public bool? IsSelected
        {
            set
            {
                isSelected = value;
               
                OnPropertyChanged("isSelected");
            }
            get { return this.isSelected; }
        }
        public PropertyNodeItem()
        {
            Children = new List<PropertyNodeItem>();

        }
        public string Icon { get; set; }
        private string displayname;
        public string DisplayName
        {
            get
            {
                return displayname;
            }
            set
            {
                displayname = value;
            }
        }
        public string Name { get; set; }
        private List<PropertyNodeItem> children;
        /// <summary>
        /// 只查儿子
        /// </summary>
        public List<PropertyNodeItem> Children
        {
            get { return children; }
            set
            {
               
                children = value;
                OnPropertyChanged("children");
            }
        }
        private PropertyNodeItem parent;
        public PropertyNodeItem Parent
        {
            set
            {
                parent = value;
                parent.AddChild(this);
                OnPropertyChanged("parent");
            }
            get { return this.parent; }
        }
        public void AddChild(PropertyNodeItem item)
        {
            Children.Add(item);
        }
        public object data { get; set; }
        /// <summary>
        /// 查询对象的所有儿子
        /// </summary>
        /// <returns></returns>
        public IList<PropertyNodeItem> FindChildAll()
        {
            List<PropertyNodeItem> list = new List<PropertyNodeItem>();
            FindChild(this, list);
            return list;
        }

        private void FindChild(PropertyNodeItem lb, List<PropertyNodeItem> list)
        {
            IList<PropertyNodeItem> childs = lb.Children;

            list.Add(lb);
            foreach (PropertyNodeItem child in childs)
            {

                FindChild(child, list);
            }

        }
        /// <summary>
        /// 设置全部的item 的 是否选择
        /// </summary>
        /// <param name="isSelected"></param>
        public void SetIsSelectAll( bool isSelected)
        {
            IList<PropertyNodeItem> childs = this.FindChildAll();
            
            foreach(PropertyNodeItem child in childs)
            {
                child.IsSelected = isSelected;
                if(isSelected)
                {
                    child.IsExpanded = true;
                }
            }
        }



        /// <summary>
        /// 得到选中的选项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<PropertyNodeItem> FindSelect(ItemCollection list)
        {
            IList<PropertyNodeItem> selectList = new List<PropertyNodeItem>();
            bool? isSelect;
            foreach (PropertyNodeItem item in list)
            {
                IList<PropertyNodeItem> all = item.FindChildAll();
                foreach (PropertyNodeItem child in all)
                {
                    isSelect = child.IsSelected;
                    if (isSelect != null && isSelect.Value)
                    {
                        selectList.Add(child);
                    }
                }
            }
            return selectList;
        }
    }
}
