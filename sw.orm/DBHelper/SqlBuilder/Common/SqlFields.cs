using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    public class SqlFields
    {
        /// <summary>
        /// 查询字段Fields解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string Analysis<T>(out string tableName)
        {
            string strSql = string.Empty;
            //获取类
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);

            //查询的列
            StringBuilder sbSelectColumns = new StringBuilder();
            //获取类各个属性(对应数据库字段)
            foreach (EntityColumnInfo info in entityInfo.Columns)
            {
                //获取列名
                sbSelectColumns.AppendFormat("{0},", info.DbColumnName);
            }
            //数据表名
            tableName = entityInfo.DbTableName;
            //字段信息
            string fields = sbSelectColumns.ToString();
            if (!string.IsNullOrEmpty(fields))
            {
                strSql = fields.TrimEnd(',');
            }
            return strSql;
        }
    }
}
