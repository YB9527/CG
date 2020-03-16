using HeibernateManager.HibernateDao;
using ReflectManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeibernateManager.Model
{
    public class SoftwareConfig
    {
        private static HibernateUtils hibernateUtils = HibernateUtils.GetInstance();
        public SoftwareConfig()
        {

        }
       


        public static void SaveRedis(string key ,object obj)
        {
            try
            {
                string str = ReflectManager.JObejctReflect.ObjectToStr(obj);
                SoftwareConfig.Refresh(key, str);
            }
            catch
            {
                Delete(key);
            }
           
        }
        /// <summary>
        /// 如果序列化对象出现异常 将创建新的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetRedis<T>(string key)
        {
            try
            {
                SoftwareConfig config = SoftwareConfig.FindConfig(key);
                T model = JObejctReflect.ToObejct<T>(config.Value);
                return model;
            }
            catch
            {
                return ReflectUtils.CreateObject<T>();
            }
         
        }

        public static T GetRedis<T>(object redisKey)
        {
            throw new NotImplementedException();
        }

        public SoftwareConfig(string key,string value)
        {
            this.Key = key;
            this.Value = value;
        }
     
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }

        public static SoftwareConfig FindConfig(string key)
        {
            SoftwareConfig config = hibernateUtils.FindEntityById<SoftwareConfig>(key);
            if(config == null)
            {
                return null;
            }
            else
            {
                return config;
            }
           
        }
        /// <summary>
        /// 刷新实体
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Refresh(string key, string value)
        {
            SoftwareConfig config = FindConfig(key);
            if(config == null)
            {
                config = new SoftwareConfig(key, value);
                hibernateUtils.SaveEntity(config);
            }
            else
            {
                if(!config.Value.Equals(value))
                {
                    config.Value = value;
                    hibernateUtils.SaveEntity(config);
                }
            }
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key)
        {
            SoftwareConfig config = hibernateUtils.FindEntityById<SoftwareConfig>(key);
            if (config != null)
            {
                hibernateUtils.DeleteEntity(config);
            }
          
        }
    }
}
