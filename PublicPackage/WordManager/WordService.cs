using System;
using System.Collections.Generic;
using NPOI.XWPF.UserModel;
using System.IO;
using ExcelManager;

namespace WordManager
{
    public class WordService
    {
        private WordWrite WordWrite { set; get; }
        private WordRead WordRead { set; get; }

        private  void Dissect(string path, string saveDir)
        {
            XWPFDocument doc = WordRead.Read(path);
            Dictionary<string, IList<RunCustomer>> docDic = WordRead.GetDocxDic(doc);
            FileInfo fileInfo = new FileInfo(path);
            FileStream fs = File.Create(saveDir + "\\"+fileInfo.Name.Substring(0,fileInfo.Name.IndexOf("."))+"解析结果.txt");
            StreamWriter sw = new StreamWriter(fs);
            foreach (String str in docDic.Keys)
            {
                sw.WriteLine(str+"                         有："+docDic[str].Count+ " 处");
            }

            sw.Close();
            fs.Close();
        }

       

        private void ReplaceText_Word(string path)
        {
             XWPFDocument doc =   WordRead.Read(path);          
             Dictionary<string,IList<RunCustomer>> dic= WordRead.GetDocxDic(doc);
             foreach(RunCustomer c in dic.Values)
            {
                XWPFRun run =    c.Run;
                if("宗".Equals(run.GetText(0)))
                {
                    run.SetBold(true);
                   
                }
            }
            WordWrite.SaveToFile( doc, path);
        }

        public static void DissectText(string path, string saveDir)
        {
            XWPFDocument doc = WordRead.Read(path);
            IList<String> docText = WordRead.GetDocxText(doc);
            FileInfo fileInfo = new FileInfo(path);
            FileStream fs = File.Create(saveDir + "\\" + fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(".")) + "解析结果.txt");
            StreamWriter sw = new StreamWriter(fs);
            foreach (String text in docText)
            {             
                sw.WriteLine(text);
            }
            sw.Close();
            fs.Close();
        }
    }
}
