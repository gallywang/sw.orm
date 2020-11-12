using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class SearchSqlBuilder
    {
        #region 单条记录查询

        /// <summary>
        /// 根据ID查找单个记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetModel<T>(string id, ref List<SWDbParameter> parameters)
        {
            //id为空，直接返回空
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            //筛选条件
            string strWhere = " WHERE ID = @ID";
            //拼接查询sql
            StringBuilder sbSql = new StringBuilder();
            string dbTableName = string.Empty;
            //获取查询字段，字段信息为空则不查询
            string sqlFields = SqlFields.Analysis<T>(out dbTableName);
            if (string.IsNullOrEmpty(sqlFields))
            {
                return null;
            }
            //必须拥有字段ID
            SWDbParameter parameter = new SWDbParameter("ID", id, DbType.String);
            parameters.Add(parameter);
            sbSql.AppendFormat("SELECT {0} FROM {1} {2}", sqlFields, dbTableName, strWhere);

            return sbSql.ToString();
        }

        /// <summary>
        /// 根据ID查找单个记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetModel<T>(int id, ref List<SWDbParameter> parameters)
        {
            //筛选条件
            string strWhere = " WHERE ID = @ID";
            //拼接查询sql
            StringBuilder sbSql = new StringBuilder();
            string dbTableName = string.Empty;
            //获取查询字段，字段信息为空则不查询
            string sqlFields = SqlFields.Analysis<T>(out dbTableName);
            if (string.IsNullOrEmpty(sqlFields))
            {
                return null;
            }
            //必须拥有字段ID
            SWDbParameter parameter = new SWDbParameter("ID", id, DbType.Int32);
            parameters.Add(parameter);
            sbSql.AppendFormat("SELECT {0} FROM {1} {2}", sqlFields, dbTableName, strWhere);

            return sbSql.ToString();
        }

        #endregion

        #region 总条数查询

        /// <summary>
        /// 获取数据总条数
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetCount<T1, T2>(Expression<Func<T1, T2>> expression, ref List<SWDbParameter> parameters)
        {
            //where条件解析
            string strWhere = SqlWhere.Analysis<T1, T2>(ref parameters, expression);
            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            //数据表名(类名)
            sbSql.AppendFormat("SELECT COUNT(1) FROM {0} {1}", SqlTable.GetTableName<T1>(), strWhere);

            //执行sql语句
            return sbSql.ToString();
        }

        #endregion

        #region 数据是否存在

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
            sbSql.AppendFormat("select isnull((select top(1) 1 from {0} {1}), 0)", SqlTable.GetTableName<T1>(), strWhere);

            //执行sql语句
            return sbSql.ToString();
        }

        #endregion

        #region 查询数据列表

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
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
            if(string.IsNullOrEmpty(sqlFields))
            {
                return null;
            }

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();
            if (searchParameter != null && searchParameter.Top != null)
            {
                sbSql.AppendFormat("SELECT TOP {0} {1} FROM {2} {3}", searchParameter.Top, sqlFields, dbTableName, strWhere);
            }
            else
            {
                sbSql.AppendFormat("SELECT {0} FROM {1} {2}", sqlFields, dbTableName, strWhere);
            }

            if (!string.IsNullOrEmpty(strOrder))
            {
                sbSql.AppendFormat(" {0} ", strOrder);
            }

            if (searchParameter != null && searchParameter.Top == null)
            {
                if (searchParameter.PageSize == null && searchParameter.PageIndex == null)
                {
                    //不分页
                }
                else
                {
                    searchParameter.PageSize = searchParameter.PageSize != null ? (Int32)searchParameter.PageSize : Const.DEFAULT_PAGESIZE;
                    searchParameter.PageIndex = searchParameter.PageIndex != null ? (Int32)searchParameter.PageIndex : Const.DEFAULT_PAGEINDEX;

                    sbSql.AppendFormat(" offset {0} rows fetch next {1} rows only", (searchParameter.PageIndex - 1) * searchParameter.PageSize, searchParameter.PageSize);
                }
            }

            //执行sql语句
            return sbSql.ToString();
        }

        /// <summary>
        /// 获取总数
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

            //拼接sql语句
            StringBuilder sbSql = new StringBuilder();

            //获取类
            string dbTableName = string.Empty;
            //获取查询字段，字段信息为空则不查询
            string sqlFields = SqlFields.Analysis<T1>(out dbTableName);
            if (string.IsNullOrEmpty(sqlFields))
            {
                //TODO
                return null;
            }

            if (searchParameter != null && searchParameter.Top != null)
            {
                sbSql.AppendFormat("SELECT TOP {0} {1} FROM {2} {3}", searchParameter.Top, sqlFields, dbTableName, strWhere);
            }
            else
            {
                sbSql.AppendFormat("SELECT {0} FROM {1} {2}", sqlFields, dbTableName, strWhere);
            }

            if (!string.IsNullOrEmpty(strOrder))
            {
                sbSql.AppendFormat(" {0} ", strOrder);
            }

            if (searchParameter != null && searchParameter.Top == null)
            {
                if (searchParameter.PageSize == null && searchParameter.PageIndex == null)
                {
                    //不分页
                }
                else
                {
                    searchParameter.PageSize = searchParameter.PageSize != null ? (Int32)searchParameter.PageSize : Const.DEFAULT_PAGESIZE;
                    searchParameter.PageIndex = searchParameter.PageIndex != null ? (Int32)searchParameter.PageIndex : Const.DEFAULT_PAGEINDEX;

                    sbSql.AppendFormat(" offset {0} rows fetch next {1} rows only", (searchParameter.PageIndex - 1) * searchParameter.PageSize, searchParameter.PageSize);
                }
            }

            //获取总条数
            sbSql.AppendFormat(";SELECT COUNT(1) ROWS FROM {0} {1}", dbTableName, strWhere);

            //执行sql语句
            return sbSql.ToString();
        }

        #endregion
    }
}
