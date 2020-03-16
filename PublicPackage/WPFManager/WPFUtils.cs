using MyUtils;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;


namespace WPFManager
{
    public class WPFUtils
    {
        /// <summary>
        /// 添加不重复的item
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="comboBox"></param>
        /// <param name="imtemTemplete">模板item</param>
        /// <param name="remove">是否移除模板数据</param>
        public static void AddComBoxItem(IList<string> texts, ComboBox comboBox, object imtemTemplete,bool remove=false,int index=0)
        {
            if (texts != null && texts.Count > 0)
            {
                Thickness padding;
                if (imtemTemplete != null)
                {
                    ComboBoxItem temp = (ComboBoxItem)(imtemTemplete);
                    padding = temp.Padding;

                }
                else
                {
                    padding = new Thickness();
                }
                ItemCollection collection = comboBox.Items;
                IList<String> list = CollectionToContents(collection);
                foreach (string text in texts)
                {
                    if (!list.Contains(text))
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = text;
                        item.Padding = padding;
                        collection.Add(item);
                    }
                }
                if(remove)
                {
                    collection.Remove(imtemTemplete);
                   
                }
                comboBox.SelectedIndex = index;
                

            }

        }

        

        /// <summary>
        /// 查检是否是整数
        /// </summary>
        /// <param name="text">检查内容</param>
        /// <param name="tip">提示内容</param>
        /// <param name="numTip">提示框</param>
        public static void IsInt(string text, string tip, Label numTip)
        {
            if (Utils.IsInt(text))
            {
                numTip.Visibility = Visibility.Hidden;
            }
            else
            {
                numTip.Content = tip;
                numTip.Visibility = Visibility.Visible;
            }
        }

