
using ArcGisManager;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using HeibernateManager.Model;
using JTSYQManager.Dao;
using JTSYQManager.JTSYQModel;
using JTSYQManager.XZDMManager;
using MyUtils;
using ProgressTask;
using System;
using System.Collections.Generic;
using System.Windows;

namespace JTSYQManager.Service
{
    public class JTSYQService : IJTSYQService
    {
        public IJTSQYDao JTSYQDao = new JTSYQDao();


        private List<MyAction> ExportCun_GongShiTuActions(IList<XZDM> cunXZDM, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            List<JTSYQ> list = new List<JTSYQ>();
            foreach (XZDM xzdm in cunXZDM)
            {
                Action action = new Action(() =>
                {
                    
                    IList<JTSYQ> jtsyqs = JTSYQCustom.GetMapJTSYQ(" BM LIKE  '*" + xzdm.DJZQDM + "*'");
                    if(Utils.CheckListExists(jtsyqs))
                    {
                      
                        xzdm.JTSYQS = jtsyqs;
                     
                        string saveDir = bootDir + "/" + xzdm.Cun + "/";
                        JTSYQDao.ExportCun_GongShiTuActions(xzdm, saveDir);
                    }

                });
                MyAction myAction = new MyAction(action, xzdm.CunZu + "公示图");
                actions.Add(myAction);
            }
           
          
            return actions;
        }
        public void ExportZu_ZhongDiTu(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = ExportZu_ZhongDiTuActions(xzdms, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "宗地图 ");
        }
        public List<MyAction> ExportZu_ZhongDiTuActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            List<JTSYQ> list = new List<JTSYQ>();
            foreach (XZDM xzdm in xzdms)
            {

                IList<JTSYQ> jtsyqs = XZDMCustom.GetSelectJTSYQ(xzdm);
                if (Utils.CheckListExists(jtsyqs))
                {
                    list.AddRange(jtsyqs);
                }
            }
            List<JTSYQ> list2 = new List<JTSYQ>();
            foreach (JTSYQ jtsyq in list)
            {
                list2.AddRange(jtsyq.GroupJTSYQ);
            }
            JTSYQCustom.SetJZD(list2);

            foreach (JTSYQ jtsyq in list)
            {
                XZDM xzdm = jtsyq.XZDM;
                Action action = new Action(() =>
                {
                    string saveDir = bootDir + xzdm.Cun + "/" + xzdm.Zu;
                    JTSYQDao.ExportZu_ZhongDiTu(jtsyq, saveDir);
                });
                MyAction myAction = new MyAction(action, xzdm.CunZu + "宗地图 ");
                actions.Add(myAction);
            }
            return actions;
        }
        public void ExportZu_ZhongDiBiao(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = ExportZu_ZhongDiBiaoActions(xzdms, bootDir);
            SingleTaskForm task = new SingleTaskForm(actions, "单宗入库表 ");
        }
        public List<MyAction> ExportZu_ZhongDiBiaoActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
          
            foreach (XZDM xzdm in xzdms)
            {

              if(xzdm.JTSYQ != null)
                {
                    Action action = new Action(() =>
                    {
                        string saveDir = bootDir + xzdm.Cun + "/" + xzdm.Zu;
                        JTSYQDao.ExportZu_ZhongDiBiao(xzdm.JTSYQ, saveDir);
                    });
                    MyAction myAction = new MyAction(action, xzdm.CunZu + "单宗入库表 ");
                    actions.Add(myAction);
                }
              
            }
          
