using FileManager;
using MyUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaiJiDi.ZaiJiDiModel;

namespace ZaiJiDi.Service
{
    class DangAnDaiService : IDangAnDaiService
    {
        public IList<DangAnDaiDirModel> GetDangAnDaiDirArray(string[] dirs)
        {
            char[] ch = new char[]
            {
                '_',')'
            };
            IList<DangAnDaiDirModel> list = new List<DangAnDaiDirModel>();

            foreach (string dir in dirs)
            {

                if (dir.Contains("_"))
                {
                    DangAnDaiDirModel model = new DangAnDaiDirModel(dir);
                    list.Add(model);
                }
            }
            return list;
        }
        public void WeiTuoShengMingCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po)
        {
            if (Utils.IsStrNull(po.WeiTuoShengMingDir))
            {
                return;
            }
            DirClass weiTuoShengMingDirClass = new DirClass(po.WeiTuoShengMingDir);
            foreach (DangAnDaiDirModel model in dangAnDaiDirArray)
            {
                //委托声明文件夹,用名字查找
                foreach (string dir in weiTuoShengMingDirClass.Dirs)
                {
                    string dirName = Path.GetFileNameWithoutExtension(dir);
                    string[] dirNameArray = dirName.Split('(');
                    if (Array.IndexOf(model.NameArray, dirNameArray[0]) != -1)
                    {
                        //po 设置
                        string path = po.SaveDir + "\\" + Path.GetFileName(model.DirPath) + "\\" + Path.GetFileName(dir);
                        model.PersonDir.Add(dirNameArray[0], path);
                        //找到复制过去
                        FileUtils.CopyDirectory(dir, path);
                    }
                }
            }
        }
        public void DangAnDaiDirCopy(IList<DangAnDaiDirModel> list, DangAnDaiMergeViewModel po)
        {
            foreach (DangAnDaiDirModel model in list)
            {
               
                FileUtils.CopyDirectory(model.DirPath, po.SaveDir + "\\" + Path.GetFileNameWithoutExtension(model.DirPath));
               // model.DirPath = model.DirPath.Insert(model.DirPath.LastIndexOf("\\")+1, "---");
            }
        }
        /// <summary>
        /// 照片复制
        /// </summary>
        /// <param name="dangAnDaiDirArray"></param>
        /// <param name="po"></param>
        public void PictureDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po)
        {
            if (Utils.IsStrNull(po.PictureDir))
            {
                return;
            }
            string path;
            DirClass pictureDirClass = new DirClass(po.PictureDir);
            foreach (DangAnDaiDirModel model in dangAnDaiDirArray)
            {

                foreach (string dir in pictureDirClass.Dirs)
                {
                    string name = Path.GetFileNameWithoutExtension(dir);
                    if (Array.IndexOf(model.NameArray, name) != -1)
                    {
                        //找到复制过去
                        if (model.PersonDir.TryGetValue(name, out path))
                        {
                            // FileUtils.CopyDirectory(dir, path + "\\" + Path.GetFileName(dir)+"_照片");
                            FileUtils.CopyDirectory(dir, path + "\\照片");
                        }
                        else
                        {
                            FileUtils.CopyDirectory(dir, po.SaveDir + "\\" + Path.GetFileName(model.DirPath) + "\\" + Path.GetFileName(dir));
                        }

                    }
                }
            }
        }

        public void PDFDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po)
        {
            if (Utils.IsStrNull(po.PDFDir))
            {
                return;
            }
            DirClass pdfDirClass = new DirClass(po.PDFDir);
            foreach (DangAnDaiDirModel model in dangAnDaiDirArray)
            {

                foreach (string file in pdfDirClass.Files)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    if (name.IndexOf(model.BM) != -1 && !file.EndsWith(".dwg"))
                    {
                        //找到复制过去                    
                        FileUtils.CopyFile(file, po.SaveDir + "\\" + Path.GetFileName(model.DirPath) + "\\" + Path.GetFileName(file));
                    }
                }
            }
        }

        public void ShenFenXinXiDirCopy(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po)
        {
            if (Utils.IsStrNull(po.ShenFenXinXiDir))
            {
                return;
            }
            string path;
            DirClass shenFenXinXiDirClass = new DirClass(po.ShenFenXinXiDir);
            foreach (DangAnDaiDirModel model in dangAnDaiDirArray)
            {

                foreach (string file in shenFenXinXiDirClass.Files)
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    foreach (string nameSigle in model.NameArray)
                    {
                        if (name.IndexOf(nameSigle) != -1)
                        {

                            //找到复制过去
                            if (model.PersonDir.TryGetValue(nameSigle, out path))
                            {
                                FileUtils.CopyFile(file, path + "\\" + Path.GetFileName(file));
                            }
                            else
                            {
                                FileUtils.CopyFile(file, po.SaveDir + "\\" + Path.GetFileName(model.DirPath) + "\\" + Path.GetFileName(file));
                            }


                        }
                    }

                }
            }
        }
        /// <summary>
        /// 在原始的档案袋里找 不一致声明，遗失声明
        /// </summary>
        /// <param name="dangAnDaiDirArray"></param>
        /// <param name="po"></param>
        public void YuanShiDangAnDaiDir(IList<DangAnDaiDirModel> dangAnDaiDirArray, DangAnDaiMergeViewModel po)
        {
            if (Utils.IsStrNull(po.YuanShiDangAnDaiDir))
            {
                return;
            }
            string path;
            DirClass yuanShiDangAnDaiDirClass = new DirClass(po.YuanShiDangAnDaiDir);
            foreach (DangAnDaiDirModel model in dangAnDaiDirArray)
            {

                foreach (string dir in yuanShiDangAnDaiDirClass.Dirs)
                {
                    string name = Path.GetFileNameWithoutExtension(dir);
                    foreach (string nameSigle in model.NameArray)
                    {
                        if (name.IndexOf(nameSigle) != -1)
                        {
                            DirClass dirClass = new DirClass(dir);
                            foreach(string file in dirClass.Files)
                            {
                                string fileName = Path.GetFileName(file);
                                bool flag = false;
                                if(fileName.Contains("不一致"))
                                {
                                    flag = true;
                                }
                                else if(fileName.Contains("遗失"))
                                {
                                    flag = true;
                                }

                                if(flag)
                                {
                                    
                                    if (model.PersonDir.TryGetValue(nameSigle, out path))
                                    {
                                        FileUtils.CopyFile(file, path + "\\" + Path.GetFileName(file));
                                  
                                    }
                                    else
                                    {
                                        FileUtils.CopyFile(file, po.SaveDir + "\\" + Path.GetFileName(model.DirPath) + "\\" + Path.GetFileName(file));
                                      
                                    }
                                }
                                if (model.PersonDir.TryGetValue(nameSigle, out path))
                                {
                                   
                                    //原始档案袋移动
                                    FileUtils.MoveDirectory(dir, path + "\\单宗档案_" + Path.GetFileName(dir));
                                }
                                else
                                {
                                  
                                    //原始档案袋移动
                                    FileUtils.MoveDirectory(dir, po.SaveDir + "\\" + Path.GetFileName(model.DirPath) + "\\" + Path.GetFileName(dir));
                                }

                            }
                            
                        }
                    }
                }
            }
        }
    }
}
