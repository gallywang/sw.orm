using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class UpdateSqlBuilder
    {
        /// <summary>
        /// 生成sql语句(更新单条数据记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public static string Update<T>(T tParameter, ref List<SWDbParameter> parameterList)
        {
            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();

            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
            //数据表名(类名)
            sbSql.AppendFormat("UPDATE {0} SET ", entityInfo.DbTableName);

            //数据表各列名
            StringBuilder sbUpdateColumns = new StringBuilder();
            //更新条件
            StringBuilder sbUpdateConditions = new StringBuilder();
            sbUpdateConditions.Append("WHERE");
            //获取类各个属性(对应数据库字段)
            foreach (EntityColumnInfo info in entityInfo.Columns)
            {
                //为自增列时跳过
                if (info.IsIdentity)
                {
                    continue;
                }
                //当某个字段未赋值时，不插入
                //当数据类型为日期类型时，未赋值，GetValue得到的结果是{0001/1/1 0:00:00} == default(DateTime)或DateTime.MinValue
                //获取该列的值
                var value = info.PropertyInfo.GetValue(tParameter);
                if (value == null || value.ToString() == default(DateTime).ToString())
                {
                    //continue;
                    sbUpdateColumns.AppendFormat(" {0} = NULL,", info.DbColumnName);
                }
                else
                {
                    //获取更新列及更新条件(默认以主键为更新条件)
                    if (typeof(int) == info.ColumnType || typeof(Nullable<int>) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.Int32));
                    }
                    else if (typeof(Boolean) == info.ColumnType || typeof(Nullable<Boolean>) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.Boolean));
                    }
                    else if (typeof(DateTime) == info.ColumnType || typeof(Nullable<DateTime>) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.DateTime));
                    }
                    else if (typeof(byte[]) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.SByte));
                    }
                    else if (typeof(DateTimeOffset) == info.ColumnType || typeof(Nullable<DateTimeOffset>) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.DateTimeOffset));
                    }
                    else if (typeof(Guid) == info.ColumnType || typeof(Nullable<Guid>) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.Guid));
                    }
                    else if (typeof(TimeSpan) == info.ColumnType || typeof(Nullable<TimeSpan>) == info.ColumnType)
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.Time));
                    }
                    else
                    {
                        if (info.IsPrimarykey)
                        {
                            sbUpdateConditions.AppendFormat(" {0} = @{0} and ", info.DbColumnName);
                        }
                        else
                        {
                            sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        }
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.String));
                    }
                }
            }

            //更新条件为空
            if ("WHERE".Equals(sbUpdateConditions.ToString()))
            {
                return null;
            }

            //补足条件，否则最后会多了一个and
            sbUpdateConditions.Append(" 1=1 ");

            //更新的字段为空
            if (string.IsNullOrEmpty(sbUpdateColumns.ToString()))
            {
                return null;
            }

            sbSql.AppendFormat("{0} {1}", sbUpdateColumns.ToString().TrimEnd(','), sbUpdateConditions.ToString());

            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 生成sql语句(更新单条数据记录:按条件更新指定列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldEx"></param>
        /// <param name="fiterEx"></param>
        /// <returns></returns>
        public static string Update<T>(Expression<Func<T, T>> fieldEx, Expression<Func<T, bool>> fiterEx, ref List<SWDbParameter> parameterList)
        {
            string updateField = string.Empty;
            if (fieldEx != null)
            {
                Expression exp = fieldEx.Body as Expression;
                updateField = string.Format(" SET {0}", GetUpdateFiters<T>(exp, ref parameterList));
            }
            else
            {
                return null;
            }

            string strWhere = string.Empty;
            if (fiterEx != null)
            {
                Expression exp = fiterEx.Body as Expression;
                ExpressionContext context = new ExpressionContext();
                strWhere = string.Format(" WHERE {0}", context.Analyze(exp));

                parameterList.AddRange(context.GetParameters());
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            //获取类
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
            //数据表名(类名)
            sbSql.AppendFormat("UPDATE {0} {1} {2}", entityInfo.DbTableName, updateField, strWhere);

            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 得到更新的列的sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldEx"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        private static string GetUpdateFiters<T>(Expression fieldEx, ref List<SWDbParameter> parameterList)
        {
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);

            var result = Expression.Lambda(fieldEx).Compile().DynamicInvoke();

            //数据表各列名
            StringBuilder sbUpdateColumns = new StringBuilder();
            //获取类各个属性(对应数据库字段)
            foreach (EntityColumnInfo info in entityInfo.Columns)
            {
                //当某个字段未赋值时，不插入
                //当数据类型为日期类型时，未赋值，GetValue得到的结果是{0001/1/1 0:00:00} == default(DateTime)或DateTime.MinValue
                //获取该列的值
                var value = info.PropertyInfo.GetValue(result);
                if (value == null || value.ToString() == default(DateTime).ToString())
                {
                    //sbUpdateColumns.AppendFormat(" {0} = NULL,", info.DbColumnName);
                    continue;
                }
                else
                {
                    //获取列名
                    if (typeof(int) == info.ColumnType || typeof(Nullable<int>) == info.ColumnType)
                    {
                        sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.Int32));
                    }
                    else if (typeof(Boolean) == info.ColumnType || typeof(Nullable<Boolean>) == info.ColumnType)
                    {
                        sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.Boolean));
                    }
                    else if (typeof(DateTime) == info.ColumnType || typeof(Nullable<DateTime>) == info.ColumnType)
                    {
                        sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.DateTime));
                    }
                    else
                    {
                        sbUpdateColumns.AppendFormat(" {0} = @{0},", info.DbColumnName);
                        parameterList.Add(new SWDbParameter(info.DbColumnName, value, DbType.String));
                    }
                }
            }
            return sbUpdateColumns.ToString().Trim(',');
        }
    }
}
