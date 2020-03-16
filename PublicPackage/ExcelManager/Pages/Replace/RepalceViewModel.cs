using FirstFloor.ModernUI.Presentation;
using MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTemplate.Views;

namespace ExcelManager.Pages.Replace
{
  
    public class ReplaceViewModel: NotifyPropertyChanged
    {

        private StrRelation strRelation;
        public StrRelation StrRelation
        {
            get { return this.strRelation; }
            set
            {
                this.strRelation = value;
                OnPropertyChanged("strRelation");
            }
        }
        private string oldText;
        /// <summary>
        /// 以前的文字
        /// </summary>
        public string OldText
        {
            get { return this.oldText; }
            set
            {
                this.oldText = value;
                OnPropertyChanged("oldText");
            }
        }
        private string newText;
        /// <summary>
        /// 现在文字
        /// </summary>
        public string NewText
        {
            get { return this.newText; }
            set
            {
                this.newText = value;
                OnPropertyChanged("newText");
            }
        }

    }
}
