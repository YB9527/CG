using FileManager;
using MyUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JTSYQManager.XZDMManager;

namespace ZaiJiDi.ZaiJiDiModel
{
    /// <summary>
    /// 宗地档案袋
    /// </summary>
    public class DangAnDai
    {
        /// <summary>
        /// 只适合打印单个文件
        /// </summary>
        public DangAnDai()
        {
        }
        public DangAnDai(string dir)
        {


            SetDangDanDai(dir);
            SetPersonZiliao();
        }

        private void SetPersonZiliao()
        {
            this.PersonZiLiaos = new List<PersonZiLiao>();
            if(this.DirClass == null)
            {
                return;
            }
            
            foreach (string dir in this.DirClass.Dirs)
            {
                PersonZiLiao personZiLiao = PersonZiLiao.GetInstance(this.DirClass.DirName, dir);
                if (personZiLiao == null)
                {
                    continue;
                }
                personZiLiao.DangAnDai = this;
                this.PersonZiLiaos.Add(personZiLiao);
                

            }
            if(PersonZiLiaos.Count == 0)
            {
                
                PersonZiLiao personZiLiao = new PersonZiLiao();
                personZiLiao.SetPersonZiLiao(this.DirClass.DirName);
                personZiLiao.Name = this.Name;
                this.PersonZiLiaos.Add(personZiLiao);
               
            }

        }

        private void SetDangDanDai(string dir)
        {
            DirClass dirClass = new DirClass(dir);
            
            string[] array = Path.GetFileName(dir).Split(new char[]{
                '(',')','（','）'
            });
            if(array.Length == 1)
            {
                return;
            }
            this.NameArrau = array[1].Split('_');
            
            if(array[1].Length > array[0].Length)
            {
                this.Name = array[0];
                this.ZDBM = array[1];
            }else
            {
                this.Name = array[1];
                this.ZDBM = array[0];
            }
            this.DirClass = dirClass;
            Dictionary<string, string> fileDic = dirClass.GetFileNameDic();
            string path;
            if (fileDic.TryGetValue("0_档案袋.doc", out path))
            {
                this.FengMian = path;
            }
            if (fileDic.TryGetValue("1_确权登记申请审批表.doc", out path))
            {
                this.ShenPiBiao = path;
            }
            //打印个人


            if (fileDic.TryGetValue("6_宅基地权籍调查表.doc", out path))
            {
                this.DiaoChaBiao = path;
            }
            //草图
            path = dirClass.FindFileOne("C.PDF", FileSelectRelation.EndsWith);
            if (!Utils.IsStrNull(path))
            {
                this.CaoTu = path;
            }
            //打印宗地图
            IList<string> filePaths = dirClass.FindCurrentDirALL("7_", FileSelectRelation.StartsWith);
            string fangWuTu = "";
            foreach (string filePath in filePaths)
            {
                if (!filePath.EndsWith("F.PDF") )
                {
                    this.ZongDiTu = filePath;
                }
                else
                {
                    fangWuTu = filePath;
                }
            }
            //房屋图
            if (!Utils.IsStrNull(fangWuTu))
            {
                this.FangWuTu = fangWuTu;
            }
            path = dirClass.FindFileOne("z.pdf", FileSelectRelation.EndsWith);
            if (!Utils.IsStrNull(path))
            {
                this.ZongDiTu = path;
            }
            //测绘报告
            path = dirClass.FindFileOne("8_", FileSelectRelation.StartsWith);
            if (!Utils.IsStrNull(path))
            {
                this.CeHuiBaoGao = path;
            }
            path = dirClass.FindFileAllSelectOne("0.", FileSelectRelation.StartsWith);
            if (!Utils.IsStrNull(path))
            {
                this.Picture = path;
            }
        }
        /// <summary>
        /// 通过档案地址得到 xzdm
        /// </summary>
        /// <returns></returns>
        public XZDM GetXZDM()
        {
            XZDM xzdm = new XZDM();
            string path = this.DirClass.DirName;
            string[] pathArray = path.Split('\\');
            string[] reg = new string[]
            {
                "四川省成都市成都高新区","乡","镇","村","组"
            };
            foreach(string name in pathArray)
            {
                if(name.Contains("四川省成都市成都高新区"))
                {

                    string[] address = name.Split(reg,StringSplitOptions.None);
                    xzdm.XiangZheng = address[1];
                    xzdm.Cun = address[2];
                    xzdm.Zu = address[3];
                    break;
                }
            }
            return xzdm;
        }

