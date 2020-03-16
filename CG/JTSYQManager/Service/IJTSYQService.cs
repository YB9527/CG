using System.Collections.Generic;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;

namespace JTSYQManager.Service
{
    public interface IJTSYQService
    {
        void ExtractJZD_Intersectant(IList<JTSYQ> jtsyqs);
        void JZDBM(IList<JTSYQ> jtsyqs);
        void ExportJZDTable(IList<JTSYQ> jtsyqs, string bootDir);
        void ExportCunDangAnDai(IList<XZDM> cunList, string bootDir);
        void ExportCunYiJianFanKuaiShu(IList<XZDM> cunList, string bootDir);
        void ExportCunYiJianFanKuaiShu_GongShi(IList<XZDM> cunList, string bootDir);
        void Export(ExportDataPageViewModel model, IList<XZDM> xzdms);
        void DeleteJZD(IList<JTSYQ> jtsyqs);
        void ExportZu_ZhongDiTu(IList<XZDM> xzdms, string bootDir);
        void ExportZu_ZhongDiBiao(IList<XZDM> xzdms, string bootDir);
        void ExportZu_QuanJiDiaoChaBiao(IList<XZDM> xzdms, string bootDir);
        void ExportCun_JiTiTuDiDiaoChaBiao(IList<XZDM> xzdms, string bootDir);
        void ExportZu_ShenPiBiao(IList<XZDM> xzdms, string bootDir);
        void ExportZu_FaRenDaiBiaoWeiTuoShu(IList<XZDM> xzdms, string bootDir);
        void ExportZu_ZhiJieRenShenFengZhengMing(IList<XZDM> xzdms, string bootDir);
        void ExportZu_DangAnDai(IList<JTSYQ> jtsyqGruop, string bootDir);
        void ExportZu_FaRenDaiBiaoShenFengZhengMing(IList<XZDM> xzdms, string bootDir);
        void ExportZu_TuDiQuanShuLaiYuanZhengMing(IList<XZDM> xzdms, string bootDir);
        void ExportCunJieGuo_GongGao(IList<XZDM> cunList, string bootDir);
        void ExportCunJieGuo_GongShi(IList<XZDM> cunList, string bootDir);
        void ExportCun_GongGao(IList<XZDM> cunList, string bootDir);
        void ExportCun_GongShi(IList<XZDM> cunList, string bootDir);
        
    }
}
