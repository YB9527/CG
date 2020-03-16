using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GISClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcMapManager.Pages.Server
{
    /// <summary>
    /// MapOnServerPage.xaml 的交互逻辑
    /// </summary>
    public partial class MapOnServerPage : UserControl
    {
        public MapOnServerPage()
        {
            InitializeComponent();
            Fun();
        }

        private void Fun()
        {
            //获得服务对象名称
            /*
            IAGSServerObjectName pServerObjectName = GetMapServer("http://services.arcgisonline.com/ArcGIS/services", "ESRI_Imagery_World_2D", false);
            IName pName = (IName)pServerObjectName;
            //访问地图服务
            IAGSServerObject pServerObject = (IAGSServerObject)pName.Open();
            IMapServer pMapServer = (IMapServer)pServerObject;

            ESRI.ArcGIS.Carto.IMapServerLayer pMapServerLayer = new ESRI.ArcGIS.Carto.MapServerLayerClass();
            //连接地图服务

            pMapServerLayer.ServerConnect(pServerObjectName, pMapServer.DefaultMapName);
            //添加数据图层
             
            axMapControl1.AddLayer(pMapServerLayer as ILayer);

            axMapControl1.Refresh();*/


        }
        public IAGSServerObjectName GetMapServer(string pHostOrUrl, string pServiceName, bool pIsLAN)
        {


            //设置连接属性
            IPropertySet pPropertySet = new PropertySetClass();
            if (pIsLAN)
                pPropertySet.SetProperty("machine", pHostOrUrl);
            else
                pPropertySet.SetProperty("url", pHostOrUrl);

            //打开连接

            IAGSServerConnectionFactory pFactory = new AGSServerConnectionFactory();
            //Type factoryType = Type.GetTypeFromProgID(
            //    "esriGISClient.AGSServerConnectionFactory");
            //IAGSServerConnectionFactory agsFactory = (IAGSServerConnectionFactory)
            //    Activator.CreateInstance(factoryType);
            IAGSServerConnection pConnection = pFactory.Open(pPropertySet, 0);

            //Get the image server.
            IAGSEnumServerObjectName pServerObjectNames = pConnection.ServerObjectNames;
            pServerObjectNames.Reset();
            IAGSServerObjectName ServerObjectName = pServerObjectNames.Next();
            while (ServerObjectName != null)
            {
                if ((ServerObjectName.Name.ToLower() == pServiceName.ToLower()) &&
                    (ServerObjectName.Type == "MapServer"))
                {

                    break;
                }
                ServerObjectName = pServerObjectNames.Next();
            }

            //返回对象
            return ServerObjectName;
        }
    }
}
