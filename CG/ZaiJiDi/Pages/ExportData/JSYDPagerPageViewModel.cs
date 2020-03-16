using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTemplate.Views;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Pages.ZJDPage.ExportData
{
    public class JSYDPagerPageViewModel:NotifyPropertyChanged
    {

        public static IList<FieldCustom> GetFieldCustoms()
        {
            IList<FieldCustom> list = new List<FieldCustom>();
            list.Add(new FieldCustom { AliasName = "宗地编码", Name = "ZDNUM", Width = 150 });
            list.Add(new FieldCustom { AliasName = "权利人", Name = "QLRMC", Width = 150 });
            list.Add(new FieldCustom { AliasName = "备注", Name = "BZ", Width = 150 });
            return list;
        }
        public JSYDPagerPageViewModel(JSYD jsyd)
        {
            ZDNUM = jsyd.ZDNUM;
            QLRMC = "";
            if(jsyd.HZs != null)
            {
                foreach (JTCY jtcy in jsyd.HZs)
                {

                    QLRMC = jtcy.XM +"、"+ QLRMC;
                }
                if(QLRMC.Length >0 )
                {
                    QLRMC =  QLRMC.Remove(QLRMC.Length-1,1);

                }
                bz = jsyd.BZ;  
            }
            //IsChecked = true;
            this.JSYD = jsyd;
        }
        public JSYD JSYD { get; set; }
        

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged("isChecked");
            }
        }

        private string bz;
        public string BZ
        {
            get
            {
                return bz;
            }
            set
            {
                bz = value;
                OnPropertyChanged("bz");
            }
        }
        private string zdnum;
        public string ZDNUM
        {
            get
            {
                return zdnum;
            }
            set
            {
                zdnum = value;
                OnPropertyChanged("zdnum");
            }
        }
        private string qlrmc;
        public string QLRMC
        {
            get
            {
                return qlrmc;
            }
            set
            {
                qlrmc = value;
                OnPropertyChanged("qlrmc");
            }
        }

        internal static IList<JSYDPagerPageViewModel> GetViewModel(IList<JSYD> jsyds)
        {
            IList<JSYDPagerPageViewModel> list = new List<JSYDPagerPageViewModel>();
            foreach(JSYD jsyd in jsyds)
            {
                list.Add(new JSYDPagerPageViewModel(jsyd));
            }
            return list;
        }
    }
}
