using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace sw.orm
{
    internal class MongodbCreate
    {
        /// <summary>
        /// 默认以主键作为更新条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public static FilterDefinition<T> GetDefaultFilter<T>(T tParameter)
        {
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>,true);

            //更新条件
            var builder = Builders<T>.Filter;
            FilterDefinition<T> sbUpdateConditions = null;
            //获取类各个属性(对应数据库字段)
            foreach (EntityColumnInfo info in entityInfo.Columns)
            {
                //当某个字段未赋值时，不插入
                //当数据类型为日期类型时，未赋值，GetValue得到的结果是{0001/1/1 0:00:00} == default(DateTime)或DateTime.MinValue
                //获取该列的值
                var value = info.PropertyInfo.GetValue(tParameter);
                if (value == null || value.ToString() == default(DateTime).ToString())
                {
                    continue;
                }

                if (info.IsPrimarykey)
                {
                    if (sbUpdateConditions != null)
                    {
                        sbUpdateConditions = builder.Eq(info.DbColumnName, value) & sbUpdateConditions;
                    }
                    else
                    {
                        sbUpdateConditions = builder.Eq(info.DbColumnName, value);
                    }
                }
            }
            return sbUpdateConditions;
        }

        /// <summary>
        /// 更新单个model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public static List<UpdateDefinition<T>> GetUpdateFields<T>(T tParameter)
        {
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);

            //要修改的字段
            var sbUpdateColumns = new List<UpdateDefinition<T>>();
            //获取类各个属性(对应数据库字段)
            foreach (EntityColumnInfo info in entityInfo.Columns)
            {
                //当某个字段未赋值时，不插入
                //当数据类型为日期类型时，未赋值，GetValue得到的结果是{0001/1/1 0:00:00} == default(DateTime)或DateTime.MinValue
                //获取该列的值
                var value = info.PropertyInfo.GetValue(tParameter);
                if (value == null || value.ToString() == default(DateTime).ToString())
                {
                    continue;
                }
                sbUpdateColumns.Add(Builders<T>.Update.Set(info.DbColumnName, value));   
            }

            //执行sql语句
            return sbUpdateColumns;
        }

        public static List<UpdateDefinition<T>> GetUpdateFields<T>(Expression<Func<T, T>> fieldEx)
        {
            string updateField = string.Empty;
            if (fieldEx != null)
            {
                Expression exp = fieldEx.Body as Expression;

                Type model = typeof(T);
                EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
                var result = Expression.Lambda(exp).Compile().DynamicInvoke();
                //要修改的字段
                var sbUpdateColumns = new List<UpdateDefinition<T>>();
                //获取类各个属性(对应数据库字段)
                foreach (EntityColumnInfo info in entityInfo.Columns)
                {
                    //当某个字段未赋值时，不插入
                    //当数据类型为日期类型时，未赋值，GetValue得到的结果是{0001/1/1 0:00:00} == default(DateTime)或DateTime.MinValue
                    //获取该列的值
                    var value = info.PropertyInfo.GetValue(result);
                    if (value == null || value.ToString() == default(DateTime).ToString())
                    {
                        continue;
                    }
                    sbUpdateColumns.Add(Builders<T>.Update.Set(info.DbColumnName, value));
                }
                return sbUpdateColumns;
            }
            else
            {
                return null;
            }
        }

    }
}
