using ArcGisManager;
using DevExpress.Xpf.Docking;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using JTSYQManager.JTSYQModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ArcMapManager.Pages
{
    public class MapTableCommand : BaseCommand
    {

        public static TabbedGroup tabbedGroup { get; set; }
        public static LayoutPanel dataPage { get; set; }

        private IMapControl3 pMapControl;
        public MapTableCommand()
        {
            base.m_caption = "属性表";
        }
        public override void OnClick()
        {
            openattribute();
        }
        public override void OnCreate(object hook)
        {
            pMapControl = (IMapControl3)hook;
        }
        private void openattribute()
        {
            ILayer pLayer = (ILayer)pMapControl.CustomProperty;
            MapTableCustom.CreateMapDataPage(pLayer);

            if (tabbedGroup.SelectedItem != dataPage)
            {
                for (int a = 0; a < tabbedGroup.Items.Count; a++)
                {
                    var item = tabbedGroup.Items[a];
                    if (item == dataPage)
                    {
                        tabbedGroup.SelectedTabIndex = a;
                    }

                }
            }
            /*
            MyAction myAction = new MyAction(new Action(() => {
              

            }
                ), "打开表格");

            CommHelper.FastTask(myAction);*/

        }
    }
    class LayerScaleCommand : BaseCommand
    {
        private IMapControl3 pMapControl;
        public LayerScaleCommand()
        {
            base.m_caption = "缩放至图层";
        }
        public override void OnClick()
        {
            openattribute();
        }
        public override void OnCreate(object hook)
        {
            pMapControl = (IMapControl3)hook;
        }
        private void openattribute()
        {
            ILayer pLayer = (ILayer)pMapControl.CustomProperty;
            IEnvelope envelope = pLayer.AreaOfInterest;
            ArcGisUtils.axMapControl.Extent = envelope;
            ArcGisUtils.axMapControl.ActiveView.Refresh();

            pLayer = (ILayer)pMapControl.CustomProperty;
            envelope = pLayer.AreaOfInterest;
            ArcGisUtils.axMapControl.Extent = envelope;
            ArcGisUtils.axMapControl.ActiveView.Refresh();
        }
    }
    class ShiftSizOkCommand : BaseCommand
    {
        private static ShiftSizOkCommand shiftSizOkCommand;
        private IMapControl3 pMapControl;
        private IList<IFeature> features;
        private PreviewKeyDownEventArgs siziE;
        private JTSYQ jtsyq;
        public static ShiftSizOkCommand GetInstance(JTSYQ jtsyq, IList<IFeature> features, PreviewKeyDownEventArgs siziE)
        {
            if(shiftSizOkCommand == null)
            {
                shiftSizOkCommand = new ShiftSizOkCommand( jtsyq,  features, siziE);
            }
            return shiftSizOkCommand;
        }
        private ShiftSizOkCommand(JTSYQ jtsyq,IList<IFeature> features, PreviewKeyDownEventArgs siziE)
        {
           string basic= "四至确定";
            switch(siziE.KeyData)
            {
                case Keys.Up:
                    base.m_caption = basic + ":北";
                    break;
                    case Keys.Right:

                    base.m_caption = basic + ":东";
                    break;
                case Keys.Down:

                    base.m_caption = basic + ":南";
                    break;
                case Keys.Left:
                    base.m_caption = basic + ":西";
                    break;
            }
           
            this.features = features;
            this.siziE = siziE;
        }
        public override void OnClick()
        {
            ok();
        }
        public override void OnCreate(object hook)
        {
            pMapControl = (IMapControl3)hook;
        }
        private void ok()
        {
            if (siziE == null || jtsyq == null)
            {
                return;
            }
            JTSYQCustom jTSYQCustom = new JTSYQCustom(jtsyq);
            Dictionary<string, IList<IFeature>> featuresDic = ArcGisUtils.GetFeatureDicByLayer(features);
            //其他集体所有权
            IList<IFeature> siziFeatures;
            if (featuresDic.TryGetValue(JTSYQCustom.JTSYQLayerName, out siziFeatures))
            {
                IList<JTSYQ> sizeJTSYQS = JTSYQCustom.FeaturesToJTSYQ(siziFeatures);
                jTSYQCustom.SetSiZi(siziE, sizeJTSYQS);

            }
            jTSYQCustom.ArcGisSave();
        }
    }
}
