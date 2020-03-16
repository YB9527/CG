using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtils
{

    public class PrintHelper : PrintDocument
    {
        public List<Image> ListImage = new List<Image>();
        bool m_bUseDefaultPaperSetting = false;
        public PrintHelper()
        {
            this.PrintPage += new PrintPageEventHandler(this.PageDraw); this.PrintController = new StandardPrintController();
        }        /// <summary>        /// 单位为毫米        /// </summary>       
        public int Top
        {
            get { return (int)(this.DefaultPageSettings.Margins.Top / 100f * 25.4f); }
            set { this.DefaultPageSettings.Margins.Top = (int)(value / 25.4 * 100); }
        }
        public int Left { get { return (int)(this.DefaultPageSettings.Margins.Left / 100f * 25.4f); } set { this.DefaultPageSettings.Margins.Left = (int)(value / 25.4 * 100); } }
        public int Bottom { get { return (int)(this.DefaultPageSettings.Margins.Bottom / 100f * 25.4f); } set { this.DefaultPageSettings.Margins.Bottom = (int)(value / 25.4 * 100); } }
        public int Right { get { return (int)(this.DefaultPageSettings.Margins.Right / 100f * 25.4f); } set { this.DefaultPageSettings.Margins.Right = (int)(value / 25.4 * 100); } }              /*设置打印机名*/
        public string PrinterName { get { return this.PrinterSettings.PrinterName; } set { this.PrinterSettings.PrinterName = value; } }
        /// <summary>        /// 纸张        /// </summary>    
        public PaperSize Paper
        {
            get { return this.DefaultPageSettings.PaperSize; }
            set
            {
                PaperSize p = null; foreach (PaperSize ps in this.PrinterSettings.PaperSizes)
                {
                    if (ps.PaperName.Equals(value))//这里设置纸张大小,但必须是定义好的                       
                        p = ps;
                }
                this.DefaultPageSettings.PaperSize = p;
            }
        }
        /// <summary>        /// 打印方向        /// </summary>        
        public bool Direction { get { return this.DefaultPageSettings.Landscape; } set { this.DefaultPageSettings.Landscape = value; } }
        /*设置是否使用缺省纸张*/
        public bool UseDefaultPaper
        {
            get { return m_bUseDefaultPaperSetting; }
            set
            {
                m_bUseDefaultPaperSetting = value; if (!m_bUseDefaultPaperSetting)
                {                    //如果不适用缺省纸张则创建一个自定义纸张，注意，必须使用这个版本的构造函数才是自定义的纸张                   
                    PaperSize ps = new PaperSize("Custom Size 1", 827, 1169);                    //将缺省的纸张设置为新建的自定义纸张                    
                    this.DefaultPageSettings.PaperSize = ps;
                }
            }
        }                /*纸张宽度 单位定义为毫米mm*/
        public double PaperWidth
        {
            get { return this.DefaultPageSettings.PaperSize.Width / 100f * 25.4f; }
            set
            {                //注意，只有自定义纸张才能修改该属性，否则将导致异常           
                if (this.DefaultPageSettings.PaperSize.Kind == PaperKind.Custom)
                    this.DefaultPageSettings.PaperSize.Width = (int)(value / 25.4 * 100);
            }
        }        /*纸张高度 单位定义为毫米mm*/
        public double PaperHeight
        {
            get { return (int)this.PrinterSettings.DefaultPageSettings.PaperSize.Height / 100f * 25.4f; }
            set
            {                //注意，只有自定义纸张才能修改该属性，否则将导致异常              
                if (this.DefaultPageSettings.PaperSize.Kind == PaperKind.Custom) this.DefaultPageSettings.PaperSize.Height = (int)(value / 25.4 * 100);
            }
        }
        private void PageDraw(object sender, PrintPageEventArgs e)
        {
            double widthT = this.PaperWidth / 25.4 * 100 - Left * 2 - 40; double heightT = 0; Image image = ListImage[0];
            Bitmap map = (Bitmap)image; heightT = widthT / map.Width * map.Height;
            if ((heightT * ListImage.Count + (ListImage.Count - 1) * 40) > (this.PaperHeight / 25.4 * 100 - Top - Bottom))
            {
                heightT = (this.PaperHeight / 25.4 * 100 - Top - Bottom - (ListImage.Count - 1) * 40) / ListImage.Count;
                widthT = heightT / map.Height * map.Width;
            }
            for (int i = 0; i < ListImage.Count; i++)
            {
                Image ima = ListImage[i]; Bitmap bmp = (Bitmap)ima;
                Bitmap mapT = GetThumbnail(bmp, (int)widthT, (int)heightT); e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half; e.Graphics.DrawImage(mapT, Left, Top + i * (mapT.Height + 40));
            }
        }
        //public void InitSetting(CPrintParameter cparameter)
        //{
        //    this.PrinterName = cparameter.PrintorName;
        //    this.Paper = new PaperSize(cparameter.Paper, (int)cparameter.Width, (int)cparameter.Height); this.Direction = Convert.ToBoolean(cparameter.Direction); this.Left = cparameter.Left; this.Top = cparameter.Top; this.Right = cparameter.Right; this.Bottom = cparameter.Bottom;
        //}
        /// <summary>        /// 图片缩放        /// </summary>       
        /// <param name="bmp">图片</param>     
        /// /// <param name="width">目标宽度，若为0，表示宽度按比例缩放</param>     
        /// /// <param name="height">目标长度，若为0，表示长度按比例缩放</param>    
        public Bitmap GetThumbnail(Bitmap bmp, int width, int height)
        {
            if (width == 0) { width = height * bmp.Width / bmp.Height; }
            if (height == 0) { height = width * bmp.Height / bmp.Width; }
            Image imgSource = bmp; Bitmap outBmp = new Bitmap(width, height); Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);            // 设置画布的描绘质量             
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality; g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgSource, new Rectangle(0, 0, width, height + 1), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel); g.Dispose();
            imgSource.Dispose(); bmp.Dispose(); return outBmp;
        }

    }
}
