using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaiJiDi.ZaiJiDiModel
{
    public class DangAnDaiDirModel
    {
        public DangAnDaiDirModel()
        {
            
        }
        private static char[] ch = new char[]
        {
            '(',')'
        };
        private static char[] nameSplite = new char[]
        {
            '_'
        };
        public DangAnDaiDirModel(string path)
        {
            string dirName = Path.GetFileNameWithoutExtension(path);
            string[] array = dirName.Split(ch);
            this.Name = array[1];
            this.NameArray = array[1].Split(nameSplite);
            this.DirPath = path;           
            this.BM = array[0];
        }
        public string BM { get; set; }
        public string Name { get; set; }
        public string[] NameArray { get; set; }
        public string DirPath { get; set; }
        public Dictionary<string, string> PersonDir = new Dictionary<string, string>();
    }
}
