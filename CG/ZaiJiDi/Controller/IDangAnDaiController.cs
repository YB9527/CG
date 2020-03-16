using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Controller
{
    /// <summary>
    /// 创建一个全新的档案袋
    /// </summary>
    public interface IDangAnDaiController
    {
        void CreateNewDangAnDai(DangAnDaiMergeViewModel model);
        void PrintDangAnDai(DangAnDai dangDanDai,string range = null);
        void CreateShenMingShu(object sender,object e);
        void CreateBuYiZiZhengMing(object sender, object e);
       
    }
}
