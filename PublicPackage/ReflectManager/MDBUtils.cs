using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Text;
using Microsoft.Office.Interop.Access.Dao;
using MyUtils;
using Newtonsoft.Json.Linq;
using ReflectManager.XMLPackage;

namespace ReflectManager
{
    public class MDBUtils
	{
		
        private string MDBPath { get; set; }
        public MDBUtils(string mdbPath)
        {
            this.MDBPath = mdbPath;
        }

        public static IList<T> MdbToJzxinfo<T>(string mdbPath, string fullName)
		{
			int length = fullName.Length;
			int num = fullName.LastIndexOf(".") + 1;
			string tableName = fullName.Substring(num, length - num);
			return ReadAllData<T>(tableName, mdbPath);
		}

        public static ObservableCollection<T> ReadAllDataToObservableCollection<T>(string tableName, string mdbPath, string xmlPath = null)
        {
            IList<T> list = ReadAllData<T>(tableName, mdbPath, xmlPath);
            ObservableCollection<T> oc = new ObservableCollection<T>();
            foreach(T t in list)
            {
                oc.Add(t);
            }
            return oc;
        }

        private static OleDbCommand ConnectMDB(string mdbPath)
		{
			string str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath;
			OleDbConnection oleDbConnection = new OleDbConnection(str + ";OLE DB Services=-4");
			oleDbConnection.Open();
			return oleDbConnection.CreateCommand();
		}

		public static IList<T> ReadAllData<T>(string tableName, string mdbPath,string xmlPath=null)
		{
            
            Dictionary<string, XMLObject> xmlObjectDic;
            if(Utils.IsStrNull(xmlPath))
            {
                xmlObjectDic = XMLRead.XmlToObjects_get<T>();
            }
            else
            {
                xmlObjectDic = XMLRead.XmlToObjects(xmlPath);
            }
            

            Type type = typeof(T);
            string fullName = type.AssemblyQualifiedName;
            object[] array = new object[1];

            IList<T> list = new List<T>();
			string str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath;
			OleDbConnection oleDbConnection = new OleDbConnection(str + ";OLE DB Services=-4");
			oleDbConnection.Open();
			OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
			oleDbCommand.CommandText = "select * from " + tableName;
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
			int fieldCount = oleDbDataReader.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				dictionary.Add(i, oleDbDataReader.GetName(i));
			}
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
            Clazz clazz;
            string title;
            XMLObject xmlObject;
            IList<XMLObject> defualts = new List<XMLObject>();
            foreach(XMLObject xml in xmlObjectDic.Values)
            {
                if(!Utils.IsStrNull(xml.Deafult))
                {
                    defualts.Add(xml);
                }
            }
            while (oleDbDataReader.Read())
			{

				object obj = Activator.CreateInstance(type);
                for (int j = 0; j < fieldCount; j++)
				{
                    title = oleDbDataReader.GetName(j);
                    if(xmlObjectDic.TryGetValue(title,out xmlObject))
                    {
                        clazz = clazzDic[xmlObject.Key];
                        array[0] =Utils.GetValueTrueType(oleDbDataReader[j], clazz.getParamterType().Name);
                        
                        if(array[0] == null)
                        {
                            continue;
                        }else
                        {
                            clazz.SetMethodInfo.Invoke(obj, array);
                        }
                       
                    }
                   
				}
                foreach(XMLObject x in defualts)
                {
                    clazz = clazzDic[x.Column];
                    array[0] = Utils.GetValueTrueType(x.Deafult, clazz.getParamterType().Name);

                    if (array[0] == null)
                    {
                        continue;
                    }
                    else
                    {
                        clazz.SetMethodInfo.Invoke(obj, array);
                    }
                }
				list.Add((T)((object)obj));
			}
			oleDbDataReader.Close();
			oleDbConnection.Close();
			return list;
		}

