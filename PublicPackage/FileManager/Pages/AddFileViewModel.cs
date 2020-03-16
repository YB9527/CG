
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Pages
{
    public class AddFileViewModel:NotifyPropertyChanged
    {
       public AddFileViewModel()
        {
            this.Files = new ObservableCollection<FileNameCustom>();
        }


        public virtual string Fileter { get; set; }
        private IList<FileNameCustom> files;
        public IList<FileNameCustom> Files
        {
            get
            {
         
                return files;
            }
            set
            {
                files = value;
                OnPropertyChanged("files");
            }
        }
        private bool isDiGuiChaZhao;
        public bool IsDiGuiChaZhao
        {
            get
            {
                return isDiGuiChaZhao;
            }
            set
            {
                if (isDiGuiChaZhao != value)
                {
                    isDiGuiChaZhao = value;
                    OnPropertyChanged("isDiGuiChaZhao");
                }
            }
        }
        private string dir;
       public string Dir
        {
            get
            {
                return dir;
            }
            set
            {
                if(dir != value)
                {
                    dir = value;
                    OnPropertyChanged("dir");
                }
            }
        }

    }
}