        public static void AddComBoxItem(IList<ControlsObject> controls, ComboBox comboBox, object imtemTemplete, bool remove = false, int index = 0)
        {
            if (controls != null && controls.Count > 0)
            {
                Thickness padding;
                if (imtemTemplete != null)
                {
                    ComboBoxItem temp = (ComboBoxItem)(imtemTemplete);
                    padding = temp.Padding;

                }
                else
                {
                    padding = new Thickness();
                }
                ItemCollection collection = comboBox.Items;
                IList<String> list = CollectionToContents(collection);
                foreach (ControlsObject obj in controls)
                {
                    if (!list.Contains(obj.Disname))
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = obj.Disname;
                        item.Padding = padding;
                        item.DataContext = obj;
                        collection.Add(item);
                    }
                }
                if (remove)
                {
                    collection.Remove(imtemTemplete);

                }
                comboBox.SelectedIndex = index;


            }

        }

        public static string GetTextBoxCurrentText(ComboBox dMModel)
        {
            var item = dMModel.SelectedItem as ComboBoxItem;
            return item.Content as string;
        }

        /// <summary>
        /// collection 内容转string
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private static IList<string> CollectionToContents(ItemCollection collection)
        {
            IList<string> list = new List<string>();
            if (collection == null)
            {
                return list;
            }
            foreach (ComboBoxItem item in collection)
            {
                object obj = item.Content;
                if (obj is string)
                {
                    list.Add((string)obj);
                }
            }
            return list;
        }

        public static void GridWidthCut(Window window, Grid grid, double width)
        {
            Grid parent = (Grid)grid.Parent;
            GridLength length = new GridLength(width);
            int index = Grid.GetColumn(grid);
            parent.ColumnDefinitions[index].Width = length;
            WPFUtils.ShowEffect(window, grid, 0, 0, 1);
        }

       

        /// <summary>
        /// 拉伸宽度
        /// </summary>
        /// <param name="window"></param>
        /// <param name="grid"></param>
        /// <param name="width"></param>
        public static void GridWidthAdd(Window window, Grid grid, double width)
        {
            Grid parent = (Grid)grid.Parent;
            GridLength length = new GridLength(width);
            int index = Grid.GetColumn(grid);
            parent.ColumnDefinitions[index].Width = length;           
            WPFUtils.ShowEffect(window, grid, 0, 0, 0.1);
        }
        

        /// <summary>
        /// 根据控件的Name获取控件对象
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="controlName">Name</param>
        /// <param name="command">主页</param>
        /// <returns></returns>
        public static T GetControlObject<T>(string controlName, object command)
        {
            try
            {
                Type type = command.GetType();
                FieldInfo fieldInfo = type.GetField(controlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    T obj = (T)fieldInfo.GetValue(command);
                    return obj;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }



        /// <summary>
        /// 没有查找出来返回为 NULL
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static object GetControlObject(string controlName, object command)
        {
            Type type = command.GetType();
            FieldInfo fieldInfo = type.GetField(controlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldInfo != null)
            {
                object obj = fieldInfo.GetValue(command);
                return obj;
            }
            return null;

        }
        /// <summary>
        /// 实体 中的数据 填写到 控件中
        /// </summary>
        /// <param name="control"></param>
        /// <param name="entity"></param>
        public static void ObjectToControl(object control, object entity)
        {
          
            if (entity == null)
            {
                return;
            }
            
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(entity);
            string strValue = null;
            object result;           
            foreach (string function in clazzDic.Keys)
            {
               
                Clazz clazz = clazzDic[function];
                MethodInfo m = clazz.GetMethodInfo;
                if(m == null)
                {
                    continue;
                }
                result =m.Invoke(entity, null);
                if (result == null)
                {
                    continue;
                }
                strValue = result.ToString();
                object com = GetControlObject(function, control);
                if (com != null)
                {
                    if (com is TextBox)
                    {
                        TextBox textBox = (TextBox)com;
                       
                        textBox.Text = strValue;
                    }
                    else if(com is ComboBox)
                    {
                        ComboBox comBox = (ComboBox)com;
                        ItemCollection ic = comBox.Items;

                        for (int a =0; a < ic.Count;a++)
                        {
                            ComboBoxItem item = (ComboBoxItem)ic[a];
                            object content = item.Content;
                            if(content is string )
                            {
                                string contentStr = (string)content;
                                if(contentStr.Equals(strValue))
                                {
                                    comBox.SelectedIndex = a;
                                }
                            }
                        }
                       
                    }else if (com is ContentControl)
                    {
                        ContentControl label = (ContentControl)com;
                        label.Content = result;
                       
                    }else if(com is DatePicker)
                    {
                        DatePicker data = (DatePicker)com;
                        data.DisplayDate = Utils.FormatDate(strValue);
                        data.Text = strValue;
                    }else if(com is TextBlock)
                    {
                        TextBlock textBlock = (TextBlock)com;
                        textBlock.Text = strValue;
                    }
                    strValue = null;
                }
            }
        }

       

        /// <summary>
        /// 控件 中数据 转为 实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control"></param>
        /// <returns></returns>
        public static T ControlToObject<T>(object control)
        {
            string fullName = typeof(T).AssemblyQualifiedName;
            Type type = Type.GetType(fullName);

            object obj = Activator.CreateInstance(type, true);//根据类型创建实例

            //dynamic obj = type.Assembly.CreateInstance(fullName);
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(type);



            object[] parameter = new object[1];
            object  value=null;
            foreach (string function in clazzDic.Keys)
            {

                Clazz clazz = clazzDic[function];
                object com = GetControlObject(function, control);
                if (com != null)
                {                   
                    if (com is TextBox)
                    {
                        TextBox textBox = (TextBox)com;
                        value = textBox.Text;                 
                        
                    }else if(com is ComboBox)
                    {
                        ComboBoxItem item = (ComboBoxItem)((ComboBox)com).SelectedItem;
                        value = item.Content;
                    }else if(com is Label)
                    {
                        Label label = (Label)com;
                        value = label.Content;
                        
                    }

                    if (value != null)
                    {
                        parameter[0] = value;
                        clazz.SetMethodInfo.Invoke(obj, parameter);
                    }
                }
            }
            return (T)obj;
        }
        public static void ControlToObject(object control,object obj)
        {

            //dynamic obj = type.Assembly.CreateInstance(fullName);
            Dictionary<string, Clazz> clazzDic = ReflectUtils.MethodToFunction(obj);



            object[] parameter = new object[1];
            object value = null;
            foreach (string function in clazzDic.Keys)
            {

                Clazz clazz = clazzDic[function];
                object com = GetControlObject(function, control);
                if(com is Control)
                {
                    if(!((Control)com).IsEnabled)
                    {
                        continue;
                    }
                }
                if (com != null)
                {
                   
                    if (com is TextBox)
                    {
                        TextBox textBox = (TextBox)com;
                        value = textBox.Text;

                    }
                    else if (com is ComboBox)
                    {
                        ComboBoxItem item = (ComboBoxItem)((ComboBox)com).SelectedItem;
                        value = item.Content;
                    }
                    else if (com is Label)
                    {
                        Label label = (Label)com;
                        value = label.Content;

                    }
                    else if(com is DatePicker)
                    {
                        
                        DatePicker data = (DatePicker)com;
                        value = data.SelectedDate.Value.ToString("yyyy/MM/dd");
                    }else
                    {
                        value = null;
                    }

                    if (value != null)
                    {
                        parameter[0] = value;
                        clazz.SetMethodInfo.Invoke(obj, parameter);
                    }
                }
            }           
        }
        /// <summary>
        /// 显示效果
        /// </summary>
        /// <param name="window"></param>
        /// <param name="red"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time"></param>
        public static void ShowEffect(Window window, Grid red, double from, double to, double time)
        {
            red.Visibility = Visibility.Visible;
            Storyboard myStoryboard = new Storyboard();

            DoubleAnimation OpacityDoubleAnimation = new DoubleAnimation();
            OpacityDoubleAnimation.From = 0;
            OpacityDoubleAnimation.To = 1;
            OpacityDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(time));
            Storyboard.SetTargetName(OpacityDoubleAnimation, red.Name);
            Storyboard.SetTargetProperty(OpacityDoubleAnimation, new PropertyPath(DataGrid.OpacityProperty));

            red.RenderTransform = new TranslateTransform();
            DependencyProperty[] propertyChain = new DependencyProperty[]
            {
               DataGrid.RenderTransformProperty,
               TranslateTransform.XProperty
            };

            DoubleAnimation InDoubleAnimation = new DoubleAnimation();
            InDoubleAnimation.From = from;
            InDoubleAnimation.To = to;
            InDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(time));
            Storyboard.SetTargetName(InDoubleAnimation, red.Name);
            Storyboard.SetTargetProperty(InDoubleAnimation, new PropertyPath("(0).(1)", propertyChain));
            myStoryboard.Children.Add(OpacityDoubleAnimation);
            myStoryboard.Children.Add(InDoubleAnimation);
            myStoryboard.Begin(window);
        }
        public static UIElement Clone(object model)
        {
           
            //rect = data.GetData(typeof(Rectangle)) as Rectangle;
            //canvas2.Children.Remove(rect);
            //canvas1.Children.Add(rect);
            //序列化Control,以深复制Control!!!!
            string rectXaml = XamlWriter.Save(model);
            StringReader stringReader = new StringReader(rectXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            UIElement clonedChild = (UIElement)XamlReader.Load(xmlReader);
            return clonedChild;
        }
        public static T Clone<T>(T templete)
        {
            string rectXaml = XamlWriter.Save(templete);
            StringReader stringReader = new StringReader(rectXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            object clonedChild = XamlReader.Load(xmlReader);
            return (T)clonedChild;
        }
    }
}
