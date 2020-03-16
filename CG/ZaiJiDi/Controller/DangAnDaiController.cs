using FileManager;
using MyUtils;
using System;
using System.Collections.Generic;
using ZaiJiDi.ZaiJiDiModel;
using ZaiJiDi.Service;

namespace ZaiJiDi.Controller
{
    public class DangAnDaiController : IDangAnDaiController
    {
        IDangAnDaiService DangAnDaiService = new DangAnDaiService();
        public void CreateNewDangAnDai(DangAnDaiMergeViewModel po)
        {

            //档案袋文件夹
            //1 得到合宗的户 用_ 区分
            if(Utils.IsStrNull( po.DangAnDaiDir))
            {
             
                return;
            }
            DirClass dirClass = new DirClass(po.DangAnDaiDir);
           
            //2 得到人名，得到编码
            IList<DangAnDaiDirModel> dangAnDaiDirArray = DangAnDaiService.GetDangAnDaiDirArray(dirClass.Dirs);
            //档案带Copy
            DangAnDaiService.DangAnDaiDirCopy(dangAnDaiDirArray,po);
            //委托声明文件夹,用名字查找
            DangAnDaiService.WeiTuoShengMingCopy(dangAnDaiDirArray, po);

            //照片移动
            DangAnDaiService.PictureDirCopy(dangAnDaiDirArray, po);

            //pdf移交
            DangAnDaiService.PDFDirCopy(dangAnDaiDirArray, po);
            //身份信息文件夹/文件夹
            DangAnDaiService.ShenFenXinXiDirCopy(dangAnDaiDirArray, po);
            //原始档案袋移动 不一致证明，遗失声明
            DangAnDaiService.YuanShiDangAnDaiDir(dangAnDaiDirArray, po);
        }

       

       

        public void PrintDangAnDai(DangAnDai dangDanDai,string range=null)
        {

            //打印封面
            string fengMian = dangDanDai.FengMian;
            if(!Utils.IsStrNull(fengMian))
            {
                PrintUtils.DocToPDFPrint(fengMian);
            }
            //1_确权登记申请审批表.doc
            string shenPiaoBiao = dangDanDai.ShenPiBiao;
            if (!Utils.IsStrNull(shenPiaoBiao))
            {
                //PrintUtils.DocToPDFPrint(shenPiaoBiao, System.Drawing.Printing.Duplex.Vertical);
                PrintUtils.RelativePrint(shenPiaoBiao);

            }
            //打印个人
            printPersonZiLiao(dangDanDai);

            //6_宅基地权籍调查表
            string diaoChaoBiao = dangDanDai.DiaoChaBiao;
            if (!Utils.IsStrNull(diaoChaoBiao))
            {
               
                PrintUtils.DocToPDFPrint(diaoChaoBiao, 1,System.Drawing.Printing.Duplex.Vertical, range);
            }

            //草图
            string caoTu = dangDanDai.CaoTu;
            if (!Utils.IsStrNull(caoTu))
            {
                PrintUtils.PrintPDF(caoTu);
            }

            //打印宗地图
            string zongDiTu = dangDanDai.ZongDiTu;
            if (!Utils.IsStrNull(zongDiTu))
            {
                PrintUtils.PrintPDF(zongDiTu);
                PrintUtils.PrintPDF(zongDiTu);
            }

            //房屋图
            string fangWuTu = dangDanDai.FangWuTu;         
            if (!Utils.IsStrNull(fangWuTu))
            {
                PrintUtils.PrintPDF(fangWuTu);
            }

            //测绘报告
            string ceHuiBaoGao = dangDanDai.CeHuiBaoGao;
            if (!Utils.IsStrNull(ceHuiBaoGao))
            {
                PrintUtils.DocToPDFPrint(ceHuiBaoGao, 1,System.Drawing.Printing.Duplex.Vertical);
            }
            //打印照片
            string picture = dangDanDai.Picture;
            if (!Utils.IsStrNull(picture))
            {
                PrintUtils.PrintPicture(picture,1, System.Drawing.Printing.Duplex.Simplex);
            }

        }
        
        /// <summary>
        /// 打印个人部份资料
        /// </summary>
        /// <param name="dangDanDai"></param>
        private void printPersonZiLiao(DangAnDai dangDanDai)
        {
            IList<PersonZiLiao> personZiLiaos = dangDanDai.PersonZiLiaos;
            if(personZiLiaos == null)
            {
                return;
            }
            foreach(PersonZiLiao personZiLiao in personZiLiaos)
            {
                string weiTuoShu = personZiLiao.WeiTuoShu;
                if(!Utils.IsStrNull(weiTuoShu))
                {
                    PrintUtils.DocToPDFPrint(weiTuoShu);
                }
                string shengMingShu = personZiLiao.ShengMingShu;
                if (!Utils.IsStrNull(shengMingShu))
                {
                    PrintUtils.DocToPDFPrint(shengMingShu);
                }
                string shenFenXinXi = personZiLiao.ShenFenXinXi;
                if (!Utils.IsStrNull(shenFenXinXi))
                {
                    foreach(string str in personZiLiao.ShenFenXinXiArray)
                    {
                        PrintUtils.PrintPDF(str);
                    }
                    //PrintUtils.PrintPDF(shenFenXinXi);
                }

                string yiShiShengMing = personZiLiao.YiShiShengMing;
                if (!Utils.IsStrNull(yiShiShengMing))
                {
                    PrintUtils.PrintDoc(yiShiShengMing);
                }
                string buYiZhiZhengMing = personZiLiao.BuYiZhiZhengMing;
                if (!Utils.IsStrNull(buYiZhiZhengMing))
                {
                    PrintUtils.PrintDoc(buYiZhiZhengMing);
                }
            }

        }
        /// <summary>
        /// 创建声明书doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateShenMingShu(object sender, object e)
        {
            
           
        }
        /// <summary>
        /// 创建不一致证明doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateBuYiZiZhengMing(object sender, object e)
        {

        }

       
    }
}
