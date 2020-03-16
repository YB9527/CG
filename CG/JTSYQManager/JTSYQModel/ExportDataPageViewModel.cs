using System;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm;
using FirstFloor.ModernUI.Presentation;
using HeibernateManager.Model;
using System.Windows;
using ReflectManager;

namespace JTSYQManager.JTSYQModel
{  
    
    public class ExportDataPageViewModel  : NotifyPropertyChanged
    {
        private static string ExportDataPageRides = "JTSYQSelectResult";

        public void SaveRedis()
        {
            string str = ReflectManager.JObejctReflect.ObjectToStr(this);
            SoftwareConfig.Refresh(ExportDataPageRides, str);
        }
        public static  ExportDataPageViewModel GetRedis()
        {
            SoftwareConfig config = SoftwareConfig.FindConfig(ExportDataPageRides);
            if(config != null)
            {
                ExportDataPageViewModel model = JObejctReflect.ToObejct<ExportDataPageViewModel>(config.Value);
                return model;
            }
            else
            {
                return  new ExportDataPageViewModel();
            }
           
        
        }

        private string saveDir;
        public string SaveDir
        {
            get { 
                    return this.saveDir;
            }
            set
            {

                    this.saveDir = value;
                    OnPropertyChanged("saveDir");

            }
        }
        private bool zu_DangAnDai;
        public bool Zu_DangAnDai
        {
            get { return this.zu_DangAnDai; }
            set
            {

                    this.zu_DangAnDai = value;
                    OnPropertyChanged("zu_DangAnDai");
                
            }
        }
        private bool zu_TuDiQuanShuZhengMing;
        public bool Zu_TuDiQuanShuZhengMing
        {
            get { return this.zu_TuDiQuanShuZhengMing; }
            set
            {

                    this.zu_TuDiQuanShuZhengMing = value;
                    OnPropertyChanged("zu_TuDiQuanShuZhengMing");
                
            }
        }

        private bool zu_FaRenDaiBiaoZhengMing;
        public bool Zu_FaRenDaiBiaoZhengMing
        {
            get { return this.zu_FaRenDaiBiaoZhengMing; }
            set
            {

                    this.zu_FaRenDaiBiaoZhengMing = value;
                    OnPropertyChanged("zu_FaRenDaiBiaoZhengMing");
                
            }
        }

        private bool zu_FaRenDaiBiaoWeiTuoShu;
        public bool Zu_FaRenDaiBiaoWeiTuoShu
        {
            get { return this.zu_FaRenDaiBiaoWeiTuoShu; }
            set
            {

                    this.zu_FaRenDaiBiaoWeiTuoShu = value;
                    OnPropertyChanged("zu_FaRenDaiBiaoWeiTuoShu");
                
            }
        }
        private bool zu_ZhiJieRenZhengMing;
        public bool Zu_ZhiJieRenZhengMing
        {
            get { return this.zu_ZhiJieRenZhengMing; }
            set
            {

                    this.zu_ZhiJieRenZhengMing = value;
                    OnPropertyChanged("zu_ZhiJieRenZhengMing");
                
            }
        }

        private bool zu_ShengPiBiao;
        public bool Zu_ShengPiBiao
        {
            get { return this.zu_ShengPiBiao; }
            set
            {

                    this.zu_ShengPiBiao = value;
                    OnPropertyChanged("zu_ShengPiBiao");
                
            }
        }

        private bool zu_JieZhiDianChengGuoBiao;
        public bool Zu_JieZhiDianChengGuoBiao
        {
            get { return this.zu_JieZhiDianChengGuoBiao; }
            set
            {

                    this.zu_JieZhiDianChengGuoBiao = value;
                    OnPropertyChanged("zu_JieZhiDianChengGuoBiao");
                
            }
        }
        private bool zu_QuanJiDiaoChaBiao;
        public bool Zu_QuanJiDiaoChaBiao
        {
            get { return this.zu_QuanJiDiaoChaBiao; }
            set
            {

                    this.zu_QuanJiDiaoChaBiao = value;
                    OnPropertyChanged("zu_QuanJiDiaoChaBiao");
                
            }
        }

