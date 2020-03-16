using FileManager.Pages;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelManager.Pages.Replace
{
    public class RepalcePageViewModel:AddFileViewModel
    {
        public static string RedisKey = "ExcelRepalcePageViewModel";
       
        private bool allSheet;
        public bool AllSheet
        {
            get
            {
                return allSheet;
            }
            set
            {
                allSheet = value;
                OnPropertyChanged("allSheet");
            }
        }

        private int replaceSheetIndex;
        public int ReplaceSheetIndex
        {
            get
            {
                return replaceSheetIndex;
            }
            set
            {
                replaceSheetIndex = value;
                OnPropertyChanged("replaceSheetIndex");
            }
        }

        private IList<ReplaceViewModel> replaceViewModels;
        public IList<ReplaceViewModel> ReplaceViewModels
        {
            get
            {
                if(replaceViewModels == null)
                {
                    replaceViewModels = new ObservableCollection<ReplaceViewModel>();
                }
                return replaceViewModels;
            }
            set
            {
                replaceViewModels = value;
                OnPropertyChanged("replaceViewModels");
            }
        }

       

    }
}
