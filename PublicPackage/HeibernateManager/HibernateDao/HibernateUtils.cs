using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using ReflectManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HeibernateManager.HibernateDao
{
    public class HibernateUtils
    {
        private static HibernateUtils utils = null;
      
        public static HibernateUtils GetInstance()
        {
            if(utils == null)
            {
                utils = new HibernateUtils();
                utils.OpenSession();
                
            }
            return utils;
        }
        public HibernateUtils()
        {
            //utils.OpenSession();
        }
        private static ISessionFactory mySessionFactory = null; //默认为空

        private static ISession Session = null;//默认为空

        private ISession GetSession()
        {
            return OpenSession();
        }

        public ISession OpenResetSession2(string xmlPath)
        {
            if(Session != null)
            {
                Session.Close();
                Session = null;
            }
                var myConfiguration = new Configuration();
            

                myConfiguration.Configure(xmlPath);
                mySessionFactory = myConfiguration.BuildSessionFactory();
                Session = mySessionFactory.OpenSession();
               
          
            utils = this;
            return Session;
        }

        public  ISession OpenSession()
        {
            if (Session == null)
            {
                var myConfiguration = new Configuration();
                myConfiguration.Configure();
                mySessionFactory = myConfiguration.BuildSessionFactory();
                Session = mySessionFactory.OpenSession();
                return Session;
            }
            utils = this;
            return Session;
        }

        public  void CloseSession()
        {

            if (mySessionFactory != null)

                mySessionFactory.Close(); //使用完毕后，关闭工厂

            if (Session != null)
            
                Session.Close(); //使用完毕后，关闭会话通道
        }
        
        /// <summary>
        /// 保存单个的对象
        /// </summary>
        /// <param name="obj"></param>
        public void SaveEntity<T>(T t)
        {
            ITransaction trans = Session.BeginTransaction();
            Session.SaveOrUpdate(t);
            try
            {
                trans.Commit();
            }
            catch(Exception e)
            {

            }
           
        }
      
        /// <summary>
        /// 通过id查找对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindEntityById<T>(object id)
        {
            try
            {
                return Session.Get<T>(id);
            }catch
            {
                return default(T);
            }
           
        }

        /// <summary>
        /// 通过sql查找对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList<T> FindEntitysBySql<T>(string sql)
        {

            IList<T> list = Session.CreateQuery(sql).List<T>();
            return list;
        }
        /// <summary>
        /// 根据泛型 得到所有的 数据库实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> FindEntitysAll<T>()
        {
          
            //Session.Clear();
            string className = typeof(T).Name;
            IList<T> list = utils.FindEntitysBySql<T>("From " + className);
       
            return list;
        }

        public IList FindEntitysBySql(string sql)
        {
            return Session.CreateQuery(sql).List();
        }


        /// <summary>
        /// 级联删除对象、parent对象的子对象集合设置为null，才能删除子对象、直接删除父对象时、所有子对象也会删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public void DeleteEntity<T>(IList<T> objs)
        {
            if (objs != null && objs.Count >0)
            {
                ITransaction trans = Session.BeginTransaction();
           
                foreach(T t in objs)
                {
                    Session.Delete(t);
                }
                try{
                    trans.Commit();

                }
                catch(Exception e)
                {

                }
                    
            
              
            }
           
        }

        /// <summary>
        /// 删除单个对象
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteEntity(object entity)
        {
            ITransaction trans = Session.BeginTransaction();
            Session.Delete(entity);
            trans.Commit();
        }
        /// <summary>
        /// 批量保存集合对象
        /// </summary>
        /// <param name="objs"></param>
        public void SaveEntity<T>(IList<T> objs)
        {
            ITransaction trans = Session.BeginTransaction();
            foreach (object obj in objs)
            {              
                Session.SaveOrUpdate(obj);
            }
            trans.Commit();
        }
           /// <summary>
           /// 修改单个对象
           /// </summary>
           /// <typeparam name="T"></typeparam>
           /// <param name="t"></param>
            public void UpdateEntity<T>(T t)
        {
            ITransaction trans = Session.BeginTransaction();
            Session.Update(t);
            trans.Commit();
        }
        /// <summary>
        /// 批量修改对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        public void UpdateEntity<T>(IList<T> ts)
        {
            ITransaction trans = Session.BeginTransaction();
            foreach (T t in ts)
            {
                Session.Update(t);
            }
            trans.Commit();
        }
        



        public IList<T> HighSelect<T>(IList<PropertPo> pos)
        {
            Conjunction andCon = Restrictions.Conjunction();
            Disjunction orCon = Restrictions.Disjunction();
            ICriteria criteria = Session.CreateCriteria(typeof(T));
            foreach (PropertPo po in pos)
            {
                AbstractCriterion criterion = PropertUtils.ReflectGetValue(po.methodName, po.Forname, po.Values.ToArray<string>());

                if (po.LogicalMethod.Equals("And"))
                {
                    andCon.Add(criterion);

                }
                else
                {
                    orCon.Add(criterion);

                }
            }
          
            if (andCon.ToString().Length > 2 && orCon.ToString().Length > 2)
            {
                criteria.Add(Restrictions.And(andCon, orCon));
            }
            else
            if (andCon.ToString().Length > 2)
            {
                criteria.Add(andCon);
            }
            else if (orCon.ToString().Length > 2)
            {
                criteria.Add(orCon);
            }


            IList<T> list = criteria.List<T>();
            return list;
        }
        public int FindCount<T>()
        {
            string hql = "select count(*) From " + typeof(T).Name;
            //object count = Session.CreateQuery(hql).UniqueResult();
            IList list = Session.CreateQuery(hql).List();
            string count = list[0].ToString();
            return int.Parse(count.ToString());
        }

        public int DeleteAllData<T>()
        {
            ITransaction trans = Session.BeginTransaction();
            int count = Session.Delete("From " + typeof(T).Name);
            trans.Commit();
            return count;
        }
    }
}
