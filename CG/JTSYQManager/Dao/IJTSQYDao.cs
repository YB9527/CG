using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;

namespace JTSYQManager.Dao
{
    public interface IJTSQYDao
    {
        /// <summary>
        /// 提取界址点
        /// </summary>
        /// <param name="jtsyq"></param>
        /// <param name="jzds"></param>
        /// <returns></returns>
        IList<JZD> ExtractJZD_Intersectant(JTSYQ jtsyq, IList<JZD> jzds);
        void ExportZu_FaRenDaiBiaoShenFengZhengMing(XZDM xzdm, string saveDir);
        /// <summary>
        /// 法人代表委托书
        /// </summary>
        /// <param name="xzdm"></param>
        /// <param name="saveDir"></param>
        void ExportZu_FaRenDaiBiaoWeiTuoShu(XZDM xzdm, string saveDir);
        void ExportZu_ZhiJieRenShenFengZhengMing(JTSYQ jtsyq, string saveDir);
        void ExportZu_QuanJiDiaoChaBiao(JTSYQ jtsyq, string saveDir);
        /// <summary>
        /// 导出公示图
        /// </summary>
        /// <param name="xzdm"></param>
        /// <param name="saveDir"></param>
        void ExportCun_GongShiTuActions(XZDM cunXZDM, string saveDir);
        void ExportCun_JiTiTuDiDiaoChaBiao(IList<JTSYQ> jtsyqs, string saveDir);
        /// <summary>
        /// 导出宗地表
        /// </summary>
        /// <param name="jtsyq"></param>
        /// <param name="saveDir"></param>
        void ExportZu_ZhongDiBiao(JTSYQ jtsyq, string saveDir);
        /// <summary>
        /// 导出宗地图
        /// </summary>
        /// <param name="jtsyq"></param>
        /// <param name="saveDir"></param>
        void ExportZu_ZhongDiTu(JTSYQ jtsyq, string saveDir);

        /// <summary>
        /// 审批表
        /// </summary>
        /// <param name="jtsyq"></param>
        /// <param name="saveDir"></param>
        void ExportZu_ShenPiBiao(JTSYQ jtsyq, string saveDir);

        /// <summary>
        /// 土地权属来源证明
        /// </summary>
        /// <param name="xzdm"></param>
        /// <param name="saveDir"></param>
        void ExportZu_TuDiQuanShuLaiYuanZhengMing(JTSYQ jtsyq, string saveDir);

        /// <summary>
        /// 导出组档案袋
        /// </summary>
        /// <param name="xzdm"></param>
        void ExportZu_DangAnDai(JTSYQ jtsyq, string saveDir);

        /// <summary>
        /// 导出村档案袋
        /// </summary>
        /// <param name="xzdm"></param>
        void ExportCunDangAnDai(XZDM xzdm, string saveBootDir);
        /// <summary>
        /// 导出村结果公告
        /// </summary>
        /// <param name="XZDM"></param>
        void ExportCunJieGuo_GongGao(XZDM xzdm, string bootDir);
        /// <summary>
        /// 导出村结果公示
        /// </summary>
        /// <param name="xzdm"></param>
        void ExportCunJieGuo_GongShi(XZDM xzdm, string bootDir);

        /// <summary>
        /// 导出集体土地所有权公告反馈意见书.docx
        /// </summary>
        /// <param name="xzdm"></param>
        void ExportCunYiJianFanKuaiShu(XZDM xzdm,string bootDir);
       

        /// <summary>
        /// 导出界址点成果表
        /// </summary>
        /// <param name="jtsyq"></param>
        void ExportJZDTable(JTSYQ jtsyq, string bootDir);
        /// <summary>
        /// 导出集体土地所有权公示反馈意见书.docx
        /// </summary>
        /// <param name="xzdm"></param>
        void ExportCunYiJianFanKuaiShu_GongShi(XZDM xzdm, string bootDir);

        /// <summary>
        /// 得到图上所有界址点
        /// </summary>
        /// <returns></returns>
        Dictionary<string, IList<JZD>> GetMapJZDS();
        /// <summary>
        /// 导出村公告表
        /// </summary>
        /// <param name="XZDM"></param>
        void ExportCun_GongGao(IList<JTSYQ> jtsyqs, XZDM xzdm, string bootDir);

        /// <summary>
        /// 界址点创建编码
        /// </summary>
        /// <param name="jtsyq"></param>
        int JZDBM(int startBH, JTSYQ jtsyq, ESRI.ArcGIS.Carto.IFeatureLayer featureLayer);
        /// <summary>
        /// 导出村公示表
        /// </summary>
        /// <param name="jtsyqs"></param>
        /// <param name="xzdm"></param>
        void ExportCun_GongShi(IList<JTSYQ> jtsyqs, XZDM xzdm, string bootDir);
      
        void DeleteJZD(IList<JZD> jZDS);
    }
}
