using FirstFloor.ModernUI.Presentation;
using JTSYQManager.XZDMManager;
using MyUtils;
using NPOI.XWPF.UserModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using WordManager;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class YiShiShengMingBuYiZhiZhengMing : NotifyPropertyChanged, IDataErrorInfo
    {

        public YiShiShengMingBuYiZhiZhengMing()
        {
            this.TuDiZhengGuanXi = "本人";
            this.FanChanZhengGuanXi = "本人";
            this.buYiZhiZhengMing = true;
        }
        private static string YiShiModelPath = "ChengDuGaoXingZaiJiDiModel/遗失声明模板.docx";
        private static string BuYiZhiModelPath = "ChengDuGaoXingZaiJiDiModel/不一致证明模板.docx";


        public YiShiShengMingBuYiZhiZhengMing(PersonZiLiao PersonZiLiao):this()
        {
            this.PersonZiLiao = PersonZiLiao;
        }

        private bool? yiShengMin = false;
        public bool? YiShengMing
        {
           
            set
            {
                this.yiShengMin = value;
                OnPropertyChanged("yiShengMin");
            }
            get { return this.yiShengMin; }
        }

        public XZDM XZDM { get; set; }

        private bool? buYiZhiZhengMing = false;
        
        public bool? BuYiZhiZhengMing
        {
            get { return this.buYiZhiZhengMing; }
            set
            {
                if (this.buYiZhiZhengMing != value)
                {
                    this.buYiZhiZhengMing = value;
                    OnPropertyChanged("yiShengMin");
                }
            }
        }

        private string peopleCount = "";
        public string PeopleCount
        {
            get { return this.peopleCount; }
            set
            {
                if (this.peopleCount != value)
                {
                    this.peopleCount = value;
                    OnPropertyChanged("peopleCount");
                }
            }
        }

        private string tuDiZheng;
        public string TuDiZheng
        {
            get { return this.tuDiZheng; }
            set
            {
                if (this.tuDiZheng != value)
                {
                    this.tuDiZheng = value;
                    OnPropertyChanged("tuDiZheng");
                }
            }
        }

        private string tuDiZhengYuanQuanLiRen = "";
        public string TuDiZhengYuanQuanLiRen
        {
            get { return this.tuDiZhengYuanQuanLiRen; }
            set
            {
                if (this.tuDiZhengYuanQuanLiRen != value)
                {
                    this.tuDiZhengYuanQuanLiRen = value;
                    OnPropertyChanged("tuDiZhengYuanQuanLiRen");
                }
            }
        }

        public  string tuDiZhengGuanXi="";
              public string TuDiZhengGuanXi
        {
            get { return this.tuDiZhengGuanXi; }
            set
            {
                if (this.tuDiZhengGuanXi != value)
                {
                    this.tuDiZhengGuanXi = value;
                    OnPropertyChanged("TuDiZhengGuanXi");
                }
            }
        }
        private string fanChanZheng = "";
        public string FanChanZheng
        {
            get { return this.fanChanZheng; }
            set
            {
                if (this.fanChanZheng != value)
                {
                    this.fanChanZheng = value;
                    OnPropertyChanged("fanChanZheng");
                }
            }
        }
        /// <summary>
        /// 创建遗失声明
        /// </summary>
        public void CreateYiShiShengMing()
        {
            if(this.YiShengMing.Value)
            {
                XWPFDocument doc = WordRead.Read(YiShiModelPath);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("[XM]", PersonZiLiao.Name);
                dic.Add("[XiangZheng]", XZDM.XiangZheng);
                dic.Add("[Cun]", XZDM.Cun);
                dic.Add("[Zu]", XZDM.Zu);

                if(!Utils.IsStrNull(TuDiZheng))
                {
                   
                    RunCustomer runCustomer = new RunCustomer();
                    runCustomer.Run = doc.Tables[0].Rows[1].GetCell(0).Paragraphs[0].Runs[0];
                    WordWrite.ReplaceSmpbol(runCustomer);

                    dic.Add("[TuDiZheng]", TuDiZheng);
                    dic.Add("[TuDiZhengYuanQuanLiRen]", TuDiZhengYuanQuanLiRen);
                    dic.Add("[TuDiZhengGuanXi]", TuDiZhengGuanXi);
                }
                if (!Utils.IsStrNull(FanChanZheng))
                {
                    RunCustomer runCustomer = new RunCustomer();
                    runCustomer.Run = doc.Tables[0].Rows[2].GetCell(0).Paragraphs[0].Runs[0];
                    WordWrite.ReplaceSmpbol(runCustomer);

                    dic.Add("[FanChanZheng]", FanChanZheng);
                    dic.Add("[FanChanZhengYuanQuanLiRen]", FanChanZhengYuanQuanLiRen);
                    dic.Add("[FanChanZhengGuanXi]", FanChanZhengGuanXi);
                }


                WordWrite.ReplaceText(doc, dic);
                string saveDir;
                if(Utils.IsStrNull(PersonZiLiao.ShengMingShu))
                {
                    saveDir = PersonZiLiao.DangAnDai.DirClass.DirName;
                }else
                {
                    saveDir = PersonZiLiao.ShengMingShu;
                }
               
                WordWrite.SaveToFile(doc,Path.GetDirectoryName( saveDir) + "\\遗失声明书.docx");
            }
            if (this.buYiZhiZhengMing.Value)
            {
                XWPFDocument doc = WordRead.Read(BuYiZhiModelPath);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("XM", PersonZiLiao.Name);
                dic.Add("RenKou", this.PeopleCount);
                bool flag = false;
                // 根据原权利人来
                //土地证
                if (!Utils.IsStrNull(TuDiZhengYuanQuanLiRen))
                {
                    flag = true;
                    if (TuDiZhengGuanXi.Contains("本人"))
                    {
                        //第一格
                        RunCustomer runCustomer = new RunCustomer();
                        runCustomer.Run = doc.Tables[0].Rows[1].GetCell(0).Paragraphs[0].Runs[0];
                        WordWrite.ReplaceSmpbol(runCustomer);
                        dic.Add("[T1]", TuDiZhengYuanQuanLiRen);
                        dic.Add("[T2]", "");
                        dic.Add("[TuDiZhengGuanXi]", "");
                    }
                    else
                    {
                        RunCustomer runCustomer = new RunCustomer();
                        runCustomer.Run = doc.Tables[0].Rows[5].GetCell(0).Paragraphs[0].Runs[0];
                        WordWrite.ReplaceSmpbol(runCustomer);
                        dic.Add("[T1]", "");
                        dic.Add("[T2]", TuDiZhengYuanQuanLiRen);
                        dic.Add("[TuDiZhengGuanXi]", TuDiZhengGuanXi);
                    }
                   
                }
                if (!Utils.IsStrNull(FanChanZhengYuanQuanLiRen))
                {
                    flag = true;
                    if (FanChanZhengGuanXi.Contains("本人"))
                    {
                        //第一格
                        RunCustomer runCustomer = new RunCustomer();
                        runCustomer.Run = doc.Tables[0].Rows[2].GetCell(0).Paragraphs[0].Runs[0];
                        WordWrite.ReplaceSmpbol(runCustomer);
                        dic.Add("[F1]", FanChanZhengYuanQuanLiRen);
                        dic.Add("[F2]", "");
                        dic.Add("[FanChanZhengGuanXi]", "");
                        
                    }
                    else
                    {
                        RunCustomer runCustomer = new RunCustomer();
                        runCustomer.Run = doc.Tables[0].Rows[4].GetCell(0).Paragraphs[0].Runs[0];
                        WordWrite.ReplaceSmpbol(runCustomer);
                        dic.Add("[F1]", "");
                        dic.Add("[F2]", this.FanChanZhengYuanQuanLiRen);
                        dic.Add("[FanChanZhengGuanXi]", FanChanZhengGuanXi);
                    }
                }
                if(flag)
                {
                    WordWrite.ReplaceText(doc, dic);

                    string saveDir;
                    if (Utils.IsStrNull(PersonZiLiao.ShengMingShu))
                    {
                        saveDir = PersonZiLiao.DangAnDai.DirClass.DirName;
                    }
                    else
                    {
                        saveDir = PersonZiLiao.ShengMingShu;
                    }
                    WordWrite.SaveToFile(doc,Path.GetDirectoryName(saveDir) + "\\不一致证明.docx");
                }
               
            }
            MessageBox.Show("完成");

        }

        private string fanChanZhengYuanQuanLiRen="";
        public string FanChanZhengYuanQuanLiRen
        {
            get { return this.fanChanZhengYuanQuanLiRen; }
            set
            {
                if (this.fanChanZhengYuanQuanLiRen != value)
                {
                    this.fanChanZhengYuanQuanLiRen = value;
                    OnPropertyChanged("fanChanZhengYuanQuanLiRen");
                }
            }
        }

        private string fanChanZhengGuanXi= "";
        public string FanChanZhengGuanXi
        {
            get { return this.fanChanZhengGuanXi; }
            set
            {
                if (this.fanChanZhengGuanXi != value)
                {
                    this.fanChanZhengGuanXi = value;
                    OnPropertyChanged("fanChanZhengGuanXi");
                }
            }
        }
        public virtual PersonZiLiao PersonZiLiao { get; set; }


        public string Error
        {
            get { return null; }
        }


        public string this[string columnName]
        {
            get
            {

                return null;
            }
        }





        /// <summary>
        /// 使用上一个缓存中的数据
        /// </summary>
        /// <param name="lastModel"></param>
        public void SetRedies(YiShiShengMingBuYiZhiZhengMing lastModel)
        {
           // this.YiShengMing = lastModel.YiShengMing;
            //this.BuYiZhiZhengMing = lastModel.BuYiZhiZhengMing;
            this.PeopleCount = lastModel.PeopleCount;
            this.TuDiZheng = lastModel.TuDiZheng;
            this.TuDiZhengYuanQuanLiRen = lastModel.TuDiZhengYuanQuanLiRen;
            this.TuDiZhengGuanXi = lastModel.TuDiZhengGuanXi;
            this.FanChanZheng = lastModel.FanChanZheng;
            this.FanChanZhengYuanQuanLiRen = lastModel.FanChanZhengYuanQuanLiRen;
            this.FanChanZhengGuanXi = lastModel.FanChanZhengGuanXi;
            
        }
    }
}