        public static IList<JObject> ReadDataToJObject(string tableName, string mdbPath)
        {
            IList<JObject> list = new List<JObject>();
            OleDbCommand oleDbCommand = ConnectMDB(mdbPath);
            oleDbCommand.CommandText = "select * from " + tableName;
            OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
            int count = oleDbDataReader.FieldCount;
            while (oleDbDataReader.Read())
            {
                JObject jObject = new JObject();
                for (int i = 0; i < count; i++)
                {
                    jObject.Add(oleDbDataReader.GetName(i), oleDbDataReader.GetValue(i).ToString());
                }
                list.Add(jObject);
            }
            oleDbDataReader.Close();
            oleDbCommand.Connection.Close();
            return list;
        }
        /*
        public  void WriteSqlData<T>( string tableName, IList<T> entitys, bool objectid = true)
		{
			Dictionary<string, Clazz> dictionary = ReflectUtils.MethodToFunction<T>();
			string str = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + MDBPath;
			OleDbConnection oleDbConnection = new OleDbConnection(str + ";OLE DB Services=-4");
			oleDbConnection.Open();
			OleDbTransaction oleDbTransaction = oleDbConnection.BeginTransaction();
			OleDbCommand oleDbCommand = oleDbConnection.CreateCommand();
			oleDbCommand.Transaction = oleDbTransaction;
			IList<string> titleField = GetTitleField(oleDbCommand, tableName, objectid);
			oleDbCommand.CommandText = "";
			StringBuilder stringBuilder = new StringBuilder("INSERT INTO " + tableName + "(");
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (string current in titleField)
			{
				stringBuilder.Append(current + ",");
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			stringBuilder.Append(") VALUES(");
			int length = stringBuilder.Length;
			foreach (T current2 in entitys)
			{
				foreach (string current3 in titleField)
				{
					Clazz clazz;
					bool flag = dictionary.TryGetValue(current3, out clazz);
					if (flag)
					{
						object obj = clazz.GetMethodInfo.Invoke(current2, null);
						bool flag2 = obj != null && obj.ToString().Contains("'");
						if (flag2)
						{
							stringBuilder.Append("'" + obj.ToString().Replace("'", "") + "',");
						}
						else
						{
							bool flag3 = obj is DateTime;
							if (flag3)
							{
								bool flag4 = ((DateTime)obj).Year == 1;
								if (flag4)
								{
									stringBuilder.Append("'',");
								}
								else
								{
									stringBuilder.Append("'" + obj + "',");
								}
							}
							else
							{
								stringBuilder.Append("'" + obj + "',");
							}
						}
					}
					else
					{
						stringBuilder.Append("'',");
					}
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append(");");
				OleDbCommand expr_251 = oleDbCommand;
				expr_251.CommandText += stringBuilder.ToString();
				stringBuilder.Remove(length, stringBuilder.Length - length);
			}
			string text = oleDbCommand.CommandText;
			text = text.Remove(text.Length - 1, 1);
			Console.WriteLine(text);
			oleDbCommand.CommandText = text;
			int num = oleDbCommand.ExecuteNonQuery();
			oleDbTransaction.Commit();
			oleDbConnection.Close();
		}*/

		public static void WriteData<T>(string MDBPath, string tableName, IList<T> entitys, bool objectid = true)
		{
			DBEngine dBEngine = new DBEngineClass();
			Database database = dBEngine.OpenDatabase(MDBPath, Type.Missing, Type.Missing, Type.Missing);
			database.BeginTrans();
			Recordset recordset = database.OpenRecordset(tableName, Type.Missing, Type.Missing, Type.Missing);
			Dictionary<Field, Clazz> filedClazz =GetFiledClazz<T>(recordset, objectid);
			foreach (T current in entitys)
			{
				recordset.AddNew();
				foreach (Field current2 in filedClazz.Keys)
				{
					object obj = filedClazz[current2].GetMethodInfo.Invoke(current, null);
					bool flag = obj is DateTime;
					if (flag)
					{
						bool flag2 = ((DateTime)obj).Year == 1;
						if (flag2)
						{
							continue;
						}
					}
					bool flag3 = obj != null;
					if (flag3)
					{
						bool flag4 = obj is int && (int)obj == 0;
						if (!flag4)
						{
							current2.Value = obj;

						}
					}
				}
				recordset.Update(1, false);
			}
			database.CommitTrans(0);
			recordset.Close();
			database.Close();
		}

		private static Dictionary<Field, Clazz> GetFiledClazz<T>(Recordset rs, bool objectid)
		{
			Dictionary<string, Clazz> dictionary = ReflectUtils.MethodToFunction<T>();
			Dictionary<Field, Clazz> dictionary2 = new Dictionary<Field, Clazz>();
			foreach (Field field in rs.Fields)
			{
				string name = field.Name;
				bool flag = !objectid || !name.Equals("OBJECTID");
				if (flag)
				{
					Clazz value;
					bool flag2 = dictionary.TryGetValue(name, out value);
					if (flag2)
					{
						dictionary2.Add(field, value);
					}
				}
			}
			return dictionary2;
		}

		public static IList<string> GetTitleField(OleDbCommand odCommand, string tableName, bool noObjectid)
		{
			IList<string> list = new List<string>();
			odCommand.CommandText = "select * from " + tableName;
			OleDbDataReader oleDbDataReader = odCommand.ExecuteReader();
			int fieldCount = oleDbDataReader.FieldCount;
			for (int i = 0; i < fieldCount; i++)
			{
				string name = oleDbDataReader.GetName(i);
				bool flag = !noObjectid || !name.Equals("OBJECTID");
				if (flag)
				{
					list.Add(name);
				}
			}
			oleDbDataReader.Close();
			return list;
		}

        /// <summary>
        /// 删除数据 通过sql语句 
        /// </summary>
        /// <param name="mdbPath"></param>
        /// <param name="sql"></param>
        public static void DeleteBySql(string mdbPath,string sql)
        {
            IList<JObject> list = new List<JObject>();
            OleDbCommand oleDbCommand = ConnectMDB(mdbPath);
            oleDbCommand.CommandText = sql;
            OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader();
           

            oleDbDataReader.Close();
            oleDbCommand.Connection.Close();
           
        }

    }
}
