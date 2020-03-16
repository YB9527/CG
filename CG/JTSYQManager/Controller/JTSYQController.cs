using ESRI.ArcGIS.Geodatabase;
using JTSYQManager.JTSYQModel;
using JTSYQManager.Service;
using JTSYQManager.XZDMManager;
using MyUtils;
using System.Collections.Generic;

namespace JTSYQManager.Controller
{
    public class JTSYQController : IJTSYQController
    {
        public IJTSYQService JTSYQService = new JTSYQService();
        public void Export(ExportDataPageViewModel model, IList<XZDM> xzdms)
        {
            if (model != null && Utils.CheckListExists(xzdms))
            {
                JTSYQService.Export(model, xzdms);
            }
        }


        public void DeleteJZD(IList<JTSYQ> jtsyqs)
        {
            if (Utils.CheckListExists(jtsyqs))
            {
                JTSYQService.DeleteJZD(jtsyqs);
            }
        }
        public void ExportZu_ZhongDiTu(IList<XZDM> xzdms, string bootDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_ZhongDiTu(xzdms, bootDir);
            }
        }
        public void ExportZu_ZhongDiBiao(IList<XZDM> xzdms, string bootDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_ZhongDiBiao(xzdms, bootDir);
            }
        }
        public void ExportZu_QuanJiDiaoChaBiao(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_QuanJiDiaoChaBiao(xzdms, booDir);
            }
        }
        public void ExportCun_JiTiTuDiDiaoChaBiao(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportCun_JiTiTuDiDiaoChaBiao(xzdms, booDir);
            }
        }
        public void ExportZu_ShenPiBiao(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_ShenPiBiao(xzdms, booDir);
            }
        }
        public void ExportZu_ZhiJieRenShenFengZhengMing(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_ZhiJieRenShenFengZhengMing(xzdms, booDir);
            }
        }
        public void ExportZu_FaRenDaiBiaoWeiTuoShu(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_FaRenDaiBiaoWeiTuoShu(xzdms, booDir);
            }
        }
        public void ExportZu_FaRenDaiBiaoShenFengZhengMing(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_FaRenDaiBiaoShenFengZhengMing(xzdms, booDir);
            }
        }
        public void ExportZu_TuDiQuanShuLaiYuanZhengMing(IList<XZDM> xzdms, string booDir)
        {
            if (Utils.CheckListExists(xzdms))
            {
                JTSYQService.ExportZu_TuDiQuanShuLaiYuanZhengMing(xzdms, booDir);
            }
        }
        public void ExportZu_DangAnDai(IList<JTSYQ> jtsyqGruop, string booDir)
        {
            if (Utils.CheckListExists(jtsyqGruop))
            {
                JTSYQService.ExportZu_DangAnDai(jtsyqGruop, booDir);
            }
        }
        public void ExportCunDangAnDai(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCunDangAnDai(cunList, booDir);
            }
        }

        public void ExportCunJieGuo_GongGao(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCunJieGuo_GongGao(cunList, booDir);
            }
        }

        public void ExportCunJieGuo_GongShi(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCunJieGuo_GongShi(cunList, booDir);
            }
        }

        public void ExportCunYiJianFanKuaiShu(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCunYiJianFanKuaiShu(cunList, booDir);
            }
        }

        public void ExportCunYiJianFanKuaiShu_GongShi(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCunYiJianFanKuaiShu_GongShi(cunList, booDir);
            }
        }

        public void ExportCun_GongGao(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCun_GongGao(cunList, booDir);
            }
        }

        public void ExportCun_GongShi(IList<XZDM> cunList, string booDir)
        {
            if (Utils.CheckListExists(cunList))
            {
                JTSYQService.ExportCun_GongShi(cunList, booDir);
            }
        }

        public void ExportJZDTable(IList<JTSYQ> jtsyqs, string booDir)
        {
            if (Utils.CheckListExists(jtsyqs))
            {
                JTSYQService.ExportJZDTable(jtsyqs, booDir);
            }

        }



        public void ExtractJZD_Intersectant(IList<JTSYQ> jtsyqs)
        {
            if (Utils.CheckListExists(jtsyqs))
            {
                JTSYQService.ExtractJZD_Intersectant(jtsyqs);
            }

        }


        public IList<JTSYQ> GetJTSYQS(IList<IFeature> list)
        {
            return JTSYQCustom.FeaturesToJTSYQ(list);
        }

        public void JZDBM(IList<JTSYQ> jtsyqs)
        {
            if (Utils.CheckListExists(jtsyqs))
            {
                JTSYQService.JZDBM(jtsyqs);
            }
        }

      
    }
 }
