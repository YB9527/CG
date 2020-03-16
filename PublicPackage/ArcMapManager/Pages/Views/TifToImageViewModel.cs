using FileManager.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcMapManager.Pages.Views
{
    public class TifToImageViewModel:AddFileViewModel
    {
        public static string RedisKey = "TifToImageViewModel";
        private string saveDir;
        public string SaveDir
        {
            get
            {
                return saveDir;
            }
            set
            {
                if(saveDir != value)
                {
                    saveDir = value;
                    OnPropertyChanged("saveDir");
                }
            }
        }
        public TifToImageViewModel()
        {
            Fileter = "Tif|*.tif";
        }

        public override string Fileter { get; set; }
    }
}
