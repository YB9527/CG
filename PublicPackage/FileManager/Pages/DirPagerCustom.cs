using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTemplate.Views;

namespace FileManager.Pages
{
    public class DirPagerCustom:NotifyPropertyChanged
    {
        public DirPagerCustom()
        {

        }
        public DirPagerCustom(string dir)
        {
            DirName = Path.GetFileNameWithoutExtension(dir);
            this.Dir = dir;
        }
        private string dirName;
        public string DirName
        {
            get { return dirName; }
            set
            {
                dirName = value;
                OnPropertyChanged(dirName);
            }
        }


        private string dir;
        public string Dir
        {
            get
            {
                if (dir == null)
                {
                    dir = "";
                }
               
                return dir;
            }
            set
            {
                 dir = value;

                OnPropertyChanged(dir);
            }
        }

        public static IList<FieldCustom> GetFieldCustoms()
        {
            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "文件夹名", Name = "DirName", Width = 200 ,Editable=false});
            list.Add(new FieldCustom { AliasName = "全路径", Name = "Dir", Width = 300, Editable = true });
            return list;
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


            return this.Dir.Equals(((DirPagerCustom)obj).Dir);

        }

        //重写GetHashCode方法（重写Equals方法必须重写GetHashCode方法，否则发生警告

        public override int GetHashCode()
        {
            return this.Dir.GetHashCode();
        }
    }
}
