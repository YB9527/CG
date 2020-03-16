using MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTemplate.Views;

namespace ExcelManager.Pages.Replace
{
    public class ReplaceCustom
    {
        private static Dictionary<StrRelation, string> relationDic;
        public static Dictionary<StrRelation, string> RelationDic
        {
            get
            {
                if(relationDic == null)
                {
                    relationDic.Add(StrRelation.StartsWith,"开始");
                    relationDic.Add(StrRelation.Contains, "包含");
                    relationDic.Add(StrRelation.EndsWith, "结束");
                    relationDic.Add(StrRelation.Equals, "相等");
                }
                return relationDic;
            }
        }

        public static IList<FieldCustom> GetFieldCustoms()
        {

            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "准备替换的文字", Name = "OldText", Width = 150 ,Editable=true});
            list.Add(new FieldCustom { AliasName = "替换后文字", Name = "NewText", Width = 150, Editable = true });
            list.Add(new ComBoxFieldCustom {DisplayMember= "DisplayName",ValueMember= "StrRelation", Items =StrRelationObj.GetStrRelationObjs(), AliasName = "替换内容关系", Name = "StrRelation", Width = 150, Editable = true });
            return list;
        }
        /// <summary>
        /// word文档替换使用
        /// </summary>
        /// <returns></returns>
        public static IList<FieldCustom> GetWordReplaceFieldCustoms()
        {

            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "准备替换的文字", Name = "OldText", Width = 150, Editable = true });
            list.Add(new FieldCustom { AliasName = "替换后文字", Name = "NewText", Width = 150, Editable = true });
            return list;
        }
    }
}
