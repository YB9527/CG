using Aspose.Words;
using Spire.Pdf;
using Spire.Xls;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
namespace MyUtils
{
    public class PrintUtils
    {
        private static string[] ImageFormatNameArray = new string[]
        {
            "jpg","png","jpeg"
        };
    

        /// <summary>
        /// 打印excel
        /// </summary>
        /// <param name="path"></param>
        /// <param name="copies"></param>
        /// <param name="horizontal"></param>
        public static void PrintXLS(string path, int copies, Duplex horizontal = Duplex.Simplex)
        {
            var tempbook = new Workbook();
            tempbook.LoadFromFile(path);
            var document = tempbook.PrintDocument;
            Print(document, path, copies, horizontal);

        }

        /// <summary>
        /// doc转为pdf打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="horizontal"></param>
        public static void DocToPDFPrint(string path, int copies=1, Duplex horizontal = Duplex.Simplex, string range = null)
        {
            string descPath = "c:/" + Path.GetFileNameWithoutExtension(path);
            WordToPDF(path, descPath);
            PrintPDF(descPath, copies, horizontal,  range);


            File.Delete(descPath);

        }

        static Word.Application application = new Word.Application();
        static int a = 0;
        public static void WordToPDF(string sourcePath, string descPath)
        {
            Word.Document document = null;
            try
            {
                if(a++ ==0 )
                {
                    application.Visible = false;
                }
               
                document = application.Documents.Open(sourcePath);
                document.ExportAsFixedFormat(descPath, Word.WdExportFormat.wdExportFormatPDF);
            }
            finally
            {
                document.Close();
            }

        }
        /// <summary>
        /// 打印PDF
        /// </summary>
        /// <param name="path"></param>
        /// <param name="horizontal"></param>
        public static void PrintPDF(string path, int Copies = 1, Duplex horizontal = Duplex.Simplex, string range = null)
        {
            PdfDocument pdf = new PdfDocument(path);//Set the printer
            PrintDocument print = pdf.PrintDocument;
           
            Print(print, path, Copies,horizontal, range);

        }
        /// <summary>
        /// word打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="horizontal"></param>
        public static void PrintDoc(string path, int copies = 1, Duplex horizontal = Duplex.Simplex)
        {
            Document document = new Document(path);
            PrinterSettings settings = new PrinterSettings();
            settings.Copies = (short)copies;
            if (settings.CanDuplex)
            {
                settings.Duplex = horizontal;

            }
            document.Print(settings);
        }
        /// <summary>
        /// 执行打印
        /// </summary>
        /// <param name="print"></param>
        /// <param name="path"></param>
        /// <param name="horizontal"></param>
        private static void Print(PrintDocument print, string path, int Copies = 1, Duplex horizontal = Duplex.Simplex, string range = null)
        {
            print.DocumentName = Path.GetFileName(path);
            PrinterSettings settings = print.PrinterSettings;
            settings.Copies = (short)Copies;
            if (settings.CanDuplex)
            {
                settings.Duplex = horizontal;

            }
            else
            {
                settings.Duplex = Duplex.Simplex;
            }
            if (!Utils.IsStrNull(range))
            {
                if (range.Contains("-"))
                {
                    string[] array = range.Split('-');
                    int[] pageArray = new int[array.Length];
                    for (int a = 0; a < array.Length; a++)
                    {
                        if (Utils.IsInt(array[a]))
                        {
                            pageArray[a] = int.Parse(array[a]);
                        }
                        else
                        {
                            MessageBox.Show("你的输入有不是数字");
                            return;
                        }
                    }

                    settings.FromPage = pageArray[0];
                    settings.ToPage = pageArray[1];
                    print.PrinterSettings.PrintRange = PrintRange.SomePages;
                    print.Print();

                }
                if (range.Contains("、"))
                {
                    string[] array = range.Split('、');
                    int[] pageArray = new int[array.Length];
                    for (int a = 0; a < array.Length; a++)
                    {
                        if (Utils.IsInt(array[a]))
                        {
                            pageArray[a] = int.Parse(array[a]);
                        }
                        else
                        {
                            MessageBox.Show("你的输入有不是数字");
                            return;
                        }
                    }
                    foreach (int pageInt in pageArray)
                    {
                        settings.FromPage = pageInt;
                        settings.ToPage = pageInt+1;
                        print.PrinterSettings.PrintRange = PrintRange.SomePages;
                        print.Print();
                    }
                }


            }
            else
            {
                print.Print();
            }

        }
        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="horizontal"></param>
        public static void PrintPicture(string path, int copies,Duplex horizontal = Duplex.Simplex)
        {/*
            PrintHelper printHelper = new PrintHelper();
            PrinterSettings settings = printHelper.PrinterSettings;
            if (settings.CanDuplex)
            {
                settings.Duplex = horizontal;

            }
            else
            {
                settings.Duplex = Duplex.Simplex;
            }
            Image image = Image.FromFile(path);
            printHelper.ListImage.Add(image);
            image = Image.FromFile(path);
            printHelper.ListImage.Add(image);
            printHelper.DocumentName = Path.GetFileName(path);
            printHelper.Print();*/
            for(int a =0; a < copies;a++)
            RelativePrint(path);

        }
        /// <summary>
        /// 根据格式 自动识别打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="horizontal"></param>
        /// <returns></returns>
        public static bool Print(string path,int copies, Duplex horizontal = Duplex.Simplex)
        {
            if (Utils.CheckFileExists(path))
            {
                string exName = Path.GetExtension(path).ToLower().Remove(0, 1);

                if (exName.Contains("doc"))
                {
                    PrintDoc(path, copies, horizontal);
                }
                if (exName.Contains("xls"))
                {
                    PrintXLS(path, copies, horizontal);
                }
                else if (exName.Equals("pdf"))
                {
                    PrintPDF(path, copies, horizontal);
                }
                else if (Array.IndexOf(ImageFormatNameArray, exName) != -1)
                {

                    PrintPicture(path,  copies, horizontal);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 调用本地打印 打印之后休眼5s
        /// </summary>
        /// <param name="path"></param>
        public static void RelativePrint(string path)
        {
            Process pro = new Process();
            pro.StartInfo.FileName = path;//文件路径
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pro.StartInfo.Verb = "Print";
            pro.Start();
            Thread.Sleep(300);
        }

    }
}
