using ArcGisManager;
using ArcMapManager.Pages.Views;
using ArcMapManager.Views;
using DevExpress.Xpf.Grid;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using JTSYQManager.JTSYQModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ArcMapManager.Pages
{
    public class MapTablePagerPage : PagerPage
    {
        public MapTablePagerPage()
        {

        }
        private int objectIndex;
        private Dictionary<int, IFeature> featureDic;
        private ESRI.ArcGIS.Carto.ILayer pLayer;
        /// <summary>
        /// Feature 必须有 OBJECTID  int类型字段 
        /// </summary>
        /// <param name="features"></param>
        public MapTablePagerPage(IList<IFeature> features, ESRI.ArcGIS.Carto.ILayer pLayer)
        {
            this.pLayer = pLayer;
            featureDic = new Dictionary<int, IFeature>();
            if (features != null)
            {

                try
                {
                    IFields fields = features[0].Fields;
                    for (int a = 0; a < fields.FieldCount; a++)
                    {
                        if (fields.Field[a].AliasName.StartsWith("OBJECTID"))
                        {
                            if (!fields.Field[a].Editable)
                            {
                                objectIndex = a;
                            }
                        }
                    }

                    foreach (IFeature feature in features)
                    {
                        featureDic.Add((int)feature.Value[objectIndex], feature);

                    }
                }
                catch
                {
                    MessageBox.Show("没有这个字段，请添加：且内容必须唯一：OBJECTID");

                }
            }
        }
        /// <summary>
        /// 表格双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Table_Click(object sender, MouseButtonEventArgs e)
        {
            GridControl gridControl = sender as GridControl;
            GridColumn column = gridControl.CurrentColumn as GridColumn;
            MapTabDictoryCustom row = gridControl.CurrentItem as MapTabDictoryCustom;
           
            if (column.VisibleIndex == objectIndex)
            {
            }
            //图形缩放
            var obj = gridControl.CurrentItem;
            int objectId = (int)row.Dic["OBJECTID"];
            IFeature feature = featureDic[objectId];
            if (pLayer.Name == JTSYQCustom.JTSYQLayerName)
            {
                MapForm.CurrentJTSYQ = JTSYQCustom.FeaturesToJTSYQ(feature);
                MapForm.CurrentJTSYQ.MapTabDictoryCustom = row;
            }
            ArcGisUtils.ExtentShp(feature, pLayer, 2);

        }
        /// <summary>
        /// 表格单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Table_SingleClick(object sender, MouseButtonEventArgs e)
        {
            GridControl gridControl = sender as GridControl;
            GridColumn column = gridControl.CurrentColumn as GridColumn;
            MapTabDictoryCustom row = gridControl.CurrentItem as MapTabDictoryCustom;

            if (column.VisibleIndex == objectIndex)
            {
            }
            //图形缩放
            var obj = gridControl.CurrentItem;
            if(obj == null)
            {
                return;
            }
            int objectId = (int)row.Dic["OBJECTID"];
            IFeature feature = featureDic[objectId];
            if (pLayer.Name == JTSYQCustom.JTSYQLayerName)
            {
                MapForm.CurrentJTSYQ = JTSYQCustom.FeaturesToJTSYQ(feature);
                MapForm.CurrentJTSYQ.MapTabDictoryCustom = row;
            }

            ArcGisUtils.axMapControl.Map.ClearSelection();
            ArcGisUtils.axMapControl.Map.SelectFeature(pLayer, feature);//第三个参数为是否只选中一个
            ArcGisUtils.axMapControl.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null); //选中要素高亮显示
            ArcGisUtils.axMapControl.ActiveView.Refresh();
        }
    }
}
