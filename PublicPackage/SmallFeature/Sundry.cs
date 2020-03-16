using ExcelManager;
using ExcelManager.Model;
using HeibernateManager.Model;
using MyUtils;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using ProgressTask;
using ReflectManager.XMLPackage;
using SmallFeature.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordManager;

namespace SmallFeature
{
   public class Sundry
   {
        /// <summary>
        /// 生成集体土地所有权证
        /// </summary>
        public void CreateJTTDSYQZ_doc(string excelPath, string saveDir)
        {
            if(Utils.CheckFileExists(excelPath) && Utils.CheckDirExists(saveDir))
            {
                List<MyAction> actions = new List<MyAction>();
                IList<JTTDSYQZ> jtsyqzs =  JTTDSYQZCustom.GetJTTDSYQZS(excelPath);
                Dictionary<string, XMLObject>   xmlObjectDic = XMLRead.XmlToObjects_get < JTTDSYQZ > ();
                foreach(JTTDSYQZ jtsyqz in jtsyqzs)
                {
                    Action action = new Action(() =>
                    {
                        
                        string th = jtsyqz.TuHao;
                        if(th != null)
                        {                         
                            string[] array = th.Split('、');

                            if(array.Length >2)
                            {
                                jtsyqz.TuHao = array[0] +"、"+ array[1];
                                string th1 = "";
                                for(int a = 2;a < array.Length;a++ )
                                {
                                    if(a %2 ==0)
                                    {
                                        th1 = th1 + "" + array[a];
                                    }
                                    else
                                    {
                                        th1 = th1 + "、" + array[a];
                                    }
                                    
                                }
                                jtsyqz.TH1 = th1;
                               
                            }
                        }
                        XWPFDocument docx = WordRead.Read(JTTDSYQZCustom.DocxTemplete);
                        WordWrite.ReplaceText(docx, xmlObjectDic, jtsyqz);


                       
                        WordWrite.SaveToFile(docx, saveDir + "\\" + jtsyqz.DH + "_" + jtsyqz.DZ.Replace("中江县", "") + ".docx");
                    });
                       
                    MyAction myAction = new MyAction(action, jtsyqz.DH + "_" + jtsyqz.DZ.Replace("中江县", "") + ".docx");
                    actions.Add(myAction);
                    //break;
                }
                SingleTaskForm task = new SingleTaskForm(actions, "生成中江档案封面 ");
            }
        }
       
   }
}
