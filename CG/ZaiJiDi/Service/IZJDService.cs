using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaiJiDi.Pages.ZJDPage;
using ZaiJiDi.Pages.ZJDPage.DataSource;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Service
{
    public interface IZJDService
    {
        /// <summary>
        /// 得到JSYD所有
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IList<JSYD> GetJSYDByZJDDataSourceViewModel(ZJDDataSourceViewModel model);
        /// <summary>
        /// 检查宅基地
        /// </summary>
        /// <param name="jsyds"></param>
        IList<string> CheckZJD(IList<JSYD> jsyds, ZJDDataSourceViewModel model);
        /// <summary>
        /// 导出档案袋
        /// </summary>
        /// <param name="jsyds"></param>
        /// <param name="model"></param>
        void ExportZJD(IList<JSYD> jsyds, ZJDExportDataViewModel model);
    }
}
