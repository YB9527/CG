using FileManager;
using FirstFloor.ModernUI.Presentation;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTemplate.Views;

namespace FileManager
{
    public class FileNameCustom : NotifyPropertyChanged
    {
        public FileNameCustom()
        {

        }
        public FileNameCustom(string filePath)
        {
            this.filePath = filePath;
            this.dir = Path.GetDirectoryName(filePath);
            this.extension = Path.GetExtension(filePath);
            this.name = Path.GetFileNameWithoutExtension(filePath);

        }
        public static IList<FieldCustom> GetFieldCustoms()
        {
            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "执行成功", Name = "IsSuccess", Width = 80 });
            list.Add(new FieldCustom { AliasName = "文件名", Name = "Name" ,Width=200});
            list.Add(new FieldCustom { AliasName = "扩展名", Name = "Extension", Width = 80 });
            list.Add(new FieldCustom { AliasName = "文件夹", Name = "Dir", Width = 300 });
            return list;
        }
        private bool isChecked;
        public virtual bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged("isChecked");
            }
        }
        private bool isSuccess;
        /// <summary>
        /// 是否导入成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return isSuccess;
            }
            set
            {
                isSuccess = value;
                OnPropertyChanged("isSuccess");
            }
        }
        private string extension;
        public string Extension
        {
            get { return this.extension; }
            set
            {

                if (this.extension != value)
                {
                    this.extension = value;
                    OnPropertyChanged("extension");
                }
            }
        }
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                
                if (this.name != value)
                {
                    this.name = value;
                    OnPropertyChanged("name");
                }
            }
        }

        private string filePath;
        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    OnPropertyChanged("filePath");
                }
            }
        }
        public string dir;
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
        //重写Equals方法

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if ((obj.GetType().Equals(this.GetType())) == false)
            {
                return false;
            }
          

            return this.filePath.Equals(((FileNameCustom)obj).filePath) ;

        }

        //重写GetHashCode方法（重写Equals方法必须重写GetHashCode方法，否则发生警告

        public override int GetHashCode()
        {
            return this.filePath.GetHashCode() ;
        }

    }
}
