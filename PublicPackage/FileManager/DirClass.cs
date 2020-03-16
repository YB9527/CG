using MyUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManager
{
    /// <summary>
    /// 文件夹对象
    /// </summary>
    public class DirClass
    {
        public DirClass(string dirName)
        {
            
            if(Utils.CheckDirExists(dirName))
            {
                this.DirName = dirName;
                this.Dirs = Directory.GetDirectories(dirName);
                this.Files = Directory.GetFiles(dirName);
            }
            
        }
        public string DirName { get; private set; }
        public string[] Dirs { get; private set; } 
        public string[] Files { get;private  set; }
       // private IList<DirClass> DirChilds {  get;  set; }

       public Dictionary<string,string> GetDirNameDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach(string dir in this.Dirs)
            {
                dic.Add(Path.GetFileName(dir), dir);
            }
            return dic;
        }
        public Dictionary<string, string> GetFileNameDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string filePath in this.Files)
            {
                dic.Add(Path.GetFileName(filePath), filePath);
            }
            return dic;
        }

        /// <summary>
        /// 查找所有的子文件夹
        /// </summary>
        /// <returns></returns>
        public IList<DirClass> FindChildAll()
        {
             IList<DirClass> DirChilds = new List<DirClass>();
            FindChildAll(this, DirChilds);
            return DirChilds;
        }
        private void FindChildAll(DirClass dirClass, IList<DirClass> childs)
        {
            string[] dirs = dirClass.Dirs;
            foreach (string dir in dirs)
            {
                DirClass child = new DirClass(dir);
                childs.Add(child);
                FindChildAll(child,childs);
            }
        }
        public IList<string> FindFileAll()
        {
            List<string> list = new List<string>();
            list.AddRange(this.Files);
            FindFileAll(this.FindChildAll(), list);
            return  list;
        }
        /// <summary>
        /// 返回 与compre 有关系文件 中第一个文件
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public string FindFileOne(string compare, FileSelectRelation relation)
        {
            IList<string> list = FindCurrentDirALL(compare, relation);
            if(list == null || list.Count == 0)
            {
                return "";
            }
            else
            {
                return list[0];
            }
        }

        public string FindFileAllSelectOne(string compare, FileSelectRelation relation)
        {
            IList<string> fileAll = this.FindFileAll();
            IList<string> select = SelectFile(compare, relation,Utils.ListToArray(fileAll));
            if(select .Count > 0)
            {
                return select[0];
            }
            return "";
        }

        /// <summary>
        /// 查的所有 与compre 有关系文件
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="relation">关系</param>
        /// <returns></returns>
        public IList<string>   FindCurrentDirALL(string compare, FileSelectRelation relation)
        {
            if(Utils.IsStrNull(compare))
            {
                return null;
            }
            return SelectFile( compare,  relation,this.Files);
            
        }

        private IList<string> SelectFile(string compare, FileSelectRelation relation,string[] files)
        {
            IList<string> paths = new List<string>();
            string fileName = "";
            compare = compare.ToLower();
            foreach (string filePath in files)
            {
                fileName = Path.GetFileName(filePath).ToLower();
                switch (relation)
                {
                    case FileSelectRelation.Contains:
                        if (fileName.Contains(compare))
                        {
                            paths.Add(filePath);
                        }
                        break;
                    case FileSelectRelation.EndsWith:
                        if (fileName.EndsWith(compare))
                        {
                            paths.Add(filePath);
                        }
                        break;
                    case FileSelectRelation.Equals:
                        if (fileName.Equals(compare))
                        {
                            paths.Add(filePath);
                        }
                        break;
                    case FileSelectRelation.StartsWith:
                        if (fileName.StartsWith(compare))
                        {
                            paths.Add(filePath);
                        }
                        break;
                }
            }
            return paths;
        }

        private void FindFileAll(IList<DirClass> dirs, IList<string> list)
        {
             
            foreach(DirClass dirClass in dirs)
            {
                string[] files = dirClass.Files;
                foreach(string file in files)
                {
                    list.Add(file);
                }
            }
        }

    }
}
