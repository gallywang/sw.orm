using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class MySqlSearchSqlBuilder
    {
        /// <summary>
        /// 判断数据是否存在
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string Exists<T1, T2>(ref List<SWDbParameter> parameters, Expression<Func<T1, T2>> expression)
        {
            //where条件解析
            string strWhere = SqlWhere.Analysis<T1, T2>(ref parameters, expression);
            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            //数据表名(类名)
            sbSql.AppendFormat("select 1 from {0} {1} limit 1", SqlTable.GetTableName<T1>(), strWhere);

            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string GetModelList<T1, T2>(ref List<SWDbParameter> parameters, SearchParameter<T1, T2> searchParameter) where T1 : new()
        {
            //筛选条件
            string strWhere = SqlWhere.Analysis<T1, T2>(ref parameters, searchParameter);

            //排序字段
            string strOrder = SqlOrder.Analysis<T1, T2>(ref parameters, searchParameter);

            string dbTableName = string.Empty;
            //获取查询字段，字段信息为空则不查询
            string sqlFields = SqlFields.Analysis<T1>(out dbTableName);
            if (string.IsNullOrEmpty(sqlFields))
            {
                return null;
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("SELECT {0} FROM {1} {2}", sqlFields, dbTableName, strWhere);
            if (!string.IsNullOrEmpty(strOrder))
            {
                sbSql.AppendFormat(" {0} ", strOrder);
            }

            //取前几条与分页不可共存
            if(searchParameter.Top != null)
            {
                sbSql.AppendFormat(" limit 0,{0} ", searchParameter.Top);
            }
            else
            {
                if (searchParameter.PageSize == null && searchParameter.PageIndex == null)
                {
                    //不分页
                }
                else
                {
                    searchParameter.PageSize = searchParameter.PageSize != null ? (Int32)searchParameter.PageSize : Const.DEFAULT_PAGESIZE;
                    searchParameter.PageIndex = searchParameter.PageIndex != null ? (Int32)searchParameter.PageIndex : Const.DEFAULT_PAGEINDEX;

                    sbSql.AppendFormat(" limit {0} offset {1}", searchParameter.PageSize, (searchParameter.PageIndex - 1) * searchParameter.PageSize);
                }
            }

            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 获取数据列表含数据总条数
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string GetModelListWithCount<T1, T2>(ref List<SWDbParameter> parameters, SearchParameter<T1, T2> searchParameter) where T1 : new()
        {
            //筛选条件
            string strWhere = SqlWhere.Analysis<T1, T2>(ref parameters, searchParameter);

            //排序字段
            string strOrder = SqlOrder.Analysis<T1, T2>(ref parameters, searchParameter);

            string dbTableName = string.Empty;
            //获取查询字段，字段信息为空则不查询
            string sqlFields = SqlFields.Analysis<T1>(out dbTableName);
            if (string.IsNullOrEmpty(sqlFields))
            {
                return null;
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("SELECT {0} FROM {1} {2}", sqlFields, dbTableName, strWhere);
            if (!string.IsNullOrEmpty(strOrder))
            {
                sbSql.AppendFormat(" {0} ", strOrder);
            }

            //取前几条与分页不可共存
            if (searchParameter.Top != null)
            {
                sbSql.AppendFormat(" limit 0,{0} ", searchParameter.Top);
            }
            else
            {
                if (searchParameter.PageSize == null && searchParameter.PageIndex == null)
                {
                    //不分页
                }
                else
                {
                    searchParameter.PageSize = searchParameter.PageSize != null ? (Int32)searchParameter.PageSize : Const.DEFAULT_PAGESIZE;
                    searchParameter.PageIndex = searchParameter.PageIndex != null ? (Int32)searchParameter.PageIndex : Const.DEFAULT_PAGEINDEX;

                    sbSql.AppendFormat(" limit {0} offset {1}", searchParameter.PageSize, (searchParameter.PageIndex - 1) * searchParameter.PageSize);
                }
            }

            //获取总条数
            sbSql.AppendFormat(";SELECT COUNT(1) FROM {0} {1}", dbTableName, strWhere);

            //执行sql语句
            return sbSql.ToString();
        }

    }
}
