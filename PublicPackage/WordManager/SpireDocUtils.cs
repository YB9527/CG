using Aspose.Words;
using ExcelManager;
using ExcelManager.Pages.Replace;
using FileManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils = MyUtils.Utils;
namespace WordManager
{
    public class SpireDocUtils
    {

        //public static string GetCellValue(TableCell cell)
        //{
        //    Paragraph p = cell.FirstChild as Paragraph;
        //    string value = p.Text;
        //    return value;
        //}



        /// <summary>
        /// 替换word中的内容
        /// </summary>
        /// <param name="replaceViewModels"></param>
        /// <param name="paths"></param>
        public static void ReplaceText(IList<ReplaceViewModel> replaceViewModels, IList<FileNameCustom> fileNameCustoms)
        {
            foreach (FileNameCustom fileNameCustom in fileNameCustoms)
            {
                ReplaceText(replaceViewModels, fileNameCustom);
            }
        }
        /// <summary>
        /// 替换word中的内容
        /// </summary>
        /// <param name="replaceViewModels"></param>
        /// <param name="fileNameCustom"></param>
        public static void ReplaceText(IList<ReplaceViewModel> replaceViewModels, FileNameCustom fileNameCustom)
        {
            string path = fileNameCustom.FilePath;
            if (Utils.CheckFileExists(path))
            {
                Document doc = new Document(path);
                ReplaceText(replaceViewModels, doc);

                doc.Save(path);
            }
        }
        /// <summary>
        /// 替换word中的内容
        /// </summary>
        /// <param name="replaceViewModels"></param>
        /// <param name="doc"></param>
        public static void ReplaceText(IList<ReplaceViewModel> replaceViewModels, Document doc)
        {
            foreach (ReplaceViewModel replaceViewModel in replaceViewModels)
            {
                string oldText = replaceViewModel.OldText;
                string newText = replaceViewModel.NewText;
                if (newText == null)
                {
                    newText = "";
                }
                if (!Utils.IsStrNull(oldText))
                {
                    int changeCount = doc.Range.Replace(oldText, newText, true, false);
                }
            }
        }
    }
}
