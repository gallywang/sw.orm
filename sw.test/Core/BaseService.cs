using sw.orm;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.test
{
    public class BaseService<T> where T : new()
    {
        /// <summary>
        /// 是否存在
        /// </summary>
        public static bool Exists(Expression<Func<T, bool>> where)
        {
            return DB.client.Exists(where);
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        public static int GetCount()
        {
            return DB.client.GetCount<T>();
        }

        /// <summary>
        /// 跟根据条件获取数量
        /// </summary>
        public static int GetCount(Expression<Func<T, bool>> where)
        {
            return DB.client.GetCount<T>(where);
        }

        /// <summary>
        /// 获取一条数据
        /// </summary>
        public static T GetModel(string id)
        {
            return DB.client.GetModel<T>(id);
        }

        /// <summary>
        /// 根据条件获取一条数据
        /// </summary>
        public static T GetModel(Expression<Func<T, bool>> where)
        {
            return DB.client.GetModel(where);
        }

        /// <summary>
        /// 根据条件获取数据集合
        /// </summary>
        public static List<T> GetModelList(Expression<Func<T, bool>> where)
        {
            return DB.client.GetModelList(where);
        }

        /// <summary>
        /// 根据条件获取数据集合
        /// </summary>
        public static List<T> GetModelList(Where<T> where)
        {
            return DB.client.GetModelList(where);
        }

        /// <summary>
        /// 根据条件获取数据集合并按指定字段排序
        /// </summary>
        public static List<T> GetModelList(Where<T> where, Expression<Func<T, object>> orderby, AscOrDesc ascOrDesc)
        {
            return DB.client.GetModelList(where, orderby, ascOrDesc);
        }

        /// <summary>
        /// 根据条件获取数据集合并按指定字段排序
        /// </summary>
        public static List<T> GetModelList(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderby, AscOrDesc ascOrDesc)
        {
            return DB.client.GetModelList(where, orderby, ascOrDesc);
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public static List<T> GetTopModelList(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderby, AscOrDesc ascOrDesc, int top)
        {
            return DB.client.GetTopModelList(where, orderby, ascOrDesc, top);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public static List<T> GetAllList()
        {
            return DB.client.GetAll<T>();
        }

        /// <summary>
        /// 获取所有数据并按指定字段排序
        /// </summary>
        /// <returns></returns>
        public static List<T> GetAllList(Expression<Func<T, object>> orderby, AscOrDesc ascOrDesc)
        {
            return DB.client.GetAll<T>(orderby, ascOrDesc);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public static int Insert(T model)
        {
            return DB.client.Insert<T>(model);
        }

        /// <summary>
        /// 增加多条数据
        /// </summary>
        public static int Insert(List<T> model)
        {
            return DB.client.Insert<T>(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public static int Update(T model)
        {
            return DB.client.Update(model);
        }

        /// <summary>
        /// 根据条件更新实体指定字段
        /// </summary>
        public static int Update(Expression<Func<T, T>> updateFiled, Expression<Func<T, bool>> where)
        {
            return DB.client.Update(updateFiled, where);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public static int Delete(T model)
        {
            return DB.client.Delete<T>(model);
        }

        /// <summary>
        /// 删除单条记录
        /// </summary>
        public static int Delete(string id)
        {
            return DB.client.Delete<T>(id);
        }

        /// <summary>
        /// 根据ID集合删除数据
        /// </summary>
        public static int Delete(List<string> idList)
        {
            return DB.client.Delete<T>(idList);
        }

        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        public static int Delete(Expression<Func<T, bool>> where)
        {
            return DB.client.Delete<T>(where);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        public static List<T> GetModelListWithPage(Where<T> where, out int count, SWPaging paging)
        {
            return DB.client.GetModelListWithCount(where, out count, paging);
        }
    }
}
