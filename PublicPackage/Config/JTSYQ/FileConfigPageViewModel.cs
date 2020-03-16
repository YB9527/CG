
using FirstFloor.ModernUI.Presentation;
using HeibernateManager.Model;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.JTSYQ
{
    public class FileConfigPageViewModel : NotifyPropertyChanged
    {
        private static string FileConfigPageRides = "JTSYQFileConfig";

        public void SaveRedis()
        {
            string str = ReflectManager.JObejctReflect.ObjectToStr(this);
            SoftwareConfig.Refresh(FileConfigPageRides, str);
        }
        public static FileConfigPageViewModel GetRedis()
        {
            SoftwareConfig config = SoftwareConfig.FindConfig(FileConfigPageRides);
            if (config != null)
            {
                FileConfigPageViewModel model = JObejctReflect.ToObejct<FileConfigPageViewModel>(config.Value);
                if(!MyUtils.Utils.CheckFileExists(model.PhotoExcelPath))
                {
                    model.PhotoExcelPath = "";
                }
                
                return model;
            }
            else
            {
                return new FileConfigPageViewModel();
            }



        }
        private string xZDMExcelPath;
        public string XZDMExcelPath
        {
            get
            {
                return this.xZDMExcelPath;
            }
            set
            {
                if (this.xZDMExcelPath != value)
                {
                    this.xZDMExcelPath = value;
                    OnPropertyChanged("xZDMExcelPath");

                }
            }
        }

        private string photoExcelPath;
        public string PhotoExcelPath
        {
            get
            {
                return this.photoExcelPath;
            }
            set
            {
                if (this.photoExcelPath != value)
                {
                    this.photoExcelPath = value;
                    OnPropertyChanged("photoExcelPath");

                }
            }
        }
    }
}
