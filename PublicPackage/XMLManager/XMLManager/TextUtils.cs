using ExcelManager;
using MyUtils;
using ReflectManager;
using ReflectManager.XMLPackage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace XMLManager
{
    public class TextUtils
    {
        public static XMLRead xmlRead = new XMLRead();
        public static IList<T> TextToObject<T>(string path, string xmlConfig, Regex reg)
        {
            Dictionary<String, String> filedDic = XMLRead.GetConfigXmlDic(xmlConfig, "property", "key", "value");
            IList<T> list = TextToObject<T>(path, filedDic, reg);
            return list;
        }

        private static IList<T> TextToObject<T>(string text, Dictionary<string, string> filedDic, Regex reg)
        {
            IList<T> lsit = new List<T>();
            FileStream fs = File.OpenRead(text);
            StreamReader sw = new StreamReader(fs);
            string line = sw.ReadLine();
            string[] titile = TitleToArray(line, reg);
            Type type = typeof(T);




            Dictionary<string, Clazz> clazzDic =ReflectUtils.MethodToFunction(type.AssemblyQualifiedName);


            string fullName = type.FullName;
            Clazz[] clazzs = TitleToClazz(titile, filedDic, clazzDic);
            if (clazzs == null)
            {
                return lsit;
            }
            //开始内容
            line = sw.ReadLine();
            object obj;
            string value;
            object[] parmeters = new object[1];
            Clazz clazz;
            StringBuilder sb;
            Clazz jsonClazz;
            if (clazzDic.TryGetValue("JSON", out jsonClazz))
            {
                //JObject json = (JObject)JsonConvert.DeserializeObject();

            }


            while (line != null)
            {
                if (Utils.IsStrNull(line))
                {
                    line = sw.ReadLine();
                    continue;
                }

                sb = new StringBuilder("{");
                string[] array = reg.Split(line);
                if (array.Length != clazzs.Length)
                {
                    //MessageBox.Show("文件是：" + text+",内容与标题不对应，内容是“"+ line+"”");
                    line = line + sw.ReadLine();
                    continue;
                }
                obj = type.Assembly.CreateInstance(fullName);

                for (int a = 0; a < clazzs.Length; a++)
                {
                    clazz = clazzs[a];
                    value = array[a];
                    if (clazz == null)
                    {
                        if (jsonClazz != null)
                        {
                            if (value.Contains("\""))
                            {
                                value = value.Replace("\"", "\\\"");
                            }
                            else if (value.Contains("\\"))
                            {
                                value = value.Replace("\\", "\\\\");
                            }
                            sb.Append("\"" + titile[a] + "\":" + "\"" + value + "\"" + ",");
                        }


                    }
                    else
                    if (value != null && !value.Trim().Equals(""))
                    {
                        parmeters[0] = value;
                        clazz.SetMethodInfo.Invoke(obj, parmeters);


                    }
                }


                sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
                if (jsonClazz != null)
                {
                    parmeters[0] = sb.ToString();

                    //string aa = sb.ToString();
                    //JObject jo = JObject.Parse(aa);

                    jsonClazz.SetMethodInfo.Invoke(obj, parmeters);

                }

                lsit.Add((T)obj);
                line = sw.ReadLine();


            }



            return lsit;
        }

        private static Clazz[] TitleToClazz(string[] titile, Dictionary<string, string> filedDic, Dictionary<string, Clazz> clazzDic)
        {
            int size = titile.Length;
            Clazz[] clazzArray = new Clazz[size];
            Clazz clazz;
            StringBuilder sb = new StringBuilder();
            IList<string> tiltleList = titile.ToList<string>();

            foreach (string text in filedDic.Keys)
            {
                int a = tiltleList.IndexOf(text);
                if (a >= 0)
                {
                    if (clazzDic.TryGetValue(filedDic[text], out clazz))
                    {
                        clazzArray[a] = clazz;
                    }
                    else
                    {
                        sb.Append("value值“" + filedDic[text] + "”与类的方法名不匹配 ");
                    }

                }
                else
                {
                    sb.Append("key值“" + text + "”,txt中没有这个标题 ");
                }
            }
            if (sb.Length > 0)
            {
                if (!Utils.MessageBoxShow(sb.ToString()))
                {
                    return null;
                }
            }
            return clazzArray;
        }

        public static string[] TitleToArray(string title, Regex reg)
        {
            string[] array = reg.Split(title);
            for (int a = 0; a < array.Length; a++)
            {
                string str = array[a];
                if (str.Contains("\""))
                {
                    array[a] = str.Replace("\"", "\\\"");
                }

            }
            return array;
        }
        public static string[] FileTitle(string filePath, Regex reg)
        {
            FileStream fs = File.OpenRead(filePath);
            StreamReader sw = new StreamReader(fs);
            string line = sw.ReadLine();
            string[] titile = TitleToArray(line, reg);
            return titile;
        }
    }
}
