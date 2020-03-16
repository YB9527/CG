using ArcGisManager;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using HeibernateManager.Model;
using JTSYQManager.JTSYQModel;
using Public;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ArcMapManager.Pages
{
    public partial class MapForm : Form
    {

        public static string JTSYQMxdPathReids = "JTSYQMxdPath";
        public static string MXDFilter = "工程文件 file(*.mxd)|*.mxd";
        private static MapDocumentClass mapDocument ;
        private IToolbarMenu m_menuMap = null;
        public static JTSYQ CurrentJTSYQ { get; set; }
        public MapForm()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            InitializeComponent();

            m_menuMap = new ToolbarMenuClass();

            //添加事件
            AddEnvet();
            mapControlTemp = this.axMapControl1;
            ArcGisUtils.axMapControl = this.axMapControl1;
          
            ArcGisUtils.axPageLayoutControl = PageLayoutForm.GetInstance().GetAxPageLayoutControl();
            //检查数据库以前是否有
            SoftwareConfig mxdConfig = SoftwareConfig.FindConfig(JTSYQMxdPathReids);
            if(mxdConfig != null)
            {
                if(this.axMapControl1.CheckMxFile(mxdConfig.Value))
                {
                    LoadMXD(mxdConfig.Value);
                    ArcGisUtils.MXDPath = mxdConfig.Value;
                }
                else
                {
                    SoftwareConfig.Delete(JTSYQMxdPathReids);
                }
            }
        }
      
     

        //右键菜单
        //  private ITOCControl2 pTocControl;
        private IMapControl3 pMapControl;
        // private IToolbarMenu pToolMenuMap;
        private IToolbarMenu pToolMenuLayer;
        private void rightMenu()
        {
            // 取得 MapControl 和 PageLayoutControl 的引用   
            ///  pTocControl = (ITOCControl2)axTOCControl1.Object;
            pMapControl = (IMapControl3)axMapControl1.Object;
            // 创建菜单   
            ///pToolMenuMap = new ToolbarMenuClass();
            pToolMenuLayer = new ToolbarMenuClass();         
            pToolMenuLayer.AddItem(new MapTableCommand(), -1, 0, true, esriCommandStyles.esriCommandStyleTextOnly);
            pToolMenuLayer.AddItem(new LayerScaleCommand(), -1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);
            pToolMenuLayer.SetHook(pMapControl);

        }
        /// <summary>
        /// 添加事件
        /// </summary>
        private void AddEnvet()
        {
           
        }
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button != 2) return;
            rightMenu();
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null; ILayer layer = null;
            object other = null; object index = null;
            this.axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                this.axTOCControl1.SelectItem(map, null);
            else if (item == esriTOCControlItem.esriTOCControlItemLayer)
                this.axTOCControl1.SelectItem(layer, null);
            axMapControl1.CustomProperty = layer;
            if (item == esriTOCControlItem.esriTOCControlItemMap)
            {
                System.Windows.MessageBox.Show("点击了Layers");
            }
            if (item == esriTOCControlItem.esriTOCControlItemLayer)
            {
                pToolMenuLayer.PopupMenu(e.x, e.y, this.axTOCControl1.hWnd);
            }
        }
        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="mxdFile"></param>
        public void LoadMXD(string mxdFile)
        {
            //axMapControl1.LoadMxFile(mxdFile);
             mapDocument = new MapDocumentClass();
            
            mapDocument.Open(mxdFile);
            axMapControl1.Map = mapDocument.Map[0];
            axMapControl1.Refresh();
            //mxdfile 路径 写入数据库
            SoftwareConfig.Refresh(JTSYQMxdPathReids, mxdFile);
        }
        /// <summary>
        /// 工程文件保存
        /// </summary>
        public static void MXDSave()
        {
            mapDocument.Save();
        }
        private static ESRI.ArcGIS.Controls.AxMapControl mapControlTemp;
        public static ESRI.ArcGIS.Controls.AxMapControl GetAxMapControl()
        {
            return mapControlTemp;
        }

        private static MapForm mapForm;
        /// <summary>
        /// 得到窗口对象
        /// </summary>
        /// <returns></returns>
        public static MapForm GetInstance()
        {
            if (mapForm == null)
            {
                mapForm = new MapForm();
            }
            return mapForm;
        }
        private GridControl JTSYQGridControl;
        public void SetJTSYQGridControl(GridControl gridControl)
        {
            JTSYQGridControl = gridControl;
        }
        /// <summary>
        /// 第二次进来，会移除以前的
        /// </summary>
        /// <param name="mapGrid"></param>
        public static void AddParentGrid(Grid mapGrid)
        {
            if (mapGrid.Children.Count > 0)
            {
                mapGrid.Children.RemoveRange(0, mapGrid.Children.Count);

            }
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            MapForm mapForm = MapForm.GetInstance();
            mapForm.Width = (int)mapGrid.ActualWidth;
            mapForm.Height = (int)mapGrid.ActualHeight;
            windowsFormsHost.Width = mapGrid.Width;
            windowsFormsHost.Height = mapGrid.Height;
            mapForm.TopLevel = false;
            windowsFormsHost.Child = mapForm;
            mapGrid.Children.Add(windowsFormsHost);
        }

        private void MapForm_Load(object sender, System.EventArgs e)
        {

        }
        public static int MapMouseDownFlag = 0;
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            IGeometry geometry;
            switch (MapMouseDownFlag)
            {
                case 3:
                    IFeatureLayer tufulayer = ArcGisUtils.GetFeatureLayer("TuFu");
                    if (tufulayer != null)
                    {
                        IFeatureClass tufuFeatureClass = tufulayer.FeatureClass;
                        geometry = axMapControl1.TrackRectangle();
                        TuFu.CreateTuFuFeautre(geometry.Envelope, tufuFeatureClass);
                        axMapControl1.ActiveView.Refresh();
                    }
                    break;  
            }

           
        }

     

        public  static void DeleteTuFuKuang()
        {
            IFeatureLayer tufulayer = ArcGisUtils.GetFeatureLayer("TuFu");
            if(tufulayer != null)
            {
                IFeatureClass tufuFeatureClass = tufulayer.FeatureClass;
                IFeatureCursor curs = tufuFeatureClass.Update(new QueryFilterClass(), true);
                IFeature feautre = curs.NextFeature();
                while (feautre != null)
                {
                    curs.DeleteFeature();
                    feautre = curs.NextFeature();

                }
                mapForm.axMapControl1.ActiveView.Refresh();
            }
            else
            {
                MessageBox.Show("没有这个图层：TuFu");
            }
        }


        private void axMapControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //做四至   
           if(MapMouseDownFlag ==4)
            {
                if(e.KeyData == Keys.Up || e.KeyData ==  Keys.Right|| e.KeyData== Keys.Down|| e.KeyData == Keys.Left)
                {
                    SetJTSYQSiZi(e);
                }
                
            }
        }

        /// <summary>
        /// 设置多个四至
        /// </summary>
        /// <param name="features"></param>
        private void SetJTSYQSiZi(PreviewKeyDownEventArgs e)
        {
           

            if(CurrentJTSYQ == null)
            {
                return;
            }
            IList<IFeature> features = ArcGisUtils.GetInstance().GetSelectFeature();
            if(!MyUtils.Utils.CheckListExists(features))
            {
                return;
            }
            JTSYQCustom jTSYQCustom = new JTSYQCustom(CurrentJTSYQ);
            Dictionary<string, IList<IFeature>> featuresDic = ArcGisUtils.GetFeatureDicByLayer(features);
            //其他集体所有权
            IList<IFeature> siziFeatures;
            if (featuresDic.TryGetValue(JTSYQCustom.JTSYQLayerName, out siziFeatures))
            {
                IList<JTSYQ> siziJTSYQS = JTSYQCustom.FeaturesToJTSYQ(siziFeatures);
                if (!MyUtils.Utils.CheckListExists(siziJTSYQS))
                {
                    return;
                }
                //移除自身地块
                int count = siziJTSYQS.Count;
                for (int a= 0; a < count; a ++)
                {
                    JTSYQ round = siziJTSYQS[a];
                    if (round.Feature.OID == CurrentJTSYQ.Feature.OID)
                    {
                        siziJTSYQS.RemoveAt(a);
                        count--;
                        a--;
                    }
                }
                jTSYQCustom.SetSiZi(e, siziJTSYQS);
            }
            
            jTSYQCustom.ArcGisSave();
         
            //更新显示的table中 的josn数据

        }
    }
}
