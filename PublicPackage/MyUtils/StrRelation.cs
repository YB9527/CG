using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtils
{
    public enum StrRelation
    {
        /// <summary>
        /// 开始
        /// </summary>
        StartsWith = 0,
        /// <summary>
        /// 末尾
        /// </summary>
        EndsWith = 1,
        /// <summary>
        /// 包含
        /// </summary>
        Contains = 2,
        /// <summary>
        /// 相等
        /// </summary>
        Equals = 3,

    }
    public class StrRelationObj
    {
        public StrRelation StrRelation { get; set; }
        public string DisplayName{get;set;}
        public StrRelationObj(StrRelation strRelation, string displayName)
        {
            StrRelation = strRelation;
            DisplayName = displayName;
        }
        /// <summary>
        ///等到所有字符串关系对象
        /// </summary>
        /// <returns></returns>
        public static System.Collections.IList GetStrRelationObjs()
        {
            System.Collections.IList list = new ObservableCollection<StrRelationObj>();
            list.Add(new StrRelationObj(StrRelation.StartsWith, "开始"));
            list.Add(new StrRelationObj(StrRelation.Contains, "包含"));
            list.Add(new StrRelationObj(StrRelation.EndsWith, "结束"));
            list.Add(new StrRelationObj(StrRelation.Equals, "相等"));
            return list;
        }
    }
}
