using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using MyUtils;

namespace ReflectManager.XMLPackage
{
    public class XMLRead
    {

        private static Dictionary<string, Dictionary<int, XMLTable>> xmlTableDicCache;
       
        private static Dictionary<String, Dictionary<String, String>> xmlDicCache = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, Dictionary<String, XMLObject>> xmlObjecDicCache = new Dictionary<string, Dictionary<string, XMLObject>>();
       
       
        private static Dictionary<string, Dictionary<string, XMLSymbol>> symbolDicCache= new Dictionary<string, Dictionary<string, XMLSymbol>>();
        private static Dictionary<string, Dictionary<string, Clazz>> TypeToClazzCache = new Dictionary<string, Dictionary<string, Clazz>>();


        public  XMLRead()
        {
            
        }

        public static IList<String> GetAttribute(String xmlPath, String tagName, String valueName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);

            IList<String> list = new List<String>();
            String value;
            foreach (XmlNode xmlNode in nodes)
            {
                XmlElement xe = (XmlElement)xmlNode;
                value = xe.GetAttribute(valueName);
                if (!Utils.IsStrNull(value))
                {
                    list.Add(value);
                }
            }
            return list;
        }
        /// <summary>
        /// 对象实体 解析成get 的 XMLObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, XMLObject> XmlToObjects_get<T>()
        {
            Dictionary<string, XMLObject> dic;

            if (xmlObjecDicCache.TryGetValue(typeof(T).AssemblyQualifiedName, out dic))
            {
                return dic;
            }
            dic = new Dictionary<string, XMLObject>();
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction<T>();
            foreach (string methodName in clazzDic.Keys)
            {
                    Clazz clazz = clazzDic[methodName];
                    MethodInfo m = clazz.GetMethodInfo;
                    if (m == null)
                    {
                        continue;
                    }
                    XMLObject xmlObj = new XMLObject();
                    xmlObj.Key = methodName;
                    xmlObj.MethodInfos.Add(m);
                    xmlObj.MethoedName.Add(m.Name);
                    xmlObj.Parameters.Add(null);
                    xmlObj.Types.Add(null);
                    dic.Add(methodName, xmlObj);
                
            }
            xmlObjecDicCache.Add(typeof(T).AssemblyQualifiedName, dic);
            return dic;
        }

        /**/
        public static Dictionary<string, string> GetXmlReplace(string xmlConfigName, Object obj)
        {
            Dictionary<String, XMLObject> xmlDkDic = XmlToObjects(xmlConfigName);
            return GetObjectValue(xmlDkDic, obj);
        }

        public static Dictionary<string, string> GetConfigXmlDic(string xmlPath)
        {
           return  GetConfigXmlDic(xmlPath, "property", "key", "value");
        }

        /// <summary>
        /// key 值的转换
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static Dictionary<string, Clazz> TypeToClazz(Type type, string xmlPath)
        {
            Dictionary<string, Clazz> dic;
            if(TypeToClazzCache.TryGetValue(xmlPath,out dic))
            {
                return dic;
            }
            dic = new Dictionary<string, Clazz>();
            Dictionary<String, String> filedDic = XMLRead.GetConfigXmlDic(xmlPath, "property", "key", "value");
            filedDic = Utils.DicReset<string, string>(filedDic);
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(type.AssemblyQualifiedName);
            string aslisName;
            foreach(string method in clazzDic.Keys)
            {

                if(filedDic.TryGetValue(method,out aslisName))
                {
                    dic.Add(filedDic[method], clazzDic[method]);
                }else
                {
                   // dic.Add(method, clazzDic[method]);
                }
            }
            dic.Remove("Equals");
            TypeToClazzCache.Add(xmlPath,  dic);
            return dic;
        }

        internal static Dictionary<string, string> GetObjectValue(Dictionary<String, XMLObject> xmlObjectDic, object obj)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, XMLObject>.KeyCollection keys = xmlObjectDic.Keys;
            Type type = obj.GetType();
            XMLObject xmlObject;
            foreach (String key in keys)
            {
                xmlObject = xmlObjectDic[key];
                Object result = GetObjectMethodResult(xmlObject, obj);
                dic.Add(key, result.ToString());

            }

