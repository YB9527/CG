using FirstFloor.ModernUI.Presentation;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.Pages.ZJDPage
{
    public class ZJDExportDataViewModel:NotifyPropertyChanged
    {
        public static string RedisKey = "ZJDExportDataViewModel";
        private bool isDangAnDai;

        public ZJDExportDataViewModel()
        {
            
        }
        private string saveDir;
        /// <summary>
        /// 保存文件夹
        /// </summary>
        public string SaveDir
        {
            get { return saveDir; }
            set
            {
                if (saveDir != value)
                {
                    saveDir = value;
                    OnPropertyChanged("saveDir");
                }
            }
        }
        /// <summary>
        /// 档案袋
        /// </summary>
        public bool IsDangAnDai
        {
            get { return isDangAnDai; }
            set
            {
                if(isDangAnDai != value)
                {
                    isDangAnDai = value;
                    OnPropertyChanged("isDangAnDai");
                }
            }
        }
        private bool isSPB;
        /// <summary>
        /// 审批表
        /// </summary>
        public bool IsSPB
        {
            get { return isSPB; }
            set
            {
                if (isSPB != value)
                {
                    isSPB = value;
                    OnPropertyChanged("isSPB");
                }
            }
        }
        private bool isWTS;
        /// <summary>
        /// 委托书
        /// </summary>
        public bool IsWTS
        {
            get { return isWTS; }
            set
            {
                if (isWTS != value)
                {
                    isWTS = value;
                    OnPropertyChanged("isWTS");
                }
            }
        }

        private bool isSMS;
        /// <summary>
        /// 声明书
        /// </summary>
        public bool IsSMS
        {
            get { return isSMS; }
            set
            {
                if (isSMS != value)
                {
                    isSMS = value;
                    OnPropertyChanged("isSMS");
                }
            }
        }

        private bool isQJDCB;
        /// <summary>
        /// 权籍调查表
        /// </summary>
        public bool IsQJDCB
        {
            get { return isQJDCB; }
            set
            {
                if (isQJDCB != value)
                {
                    isQJDCB = value;
                    OnPropertyChanged("isQJDCB");
                }
            }
        }
        private bool isCHBG;
        /// <summary>
        /// 测绘报告
        /// </summary>
        public bool IsCHBG
        {
            get { return isCHBG; }
            set
            {
                if (isCHBG != value)
                {
                    isCHBG = value;
                    OnPropertyChanged("isCHBG");
                }
            }
        }
        private bool _isAll;
        public bool _IsAll
        {
            get { return _isAll; }
            set
            {
                ReflectUtils.UnFlag(this,"Is", value);
                _isAll = value;
               OnPropertyChanged("_isAll");
            }
        }
    }
}
