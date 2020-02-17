using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class DeleteSqlBuilder
    {
        /// <summary>
        /// 根据ID列删除数据(数据列中必须包含ID)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Delete<T, T2>(T2 id, ref List<SWDbParameter> parameters)
        {
            if (id == null)
            {
                return null;
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            //获取类
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
            //数据表名(类名)
            if (typeof(T2) == typeof(int))
            {
                sbSql.AppendFormat("DELETE FROM {0} WHERE ID = @ID", entityInfo.DbTableName);
                parameters.Add(new SWDbParameter("ID", id, DbType.Int32));
            }
            else
            {
                sbSql.AppendFormat("DELETE FROM {0} WHERE ID = @ID", entityInfo.DbTableName);
                parameters.Add(new SWDbParameter("ID", id, DbType.String));
            }
            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 根据ID列删除数据(数据列中必须包含ID)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string Delete<T, T2>(List<T2> listId)
        {
            if (listId == null || listId.Count == 0)
            {
                return null;
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            //获取类
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
            //数据表名(类名)
            string ids = string.Empty;
            if (typeof(T2) == typeof(int))
            {
                for (int i = 0; i < listId.Count; i++)
                {
                    ids += string.Format("{0},", listId[i]);
                }
            }
            else
            {
                for (int i = 0; i < listId.Count; i++)
                {
                    ids += string.Format("'{0}',", listId[i]);
                }
            }
            sbSql.AppendFormat("DELETE FROM {0} WHERE ID in ({1})", entityInfo.DbTableName, ids.TrimEnd(','));
            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 删除：根据主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public static string Delete<T>(T tParameter)
        {
            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();

            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
            //数据表名(类名)
            sbSql.AppendFormat("DELETE FROM {0} ", entityInfo.DbTableName);

            //更新条件
            StringBuilder sbDeleteConditions = new StringBuilder();
            sbDeleteConditions.Append("WHERE");
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

                if (typeof(int) == info.ColumnType || typeof(Nullable<int>) == info.ColumnType)
                {
                    if (info.IsPrimarykey)
                    {
                        sbDeleteConditions.AppendFormat(" {0} = {1} and ", info.DbColumnName, value);
                    }
                }
                else if (typeof(Boolean) == info.ColumnType || typeof(Nullable<Boolean>) == info.ColumnType)
                {
                    if (info.IsPrimarykey)
                    {
                        sbDeleteConditions.AppendFormat(" {0} = {1} and ", info.DbColumnName, Convert.ToBoolean(value) ? 1 : 0);
                    }
                }
                else if (typeof(DateTime) == info.ColumnType || typeof(Nullable<DateTime>) == info.ColumnType)
                {
                    if (info.IsPrimarykey)
                    {
                        sbDeleteConditions.AppendFormat(" {0} = '{1}' and ", info.DbColumnName, Convert.ToDateTime(value).ToString(Const.DATE_FORMAT));
                    }
                }
                else
                {
                    if (info.IsPrimarykey)
                    {
                        sbDeleteConditions.AppendFormat(" {0} = '{1}' and ", info.DbColumnName, value);
                    }
                }
            }

            //更新条件为空
            if ("WHERE".Equals(sbDeleteConditions.ToString()))
            {
                return null;
            }

            //补足条件，否则最后会多了一个and
            sbDeleteConditions.Append(" 1=1 ");

            sbSql.Append(sbDeleteConditions.ToString());

            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 生成sql语句(根据条件删除)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiterEx"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string Delete<T>(Expression<Func<T, bool>> fiterEx, ref List<SWDbParameter> parameters)
        {
            string strWhere = SqlWhere.Analysis<T, bool>(ref parameters, fiterEx);
            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("DELETE FROM {0} {1}", SqlTable.GetTableName<T>(), strWhere);
            //执行sql语句
            return sbSql.ToString();
        }
    }
}
