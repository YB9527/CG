using System.Windows;
using System.Windows.Controls;

namespace ArcMapManager.Pages
{
    /// <summary>
    /// MapFormCustom.xaml 的交互逻辑
    /// </summary>
    public partial class MapFormCustom : UserControl
    {
        private static MapFormCustom mapFormCustom;
        private MapFormCustom()
        {
            InitializeComponent();
            MapForm.AddParentGrid(mapGrid);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MapForm.AddParentGrid(mapGrid);
        }

        public static MapFormCustom GetInstance()
        {
           if(mapFormCustom == null)
            {
                mapFormCustom = new MapFormCustom();
            }
            return mapFormCustom;
        }
        



    }
}