            return actions;
        }
        public void ExportZu_QuanJiDiaoChaBiao(IList<XZDM> xzdms,string bootDir)
        {
            List<MyAction> actions = ExportZu_QuanJiDiaoChaBiaoActions(xzdms, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "权籍调查表 ");
        }
        public List<MyAction> ExportZu_QuanJiDiaoChaBiaoActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            List<JTSYQ> list2 = new List<JTSYQ>();
            foreach (XZDM xzdm in xzdms)
            {
                IList<JTSYQ> jtsyqs = xzdm.JTSYQS;
                if (Utils.CheckListExists(jtsyqs))
                {
                    list2.AddRange(jtsyqs);
                }
            }
           
            JTSYQCustom.SetJZD(list2);
            JTSYQCustom.SetQZB(list2);
            string saveDir;

            foreach (XZDM xzdm in xzdms)
            {
                
                if(xzdm.JTSYQ != null)
                {
                    Action action = new Action(() =>
                    {
                        if (Utils.IsStrNull(xzdm.Zu))
                        {
                            saveDir = bootDir + xzdm.Cun + "//村集体";
                        }
                        else
                        {
                            saveDir = bootDir + xzdm.Cun + "//" + xzdm.Zu;
                        }
                        JTSYQDao.ExportZu_QuanJiDiaoChaBiao(xzdm.JTSYQ, saveDir);
                    });
                    MyAction myAction = new MyAction(action, xzdm.CunZu + "权籍调查表 ");
                    actions.Add(myAction);
                }
              
            }
            return actions;
        }
        public void ExportCun_JiTiTuDiDiaoChaBiao(IList<XZDM> xzdms, string bootDir)
        {
//            List<MyAction> actions = ExportCun_JiTiTuDiDiaoChaBiaoActions(xzdms, bootDir);


  //          SingleTaskForm task = new SingleTaskForm(actions, "集体土地调查表 ");
        }
        public List<MyAction> ExportCun_JiTiTuDiDiaoChaBiaoActions(IList<XZDM> cunXZDM ,IList<XZDM> zuXZDM, string bootDir)
        {
          
            IList<XZDM> zus;
            IList<IList<XZDM>> cunGroup = new List<IList<XZDM>>();

            foreach (XZDM cun in cunXZDM)
            {
                zus = new List<XZDM>();
                if(cun.JTSYQ != null)
                {
                    zus.Add(cun);
                }
                cunGroup.Add(zus);
                string bm = cun.DJZQDM;

                foreach(XZDM zu in zuXZDM)
                {
                    string zuBM = zu.DJZQDM;
                    if(zuBM.Substring(0,12).Equals(bm))
                    {
                        zus.Add(zu);
                    }
                }
            }

            List<MyAction> actions = new List<MyAction>();
            foreach (IList < XZDM > zuGroup in cunGroup)
            {
                           
                if (zuGroup.Count !=0 )
                {
        string saveDir = bootDir + zuGroup[0].Cun;

                    Action action = new Action(() =>
                    {
                        IList<JTSYQ> jtsyqs = new List<JTSYQ>();
                       foreach(XZDM zu in zuGroup)
                        {
                            jtsyqs.Add(zu.JTSYQ);
                        }
                        JTSYQDao.ExportCun_JiTiTuDiDiaoChaBiao(jtsyqs, saveDir);
                    });
                    MyAction myAction = new MyAction(action, zuGroup[0].Cun + "集体土地调查表 ");
                    actions.Add(myAction);
                }
            }
            return actions;
        }
        public void ExportZu_ShenPiBiao(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = ExportZu_ShenPiBiaoActions(xzdms, bootDir);
          
            SingleTaskForm task = new SingleTaskForm(actions, "审批表");
        }
        public List<MyAction> ExportZu_ShenPiBiaoActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            string saveDir;
            foreach (XZDM xzdm in xzdms)
            {

               
                if (xzdm.JTSYQ != null)
                {
                    Action action = new Action(() =>
                    {
                        if (Utils.IsStrNull(xzdm.Zu))
                        {
                            saveDir = bootDir + xzdm.Cun + "//村集体";
                        }
                        else
                        {
                            saveDir = bootDir + xzdm.Cun + "//" + xzdm.Zu;
                        }

                        JTSYQDao.ExportZu_ShenPiBiao(xzdm.JTSYQ, saveDir);
                    });
                    MyAction myAction = new MyAction(action, xzdm.CunZu + "审批表");
                    actions.Add(myAction);
                }
            }
            return actions;
        }

        public void ExportZu_ZhiJieRenShenFengZhengMing(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = ExportZu_ZhiJieRenShenFengZhengMingActions(xzdms, bootDir);
          
            SingleTaskForm task = new SingleTaskForm(actions, "指界人证明");
        }
        public List<MyAction> ExportZu_ZhiJieRenShenFengZhengMingActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            string saveDir;
            foreach (XZDM xzdm in xzdms)
            {
                JTSYQ jtsyq = xzdm.JTSYQ;
                if (jtsyq != null)
                {
                    Action action = new Action(() =>
                    {
                        if (Utils.IsStrNull(xzdm.Zu))
                        {
                            saveDir = bootDir + xzdm.Cun + "//村集体";
                        }
                        else
                        {
                            saveDir = bootDir + xzdm.Cun + "//" + xzdm.Zu;
                        }
                        JTSYQDao.ExportZu_ZhiJieRenShenFengZhengMing(jtsyq, saveDir);
                    });
                    MyAction myAction = new MyAction(action, xzdm.CunZu + "指界人证明");
                    actions.Add(myAction);
                }

            
            }
            return actions;
        }
        public void ExportZu_FaRenDaiBiaoWeiTuoShu(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = ExportZu_FaRenDaiBiaoWeiTuoShuActions(xzdms, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "法人代表授权委托书");
        }
        public List<MyAction> ExportZu_FaRenDaiBiaoWeiTuoShuActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            string saveDir;
            foreach (XZDM xzdm in xzdms)
            {
                Action action = new Action(() =>
                {
                    if (Utils.IsStrNull(xzdm.Zu))
                    {
                        saveDir = bootDir + xzdm.Cun + "//村集体";
                    }
                    else
                    {
                        saveDir = bootDir + xzdm.Cun + "//" + xzdm.Zu;
                    }
                    JTSYQDao.ExportZu_FaRenDaiBiaoWeiTuoShu(xzdm, saveDir);
                });
                MyAction myAction = new MyAction(action, xzdm.CunZu + "法人代表授权委托书");
                actions.Add(myAction);
            }
            return actions;
        }

        public void ExportZu_FaRenDaiBiaoShenFengZhengMing(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = ExportZu_FaRenDaiBiaoShenFengZhengMingActions(xzdms, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "法人代表身份证明");

        }
        public List<MyAction> ExportZu_FaRenDaiBiaoShenFengZhengMingActions(IList<XZDM> xzdms, string bootDir)
        {
            List<MyAction> actions = new List<MyAction>();
            string saveDir;
            foreach (XZDM xzdm in xzdms)
            {
                Action action = new Action(() =>
                {
                    if (Utils.IsStrNull(xzdm.Zu))
                    {
                        saveDir = bootDir + xzdm.Cun + "//村集体";
                    }
                    else
                    {
                        saveDir = bootDir + xzdm.Cun + "//" + xzdm.Zu;
                    }
                    JTSYQDao.ExportZu_FaRenDaiBiaoShenFengZhengMing(xzdm, saveDir);
                });
                MyAction myAction = new MyAction(action, xzdm.CunZu + "法人代表身份证明");
                actions.Add(myAction);
            }
            return actions;
        }
        public void ExportZu_TuDiQuanShuLaiYuanZhengMing(IList<XZDM> xzdms, string bootDir)
        {
            IList<MyAction> actions = ExportZu_TuDiQuanShuLaiYuanZhengMingActions(xzdms, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "土地权属来源证明");
        }
        public IList<MyAction> ExportZu_TuDiQuanShuLaiYuanZhengMingActions(IList<XZDM> xzdms, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            string saveDir;
            foreach (XZDM xzdm in xzdms)
            {
                JTSYQ jtsyq = xzdm.JTSYQ;
                if(jtsyq != null)
                {
                    Action action = new Action(() =>
                    {
                        if (Utils.IsStrNull(xzdm.Zu))
                        {
                            saveDir = bootDir + xzdm.Cun + "//村集体";
                        }
                        else
                        {
                            saveDir = bootDir + xzdm.Cun + "//" + xzdm.Zu;
                        }
                        JTSYQDao.ExportZu_TuDiQuanShuLaiYuanZhengMing(jtsyq, saveDir);
                    });
                    MyAction myAction = new MyAction(action, xzdm.CunZu + "土地权属来源证明");
                    actions.Add(myAction);
                }
            }
            return actions;
        }
        public void ExportZu_DangAnDai(IList<JTSYQ> jtsyqGruop, string bootDir)
        {
          
            IList<MyAction> actions = ExportZu_DangAnDaiActions(jtsyqGruop, bootDir);
            SingleTaskForm task = new SingleTaskForm(actions, "导出组档案袋");
        }

        public IList<MyAction> ExportZu_DangAnDaiActions(IList<JTSYQ> jtsyqGruop, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            string saveDir;
            foreach (JTSYQ jtsyq in jtsyqGruop)
            {

                Action action = new Action(() =>
                {
                    if (Utils.IsStrNull(jtsyq.XZDM.Zu))
                    {
                        saveDir = bootDir + jtsyq.XZDM.Cun + "//村集体";
                    }
                    else
                    {
                        saveDir = bootDir + jtsyq.XZDM.Cun + "//" + jtsyq.XZDM.Zu;
                    }
                    JTSYQDao.ExportZu_DangAnDai(jtsyq, saveDir);
                });
                MyAction myAction = new MyAction(action, jtsyq.XZDM.CunZu + "档案袋");
                actions.Add(myAction);
            }
           return actions;
        }


        public void ExportCunDangAnDai(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = ExportCunDangAnDaiActions( cunList,  bootDir);
          
            SingleTaskForm task = new SingleTaskForm(actions,"导出村档案袋");
        }
        public IList<MyAction> ExportCunDangAnDaiActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {

                Action action = new Action(() =>
                {
                    JTSYQDao.ExportCunDangAnDai(xzdm, bootDir + "//"  + xzdm.Cun);

                });
                MyAction myAction = new MyAction(action, xzdm.XiangZheng + xzdm.Cun + "档案袋");
                actions.Add(myAction);
            }
            return actions;
        }
        public void ExportCunJieGuo_GongGao(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = ExportCunJieGuo_GongGaoActions(cunList, bootDir);
            SingleTaskForm task = new SingleTaskForm(actions, "村结果公告");
        }
        public IList<MyAction> ExportCunJieGuo_GongGaoActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {
                Action action = new Action(() =>
                {
                    JTSYQDao.ExportCunJieGuo_GongGao(xzdm, bootDir + "\\" + xzdm.Cun);

                });
                MyAction myAction = new MyAction(action, xzdm.Cun + "结果公告");
                actions.Add(myAction);
            }
            return actions;
        }
        public void ExportCunJieGuo_GongShi(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = ExportCunJieGuo_GongShiActions(cunList, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "村结果公示");
        }
        public IList<MyAction> ExportCunJieGuo_GongShiActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {
                Action action = new Action(() =>
                {
                    JTSYQDao.ExportCunJieGuo_GongShi(xzdm, bootDir + "\\" + xzdm.Cun);

                });
                MyAction myAction = new MyAction(action, xzdm.Cun + "结果公示");
                actions.Add(myAction);
            }
            return actions;
        }
        public void ExportCunYiJianFanKuaiShu(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = ExportCunYiJianFanKuaiShuActions(cunList, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "集体土地所有权公告反馈意见书");
        }
        public IList<MyAction> ExportCunYiJianFanKuaiShuActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {
                Action action = new Action(() =>
                {

                    JTSYQDao.ExportCunYiJianFanKuaiShu(xzdm, bootDir + "\\" + xzdm.Cun);

                });
                MyAction myAction = new MyAction(action, xzdm.Cun + "集体土地所有权公告反馈意见书");
                actions.Add(myAction);

            }
            return actions;
        }
        public void ExportCunYiJianFanKuaiShu_GongShi(IList<XZDM> cunList,string bootDir)
        {
            IList<MyAction> actions = ExportCunYiJianFanKuaiShu_GongShiActions(cunList, bootDir);
            SingleTaskForm task = new SingleTaskForm(actions, "集体土地所有权公示反馈意见书");
        }
        public IList<MyAction> ExportCunYiJianFanKuaiShu_GongShiActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {
                Action action = new Action(() =>
                {
                    JTSYQDao.ExportCunYiJianFanKuaiShu_GongShi(xzdm, bootDir + "\\" + xzdm.Cun);
                });
                MyAction myAction = new MyAction(action, xzdm.Cun + "集体土地所有权公示反馈意见书");
                actions.Add(myAction);
            }
            return actions;
        }

        public void ExportCun_GongGao(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = ExportCun_GongGaoActions(cunList, bootDir);


            SingleTaskForm task = new SingleTaskForm(actions, "村公告");
        }
        public IList<MyAction> ExportCun_GongGaoActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {
                IList<JTSYQ> jtsyqs = XZDMCustom.GetCunZuAllJTSYQ(xzdm);
                if (Utils.CheckListExists(jtsyqs))
                {
                    Action action = new Action(() =>
                    {
                        JTSYQDao.ExportCun_GongGao(jtsyqs, xzdm, bootDir + "\\" + xzdm.Cun);
                    });
                    MyAction myAction = new MyAction(action, xzdm.Cun + "公告");
                    actions.Add(myAction);
                }
            }
            return actions;
        }
        public void ExportCun_GongShi(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = ExportCun_GongShiActions(cunList, bootDir);
         
            SingleTaskForm task = new SingleTaskForm(actions, "村公示");
        }
        public IList<MyAction> ExportCun_GongShiActions(IList<XZDM> cunList, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (XZDM xzdm in cunList)
            {
                IList<JTSYQ> jtsyqs = XZDMCustom.GetCunZuAllJTSYQ(xzdm);
                if (Utils.CheckListExists(jtsyqs))
                {
                    Action action = new Action(() =>
                    {
                        JTSYQDao.ExportCun_GongShi(jtsyqs, xzdm, bootDir + "\\" + xzdm.Cun);
                    });
                    MyAction myAction = new MyAction(action, xzdm.Cun + "公示");
                    actions.Add(myAction);

                }

            }
            return actions;
        }
        public void ExportJZDTable(IList<JTSYQ> jtsyqs, string bootDir)
        {
            IList<MyAction> actions = ExportJZDTableActions(jtsyqs, bootDir);
            SingleTaskForm task = new SingleTaskForm(actions, "导出界址点成果表");
        }

        public IList<MyAction> ExportJZDTableActions(IList<JTSYQ> jtsyqs, string bootDir)
        {
            IList<MyAction> actions = new List<MyAction>();
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                Action action = new Action(() =>
                {
                    JTSYQDao.ExportJZDTable(jtsyq, bootDir + "\\" + jtsyq.XZDM.Cun + "\\" + jtsyq.XZDM.Zu);
                });
                MyAction myAction = new MyAction(action, jtsyq.QLR + "界址点成果表");
                actions.Add(myAction);

            }
            return actions;
        }

        public void ExtractJZD_Intersectant(IList<JTSYQ> jtsyqs)
        {
            //拿到图上所有界址点
            //Dictionary<string, IList<JZD>> jzdDic = JTSYQDao.GetMapJZDS();
            IList<MyAction> actions = new List<MyAction>();
            IList<JZD> jzds;
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                //jzdDic.TryGetValue("", out jzds);
               
                Action action = new Action(() =>
                {

                    foreach(JTSYQ child in jtsyq.GroupJTSYQ)
                    {
                        IList<JZD> newJzds = JTSYQDao.ExtractJZD_Intersectant(child, null);
                    }
                    
                });
                MyAction myAction = new MyAction(action, jtsyq.QLR + "提取界址点");
                actions.Add(myAction);
            }
            SingleTaskForm task = new SingleTaskForm(actions, "提取界址点");
        }

        public void JZDBM(IList<JTSYQ> jtsyqs)
        {
            IList<MyAction> actions = new List<MyAction>();
            //本组可能是多部件
          
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                Action action = new Action(() =>
                {
                
                    int index = 1;
                    foreach(JTSYQ child in jtsyq.GroupJTSYQ)
                    {
                      
                        index += JTSYQDao.JZDBM(index, child, JZDCustom.GetJZDLayer());
                    }
                });
                MyAction myAction = new MyAction(action, jtsyq.QLR + "界址点编码");
                actions.Add(myAction);

            }
            SingleTaskForm task = new SingleTaskForm(actions, "界址点编码");
        }

    
        public void DeleteJZD(IList<JTSYQ> jtsyqs)
        {
            IList<MyAction> actions = new List<MyAction>();
            //本组可能是多部件
            foreach (JTSYQ jtsyq in jtsyqs)
            {
                Action action = new Action(() =>
                {
                    foreach (JTSYQ child in jtsyq.GroupJTSYQ)
                    {
                        JTSYQDao.DeleteJZD(child.JZDS);
                    }
                });
                MyAction myAction = new MyAction(action, jtsyq.QLR + "界址点删除");
                actions.Add(myAction);
            }
            SingleTaskForm task = new SingleTaskForm(actions, "界址点删除");
        }
        /// <summary>
        /// 导出checkbox 中的资料
        /// </summary>
        /// <param name="model"></param>
        /// <param name="xzdms"></param>
        public void Export(ExportDataPageViewModel model,IList<XZDM> xzdms)
        {
            //更新数据
            IList<XZDM> cunXZDM = new List<XZDM>();
            IList<XZDM> zuXZDM = new List<XZDM>();
            IList<JTSYQ> groupJTSYQ = new List<JTSYQ>();


            
            //IList<JTSYQ>
            foreach (XZDM xzdm in xzdms)
            {
                string bm = xzdm.DJZQDM;
                if(bm.Length ==14)
                {
                    zuXZDM.Add(xzdm);
                  
                }
                else
                {
                    cunXZDM.Add(xzdm);
                }
              
                if(xzdm.JTSYQ != null)
                {
                    
                    Dictionary<string, IList<JZD>> jzdDic = MyUtils.Utils.GetGroupDicToList("JTSYQOBJECTID", JZDCustom.GetMapJZD());
                    IList<JZD> jzds;
                    groupJTSYQ.Add(xzdm.JTSYQ);
                    foreach(JTSYQ child in xzdm.JTSYQ.GroupJTSYQ)
                    {
                        if (jzdDic.TryGetValue(child.OBJECTID+"", out jzds))
                        {
                            
                                if (jzdDic.TryGetValue(child.OBJECTID + "", out jzds))
                                {
                                    //注入界址点
                                    child.JZDS = jzds;
                                }
                        }
                    }
                    //设置包含的面积
                   
                }
            }
            List<MyAction> actions = new List<MyAction>();
            if(model.Cun_JiTiTuDiDiaoChaBiao || model.Zu_QuanJiDiaoChaBiao || model.Zu_ShengPiBiao)
            {
                actions.Add(new MyAction(new Action(() =>
                {
                    foreach (JTSYQ jtsyq in groupJTSYQ)
                    {
                        JTSYQCustom.SetContainsFeatureArea(jtsyq);
                    }

                }), "刷新包含的二调面积"));
            }
           

                 
            
            string saveDir = model.SaveDir+"\\";
         
            if(model.Zu_DangAnDai)
            {
                actions.AddRange(ExportZu_DangAnDaiActions(groupJTSYQ, saveDir));
            }
            if (model.Zu_TuDiQuanShuZhengMing)
            {
                actions.AddRange(ExportZu_TuDiQuanShuLaiYuanZhengMingActions(xzdms, saveDir));
            }
            if (model.Zu_FaRenDaiBiaoZhengMing)
            {
                actions.AddRange(ExportZu_FaRenDaiBiaoShenFengZhengMingActions(xzdms, saveDir));
            }
            if (model.Zu_FaRenDaiBiaoWeiTuoShu)
            {
                actions.AddRange(ExportZu_FaRenDaiBiaoWeiTuoShuActions(xzdms, saveDir));
            }
            if (model.Zu_ZhiJieRenZhengMing)
            {
                actions.AddRange(ExportZu_ZhiJieRenShenFengZhengMingActions(xzdms, saveDir));
            }
            if (model.Zu_ShengPiBiao)
            {
                actions.AddRange(ExportZu_ShenPiBiaoActions(xzdms, saveDir));
            }
            if (model.Zu_JieZhiDianChengGuoBiao)
            {
                actions.AddRange(ExportJZDTableActions(groupJTSYQ, saveDir));
            }

            if (model.Zu_QuanJiDiaoChaBiao)
            {
                actions.AddRange(ExportZu_QuanJiDiaoChaBiaoActions(xzdms, saveDir));
            }
            if (model.Zu_RuKuDanZhongDiBiao)
            {
                actions.AddRange(ExportZu_ZhongDiBiaoActions(xzdms, saveDir));
            }
            if (model.Zu_ZhongDiTu)
            {
                actions.AddRange(ExportZu_ZhongDiTuActions(xzdms, saveDir));
            }
            if (model.Cun_DangAnDai)
            {
                actions.AddRange(ExportCunDangAnDaiActions(cunXZDM, saveDir));
            }
            if (model.Cun_GongGaoFanKuiYiJianShu)
            {
                actions.AddRange(ExportCunYiJianFanKuaiShuActions(cunXZDM, saveDir));
            }
            if (model.Cun_GongShiFanKuiYiJianShu)
            {
                actions.AddRange(ExportCunYiJianFanKuaiShu_GongShiActions(cunXZDM, saveDir));
            }
            if (model.Cun_QueQuanJieGuoGongGao)
            {
                actions.AddRange(ExportCunJieGuo_GongGaoActions(cunXZDM, saveDir));
            }
            if (model.Cun_QueQuanJieGuoGongShi)
            {
                actions.AddRange(ExportCunJieGuo_GongShiActions(cunXZDM, saveDir));
            }
            if (model.Cun_GongGaoBiao)
            {
                actions.AddRange(ExportCun_GongShiActions(cunXZDM, saveDir));
            }
            if (model.Cun_GongShiBiao)
            {
                actions.AddRange(ExportCun_GongGaoActions(cunXZDM, saveDir));
            }
            if (model.Cun_JiTiTuDiDiaoChaBiao)
            {
                actions.AddRange(ExportCun_JiTiTuDiDiaoChaBiaoActions(cunXZDM,zuXZDM, saveDir));
            }
            if (model.Cun_GongShiTu)
            {
                actions.AddRange(ExportCun_GongShiTuActions(cunXZDM, saveDir));
            }
            SingleTaskForm task = new SingleTaskForm(actions, "导表");
        }

    }
}
