using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class AddSqlBuilder
    {
        /// <summary>
        /// 生成sql语句(插入单条数据记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public static string Insert<T>(T tParameter, ref List<SWDbParameter> parameterList)
        {
            if (tParameter == null)
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
            StringBuilder sbColumnVals = new StringBuilder();

            foreach (EntityColumnInfo columnInfo in entityInfo.Columns)
            {
                var value = columnInfo.PropertyInfo.GetValue(tParameter);
                if (value == null || value.ToString() == default(DateTime).ToString())
                {
                    continue;
                }

                sbColumns.AppendFormat("{0},", columnInfo.DbColumnName);
                sbColumnVals.AppendFormat("@{0},", columnInfo.DbColumnName);

                parameterList.Add(ParameterGenerate.Generate(columnInfo.DbColumnName, value, columnInfo.ColumnType));
            }

            //列为空
            if (string.IsNullOrEmpty(sbColumns.ToString()))
            {
                return null;
            }

            //所有属性/数据列未赋值
            if (string.IsNullOrEmpty(sbColumnVals.ToString()))
            {
                return null;
            }

            sbSql.AppendFormat("{0})VALUES({1})", sbColumns.ToString().TrimEnd(','), sbColumnVals.ToString().TrimEnd(','));

            //返回sql语句
            return sbSql.ToString();
        }
    }
}