        public string Name { get; set; }
        public string ZDBM { get; set; }
        public string[] NameArrau { get; set; }
        public DirClass DirClass { get; private set; }
        public string FengMian { get; set; }
        public string ShenPiBiao { get; set; }
        public string CaoTu { get; set; }
        public string DiaoChaBiao { get; set; }
        public string ZongDiTu { get; set; }
        public string FangWuTu { get; set; }
        public string CeHuiBaoGao { get; set; }
        public string Picture { get; set; }
        public IList<PersonZiLiao> PersonZiLiaos { get; set; }

       
    }
    /// <summary>
    /// 存放个人资料 委托书、声明书、照片、证书、不一致证明、遗失声明
    /// </summary>
    public class PersonZiLiao
    {
        private string dirName;

        public PersonZiLiao()
        {
            this.Picutres = new List<string>();
            this.ShenFenXinXiArray = new List<string>();
        }

        public PersonZiLiao(string dirName)
        {
            this.dirName = dirName;
        }

        public DangAnDai DangAnDai { get; set; }
        public string WeiTuoShu { get; set; }
        public IList<string> Picutres { get; set; }
        public string ShengMingShu { get; set; }
        public string ShenFenXinXi { get; set; }
        public IList<string> ShenFenXinXiArray { get; set; }
        public string BuYiZhiZhengMing { get; set; }
        public string YiShiShengMing { get; set; }
        public string Name { get; set; }

        internal static PersonZiLiao GetInstance(string dirRoot, string personDir)
        {
            //检查是否是个人档案袋
            string[] nameArray = Path.GetFileName(dirRoot).Split(new char[]
            {
                '(',')','_'
            });
            string currentName = Path.GetFileName(personDir).Split('(')[0];
            foreach (string name in nameArray)
            {
                if (name.Equals(currentName))
                {
                    PersonZiLiao personZiLiao = new PersonZiLiao();
                    personZiLiao.Name = name;
                    personZiLiao.SetPersonZiLiao(personDir);
                    return personZiLiao;
                }
            }
            return null;
        }

        

        public void SetPersonZiLiao(string personDir)
        {

            DirClass dirClass = new DirClass(personDir);
            foreach (string dir in dirClass.Dirs)
            {
                if (dir.Contains("照片"))
                {
                    this.setPicutures(dir);
                }
            }
            foreach (string file in dirClass.Files)
            {
                string fileName = Path.GetExtension(file).ToLower();
                //Console.WriteLine(fileName.Equals(".pdf"));
                
                if (fileName.StartsWith("4_") || (!fileName.Contains("_") && fileName.Equals(".pdf")))
                {
                    if(Utils.IsStrNull(this.ShenFenXinXi))
                    {
                        this.ShenFenXinXi = file;
                    }else  if(file.Length < this.ShenFenXinXi.Length)
                    {
                        //谁短要谁
                        this.ShenFenXinXi = file;
                    }

                    ShenFenXinXiArray.Add(file);
                } else
                if (file.Contains("遗失"))
                {
                    this.YiShiShengMing = file;
                } else
                if (file.Contains("一致"))
                {
                    this.BuYiZhiZhengMing = file;
                }else if(file.Contains("委托"))
                {
                    this.WeiTuoShu = file;
                }else if(file.Contains("声明"))
                {
                    this.ShengMingShu = file;
                }
            }


        }

        private void setPicutures(string dir)
        {
            DirClass dirClass = new DirClass(dir);
            foreach (string path in dirClass.Files)
            {
                this.Picutres.Add(path);
            }

        }
    }
}
