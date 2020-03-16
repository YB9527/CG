using FileManager.Pages;
using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.Pages.ZJDPage.DataSource
{
    public class ZJDDataSourceViewModel : NotifyPropertyChanged
    {
        public static string RedisKey = "ZJDDataSourceViewModel";


        private string dwgPath;
        /// <summary>
        /// 地籍图
        /// </summary>
        public string DWGPath
        {
            get { return dwgPath; }
            set
            {
                if (dwgPath != value)
                {
                    dwgPath = value;
                    OnPropertyChanged("dwgPath");
                }
            }
        }

        private string qz_BSMDBPath;
        /// <summary>
        /// 两表模板
        /// </summary>
        public string QZ_BSMDBPath
        {
            get { return qz_BSMDBPath; }
            set
            {
                if (qz_BSMDBPath != value)
                {
                    qz_BSMDBPath = value;
                    OnPropertyChanged("qz_BSMDBPath");
                }
            }
        }
        private string zdinfoMDBPath;
        /// <summary>
        /// 宗地图导出的MDB
        /// </summary>
        public string ZdinfoMDBPath
        {
            get { return zdinfoMDBPath; }
            set
            {
                if (zdinfoMDBPath != value)
                {
                    zdinfoMDBPath = value;
                    OnPropertyChanged("zdinfoMDBPath");
                }
            }
        }
        private string zjdXZDMTablePath;
        /// <summary>
        /// 建设用地表路径
        /// </summary>
        public string ZJDXZDMTablePath
        {
            get { return zjdXZDMTablePath; }
            set
            {
                if (zjdXZDMTablePath != value)
                {
                    zjdXZDMTablePath = value;
                    OnPropertyChanged("zjdXZDMTablePath");
                }
            }
        }

        private string jtcyTablePath;
        /// <summary>
        /// 家庭成员表路径
        /// </summary>
        public string JTCYTablePath
        {
            get { return jtcyTablePath; }
            set
            {
                if (jtcyTablePath != value)
                {
                    jtcyTablePath = value;
                    OnPropertyChanged("jtcyTablePath");
                }
            }
        }
        private int jtcyTableErrorCellIndex;
        public int JTCYTableErrorCellIndex
        {
            get { return jtcyTableErrorCellIndex; }
            set
            {
                if (jtcyTableErrorCellIndex != value)
                {
                    jtcyTableErrorCellIndex = value;
                    OnPropertyChanged("jtcyTableErrorCellIndex");
                }
            }
        }
        private string jsydTablePath;
        /// <summary>
        /// 建设用地表路径
        /// </summary>
        public string JSYDTablePath
        {
            get { return jsydTablePath; }
            set
            {
                if (jsydTablePath != value)
                {
                    jsydTablePath = value;
                    OnPropertyChanged("jsydTablePath");
                }
            }
        }
        private int jsydTableErrorCellIndex;
        public int JSYDTableErrorCellIndex
        {
            get { return jsydTableErrorCellIndex; }
            set
            {
                if (jsydTableErrorCellIndex != value)
                {
                    jsydTableErrorCellIndex = value;
                    OnPropertyChanged("jsydTableErrorCellIndex");
                }
            }
        }
        private string nfTablePath;
        /// <summary>
        /// 农房表路径
        /// </summary>
        public string NFTablePath
        {
            get { return nfTablePath; }
            set
            {
                if (nfTablePath != value)
                {
                    nfTablePath = value;
                    OnPropertyChanged("nfTablePath");
                }
            }
        }
        private int nfTableErrorCellIndex;
        public int NFTableErrorCellIndex
        {
            get { return nfTableErrorCellIndex; }
            set
            {
                if (nfTableErrorCellIndex != value)
                {
                    nfTableErrorCellIndex = value;
                    OnPropertyChanged("nfTableErrorCellIndex");
                }
            }
        }
        private int floorTableErrorCellIndex;
        public int FloorTableErrorCellIndex
        {
            get { return floorTableErrorCellIndex; }
            set
            {
                if (floorTableErrorCellIndex != value)
                {
                    floorTableErrorCellIndex = value;
                    OnPropertyChanged("floorTableErrorCellIndex");
                }
            }
        }
        private IList<string> errosrs;
        public IList<string> Errors
        {
            get { return errosrs; }
            set
            {
                errosrs = value;
                OnPropertyChanged("errosrs"); 
            }
        }

        private IList<DirPagerCustom> dirPagerCustoms;
        public IList<DirPagerCustom> DirPagerCustoms
        {
            get
            {
                if(dirPagerCustoms == null)
                {
                    dirPagerCustoms = new List<DirPagerCustom>();
                }
                for(int a =0; a < dirPagerCustoms.Count;a++)
                {
                    if(!MyUtils.Utils.CheckDirExists( dirPagerCustoms[a].Dir))
                    {
                        dirPagerCustoms.RemoveAt(a);
                        a--;
                    }
                }
                return dirPagerCustoms;
            }
            set
            {
                dirPagerCustoms = value;
                OnPropertyChanged("dirPagerCustoms");
            }
        }
     
    }
}
