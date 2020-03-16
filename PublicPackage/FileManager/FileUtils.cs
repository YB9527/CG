using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using MyUtils;
using System.Diagnostics;

namespace FileManager
{
    public class FileUtils
    {
        // public static string ExcelFileter = "*.xls;*.xlsx";
        public static string ExcelFileter = "Excel(*.xls,*.xlsx)|*.xls;*.xlsx";
        public static string TextFileter = "文本 files(*.txt)|*.txt";
        public static string WordFileter = "文档|*.docx;*.doc";
        private static Process Pnotepad = new Process();
        public static IList<String> SelectAllDirFiles(String dirName, String filterName, FileSelectRelation relation, bool level)
        {
            IList<String> list = new List<String>();
            return SelectAfterDirFiles(filterName, dirName, list, relation, level);
          
        }

        public static string SelectSingleFileMDB(string title="选择MDB文件")
        {

            return SelectSingleFile(title, "Access数据库(*.mdb)|*.mdb");
        }

        public static string SelectSingleExcelFile()
        {
            return SelectSingleFile("Excel(*.xls,*.xlsx)|*.xls;*.xlsx");
        }

        

        public IList<string> FileInforToNames(IList<FileInfo> fileInfos)
        {
            IList<string> list = new List<String>();
            foreach (FileInfo file in fileInfos)
            {
                list.Add(file.Name);
            }
            return list;
        }

        public IList<string> FileInforToDirs(IList<FileInfo> fileInfos)
        {
            IList<string> list = new List<String>();
            foreach (FileInfo file in fileInfos)
            {
                list.Add(file.DirectoryName);
            }
            return list;
        }

        public static IList<string> SelectExcelFiles()
        {

            return SelectFiles("Excel files(*.xls)|*.xls;*.xlsx");
        }

        public IList<string> FileInforTofullNames(IList<FileInfo> fileInfos)
        {
            IList<string> list = new List<String>();
            foreach (FileInfo file in fileInfos)
            {
                list.Add(file.FullName);
            }
            return list;
        }

        public static Dictionary<string, IList<string>> GetFileNameDic(IList<string> paths)
        {
            Dictionary<string, IList<string>> dic = new Dictionary<string, IList<string>>();
            IList<string> list;
            string fileName;
            foreach (string path in paths)
            {
                fileName = Path.GetFileNameWithoutExtension(path);
                if (dic.TryGetValue(fileName, out list))
                {
                    list.Add(path);
                }
                else
                {
                    list = new List<String>();
                    list.Add(path);
                    dic.Add(fileName, list);
                }
            }
            return dic;
        }

        public static IList<FileInfo> PathToFileInfo(IList<string> pathList)
        {
            IList<FileInfo> files = new List<FileInfo>();
            foreach (String path in pathList)
            {
                if (File.Exists(path))
                {
                    files.Add(new FileInfo(path));
                }
            }
            return files;
        }

        public static void CopyDir(string fileDir, string saveDir, bool cover)
        {
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            IList<String> paths = SelectAllDirFiles(fileDir, "", FileSelectRelation.All, cover);
            IList<FileInfo> fileInfos = PathToFileInfo(paths);
            foreach (FileInfo fileInfo in fileInfos)
            {
                CopyFile(fileInfo.FullName, saveDir + "\\" + fileInfo.Name, cover);
            }

        }





        public static void CopyFileTip(Dictionary<string, string> dic)
        {
            string[] keys = dic.Keys.ToArray<string>();
            foreach (String key in keys)
            {
                if (!CopyFileTip(key, dic[key]))
                {
                    dic.Remove(key);
                }

            }
        }

        public static bool CopyFileTip(string key, string value)
        {
            string dir = Path.GetDirectoryName(value);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(value))
            {

                if (Utils.MessageBoxShow("文件已经存在是否覆盖：" + Path.GetFullPath(value)))
                {
                    File.Copy(key, value, true);

                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (File.Exists(key))
                {
                    File.Copy(key, value, true);
                }
                else
                {
                    return false;
                }

            }
            return true;

        }




        public static void CopyFile(Dictionary<string, IList<string>> dic)
        {
            Dictionary<string, IList<string>>.KeyCollection keys = dic.Keys;

            IList<string> list;
            foreach (String key in keys)
            {
                dic.TryGetValue(key, out list);

                foreach (string path in list)
                {
                    if (CopyFile(key, path, true))
                    {

                    }
                }
            }
        }

