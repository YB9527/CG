using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;

namespace JTSYQManager.Controller
{
    public interface IJTSYQController
    {

        /// <summary>
        /// 根据选择导出文件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="xzdms">选中的行政代码表</param>
        void Export(ExportDataPageViewModel model, IList<XZDM> xzdms);
        /// <summary>
        /// 提取相交的界址点
        /// </summary>
        /// <param name="selectFeatures"></param>
        void ExtractJZD_Intersectant(IList<JTSYQ> jtsyqs);
        /// <summary>
        /// 转换为对象
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        IList<JTSYQ> GetJTSYQS(IList<IFeature> list);
        /// <summary>
        /// 界址点编码
        /// </summary>
        /// <param name="list"></param>
        void JZDBM(IList<JTSYQ> list);
        /// <summary>
        /// 导出所有界址点成果表
        /// </summary>
        /// <param name="selecteds"></param>
        void ExportJZDTable(IList<JTSYQ> jtsyqs, string bootDir);
        /// <summary>
        /// 导出档案袋
        /// </summary>
        /// <param name="cunList"></param>
        void ExportCunDangAnDai(IList<XZDM> cunList, string bootDir);
       

        /// <summary>
        /// 导出集体土地所有权公告反馈意见书.docx
        /// </summary>
        /// <param name="cunList"></param>
        void ExportCunYiJianFanKuaiShu(IList<XZDM> cunList, string bootDir);
        /// <summary>
        /// 导出集体土地所有权公示反馈意见书.docx
        /// </summary>
        /// <param name="cunList"></param>
        void ExportCunYiJianFanKuaiShu_GongShi(IList<XZDM> cunList, string bootDir);
        /// <summary>
        /// 导出村结果公告
        /// </summary>
        /// <param name="cunList"></param>
        void ExportCunJieGuo_GongGao(IList<XZDM> cunList, string bootDir);
        void ExportCunJieGuo_GongShi(IList<XZDM> cunList, string bootDir);
        /// <summary>
        /// 导出村公告表
        /// </summary>
        /// <param name="cunList"></param>
        void ExportCun_GongGao(IList<XZDM> cunList, string bootDir);
        /// <summary>
        /// 导出村公示表
        /// </summary>
        void ExportCun_GongShi(IList<XZDM> cunList, string bootDir);
        /// <summary>
        /// 导出组档案袋
        /// </summary>
        void ExportZu_DangAnDai(IList<JTSYQ> jtsyqGruop, string bootDir);
   

        /// <summary>
        /// 土地权属来源证明
        /// </summary>
        void ExportZu_TuDiQuanShuLaiYuanZhengMing(IList<XZDM> xdms, string bootDir);
        /// <summary>
        /// 法人代表身份证明
        /// </summary>
        /// <param name="xdms"></param>
        void ExportZu_FaRenDaiBiaoShenFengZhengMing(IList<XZDM> xdms, string vbootDir);
        /// <summary>
        /// 法人代表委托书
        /// </summary>
        /// <param name="xdms"></param>
        void ExportZu_FaRenDaiBiaoWeiTuoShu(IList<XZDM> xdms, string bootDir);
        /// <summary>
        /// 指界人身份证明
        /// </summary>
        /// <param name="xdms"></param>
        void ExportZu_ZhiJieRenShenFengZhengMing(IList<XZDM> xdms, string bootDir);
        /// <summary>
        /// 审批表
        /// </summary>
        /// <param name="xdms"></param>
        void ExportZu_ShenPiBiao(IList<XZDM> xzdms, string bootDir);
        /// <summary>
        /// 集体土地调查表
        /// </summary>
        /// <param name="xzdms"></param>
        void ExportCun_JiTiTuDiDiaoChaBiao(IList<XZDM> xzdms, string bootDir);
        /// <summary>
        /// 导出权籍调查表
        /// </summary>
        /// <param name="xzdms"></param>
        void ExportZu_QuanJiDiaoChaBiao(IList<XZDM> xzdms, string bootDir);
        /// <summary>
        /// 导出单宗地
        /// </summary>
        /// <param name="xzdms"></param>
        /// <param name="bootDir"></param>
        void ExportZu_ZhongDiBiao(IList<XZDM> xzdms, string bootDir);
        /// <summary>
        /// 导出宗地图
        /// </summary>
        /// <param name="xzdms"></param>
        /// <param name="bootDir"></param>
        void ExportZu_ZhongDiTu(IList<XZDM> xzdms, string bootDir);
        /// <summary>
        /// 以组为单位删除界址点
        /// </summary>
        /// <param name="jtsyqs"></param>
        void DeleteJZD(IList<JTSYQ> jtsyqs);
    }
}
