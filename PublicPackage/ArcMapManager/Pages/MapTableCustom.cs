using ArcGisManager;
using ArcMapManager.Views;
using DevExpress.Xpf.WindowsUI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.Windows;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using stdole;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;
using System;

namespace ArcMapManager.Pages
{
    public class MapTableCustom
    {
        public static Dictionary<ILayer, PagerPage> userControlReids = new Dictionary<ILayer, PagerPage>();
        /// <summary>
        /// 创建 dataPage页面，首先要传入  装载页面的容器 PageView，这个方法有缓存
        /// </summary>
        /// <param name="pLayer"></param>
        /// <returns></returns>
        public static PagerPage CreateMapDataPage(ILayer pLayer)
        {
            PagerPage page;
            if (userControlReids.TryGetValue(pLayer, out page))
            {
                for (int a = 0; a < pageView.Items.Count; a++)
                {
                    if ((pageView.Items[a] as PageViewItem).Header.Equals(pLayer.Name))
                    {
                        //pageView.SelectedIndex = a;
                        if (a != pageView.Items.Count - 1)
                        {
                            var temp = pageView.Items[a];
                            pageView.Items.RemoveAt(a);
                            pageView.Items.Add(temp);
                            pageView.SelectedIndex = pageView.Items.Count - 1;
                            //pageView.Items.Insert(0,temp);
                            // pageView.SelectedIndex = 0;
                        }
                        break;
                    }
                }
                return page;
            }
            IList<IFeature> features = ArcGisUtils.GetFeatures(pLayer);
            page = CreateMapDataPage(pLayer, features);
            userControlReids.Add(pLayer, page);
            return page;
        }
        /// <summary>
        /// 装载页面的容器
        /// </summary>
        private static PageView pageView;
        public static void SetPageSource(PageView pageView1)
        {
            pageView = pageView1;
        }
        /// <summary>
        /// 创建 dataPage页面，首先要传入  装载页面的容器 PageView，这个方法  没有 缓存
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        private static PagerPage CreateMapDataPage(ILayer pLayer, IList<IFeature> features)
        {

            PageViewItem pageViewItem = new PageViewItem();
            pageViewItem.Header = pLayer.Name;
            DataGrid dataGrid = new DataGrid();
            pageView.Items.Add(pageViewItem);

            //pageView.Items.Insert(0, pageViewItem);
            pageView.SelectedIndex = pageView.Items.Count - 1;

            PagerPage pagerPage = GetPagerPage(pLayer, features);
            pageViewItem.Padding = new Thickness(0);
            pageViewItem.Margin = new Thickness(3, 0, 3, 0);



            pageViewItem.Content = pagerPage;
            //  pageViewItem.MouseDoubleClick += PageViewItem_DBClick;

            return pagerPage;

        }
        
        /// <summary>
        ///设置PageViewItem 的内容
        /// </summary>
        /// <param name="pageViewItem"></param>
        /// <param name="features"></param>
        private static PagerPage GetPagerPage(ILayer pLayer, IList<IFeature> features)
        {
            IList<MapTabDictoryCustom> datas = new List<MapTabDictoryCustom>();



            IFields fs = (pLayer as IFeatureLayer).FeatureClass.Fields;
            IList<FieldCustom> fieldCustoms = new List<FieldCustom>();
            for (int a = 0; a < fs.FieldCount; a++)
            {
                IField field = fs.Field[a];


                if (!field.AliasName.Equals("Shape"))
                {
                    FieldCustom fieldCustom = new FieldCustom { Index = a, Editable = field.Editable, AliasName = field.AliasName, Name = field.Name };

                    fieldCustoms.Add(fieldCustom);
                }
               

            }
            IList<JToken> jTokens = new List<JToken>();
            Dictionary<string, object> dic;
            foreach (IFeature feature in features)
            {
                dic = new Dictionary<string, object>();
                foreach (FieldCustom fieldCustom in fieldCustoms)
                {

                    dic.Add(fieldCustom.AliasName, feature.Value[fieldCustom.Index]);
                }

                //JToken jToken = JToken.FromObject(dic);
                //jTokens.Add(jToken);

                MapTabDictoryCustom data = new MapTabDictoryCustom { Dic = dic };
                datas.Add(data);
            }
            PagerPage mapTablePagerPage = new MapTablePagerPage(features, pLayer);

            mapTablePagerPage.SetPagerPage(datas, fieldCustoms);
           
            return mapTablePagerPage;
          
            //return PagerPage.GetPagerPage<TestGrid>(PagerPage.GetList(110), fieldCustoms);
            // return PagerPage.GetPagerPage<IFeature>(features, fieldCustoms);
        }

        

        /// <summary>
        /// 双击移除 PageViewItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PageViewItem_DBClick(object sender, MouseButtonEventArgs e)
        {
            // pageView.Items.Remove(sender as PageViewItem);
        }
    }
}