        public static IList<String> SelectFileAll(string title)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true; ;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            List<String> paths = new List<string>();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                String[] text = ofd.FileNames;
                for (int a = 0; a < text.Length; a++)
                {
                    String str = text[a];
                    paths.Add(str);
                }

            }
            return paths;
        }

        public static bool CopyFile(string fileName, string saveFileName, bool cover = true)
        {


            if (!cover && File.Exists(saveFileName))
            {

                MessageBox.Show("文件已经存在：" + saveFileName);
                return false;
            }
            if (File.Exists(fileName))
            {
                String parent = saveFileName.Substring(0, saveFileName.LastIndexOf("\\"));
                //创建路径
                if (!Directory.Exists(parent))
                {
                    try
                    {
                        Directory.CreateDirectory(parent);

                    }
                    catch
                    {
                        throw new Exception("路径不合法：" + parent);
                    }

                }
                try
                {
                    File.Copy(fileName, saveFileName, true);
                    return true;

                }
                catch
                {
                    throw new Exception("文件名不合法：" + parent);

                }

            }
            return false;
        }
        public static bool CopyFileCover(string fileName, string saveFileName)
        {
            String parent = Path.GetDirectoryName(saveFileName);
            //创建路径
            if (!Directory.Exists(parent))
            {
                try
                {
                    Directory.CreateDirectory(parent);

                }
                catch
                {
                    throw new Exception("路径不合法：" + parent);
                }

            }
            try
            {
                File.Copy(fileName, saveFileName, true);
                return true;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
                //return false;
            }

        }
        /// <summary>
        /// 文件夹移动
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="saveDir"></param>
        public static void MoveDirectory(string dir, string saveDir)
        {
            CopyDirectory(dir, saveDir);
            DeleteDirectory(dir);
        }
        /// <summary>
        /// 删除文件夹以及文件
        /// </summary>
        /// <param name="directoryPath"> 文件夹路径 </param>
        /// <param name="fileName"> 文件名称 </param>
        public static void DeleteDirectory(string directoryPath)
        {
            if(!Directory.Exists(directoryPath))
            {
                return;
            }
            
            //删除文件
            for (int i = 0; i < Directory.GetFiles(directoryPath).ToList().Count; i++)
            {
                File.Delete(Directory.GetFiles(directoryPath)[i]);
            }

            //删除文件夹
            for (int i = 0; i < Directory.GetDirectories(directoryPath).ToList().Count; i++)
            {

                Directory.Delete(Directory.GetDirectories(directoryPath)[i], true);

            }
            Directory.Delete(directoryPath, true);
            
        }
        public static bool CopyFileNoCover(string fileName, string saveFileName)
        {
            if (File.Exists(saveFileName))
            {
                return false;
            }
            return CopyFileCover(fileName, saveFileName);
        }

        public static bool OpenFile(string path)
        {
           if(Utils.CheckFileExists(path))
            {
                Pnotepad.StartInfo.FileName = path;
                Pnotepad.Start();
                return true;
            }
            return false;
           
        }

        public void RomveExcept(IList<string> list, IList<string> filters, FileSelectRelation relation)
        {
            int length = list.Count;
            string fileName;

            for (int a = 0; a < length; a++)
            {
                fileName = list[a];

                if (!CheckFile(fileName, filters, relation))
                {
                    list.RemoveAt(a);
                    a--;
                    length--;
                }
            }
        }

        private bool CheckFile(string fileName, IList<string> filters, FileSelectRelation relation)
        {
            int index = fileName.LastIndexOf("\\") + 1;
            fileName = fileName.Substring(index, fileName.Length - index);
            foreach (String fileterName in filters)
            {
                if (CheckFile(fileName, fileterName, relation))
                {
                    return true;
                }
            }
            return false;



        }

        private bool CheckFile(string compareName, string filterName, FileSelectRelation relation)
        {
            switch (relation)
            {
                case FileSelectRelation.StartsWith:
                    if (compareName.StartsWith(filterName))
                    {
                        return true;
                    }
                    break;
                case FileSelectRelation.EndsWith:
                    if (compareName.EndsWith(filterName))
                    {
                        return true;
                    }
                    break;
                case FileSelectRelation.Contains:
                    if (compareName.Contains(filterName))
                    {
                        return true;
                    }
                    break;
                case FileSelectRelation.Equals:
                    if (compareName.Equals(filterName))
                    {
                        return true;
                    }
                    break;
                case FileSelectRelation.All:
                    return true;

            }
            return false;
        }

        /// <summary>
        /// 查找文件夹中所有的文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IList<String> SeleFileDir(string dir, bool level = true)
        {

            IList<String> list = new List<String>();
            if (dir == null || dir.Equals(""))
            {
                return list;
            }
            return SelectAfterDirFiles(dir, list, level);
        }
        public static IList<String> SelectAfterDirFiles(String dirName, IList<String> list, bool level)
        {

            //循环查找，如果有文件夹再深入

            String[] fileArray = Directory.GetFiles(dirName);
            foreach (String str in fileArray)
            {
                list.Add(str);
            }
            if (level == false)
            {
                String[] dirArray = Directory.GetDirectories(dirName);
                foreach (String fileName in dirArray)
                {
                    if (Directory.Exists(fileName))
                    {
                        SelectAfterDirFiles(fileName, list, level);
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 单个文件重命名
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public void ReplaceFileName(FileInfo fileInfo, string oldName, string newName)
        {
            // 列表中的原始文件全路径名
            string oldStr = fileInfo.Name;
            // 新文件名
            string newStr = fileInfo.Directory + "\\" + oldStr.Replace(oldName, newName);
            // 改名方法
            fileInfo.MoveTo(Path.Combine(newStr));
        }
        /// <summary>
        /// 检查打开文件夹
        /// </summary>
        /// <param name="text"></param>
        public static bool OpenDir(string dir)
        {
            if(Utils.CheckDirExists(dir))
            {
                System.Diagnostics.Process.Start(dir);
                return true;
            }
            return false;
           
        }

        public static IList<string> SelectDirs(string dirRoot, FileSelectRelation relation = FileSelectRelation.All, bool level = false)
        {
            IList<string> list = SeleFileDir(dirRoot, level);
            if (list.Count == 0)
            {
                return list;
            }
            switch (relation)
            {
                case FileSelectRelation.All:
                    return list;
                case FileSelectRelation.Contains:
                    return DirContains(list);
            }
            return null;
        }

        private static IList<string> DirContains(IList<string> paths)
        {
            string dir;
            IList<string> list = new List<string>();
            foreach (string path in paths)
            {
                dir = Path.GetDirectoryName(path);
                if (dir.Contains(dir))
                {
                    list.Add(path);
                }
            }
            return list;
        }

        public static IList<String> SelectDirs(string dir)
        {
            IList<String> list = new List<String>();
            SelectDirs(dir, list);
            return list;
        }

        private static void SelectDirs(string dir, IList<string> list)
        {

            String[] dirArray = Directory.GetDirectories(dir);
            foreach (string path in dirArray)
            {
                list.Add(path);
                SelectDirs(path, list);

            }

        }

        /// <summary>
        /// 查找符合要求的文件
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="dirName"></param>
        /// <param name="list"></param>
        /// <param name="relation"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IList<String> SelectAfterDirFiles(String filterName, String dirName, IList<String> list, FileSelectRelation relation, bool level)
        {
            //循环查找，如果有文件夹再深入
            
            String[] fileArray = Directory.GetFiles(dirName);
            String compareName;
            foreach (String str in fileArray)
            {
                compareName = str.Substring(str.LastIndexOf("\\")).Replace("\\", "");
                switch (relation)
                {
                    case FileSelectRelation.StartsWith:
                        if (compareName.StartsWith(filterName))
                        {
                            list.Add(str);
                        }
                        break;
                    case FileSelectRelation.EndsWith:
                        if (compareName.EndsWith(filterName))
                        {
                            list.Add(str);
                        }
                        break;
                    case FileSelectRelation.Contains:
                        if (compareName.Contains(filterName))
                        {
                            list.Add(str);
                        }
                        break;
                    case FileSelectRelation.Equals:
                        if (compareName.Equals(filterName))
                        {
                            list.Add(str);
                        }
                        break;
                    case FileSelectRelation.All:
                        list.Add(str);
                        break;
                    default:

                        break;

                }


            }
            if (level == false)
            {
                String[] dirArray = Directory.GetDirectories(dirName);
                foreach (String fileName in dirArray)
                {
                    if (Directory.Exists(fileName))
                    {
                        SelectAfterDirFiles(filterName, fileName, list, relation, level);
                    }
                }
            }

            return list;
        }

        public static List<String> SelectFiles(String filter, String title)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true; ;
            ofd.Filter = filter;
            ofd.Title = title;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            List<String> paths = new List<string>();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                String[] text = ofd.FileNames;
                for (int a = 0; a < text.Length; a++)
                {
                    String str = text[a];
                    paths.Add(str);
                }

            }
            return paths;
        }
        public static List<String> SelectFiles(String filter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true; ;

            ofd.Filter = filter;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            List<String> paths = new List<string>();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                String[] text = ofd.FileNames;
                for (int a = 0; a < text.Length; a++)
                {
                    String str = text[a];
                    paths.Add(str);
                }

            }
            return paths;
        }

        public static String SelectSingleFile(String filter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filter;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            String path = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            return path;
        }
        public static String SelectSingleFile(String title, String filter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = title;
            ofd.Filter = filter;
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            String path = "";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
            }
            return path;
        }
        public static string SelectDir()
        {
          

            FolderBrowserDialog dir = new FolderBrowserDialog();
            dir.ShowDialog();
            return dir.SelectedPath;

        }
        public static string SeleFileDir(String title)
        {
            FolderBrowserDialog dir = new FolderBrowserDialog();
            dir.Description = title;
            dir.ShowDialog();
            return dir.SelectedPath;
            /*
            CommonOpenFileDialog open = new CommonOpenFileDialog();
            open.Title = title;
            open.IsFolderPicker = true;
            open.RestoreDirectory = false;
            var result = open.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                return open.FileName;
            else
                return "";*/
        }
        public static String[] GetFileNameArray(String fileName)
        {
            String[] ar = new String[2];
            ar[0] = fileName.Substring(0, fileName.LastIndexOf("\\"));
            ar[1] = fileName.Substring(ar[0].Length, fileName.Length - ar[0].Length);
            return ar;
        }
        public static string SaveFile(String filter)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = filter;
            saveFileDialog1.RestoreDirectory = true;
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
            {

                return saveFileDialog1.FileName;
            }
            return null;
        }
        /// <summary>
        /// 文件夹复制
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="destPath"></param>
        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                //destPath = destPath  + srcPath.Substring(srcPath.LastIndexOf("\\"))+"\\";
                DirectoryInfo dir = new DirectoryInfo(srcPath);

                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }

        public static Dictionary<string, IList<string>> GetDirDic(IList<string> fileList)
        {
            Dictionary<string, IList<string>> dic = new Dictionary<string, IList<string>>();
            IList<string> list;
            foreach (string path in fileList)
            {
                string dir = Path.GetDirectoryName(path);
                if (dic.TryGetValue(dir, out list))
                {
                    list.Add(path);
                }
                else
                {
                    list = new List<string>();
                    list.Add(path);
                    dic.Add(dir, list);
                }

            }
            return dic;
        }

        public static string FindContStrDir(IList<string> dirList, string str)
        {
            foreach (string dir in dirList)
            {
                if (dir.Contains(str))
                {
                    return dir;
                }
            }
            return null;
        }

        public static string FindContStrPath(IList<string> list, string str1, string str2)
        {
            if (str2 == null || str2.Trim().Equals(""))
            {
                foreach (string str in list)
                {
                    if (str.Contains(str1))
                    {
                        return str;
                    }
                }
            }
            else
            {
                foreach (string str in list)
                {
                    if (str.Contains(str1) && str.Contains(str2))
                    {
                        return str;
                    }
                }
            }
            return null;
        }


        public static string SelectSingleExcelFile(string title)
        {
            return SelectSingleFile(title, "Excel files(*.xls,*.xlsx)|*.xls;*.xlsx");
        }
        public static string SelectSingleTxtFile(string title = null)
        {

            return SelectSingleFile(title, "文本 files(*.txt)|*.txt");
        }
        public static IList<string> SelectTxtFiles(string title = null)
        {
            return SelectFiles("文本 files(*.txt)|*.txt", title);
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string SaveFileName(string saveName, string filter)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = filter;
            saveFileDialog1.FileName = saveName;
            saveFileDialog1.RestoreDirectory = true;
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
            {

                return saveFileDialog1.FileName;
            }
            return null;
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="saveName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string SaveFileName(string dir, string saveName, string filter)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = dir;
            saveFileDialog1.Filter = filter;
            saveFileDialog1.FileName = saveName;
            saveFileDialog1.RestoreDirectory = true;

            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
            {

                return saveFileDialog1.FileName;
            }
            return null;
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string SaveFileName(string dir)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = dir;
            saveFileDialog1.RestoreDirectory = true;

            DialogResult dr = saveFileDialog1.ShowDialog();

            if (dr == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
            {

                return saveFileDialog1.FileName;
            }
            return null;
        }
    }
}
