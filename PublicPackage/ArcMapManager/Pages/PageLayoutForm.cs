using ESRI.ArcGIS.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcMapManager.Pages
{
    public partial class PageLayoutForm : Form
    {
       
        private static PageLayoutForm pageLayoutForm;


        public PageLayoutForm()
        {
            this.InitializeComponent();

            //this.axMapControl1.LoadMxFile(srcPath);

        }


        public AxPageLayoutControl GetAxPageLayoutControl()
        {
            return this.axPageLayoutControl1;
        }


        public static PageLayoutForm GetInstance()
        {
            bool flag = PageLayoutForm.pageLayoutForm == null;
            if (flag)
            {
                PageLayoutForm.pageLayoutForm = new PageLayoutForm();
            }
            return PageLayoutForm.pageLayoutForm;
        }

    }
}
