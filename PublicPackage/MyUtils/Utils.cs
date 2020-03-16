using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MyUtils
{
    public class Utils
    {
        private static Regex numReg = new Regex(@"^[0-9]*$");
        private static Regex doubleReg = new Regex(@"(^[0-9]*[1-9][0-9]*$)|(^([0-9]{1,}[.][0-9]*)$)");//写正则表达式，只能输入数字&小数

        /// <summary>
        /// 检查集合是否为空 有对象是 true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns>有对象是 true</returns>
        public static bool CheckListExists<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static Regex zjhmReg = new Regex("\\d{17}[0-9X]");
        public static bool IsStrNull(string str)
        {
            if (str == null || str.Trim().Equals(""))
            {
                return true;
            }
            return false;
        }

        public static Dictionary<T2, T1> DicReset<T1, T2>(Dictionary<T1, T2> srcDic)
        {
            Dictionary<T2, T1> dic = new Dictionary<T2, T1>();
            foreach (T1 t in srcDic.Keys)
            {

                dic.Add(srcDic[t], t);
            }
            return dic;
        }
        /// <summary>
        /// 方法的名字改成小写
        /// </summary>
        /// <param name="functionMap"></param>
        /// <returns></returns>
        public static Dictionary<int, String> ValuesToLower(Dictionary<int, String> functionMap)
        {
            Dictionary<int, String> dic = new Dictionary<int, string>();
            Dictionary<int, String>.KeyCollection keys = functionMap.Keys;
            String value;
            foreach (int key in keys)
            {
                functionMap.TryGetValue(key, out value);
                dic.Add(key, value.ToLower());

            }
            return dic;
        }

        public static bool MessageBoxShow(string vaule, string tip = "提示")
        {
            if (System.Windows.Forms.MessageBox.Show(vaule, tip, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }

      


        public static bool CheckFileExists(string xmlPath)
        {
            if (xmlPath == null)
            {
                return false;
            }
            if (File.Exists(xmlPath))
            {
                return true;
            }
            return false;
        }

        public static bool IsInt(string text)
        {
            if (numReg.IsMatch(text))
            {
                return true;
            }
            return false;
        }

        //时间格式转换
        public static DateTime FormatDate(String dateTime)
        {
            DateTime dt;

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

            bool flag = false;

            if (dateTime.Contains("年"))
            {
                if (dateTime.Contains("号"))
                {
                    dtFormat.ShortDatePattern = "yyyy年MM月dd号";
                }
                else
                {
                    dtFormat.ShortDatePattern = "yyyy年MM月dd日";
                }


            }
            else if (dateTime.Contains("/"))
            {
                dtFormat.ShortDatePattern = "yyyy/MM/dd";
            }
            else if (dateTime.Contains(@"-"))
            {
                dtFormat.ShortDatePattern = "yyyy-MM-dd";
            }
            else if (dateTime.Length == 8)
            {
                dateTime = dateTime.Insert(4, "/");
                dateTime = dateTime.Insert(7, "/");
                dtFormat.ShortDatePattern = "yyyyMMdd";

            }
            else if (dateTime.Contains("."))
            {
                dtFormat.ShortDatePattern = "yyyy.MM.dd";
            }else
            {
                flag = true;
            }
            if(flag == true)
            {
                //日期有问题
                return new DateTime();
            }else
            {
                dt = Convert.ToDateTime(dateTime, dtFormat);
            }

            return dt;

        }

        public static void ArrayReplaceAfter(string[] addressArray)
        {
            bool flag = addressArray == null;
            if (!flag)
            {
                for (int i = addressArray.Length - 1; i > 0; i--)
                {
                    string text = addressArray[i];
                    string text2 = addressArray[i - 1];
                    bool flag2 = text != null && text2 != null;
                    if (flag2)
                    {
                        addressArray[i] = text.Replace(text2, "");
                    }
                }
            }
        }


        public static string[] ArrayRemoveNull(string[] addressArray)
        {
            int i;
            for (i = addressArray.Length - 1; i >= 0; i--)
            {
                bool flag = addressArray[i] != null;
                if (flag)
                {
                    break;
                }
            }
            return Utils.ArrayCopyToIndex<string>(addressArray, i + 1);
        }

        private static T[] ArrayCopyToIndex<T>(T[] array, int v)
        {
            T[] array2 = new T[v];
            for (int i = 0; i < v; i++)
            {
                array2[i] = array[i];
            }
            return array2;
        }

        /// <summary>
        /// 检查两个字符串是否相等
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool StrEquals(string str1, string str2)
        {
                return str2 == str1;
        }

        /// <summary>
        /// list 转换成 Array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] ListToArray<T>(IList<T> list)
        {
            if (list == null)
            {
                return null;
            }
            T[] array = new T[list.Count];
            int i = 0;
            foreach (T t in list)
            {
                array[i++] = t;
            }
            return array;
        }

        /// <summary>
        /// list 转换成 ObservableCollection 如果是null 返回null，如果count=0；返回 一个new 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ListToObservableCollection<T>(IList<T> list)
        {
            if (list == null )
            {
                return null;
            }
            ObservableCollection<T> ob = new ObservableCollection<T>();
            if(list.Count == 0)
            {
                return ob;
            }
            foreach (T t in list)
            {
                ob.Add(t);
            }
            return ob;
        }

        /// <summary>
        /// 检查文件夹是否存在
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool CheckDirExists(string dir)
        {
            if (dir == null)
            {

                return false;

            }
            if (Directory.Exists(dir))
            {
                return true;
            }
            else
            {

            }
            return false;
        }
        /// <summary>
        /// list 转为string
        /// </summary>
        /// <param name="list"></param>
        /// <param name="connect"></param>
        /// <returns></returns>
        public static string ListToString(IList<string> list, string connect)
        {
            StringBuilder sb = new StringBuilder();
            if (list.Count == 0)
            {
                return "";
            }
            else
            {
                sb.Append(list[0]);
            }
            for (int a = 1; a < list.Count; a++)
            {
                sb.Append("、" + list[a]);
            }
            return sb.ToString();
        }

        public static Dictionary<string, T> GetGroupDic<T>(string methodName, IList<T> list)
        {
            IList<string> errosrs = new List<string>();
            Dictionary<string, T> dic = new Dictionary<string, T>();
            if (list == null || list.Count == 0)
            {
                return dic;
            }
            
            MethodInfo method = typeof(T).GetMethod("get_" + methodName);
            if (method == null)
            {
                System.Windows.Forms.MessageBox.Show("类" + typeof(T).AssemblyQualifiedName + "没有这个方法：" + "get_" + methodName);
                return dic;
            }
            T result;
            foreach (T t in list)
            {
                object obj = method.Invoke(t, null);
                if(obj == null)
                {
                    continue;
                }
                string value = obj.ToString();
                if (value == null)
                {
                    errosrs.Add("集合类" + typeof(T).FullName + ",主键为空");
                }
                if (dic.TryGetValue(value, out result))
                {
                    errosrs.Add("集合类" + typeof(T).FullName + ",主键重复:" + value);
                }
                else
                {
                    dic.Add(value, t);
                }
            }
            if(errosrs.Count >0)
            {

                System.Windows.Forms.MessageBox.Show(errosrs[0]);
            }
            return dic;
        }
        public static Dictionary<T1, IList<T2>> GetGroupDicToList<T1, T2>(T1 key, IList<T2> list)
        {
            Dictionary<T1, IList<T2>> dic = new Dictionary<T1, IList<T2>>();
            if (list == null || list.Count == 0)
            {
                return dic;
            }
            
            MethodInfo method = typeof(T2).GetMethod("get_" + key.ToString());
            if (method == null)
            {
                throw new Exception("类" + typeof(T2).AssemblyQualifiedName + "没有这个方法：" + "get_" + key.ToString());
            }
            IList<T2> outlist;
            foreach (T2 t2 in list)
            {
                object value = Utils.GetValueTrueType(method.Invoke(t2, null), typeof(T1).Name);
                if (value == null)
                {
                    System.Windows.Forms.MessageBox.Show("集合类" + typeof(T2).FullName + ",主键为空");
                }
                T1 t1 = (T1)value;
                if (dic.TryGetValue(t1, out outlist))
                {
                    outlist.Add(t2);
                }
                else
                {
                    outlist = new List<T2>();
                    outlist.Add(t2);
                    dic.Add(t1, outlist);
                }


            }
            return dic;
        }

        public static Dictionary<T1, ObservableCollection<T2>> GetGroupDicToList<T1, T2>(T1 key, ObservableCollection<T2> list)
        {
            Dictionary<T1, ObservableCollection<T2>> dic = new Dictionary<T1, ObservableCollection<T2>>();
            if (list == null || list.Count == 0)
            {
                return dic;
            }

            MethodInfo method = typeof(T2).GetMethod("get_" + key.ToString());
            if (method == null)
            {
                throw new Exception("类" + typeof(T2).AssemblyQualifiedName + "没有这个方法：" + "get_" + key.ToString());
            }
            ObservableCollection<T2> outlist;
            foreach (T2 t2 in list)
            {
                object value = Utils.GetValueTrueType(method.Invoke(t2, null), typeof(T1).Name);
                if (value == null)
                {
                    System.Windows.Forms.MessageBox.Show("集合类" + typeof(T2).FullName + ",主键为空");
                }
                T1 t1 = (T1)value;
                if (dic.TryGetValue(t1, out outlist))
                {
                    outlist.Add(t2);
                }
                else
                {
                    outlist = new ObservableCollection<T2>();
                    outlist.Add(t2);
                    dic.Add(t1, outlist);
                }


            }
            return dic;
        }

        public static Object GetValueTrueType(object obj, String reultType)
        {
            if (obj == null)
            {
                return null;
            }
            Object ob = null;
            String strValue = obj.ToString();
            if (strValue.Equals(""))
            {
                return null;
            }
            var v = obj.GetType().ToString();
            if (!obj.GetType().ToString().Equals(reultType))
            {

                switch (reultType)
                {
                    case "String":
                        ob = strValue;
                        break;
                    case "DateTime":


                        if (!strValue.Equals(""))
                        {

                            //日期格式化  - - - 
                            try
                            {
                                DateTime dt = Utils.FormatDate(strValue);
                                ob = dt;
                            }
                            catch
                            {
                                //报出错误
                                // throw new Exception(cell.Sheet.SheetName + "数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                            }
                        }


                        break;
                    case "Int32":


                        if (Regex.Match(strValue, @"^-?\d+$").Success)
                        {
                            ob = int.Parse(strValue);
                        }
                        else
                        {
                            //报出错误
                            //throw new Exception("数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                        }


                        break;
                    case "Single":



                        if (Regex.Match(strValue, @"^(-?\d+)(\.\d+)?$").Success)
                        {
                            ob = float.Parse(strValue);
                        }
                        else
                        {
                            if (strValue.StartsWith("."))
                            {
                                ob = float.Parse("0" + strValue);
                            }
                            else
                            {
                                //报出错误
                                // throw new Exception("数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                            }

                        }

                        break;
                    case "Double":

                        if (Regex.Match(strValue, @"^(-?\d+)(\.\d+)?$").Success)
                        {
                            ob = double.Parse(strValue);
                        }
                        else
                        {
                            //报出错误
                            // throw new Exception("数据第：" + (cell.RowIndex + 1) + " 行，第：" + (cell.ColumnIndex + 1) + " 列," + "数据格式不正确,值是：" + cellValue);
                        }

                        break;

                    default:
                        //有无法识别的类型
                        break;
                }
            }
            else
            {
                return obj;
            }
            return ob;
        }
        
        /// <summary>
        /// 集合复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public T[] CopyArray<T>(T[] array)
        {
            int num = array.Length;
            T[] array2 = new T[num];
            for (int i = 0; i < num; i++)
            {
                array2[i] = array[i];
            }
            return array2;
        }
        /// <summary>
        /// 得到唯一一个一个正确的随机身份证
        /// </summary>
        /// <param name="sfz"></param>
        /// <param name="nv"></param>
        /// <returns></returns>
        public static string GetRealyOnlySFZ(string sfz, bool nv = true)
        {
            IList<string> sfzs = GetRealySFZ(sfz, nv);
            Random rd = new Random();
            int r = rd.Next(0, sfzs.Count);// (生成1~10之间的随机数，不包括10)
            if (sfzs.Count == 0)
            {
                return "有问题";
            }
            string result = sfzs[r];
            return result;
        }
        private static IList<string> nvsfzArray;
        private static IList<string> nansfzArray;
        private static IList<string> NvSFZArray
        {
            get
            {
                if (nvsfzArray == null)
                {
                    GetSFZAarray();
                }
                return nvsfzArray;
            }
        }
        private static IList<string> NanSFZArray
        {
            get
            {
                if (nansfzArray == null)
                {
                    GetSFZAarray();
                }
                return nansfzArray;
            }
        }
        private static void GetSFZAarray()
        {


            string[] array1 = new string[]
 {
                "0","1","2","3","4","5","6","7","8","9"
 };
            string[] array2 = new string[]
          {
                "0","1","2","3","4","5","6","7","8","9"
          };


            string[] array31 = new string[] { "1", "3", "5", "7", "9" };


            string[] array32 = new string[] { "0", "2", "4", "6", "8" };

            string[] array4 = new string[]
            {
                "0","1","2","3","4","5","6","7","8","9", "X"
            };

            nvsfzArray = new List<string>();
            nansfzArray = new List<string>();
            foreach (string str1 in array1)
            {
                foreach (string str2 in array2)
                {

                    foreach (string str3 in array31)
                    {
                        foreach (string str4 in array4)
                        {
                            nansfzArray.Add(str1 + str2 + str3 + str4);
                        }
                    }
                    foreach (string str3 in array32)
                    {
                        foreach (string str4 in array4)
                        {
                            nvsfzArray.Add(str1 + str2 + str3 + str4);
                        }
                    }
                }

            }

        }

        /// <summary>
        /// 得到所有正确的身份证
        /// </summary>
        /// <param name="sfz"></param>
        /// <param name="nv">true 为男 false 为女</param>
        /// <returns></returns>
        public static IList<string> GetRealySFZ(string sfz, bool nv = true)
        {


            IList<string> sfzs = new List<string>();
            IList<string> list;
            sfz = sfz.Substring(0, 14);
            if (nv)
            {
                list = NanSFZArray;
            }
            else
            {
                list = NvSFZArray;
            }
            foreach (string str in list)
            {
                if (Utils.CheckIDCard18(sfz+str))
                {
                    sfzs.Add(sfz + str);
                }
            }
            return sfzs;

        }

        public static Dictionary<String, String> ValueToLower(Dictionary<String, String> dic)
        {
            Dictionary<String, String>.KeyCollection keys = dic.Keys;
            Dictionary<String, String> newDic = new Dictionary<string, string>();
            String t;
            foreach (String str in keys)
            {
                dic.TryGetValue(str, out t);
                if (str.Contains("类的全名称"))
                {
                    newDic.Add(str, t);
                }
                else
                {
                    newDic.Add(str, t.ToLower());
                }


            }
            return newDic;
        }

        /// <summary>  
        /// 18位身份证号码验证  
        /// </summary>  
        public static bool CheckIDCard18(string idNumber)
        {
            if (idNumber==null || idNumber.Length != 18)
            {
                return false;
            }

            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
                || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }

       
       

    }
}
