using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Service
{
    public interface IDangAnDaiService
    {
        /// <summary>
        /// 得到合宗的档案袋
        /// </summary>
        /// <param name="dirs"></param>
        /// <returns></returns>
        IList<DangAnDaiDirModel> GetDangAnDaiDirArray(string[] dirs);
        void WeiTuoShengMingCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po);
        void DangAnDaiDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po);
        void PictureDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po);
        void PDFDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po);
        void ShenFenXinXiDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po);
        void YuanShiDangAnDaiDir(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po);
    }
}
