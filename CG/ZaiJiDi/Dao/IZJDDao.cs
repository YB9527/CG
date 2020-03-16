using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Dao
{
    public interface IZJDDao
    {
        /// <summary>
        /// 检查内容是否和理
        /// </summary>
        /// <param name="jsyd"></param>
        IList<string> CheckZJD(JSYD jsyd,ZJDDataSourceViewModel model);
        /// <summary>
        /// 检查身份证重复
        /// </summary>
        /// <param name="hzs"></param>
        /// <returns></returns>
        IList<string> ChecSFZ(IList<JTCY> hzs,ZJDDataSourceViewModel model);
        /// <summary>
        /// 档案袋
        /// </summary>
        /// <param name="jsyd"></param>
        void ExportZJD_DangAnDai(JSYD jsyd, string saveDir);
        /// <summary>
        /// 审批表
        /// </summary>
        /// <param name="jsyd"></param>
        void ExportZJD_SPB(JSYD jsyd,string saveDir);
        /// <summary>
        /// 委托书
        /// </summary>
        /// <param name="jsyd"></param>
        void ExportZJD_WTS(JSYD jsyd, string saveDir);
        /// <summary>
        /// 声明书
        /// </summary>
        /// <param name="jsyd"></param>
        void ExportZJD_SMS(JSYD jsyd, string saveDir);
        /// <summary>
        /// 权籍调查表
        /// </summary>
        /// <param name="jsyd"></param>
        void ExportZJD_QJDCB(JSYD jsyd, string saveDir);
        /// <summary>
        /// 测绘报告
        /// </summary>
        /// <param name="jsyd"></param>
        void ExportZJD_CHBG(JSYD jsyd, string saveDir);
    }
}
