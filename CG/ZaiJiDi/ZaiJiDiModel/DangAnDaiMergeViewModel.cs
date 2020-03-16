
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class DangAnDaiMergeViewModel : NotifyPropertyChanged, IDataErrorInfo
    {
        public DangAnDaiMergeViewModel()
        {

        }
        private string saveDir = "";
        private string dangAnDaiDir = "";
        private string pdfDir = "";
        private string weiTuoShengMingDir = "";
        private string shenFenXinXiDir = "";
        private string pictureDir = "";
        private string yuanShiDangAnDaiDir = "";

        public string YuanShiDangAnDaiDir
        {
            get { return this.yuanShiDangAnDaiDir; }
            set
            {
                if (this.yuanShiDangAnDaiDir != value)
                {
                    this.yuanShiDangAnDaiDir = value;
                    OnPropertyChanged("yuanShiDangAnDaiDir");
                }
            }
        }
        public string SaveDir
        {
            get { return this.saveDir; }
            set
            {
                if (this.saveDir != value)
                {
                    this.saveDir = value;
                    OnPropertyChanged("saveDir");
                }
            }
        }
        public string DangAnDaiDir
        {
            get { return this.dangAnDaiDir; }
            set
            {
                if (this.dangAnDaiDir != value)
                {
                    this.dangAnDaiDir = value;
                    OnPropertyChanged("dangAnDaiDir");
                }
            }
        }
        public string PDFDir
        {
            get { return this.pdfDir; }
            set
            {
                if (this.pdfDir != value)
                {
                    this.pdfDir = value;
                    OnPropertyChanged("pdfDir");
                }
            }
        }
        public string WeiTuoShengMingDir
        {
            get { return this.weiTuoShengMingDir; }
            set
            {
                if (this.weiTuoShengMingDir != value)
                {
                    this.weiTuoShengMingDir = value;
                    OnPropertyChanged("weiTuoShengMingDir");
                }
            }
        }


        public string ShenFenXinXiDir
        {
            get { return this.shenFenXinXiDir; }
            set
            {
                if (this.shenFenXinXiDir != value)
                {
                    this.shenFenXinXiDir = value;
                    OnPropertyChanged("shenFenXinXiDir");
                }
            }
        }

        public string PictureDir
        {
            get { return this.pictureDir; }
            set
            {
                if (this.pictureDir != value)
                {
                    this.pictureDir = value;
                    OnPropertyChanged("pictureDir");
                }
            }
        }
        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {


                if (columnName == "SaveDir")
                {
                    Console.WriteLine(string.IsNullOrEmpty(this.saveDir) ? "必须要选择" : null);
                    return string.IsNullOrEmpty(this.saveDir) ? "必须要选择" : null;
                }

                return null;
            }
        }

    }
}
