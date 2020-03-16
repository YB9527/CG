using ExcelManager.Pages.Replace;
using FileManager.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordManager.Pages.Repalce
{
    class WordReplacePageViewModel: AddFileViewModel
    {
        public static string RedisKey = "WordReplacePageViewModel";
        private IList<ReplaceViewModel> replaceViewModels;
        public IList<ReplaceViewModel> ReplaceViewModels
        {
            get
            {
                if (replaceViewModels == null)
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