            return dic;
        }
        internal static void GetObjectValue(Dictionary<string,XMLSymbol> xmlSymbol, object obj)
        {
      
            Dictionary<string, XMLSymbol>.KeyCollection keys = xmlSymbol.Keys;
            Type type = obj.GetType();
            XMLSymbol sm;
            foreach (String key in keys)
            {
                sm = xmlSymbol[key];
                Object result = GetObjectMethodResult(sm.XmlObject, obj);             
                sm.Result = sm.RepDic[result.ToString()];
            }

       
        }

        public static object GetObjectMethodResult(XMLObject xmlObject, object obj)
        {
            IList<String> ms = xmlObject.MethoedName;
            IList<MethodInfo> methodInfos = xmlObject.MethodInfos;
            Object result = null;
            
            if (methodInfos.Count > 0)
            {
                result = GetObjMethodValue(obj, methodInfos[0], xmlObject.Parameters[0]);
                for (int a = 1; a < ms.Count; a++)
                {
                    if (result == null)
                    {
                        continue;
                    }
                    result = GetObjMethodValue(result, methodInfos[a], xmlObject.Parameters[a]);
                }
            }
            if (result == null)
            {
                return xmlObject.Deafult;
            }
            else if (result is string)
            {
                string resultStr = (string)result;
                if ("".Equals(resultStr))
                {
                    return xmlObject.Deafult;
                }
                else
                {
                    return resultStr;
                }
            }
            else
            {
                return result;
            }

        }

        public static object GetObjMethodValue(object obj, MethodInfo m, object[] parameters)
        {


            object reuslt = m.Invoke(obj, parameters);
            
          
            return reuslt;
           // return null;
        }

        private static Type[] GetParametersType(object[] parameters)
        {

            int length = parameters.Length;
            Type[] types = new Type[length];
            for (int a = 0; a < length; a++)
            {
                types[a] = parameters[a].GetType();
            }
            return types;
        }

        public static XmlDocument GetXmlDocument(String xmlPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
           
            return xmlDoc;
        }
        public static Dictionary<String, String> GetNbernateDic(String xmlValue)
        {
            Dictionary<String, String> dic = new Dictionary<string, string>();
            //Server=localhost;database=xx;uid=root;pwd=123;
            String[] str = xmlValue.Split(';');
            String[] oneArray;
            foreach (String one in str)
            {
                oneArray = one.Split('=');
                if (oneArray.Length == 2)
                {
                    dic.Add(oneArray[0], oneArray[1]);
                }
            }
            return dic;
        }


        public static Dictionary<String, String> GetConfigXmlDic(String xmlPath, String tagName, String name, String value)
        {

            Dictionary<String, String> dic;
           
            if (xmlDicCache.TryGetValue(xmlPath, out dic) )
            {
                return dic;
            }
            dic = GetXmlDic(xmlPath, tagName, name, value);
            //dic存入缓存中
            if (xmlDicCache.ContainsKey(xmlPath))
            {
                xmlDicCache[xmlPath] = dic;
            }
            else
            {
                xmlDicCache.Add(xmlPath, dic);
            }
            return dic;                  
        }
        /// <summary>
        /// xml转成 dic《String,Stirng》
        /// </summary>
        /// <param name="xmlPath">路径</param>
        /// <param name="tagName">标签名字</param>
        /// <param name="name">key</param>
        /// <param name="value">value 如果为空用的name值</param>
        /// <param name="cache">缓存</param>
        /// <returns></returns>
        public static Dictionary<String, String> GetXmlDic(String xmlPath, String tagName, String name, String value)
        {
            Dictionary<String, String> dic;
            if (xmlDicCache == null)
            {
                xmlDicCache = new Dictionary<string, Dictionary<string, string>>();
            }
            else if (xmlDicCache.TryGetValue(xmlPath, out dic))
            {
                return dic;
            }
            dic = new Dictionary<string, string>();
           
            XmlDocument xmlDoc = GetXmlDocument(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName(tagName);
            String nameVaule;
            String valueResult;
            foreach (XmlNode node in nodes)
            {
                XmlElement xe = (XmlElement)node;
                nameVaule = xe.GetAttribute(name);
                if(xe.HasAttribute(value))
                {
                    valueResult = xe.GetAttribute(value);
                }
                else
                {
                    valueResult = nameVaule;
                }
               
                if (Utils.IsStrNull(nameVaule) || Utils.IsStrNull(valueResult))
                {
                    continue;
                }
                if (dic.ContainsKey(nameVaule))
                {
                    throw new XmlException(xmlPath + "，配置:   " + nameVaule + " 值重复");
                }
                else
                {
                    dic.Add(nameVaule, valueResult);
                }
            }

            return dic;
        }
        public static Dictionary<String, XMLObject> XmlToObjects(String xmlPath, bool methodAll=true)
        {
            Dictionary<String, XMLObject> dic;
           
             if (xmlObjecDicCache.TryGetValue(xmlPath, out dic))
            {
                return dic;
            }
            dic = new Dictionary<String, XMLObject>();
        
            XmlDocument xmlDoc = GetXmlDocument(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName("property");
            XmlNodeList v = xmlDoc.GetElementsByTagName("propertys");
            string defaltClassType = null;
            string defualtMehtod = null;
            if (v.Count >0)
            {
                    defaltClassType =((v.Item(0)) as XmlElement).GetAttribute("ClassType");
                 defualtMehtod = ((v.Item(0)) as XmlElement).GetAttribute("method");
            }
           
            XMLObject obj;
            IList<String> methods;
            IList<object[]> parameterList;
            IList<Type[]> types;
            IList<MethodInfo> methodInfos;
            foreach (XmlNode node in nodes)
            {
                methods = new List<string>();
                parameterList = new List<object[]>();
                types = new List<Type[]>();
                obj = new XMLObject();
                methodInfos = new List<MethodInfo>();
                obj.MethoedName = methods;
                obj.Parameters = parameterList;
                obj.Types = types;
                obj.MethodInfos = methodInfos;
                XmlElement xe = (XmlElement)node;
                if(xe.HasAttribute("key"))
                {
                    obj.Key = xe.GetAttribute("key");
                }else
                {
                    obj.Key = xe.GetAttribute("name");
                }
               
                if(xe.HasAttribute("column"))
                {
                    obj.Column = xe.GetAttribute("column");
                }
                obj.Deafult = xe.GetAttribute("default");
                NodeValueToXmlObject(defaltClassType,xe, methods, methodInfos, parameterList, types);
                dic.Add(obj.Key, obj);
            }
            //添加还未加入的方法，确保所有get方法都能使用
            if(methodAll)
            {
                Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(defaltClassType);
                foreach (string methodName in clazzDic.Keys)
                {

                    if (!dic.ContainsKey(methodName))
                    {

                        Clazz clazz = clazzDic[methodName];
                        MethodInfo m;
                        if ("set_".Equals(defualtMehtod))
                        {
                            m = clazz.SetMethodInfo;
                        }
                        else
                        {
                            m = clazz.GetMethodInfo;

                        }

                        if (m == null)
                        {
                            continue;
                        }
                        XMLObject xmlObj = new XMLObject();
                        xmlObj.Key = methodName;
                        xmlObj.MethodInfos.Add(m);
                        xmlObj.MethoedName.Add(m.Name);
                        xmlObj.Parameters.Add(null);
                        xmlObj.Types.Add(null);
                        dic.Add(methodName, xmlObj);
                    }
                }
            
            }
            //得到表格对象
            //XmlNodeList tables = xmlDoc.GetElementsByTagName("table");
            if (xmlObjecDicCache.ContainsKey(xmlPath))
            {
                xmlObjecDicCache[xmlPath] = dic;
            }
            else
            {
                xmlObjecDicCache.Add(xmlPath, dic);
            }
            return dic;
        }

    

      

        /// <summary>
        /// docx 表格映射成对象
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="xmlObjectDic"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        //public static IList<XMLTable> GetXmlToXmlTabl(String xmlPath)
        //{
        //    IList<XMLTable> dic;         
        //    if (xmlTableDicCache.TryGetValue(xmlPath, out dic))
        //    {
        //        return dic;
        //    }
        //    XmlDocument xmlDoc = GetXmlDocument(xmlPath);
        //    XmlNodeList nodes = xmlDoc.GetElementsByTagName("table");
        //    Dictionary<String, XMLObject> xmlObjectDic = XmlToObjects(xmlPath);
        //    dic = GetXmlToXmlTabl(nodes, xmlObjectDic);
        //    return dic;
        //}
        public static Dictionary<int, XMLTable> GetXmlToXMLTabl(String xmlPath)
        {
            Dictionary<int, XMLTable> dic;
            if (xmlTableDicCache == null)
            {
                xmlTableDicCache = new Dictionary<string, Dictionary<int, XMLTable>>();
            }
            else if (xmlTableDicCache.TryGetValue(xmlPath, out dic) )
            {
                return dic;
            }
          
            XmlDocument xmlDoc = GetXmlDocument(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName("table");
            Dictionary<String, XMLObject> xmlObjectDic = XmlToObjects(xmlPath);
            
            dic = GetXmlToXmlTabl(nodes, xmlObjectDic);

            return dic;
        }
        public static Dictionary<int, XMLTable> GetXmlToXmlTabl(XmlNodeList nodes, Dictionary<String, XMLObject> xmlObjectDic)
        {
            Dictionary<int, XMLTable> dic = new Dictionary<int, XMLTable>();
            if (nodes == null)
            {
                return dic;
            }
            XMLTable xmlTable;
            foreach (XmlNode node in nodes)
            {

                xmlTable = GetXmlTable(node, xmlObjectDic);
                dic.Add(xmlTable.Index, xmlTable);
            }
            return dic;
        }
        private static IList<XMLTable> GetXmlToXmlTabl2(XmlNodeList nodes, Dictionary<String, XMLObject> xmlObjectDic)
        {
            IList<XMLTable> dic = new List<XMLTable>();
            if (nodes == null)
            {
                return dic;
            }
            XMLTable xmlTable;
            foreach (XmlNode node in nodes)
            {

                xmlTable = GetXmlTable(node, xmlObjectDic);
                dic.Add( xmlTable);
            }
            return dic;
        }

        private static XMLTable GetXmlTable(XmlNode node, Dictionary<String, XMLObject> xmlObjectDic)
        {
            XmlElement xe = (XmlElement)node;
            
           
            XMLTable xmlTable = new XMLTable();
           
            XmlNodeList rows = xe.GetElementsByTagName("row");

            xmlTable.XmlRows = GetXmlRows(rows,xmlObjectDic);
            foreach(  XMLRow  r in xmlTable.XmlRows)
            {
                r.XmlTable = xmlTable;
            }

            XmlElement rowXe = (XmlElement)rows[0];
            xmlTable.RowStartIndex = int.Parse(rowXe.GetAttribute("rowStartIndex"));
            if (rowXe.HasAttribute("rows"))
            {
                xmlTable.Rows = int.Parse(rowXe.GetAttribute("rows"));
            }
            if (rowXe.HasAttribute("end"))
            {
                xmlTable.RowEndIndex = int.Parse(rowXe.GetAttribute("end"));
            }

            string cellTotal = rowXe.GetAttribute("cellTotal");
            if (!"".Equals(cellTotal))
            {
                xmlTable.CellTotal = int.Parse(cellTotal);
            }
            string TableCount = xe.GetAttribute("tableCount");
            if (!"".Equals(TableCount))
            {
                xmlTable.TableCount = int.Parse(TableCount);
            }
            if(rowXe.HasAttribute("toRow"))
            {
                xmlTable.ToRow = int.Parse(rowXe.GetAttribute("toRow"));
            }
            else
            {
                xmlTable.ToRow = 0;
            }
            String writeIndex = rowXe.GetAttribute("WriteIndex");

            if(!"".Equals(writeIndex))
            {
                xmlTable.Index = int.Parse(writeIndex);
            }
            string talbeIndex = xe.GetAttribute("tableIndex");
            if (!"".Equals(talbeIndex))
            {
                xmlTable.TableIndex = int.Parse(talbeIndex);
            }
            string index = xe.GetAttribute("index");
            if (!"".Equals(index))
            {
                xmlTable.Index = int.Parse(index);
            }
            XmlNodeList cells = rowXe.GetElementsByTagName("cell");
            xmlTable.CellDic = GetXmlCellDic(cells, xmlObjectDic);

            //解析grop
            XmlNodeList groups = rowXe.GetElementsByTagName("group");
            xmlTable.XmlGroups = GetXmlGrougs(groups, xmlObjectDic);

            return xmlTable;
        }

        private static IList<XMLRow> GetXmlRows(XmlNodeList rows, Dictionary<String, XMLObject> xmlObjectDic)
        {
            IList<XMLRow> xmlRows = new List<XMLRow>();
            foreach(XmlElement rowXe in rows)
            {
                XMLRow xmlRow = GetXmlRow(rowXe, xmlObjectDic);
                xmlRows.Add(xmlRow);
            }
            return xmlRows;
           
        }

        private static XMLRow GetXmlRow(XmlElement rowXe, Dictionary<String, XMLObject> xmlObjectDic)
        {
            XMLRow xmlRow = new XMLRow();
            xmlRow.RowStartIndex = int.Parse(rowXe.GetAttribute("rowStartIndex"));
            if (rowXe.HasAttribute("rows"))
            {
                xmlRow.Rows = int.Parse(rowXe.GetAttribute("rows"));
            }
            string PageCount =rowXe.GetAttribute("PageCount");
            if(!"".Equals(PageCount))
            {
                xmlRow.PageCount = int.Parse(PageCount);
            }
            string cellTotal = rowXe.GetAttribute("cellTotal");
            if (!"".Equals(cellTotal))
            {
                xmlRow.CellTotal = int.Parse(cellTotal);
            }
            string rowStep = rowXe.GetAttribute("rowStep");
            if (!"".Equals(rowStep))
            {
                xmlRow.RowStep = int.Parse(rowStep);
            }
            else
            {
                xmlRow.RowStep = 1;
            }
            String writeIndex = rowXe.GetAttribute("WriteIndex");

            if (!"".Equals(writeIndex))
            {
                xmlRow.Index = int.Parse(writeIndex);
            }
            XmlNodeList cells = rowXe.GetElementsByTagName("cell");
            xmlRow.cellDic = GetXmlCellDic(cells, xmlObjectDic);

            //解析grop
            XmlNodeList groups = rowXe.GetElementsByTagName("group");
            xmlRow.XmlGroups = GetXmlGrougs(groups, xmlObjectDic);

            return xmlRow;
        }

        internal static Dictionary<string, XMLSymbol> XmlToSymbol(string xmlPath)
        {
            Dictionary<String, XMLSymbol> dic;

            if (symbolDicCache.TryGetValue(xmlPath, out dic))
            {
                return dic;
            }
            Dictionary<String, XMLObject> xmlobjectDic = XmlToObjects(xmlPath);       
            XmlDocument xmlDoc = GetXmlDocument(xmlPath);
            XmlNodeList nodes = xmlDoc.GetElementsByTagName("Sympbol");
            dic = GetXmlSmybol(nodes);
            foreach(string key in dic.Keys)
            {
                dic[key].XmlObject = xmlobjectDic[key];
            }
            symbolDicCache.Add(xmlPath,dic);
            return dic;
        }
        private static Dictionary<string, XMLSymbol> GetXmlSmybol(XmlNodeList nodes)
        {
            Dictionary<string, XMLSymbol> dic = new Dictionary<string, XMLSymbol>();
            if (nodes.Count == 0)
            {
                return null;
            }
            XmlElement xe;
            String refXmobject;
            foreach (XmlNode node in nodes)
            {
                XMLSymbol sm = new XMLSymbol();
                xe = (XmlElement)node;
                refXmobject = xe.GetAttribute("ref");
                String mapStr = xe.GetAttribute("json");
                String[] mapArray = mapStr.Split(',');
                Dictionary<string, string> rep = new Dictionary<string, string>();
                foreach (String map in mapArray)
                {
                    String[] one = map.Split(':');
                    rep.Add(one[0], one[1]);
                }
                sm.RepDic = rep;
                dic.Add(refXmobject, sm);
            }
            return dic;
        }
            private static IList<XMLGroup> GetXmlGrougs(XmlNodeList groups, Dictionary<string, XMLObject> xmlObjectDic)
        {
            if(groups.Count == 0)
            {
                return null;
            }
            IList<XMLGroup> list = new List<XMLGroup>();
            XmlElement xe;
            String refXmobject;
            XMLGroup group;
            Dictionary<String, int> dic;
            foreach (XmlNode node in groups)
            {
                group = new XMLGroup();
                dic = new Dictionary<string, int>();
                xe = (XmlElement)node;
                refXmobject = xe.GetAttribute("ref");
                group.XmlObject = xmlObjectDic[refXmobject];
                group.Result = xe.GetAttribute("result");
                String mapStr = xe.GetAttribute("json");
                String[] mapArray = mapStr.Split(',');
                foreach(String map in mapArray)
                {
                    String[] one = map.Split(':');
                    dic.Add(one[0], int.Parse(one[1]));
                }
                group.GroupDic = dic;
                list.Add(group);
            }
            return list;
        }

        private static Dictionary<int, XMLObject> GetXmlCellDic(XmlNodeList cells, Dictionary<string, XMLObject> xmlObjectDic)
        {
            Dictionary<int, XMLObject> dic = new Dictionary<int, XMLObject>();
            if (cells == null)
            {
                return dic;
            }
            XmlElement xe;
            int cellIndex;
            XMLObject xmlObject;
            String refXmobject;

            foreach (XmlNode node in cells)
            {
                xe = (XmlElement)node;
                cellIndex = int.Parse(xe.GetAttribute("index"));
                refXmobject = xe.GetAttribute("ref");
                dic.Add(cellIndex, xmlObjectDic[refXmobject]);
            }
            return dic;
        }

        /// <summary>
        /// 递归投影对象
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="methods"></param>
        /// <param name="parameterList"></param>
        /// <param name="types"></param>
        private static void NodeValueToXmlObject(string defaltClassType, XmlElement xe, IList<String> methods, IList<MethodInfo> methodInfos, IList<object[]> parameterList, IList<Type[]> types)
        {
            if (xe == null)
            {
                return;
            }
            string method;
            //先吃完本级的
            if (xe.HasAttribute("value"))
            {
                 method = xe.GetAttribute("value");

                if (method.Equals(""))
                {
                    return;
                }
            }else
            {
                method = "get_" + xe.GetAttribute("key");
            }
           
            methods.Add(method);

            String[] typesArray = xe.GetAttribute("parameterClassType").Split(',');
            Type[] stringType;
            Type[] type = null;
            if (!typesArray[0].Equals(""))
            {
                stringType = new Type[1];
                stringType[0] = Type.GetType("System.String");
                int length = typesArray.Length;
                type = new Type[length];
                object[] pArray = xe.GetAttribute("parameters").Split(',');
                object[] objP = new object[1];
                object[] reulstP = new object[length];
                for (int a = 0; a < length; a++)
                {
                    Type t = Type.GetType(typesArray[a]);
                    type[a] = Type.GetType(typesArray[a]);

                    //设置字符串正确的类型  
                    MethodInfo mParse = t.GetMethod("Parse", stringType);
                    if (mParse != null)
                    {
                        objP[0] = pArray[a];
                        reulstP[a] = mParse.Invoke(null, objP);

                    }
                    else
                    {
                        reulstP[a] = pArray[a];
                    }
                }
                types.Add(type);
                parameterList.Add(reulstP);




            }
            else
            {
                types.Add(null);
                parameterList.Add(null);
            }
            string typeName;
            if (xe.HasAttribute("ClassType"))
            {
                typeName = xe.GetAttribute("ClassType");
            }
            else
            {
                typeName = defaltClassType;
            }
         
            Type typeA = Type.GetType(typeName);
            if (type == null)
            {
                //MethodInfo[] ms = typeA.GetMethods();
                methodInfos.Add(typeA.GetMethod(method));
            }
            else
            {
                methodInfos.Add(typeA.GetMethod(method, type));
            }
            //再查询所有字节点
            XmlNodeList nodes = xe.ChildNodes;
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    NodeValueToXmlObject(defaltClassType,(XmlElement)node, methods, methodInfos, parameterList, types);
                }
            }
        }


    }
}
