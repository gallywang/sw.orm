using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    public class MySqlAddSqlBuilder
    {
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public static string Insert<T>(List<T> parameterList)
        {
            if (parameterList == null && parameterList.Count <= 0)
            {
                return null;
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();

            //注：typeof 运算符比对象实例上的 GetType 方法要快
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);

            //数据表名(类名)
            sbSql.AppendFormat("INSERT INTO {0} (", entityInfo.DbTableName);

            //数据表各列名
            StringBuilder sbColumns = new StringBuilder();

            foreach (EntityColumnInfo columnInfo in entityInfo.Columns)
            {
                sbColumns.AppendFormat("{0},", columnInfo.DbColumnName);
            }

            //列为空
            if (string.IsNullOrEmpty(sbColumns.ToString()))
            {
                return null;
            }

            sbSql.AppendFormat("{0})VALUES", sbColumns.ToString().TrimEnd(','));

            for (int i = 0; i < parameterList.Count; i++)
            {
                StringBuilder sbColumnVals = new StringBuilder();
                foreach (EntityColumnInfo columnInfo in entityInfo.Columns)
                {
                    var value = columnInfo.PropertyInfo.GetValue(parameterList[i]);
                    if (value == null || value.ToString() == default(DateTime).ToString())
                    {
                        sbColumnVals.Append(GetStrSqlFormat(DBNull.Value, columnInfo.ColumnType));
                    }
                    else
                    {
                        sbColumnVals.Append(GetStrSqlFormat(value, columnInfo.ColumnType));
                    }
                }
                sbSql.AppendFormat("({0}),", sbColumnVals.ToString().TrimEnd(','));
            }

            //sbSql.AppendFormat("{0})VALUES({1})", sbColumns.ToString().TrimEnd(','), sbColumnVals.ToString().TrimEnd(','));

            //返回sql语句
            return sbSql.ToString().TrimEnd(',');
        }

        /// <summary>
        /// 转sql 拼接字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetStrSqlFormat(object value, Type type)
        {
            string result = null;
            if (typeof(int) == type || typeof(Nullable<int>) == type)
            {
                result = string.Format("{0},", value);
            }
            else if (typeof(Boolean) == type || typeof(Nullable<Boolean>) == type)
            {
                result = string.Format("{0},", Convert.ToBoolean(value) ? 1 : 0);
            }
            else if (typeof(DateTime) == type || typeof(Nullable<DateTime>) == type)
            {
                result = string.Format("'{0}',", Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                result = string.Format("'{0}',", value);
            }
            return result;
        }
    }
}
