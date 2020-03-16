using FileManager;
using FileManager.Pages;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ExcelManager.Pages
{

    public enum MergeSytle
    {
        /// <summary>
        /// 合并到一个 sheet
        /// </summary>
        OneSheet,
        /// <summary>
        /// 所有的sheet 放到 work
        /// </summary>
        MoreSheet
    }
    public class EnumToBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false : value.Equals(parameter);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(true) ? parameter : Binding.DoNothing;
        }


    }

    public class ExcelMergeModel : AddFileViewModel
    {
        public static string RedisKey = "ExcelMergeModel";

        public ExcelMergeModel()
        {

        }

        private MergeSytle mergeModel;

        public MergeSytle MergeModel
        {
            get
            {
                return this.mergeModel;
            }
            set
            {
                this.mergeModel = value;
                OnPropertyChanged("saveName");
            }
        }

        private string saveName;
        public string SaveName
        {
            get { return this.saveName; }
            set
            {

                if (this.saveName != value)
                {
                    this.saveName = value;
                    OnPropertyChanged("saveName");
                }
            }
        }
        private string dir;
        public string Dir
        {
            get { return this.dir; }
            set
            {

                if (this.dir != value)
                {
                    this.dir = value;
                    OnPropertyChanged("dir");
                }
            }
        }

        private int sheetIndex;
        public int SheetIndex
        {
            get { return this.sheetIndex; }
            set
            {
                if (this.sheetIndex != value)
                {
                    this.sheetIndex = value;
                    OnPropertyChanged("sheetIndex");
                }
            }
        }

        private int reduceStartRowCount;
        public int ReduceStartRowCount
        {
            get { return this.reduceStartRowCount; }
            set
            {
                if (this.reduceStartRowCount != value)
                {
                    this.reduceStartRowCount = value;
                    OnPropertyChanged("reduceStartRowCount");
                }
            }
        }

        private int reduceEndRowCount;
        public int ReduceEndRowCount
        {
            get { return this.reduceEndRowCount; }
            set
            {
                if (this.reduceEndRowCount != value)
                {
                    this.reduceEndRowCount = value;
                    OnPropertyChanged("reduceEndRowCount");
                }
            }
        }

        private IList<FileNameCustom> fileNameCustoms;
        public IList<FileNameCustom> FileNameCustoms
        {
            get
            {
                if (this.fileNameCustoms == null)
                {
                    this.fileNameCustoms = new ObservableCollection<FileNameCustom>();
                }
                return this.fileNameCustoms;
            }
            set
            {
                //必须重置一次，才能更新
                this.fileNameCustoms = value;
                OnPropertyChanged("fileNameCustoms");

            }
        }
        public void SetPaths(IList<FileNameCustom> paths)
        {
            this.fileNameCustoms = null;
            this.FileNameCustoms = paths;
        }
        private bool isAllPageMerge;
        /// <summary>
        /// 是否每页合并
        /// </summary>
        public bool IsAllPageMerge
        {
            get { return this.isAllPageMerge; }
            set
            {
                if (this.isAllPageMerge != value)
                {
                    this.isAllPageMerge = value;
                    OnPropertyChanged("isAllPageMerge");
                }
            }
        }
    }
}