        private bool zu_RuKuDanZhongDiBiao;
        public bool Zu_RuKuDanZhongDiBiao
        {
            get { return this.zu_RuKuDanZhongDiBiao; }
            set
            {

                    this.zu_RuKuDanZhongDiBiao = value;
                    OnPropertyChanged("zu_RuKuDanZhongDiBiao");
                
            }
        }


        private bool zu_ZhongDiTu;
        public bool Zu_ZhongDiTu
        {
            get { return this.zu_ZhongDiTu; }
            set
            {

                    this.zu_ZhongDiTu = value;
                    OnPropertyChanged("zu_ZhongDiTu");
                
            }
        }

        private bool cun_DangAnDai;
        public bool Cun_DangAnDai
        {
            get { return this.cun_DangAnDai; }
            set
            {

                    this.cun_DangAnDai = value;
                    OnPropertyChanged("cun_DangAnDai");
                
            }
        }

        private bool cun_GongGaoFanKuiYiJianShu;
        public bool Cun_GongGaoFanKuiYiJianShu
        {
            get { return this.cun_GongGaoFanKuiYiJianShu; }
            set
            {

                    this.cun_GongGaoFanKuiYiJianShu = value;
                    OnPropertyChanged("cun_GongGaoFanKuiYiJianShu");
                
            }
        }

        private bool cun_GongShiFanKuiYiJianShu;
        public bool Cun_GongShiFanKuiYiJianShu
        {
            get { return this.cun_GongShiFanKuiYiJianShu; }
            set
            {

                    this.cun_GongShiFanKuiYiJianShu = value;
                    OnPropertyChanged("cun_GongShiFanKuiYiJianShu");
                
            }
        }
        private bool cun_QueQuanJieGuoGongGao;
        public bool Cun_QueQuanJieGuoGongGao
        {
            get { return this.cun_QueQuanJieGuoGongGao; }
            set
            {

                    this.cun_QueQuanJieGuoGongGao = value;
                    OnPropertyChanged("cun_QueQuanJieGuoGongGao");
                
            }
        }

        private bool cun_QueQuanJieGuoGongShi;
        public bool Cun_QueQuanJieGuoGongShi
        {
            get { return this.cun_QueQuanJieGuoGongShi; }
            set
            {

                    this.cun_QueQuanJieGuoGongShi = value;
                    OnPropertyChanged("cun_QueQuanJieGuoGongShi");
                
            }
        }

        private bool cun_GongGaoBiao;
        public bool Cun_GongGaoBiao
        {
            get { return this.cun_GongGaoBiao; }
            set
            {

                    this.cun_GongGaoBiao = value;
                    OnPropertyChanged("cun_GongGaoBiao");
                
            }
        }


        private bool cun_GongShiBiao;
        public bool Cun_GongShiBiao
        {
            get { return this.cun_GongShiBiao; }
            set
            {

                    this.cun_GongShiBiao = value;
                    OnPropertyChanged("cun_GongShiBiao");
                
            }
        }

        private bool cun_JiTiTuDiDiaoChaBiao;
        public bool Cun_JiTiTuDiDiaoChaBiao
        {
            get { return this.cun_JiTiTuDiDiaoChaBiao; }
            set
            {

                    this.cun_JiTiTuDiDiaoChaBiao = value;
                    OnPropertyChanged("cun_JiTiTuDiDiaoChaBiao");
                
            }
        }

        private bool cun_GongShiTu;
        public bool Cun_GongShiTu
        {
            get { return this.cun_GongShiTu; }
            set
            {

                    this.cun_GongShiTu = value;
                    OnPropertyChanged("cun_GongShiTu");
                
            }
        }

    }
}