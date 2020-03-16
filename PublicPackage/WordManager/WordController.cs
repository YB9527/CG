using FileManager;
using MyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WordManager
{
    public class WordController
    {
        private WordService WordService { get; set; }

        internal void Dissect()
        {
            String path = FileUtils.SelectSingleFile("选择需要解析的word", "Word files(*.docx) | *.docx");
            String saveDir = FileUtils.SeleFileDir("选择要保存的文件夹");
            if(Utils.IsStrNull(path) || Utils.IsStrNull(saveDir))
            {
                return;
            }
            WordService.DissectText(path,saveDir);
        }

       
    }
}
