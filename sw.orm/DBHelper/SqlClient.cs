using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace sw.orm
{
    internal class SqlClient : DBClient
    {
        #region 初始化

        /// <summary>
        /// 数据库处理类
        /// </summary>
        private IDBHelper sqlHelper;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _conn;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strConn"></param>
        /// <param name="dBType"></param>
        public SqlClient(string strConn, DBType dBType)
        {
            _conn = strConn;
            if (dBType == DBType.SQLServer)
            {
                sqlHelper = new SqlHelper(strConn);
            }
            else if (dBType == DBType.MySql)
            {
                sqlHelper = new MySqlHelper(strConn);
            }
            else if (dBType == DBType.SQLite)
            {
                sqlHelper = new SQLiteHelper(strConn);
            }
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public string CurConnection
        {
            get { return _conn; }
        }

        #endregion

        #region 插入

        /// <summary>
        /// 插入单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Insert<T>(T tParameter)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = AddSqlBuilder.Insert<T>(tParameter, ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            if (parameters.Count == 0)
            {
                return sqlHelper.ExecuteQueryNone(sql);
            }
            else
            {
                return sqlHelper.ExecuteQuery(sql, parameters);
            }
        }

        /// <summary>
        /// 插入多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Insert<T>(List<T> tParameter)
        {
            if(tParameter == null)
            {
                return SWErrorCode.ParamsEmpty;
            }
            List<string> sqlList = new List<string>();
            List<List<SWDbParameter>> parametersList = new List<List<SWDbParameter>>();
            for (int i = 0,count = tParameter.Count;i < count; i++)
            {
                List<SWDbParameter> parameters = new List<SWDbParameter>();
                string sql = AddSqlBuilder.Insert<T>(tParameter[i], ref parameters);
                if (string.IsNullOrEmpty(sql))
                {
                    return SWErrorCode.SqlError;
                }
                sqlList.Add(sql);
                parametersList.Add(parameters);
            }
            if (sqlHelper.ExecTrans(sqlList, parametersList))
            {
                return sqlList.Count;
            }
            else
            {
                return SWErrorCode.ExecFailed;
            }
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新(以主键为条件更新)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Update<T>(T tParameter)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = UpdateSqlBuilder.Update<T>(tParameter, ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            if (parameters.Count == 0)
            {
                return sqlHelper.ExecuteQueryNone(sql);
            }
            else
            {
                return sqlHelper.ExecuteQuery(sql, parameters);
            }
        }

        /// <summary>
        /// 更新集合(以主键为条件更新)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Update<T>(List<T> tParameter)
        {
            if (tParameter != null && tParameter.Count > 0)
            {
                List<string> sqlList = new List<string>();
                List<List<SWDbParameter>> parametersList = new List<List<SWDbParameter>>();
                for (int i = 0; i < tParameter.Count; i++)
                {
                    List<SWDbParameter> parameters = new List<SWDbParameter>();
                    string sql = UpdateSqlBuilder.Update<T>(tParameter[i], ref parameters);
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sqlList.Add(sql);
                        parametersList.Add(parameters);
                    }
                }
                if (sqlList.Count == 0)
                {
                    return SWErrorCode.SqlError;
                }
                if(sqlHelper.ExecTrans(sqlList, parametersList))
                {
                    return sqlList.Count;
                }
                else
                {
                    return SWErrorCode.ExecFailed;
                }
            }
            return SWErrorCode.ParamsEmpty;
        }

        /// <summary>
        /// 根据条件更新指定列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldEx">更新列</param>
        /// <param name="fiterEx">更新条件</param>
        /// <returns></returns>
        public override int Update<T>(Expression<Func<T, T>> fieldEx, Expression<Func<T, bool>> fiterEx)
        {
            if (fieldEx != null)
            {
                List<SWDbParameter> parameters = new List<SWDbParameter>();
                string sql = UpdateSqlBuilder.Update<T>(fieldEx, fiterEx, ref parameters);
                if (string.IsNullOrEmpty(sql))
                {
                    return SWErrorCode.SqlError;
                }
                return sqlHelper.ExecuteQuery(sql, parameters);
            }
            return SWErrorCode.ParamsEmpty;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override int Delete<T>(string id)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = DeleteSqlBuilder.Delete<T,string>(id, ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQuery(sql, parameters);
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public override int Delete<T>(List<string> listId)
        {
            string sql = DeleteSqlBuilder.Delete<T, string>(listId);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQueryNone(sql);
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override int Delete<T>(int id)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = DeleteSqlBuilder.Delete<T, int>(id, ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQuery(sql, parameters);
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public override int Delete<T>(List<int> listId)
        {
            string sql = DeleteSqlBuilder.Delete<T, int>(listId);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQueryNone(sql);
        }

        /// <summary>
        /// 删除记录:不带参数时，以主键作为删除条件，未设置主键时不可删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        public override int Delete<T>(T tParameter)
        {
            string sql = DeleteSqlBuilder.Delete<T>(tParameter);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQueryNone(sql);
        }

        /// <summary>
        /// 删除记录:不带参数时，以主键作为删除条件，未设置主键时不可删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        public override int Delete<T>(List<T> tParameter)
        {
            if (tParameter != null && tParameter.Count > 0)
            {
                string sql = string.Empty;
                for (int i = 0; i < tParameter.Count; i++)
                {
                    sql += string.Format("{0};", DeleteSqlBuilder.Delete<T>(tParameter[i]));
                }
                if (string.IsNullOrEmpty(sql))
                {
                    return SWErrorCode.SqlError;
                }
                return sqlHelper.ExecuteQueryNone(sql);
            }
            return SWErrorCode.ParamsEmpty;
        }

        /// <summary>
        /// 根据条件删除，条件为空时删除所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiterEx"></param>
        /// <returns></returns>
        public override int Delete<T>(Expression<Func<T, bool>> fiterEx)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = DeleteSqlBuilder.Delete<T>(fiterEx, ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQuery(sql, parameters);
        }

        /// <summary>
        /// 根据Where条件删除，条件为空时删除所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public override int Delete<T>(Where<T> where)
        {
            if (where == null || where.GetExpression() == null)
            {
                return SWErrorCode.SqlError;
            }
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = DeleteSqlBuilder.Delete<T>(where.GetExpression(), ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.SqlError;
            }
            return sqlHelper.ExecuteQuery(sql, parameters);
        }

        #endregion

        #region 查询

        #region 查询单条记录

        /// <summary>
        /// 根据条件获取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override T GetModel<T>(Expression<Func<T, bool>> expression)
        {
            //查询参数
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                Top = 1
            };
            List<T> ts = GetModelList(searchParameter);
            if (ts != null && ts.Count > 0)
            {
                return ts[0];
            }
            return default(T);
        }

        /// <summary>
        /// 根据ID获取单条数据(实体对应数据表中必须存在ID字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override T GetModel<T>(string id)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = SearchSqlBuilder.GetModel<T>(id, ref parameters);
            if (!string.IsNullOrEmpty(sql))
            {
                List<T> dataList = DataConvertList<T>(sql, parameters);
                if (dataList != null && dataList.Count > 0)
                {
                    return dataList[0];
                }
            }
            return default(T);
        }

        /// <summary>
        /// 根据ID获取单条数据(实体对应数据表中必须存在ID字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override T GetModel<T>(int id)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = SearchSqlBuilder.GetModel<T>(id, ref parameters);
            if (!string.IsNullOrEmpty(sql))
            {
                List<T> dataList = DataConvertList<T>(sql, parameters);
                if (dataList != null && dataList.Count > 0)
                {
                    return dataList[0];
                }
            }
            return default(T);
        }

        #endregion

        #region 查询所有记录

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<T> GetAll<T>()
        {
            return GetModelList<T, bool>(null);
        }

        /// <summary>
        /// 获取排序后所有记录，单个排序(排序字段为字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <returns></returns>
        public override List<T> GetAll<T>(string orderFieldName, AscOrDesc ascOrDesc = AscOrDesc.Asc)
        {
            //查询参数
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取排序后所有记录，多个排序(排序字段为字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public override List<T> GetAll<T>(List<SWOrder> orderList)
        {
            //查询参数
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                OrderStrList = orderList
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取排序后所有记录，单个排序(排序字段为lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <returns></returns>
        public override List<T> GetAll<T>(Expression<Func<T, object>> orderBy, AscOrDesc ascOrDesc = AscOrDesc.Asc)
        {
            //查询参数
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取排序后所有记录，多个排序(排序字段为lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderList"></param>
        /// <returns></returns>
        public override List<T> GetAll<T>(List<SWOrder<T>> orderList)
        {
            //查询参数
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                OrderByExpList = orderList
            };
            return GetModelList<T, bool>(searchParameter);
        }

        #endregion

        #region 判断数据是否存在

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override bool Exists<T>(Expression<Func<T, bool>> expression)
        {
            return Exists<T, bool>(expression);
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override bool Exists<T>(Expression<Func<T, T>> expression)
        {
            return Exists<T, T>(expression);
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private bool Exists<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            bool result = false;
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = "";
            if (SWClient.CurDBType == DBType.SQLServer)
            {
                sql = SearchSqlBuilder.Exists<T1, T2>(ref parameters, expression);
            }
            else
            {
                //TODO 待验证sqlite/oracle
                sql = MySqlSearchSqlBuilder.Exists<T1, T2>(ref parameters, expression);
            }

            if (string.IsNullOrEmpty(sql))
            {
                return result;
            }
            DataTable dataTable = sqlHelper.ExecuteDataTable(sql, parameters);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                if (Const.STR_ONE.Equals(dataTable.Rows[0][0].ToString()))
                {
                    result = true;
                }
            }
            return result;
        }

        #endregion

        #region 查询记录总数

        /// <summary>
        /// 获取所有记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override int GetCount<T>()
        {
            return GetCount<T, T>(null);
        }

        /// <summary>
        /// 根据条件获取所有记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int GetCount<T>(Expression<Func<T, bool>> expression)
        {
            return GetCount<T, bool>(expression);
        }

        /// <summary>
        /// 根据条件获取所有记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int GetCount<T>(Expression<Func<T, T>> expression)
        {
            return GetCount<T, T>(expression);
        }

        /// <summary>
        /// 根据Where条件获取所有记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public override int GetCount<T>(Where<T> where)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetCount<T>(expression);
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private int GetCount<T1, T2>(Expression<Func<T1, T2>> expression)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = SearchSqlBuilder.GetCount<T1, T2>(expression, ref parameters);
            if (string.IsNullOrEmpty(sql))
            {
                return SWErrorCode.ParamsEmpty;
            }
            //执行sql语句
            DataTable dataTable = sqlHelper.ExecuteDataTable(sql, parameters);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dataTable.Rows[0][0].ToString()))
                {
                    return Convert.ToInt32(dataTable.Rows[0][0].ToString());
                }
            }
            return SWErrorCode.ExecFailed;
        }

        #endregion

        #region 获取数据记录

        /// <summary>
        /// 获取数据记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<T> GetModelList<T>()
        {
            return GetModelList<T, bool>(null);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderByList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderByExpList = orderByList,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderStrList = orderFieldNameList,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderByList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderByExpList = orderByList,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderStrList = orderFieldNameList,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(SWPaging)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, SWPaging paging)
        {
            int? pageSize = null;
            int? pageIndex = null;
            List<SWOrder> orderList = new List<SWOrder>();
            if (paging != null)
            {
                if (!string.IsNullOrEmpty(paging.Sort))
                {
                    //排序名称按分号分割
                    string[] arrOrderName = paging.Sort.Split(Const.COMMA);
                    string[] arrOrderSort = null;
                    if (!string.IsNullOrEmpty(paging.Order))
                    {
                        arrOrderSort = paging.Order.Split(Const.COMMA);
                    }

                    for(int i = 0, count = arrOrderName.Length; i < count; i++)
                    {
                        SWOrder swOrder = new SWOrder();
                        swOrder.OrderName = arrOrderName[i];
                        if(arrOrderSort != null && arrOrderSort.Length > i && !string.IsNullOrEmpty(arrOrderSort[i]))
                        {
                            swOrder.AscOrDesc = Const.ASC.ToLower().Equals(arrOrderSort[i].ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                        }
                        orderList.Add(swOrder);
                    }
                }
                else
                {
                    //TODO Log
                    orderList.Add(new SWOrder() { OrderName = Const.DEFAULT_FIELD });
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelList<T>(expression, orderList, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据筛选条件Where获取数据记录集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Where<T> where)
        {
            if (where == null || where.GetExpression() == null)
            {
                return GetModelList<T>();
            }
            return GetModelList<T>(where.GetExpression());
        }

        /// <summary>
        /// 根据筛选条件Where获取数据记录集合：含分页排序(单个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据筛选条件Where获取数据记录集合：含分页排序(多个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderByList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Where<T> where, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderByList, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据筛选条件Where获取数据记录集合：含分页排序(单个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据筛选条件Where获取数据记录集合：含分页排序(多个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Where<T> where, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderFieldNameList, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据筛选条件Where获取数据记录集合：含分页排序(SWPaging)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Where<T> where, SWPaging paging)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }

            List<SWOrder> orderList = new List<SWOrder>();
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                if (!string.IsNullOrEmpty(paging.Sort))
                {
                    //排序名称按分号分割
                    string[] arrOrderName = paging.Sort.Split(Const.COMMA);
                    string[] arrOrderSort = null;
                    if (!string.IsNullOrEmpty(paging.Order))
                    {
                        arrOrderSort = paging.Order.Split(Const.COMMA);
                    }

                    for (int i = 0, count = arrOrderName.Length; i < count; i++)
                    {
                        SWOrder swOrder = new SWOrder();
                        swOrder.OrderName = arrOrderName[i];
                        if (arrOrderSort != null && arrOrderSort.Length > i && !string.IsNullOrEmpty(arrOrderSort[i]))
                        {
                            swOrder.AscOrDesc = Const.ASC.ToLower().Equals(arrOrderSort[i].ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                        }
                        orderList.Add(swOrder);
                    }
                }
                else
                {
                    //TODO Log
                    orderList.Add(new SWOrder() { OrderName = Const.DEFAULT_FIELD });
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelList<T>(expression, orderList, pageSize, pageIndex);
        }

        /// <summary>
        /// 获取数据列表集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        private List<T1> GetModelList<T1, T2>(SearchParameter<T1, T2> searchParameter) where T1 : new()
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = string.Empty;
            if (SWClient.CurDBType == DBType.SQLServer)
            {
                sql = SearchSqlBuilder.GetModelList<T1, T2>(ref parameters, searchParameter);
            }
            else if (SWClient.CurDBType == DBType.MySql)
            {
                sql = MySqlSearchSqlBuilder.GetModelList<T1, T2>(ref parameters, searchParameter);
            }
            else if (SWClient.CurDBType == DBType.SQLite)
            {
                sql = SQLiteSearchSqlBuilder.GetModelList<T1, T2>(ref parameters, searchParameter);
            }
            else
            {
                throw new Exception(message: "not support modellist by page.");
            }

            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            return DataConvertList<T1>(sql, parameters);
        }

        /// <summary>
        /// 执行sql并返回集合列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<T> DataConvertList<T>(string sql, List<SWDbParameter> parameters) where T : new()
        {
            //执行sql语句
            DataTable dataTable = sqlHelper.ExecuteDataTable(sql, parameters);
            if (SWClient.CurDBType == DBType.SQLServer)
            {
                return EntityGenerator.ConvertTableToEntity<T>(dataTable);
            }
            else if (SWClient.CurDBType == DBType.MySql)
            {
                return EntityGenerator.ConvertTableToEntity2<T>(dataTable);
            }
            else if (SWClient.CurDBType == DBType.SQLite)
            {
                return EntityGenerator.ConvertTableToEntity2<T>(dataTable);
            }
            else
            {
                throw new Exception(message: "not support the type of db.");
            }
        }

        #endregion

        #region  获取数据列表并返回数据总条数(分页查询)

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：lambda表达式)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, bool>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：lambda表达式)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderByList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderByExpList = orderByList,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, bool>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：字符串)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, bool>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：字符串)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderStrList = orderFieldNameList,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, bool>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：lambda表达式)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, T>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：lambda表达式)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderByList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderByExpList = orderByList,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, T>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(单个排序字段：字符串)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, T>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(多个排序字段：字符串)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderStrList = orderFieldNameList,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
            return GetModelListWithCount<T, T>(searchParameter, out count);
        }

        /// <summary>
        /// 根据筛选条件获取数据记录集合：含分页排序(SWPaging)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, SWPaging paging)
        {
            List<SWOrder> orderList = new List<SWOrder>();
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                if (!string.IsNullOrEmpty(paging.Sort))
                {
                    //排序名称按分号分割
                    string[] arrOrderName = paging.Sort.Split(Const.COMMA);
                    string[] arrOrderSort = null;
                    if (!string.IsNullOrEmpty(paging.Order))
                    {
                        arrOrderSort = paging.Order.Split(Const.COMMA);
                    }

                    for (int i = 0, icount = arrOrderName.Length; i < icount; i++)
                    {
                        SWOrder swOrder = new SWOrder();
                        swOrder.OrderName = arrOrderName[i];
                        if (arrOrderSort != null && arrOrderSort.Length > i && !string.IsNullOrEmpty(arrOrderSort[i]))
                        {
                            swOrder.AscOrDesc = Const.ASC.ToLower().Equals(arrOrderSort[i].ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                        }
                        orderList.Add(swOrder);
                    }
                }
                else
                {
                    //TODO Log
                    orderList.Add(new SWOrder() { OrderName = Const.DEFAULT_FIELD });
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelListWithCount<T>(expression, out count, orderList, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据Where筛选条件获取数据记录集合：含分页排序(单个排序字段：lambda表达式)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据Where筛选条件获取数据记录集合：含分页排序(多个排序字段：lambda表达式)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <param name="orderByList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderByList, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据Where筛选条件获取数据记录集合：含分页排序(单个排序字段：字符串)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据Where筛选条件获取数据记录集合：含分页排序(多个排序字段：字符串)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldNameList, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据Where筛选条件获取数据记录集合：含分页排序(SWPaging)，并返回记录总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <param name="paging"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, SWPaging paging)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }

            List<SWOrder> orderList = new List<SWOrder>();
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                if (!string.IsNullOrEmpty(paging.Sort))
                {
                    //排序名称按分号分割
                    string[] arrOrderName = paging.Sort.Split(Const.COMMA);
                    string[] arrOrderSort = null;
                    if (!string.IsNullOrEmpty(paging.Order))
                    {
                        arrOrderSort = paging.Order.Split(Const.COMMA);
                    }

                    for (int i = 0, icount = arrOrderName.Length; i < icount; i++)
                    {
                        SWOrder swOrder = new SWOrder();
                        swOrder.OrderName = arrOrderName[i];
                        if (arrOrderSort != null && arrOrderSort.Length > i && !string.IsNullOrEmpty(arrOrderSort[i]))
                        {
                            swOrder.AscOrDesc = Const.ASC.ToLower().Equals(arrOrderSort[i].ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                        }
                        orderList.Add(swOrder);
                    }
                }
                else
                {
                    //TODO Log
                    orderList.Add(new SWOrder() { OrderName = Const.DEFAULT_FIELD });
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelListWithCount<T>(expression, out count, orderList, pageSize, pageIndex);
        }

        /// <summary>
        /// 获取数据集合并返回集合总数
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="searchParameter"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<T1> GetModelListWithCount<T1, T2>(SearchParameter<T1, T2> searchParameter, out int count) where T1 : new()
        {
            count = -1;
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = null;
            if (SWClient.CurDBType == DBType.SQLServer)
            {
                sql = SearchSqlBuilder.GetModelListWithCount<T1, T2>(ref parameters, searchParameter);
            }
            else
            {
                sql = MySqlSearchSqlBuilder.GetModelListWithCount<T1, T2>(ref parameters, searchParameter);
            }

            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }
            //执行sql语句
            return DataConvertListWithCount<T1>(sql, parameters, out count);
        }

        /// <summary>
        /// 执行sql并返回集合列表及集合总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<T> DataConvertListWithCount<T>(string sql, List<SWDbParameter> parameters, out int count) where T : new()
        {
            count = -1;
            //执行sql语句
            DataSet dataSet = sqlHelper.ExecuteDataSet(sql, parameters);
            if (dataSet != null && dataSet.Tables.Count > 1)
            {
                DataTable countTable = dataSet.Tables[1];
                if (countTable != null && countTable.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(countTable.Rows[0][0].ToString()))
                    {
                        count = Convert.ToInt32(countTable.Rows[0][0].ToString());
                    }
                }
                DataTable dataTable = dataSet.Tables[0];
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    if (SWClient.CurDBType == DBType.SQLServer)
                    {
                        return EntityGenerator.ConvertTableToEntity<T>(dataTable);
                    }
                    else
                    {
                        return EntityGenerator.ConvertTableToEntity2<T>(dataTable);
                    }
                }
            }
            return null;
        }

        #endregion

        #region 获取数据列表前n条数据

        /// <summary>
        /// 获取数据列表前n条数据，含排序(单个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                Top = top
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(多个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderByList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder<T>> orderByList = null, int? top = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderByExpList = orderByList,
                Top = top
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(单个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                Top = top
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(多个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder> orderFieldNameList = null, int? top = null)
        {
            SearchParameter<T, bool> searchParameter = new SearchParameter<T, bool>
            {
                FilterExp = expression,
                OrderStrList = orderFieldNameList,
                Top = top
            };
            return GetModelList<T, bool>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(单个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                Top = top
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(多个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderByList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, List<SWOrder<T>> orderByList = null, int? top = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderByExpList = orderByList,
                Top = top
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(单个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                Top = top
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 获取数据列表前n条数据，含排序(多个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, List<SWOrder> orderFieldNameList = null, int? top = null)
        {
            SearchParameter<T, T> searchParameter = new SearchParameter<T, T>
            {
                FilterExp = expression,
                OrderStrList = orderFieldNameList,
                Top = top
            };
            return GetModelList<T, T>(searchParameter);
        }

        /// <summary>
        /// 根据Where条件获取数据列表前n条数据，含排序(单个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderBy, ascOrDesc, top);
        }

        /// <summary>
        /// 根据Where条件获取数据列表前n条数据，含排序(多个排序字段：lambda表达式)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderByList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Where<T> where, List<SWOrder<T>> orderByList = null, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderByList, top);
        }

        /// <summary>
        /// 根据Where条件获取数据列表前n条数据，含排序(单个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderFieldName"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderFieldName, ascOrDesc, top);
        }

        /// <summary>
        /// 根据Where条件获取数据列表前n条数据，含排序(多个排序字段：字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderFieldNameList"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Where<T> where, List<SWOrder> orderFieldNameList = null, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderFieldNameList, top);
        }

        #endregion

        #endregion

        #region 直接执行存储过程/SQL

        /// <summary>
        /// 执行存储过程返回实体类集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strProcName"></param>
        /// <param name="sqlParameters"></param>
        /// <returns></returns>
        public override List<T> GetProcDataList<T>(string strProcName, List<SWDbParameter> sqlParameter)
        {
            DataTable dt = sqlHelper.ExecProcTable(strProcName, sqlParameter);
            if (dt != null && dt.Rows.Count > 0)
            {
                return EntityGenerator.ConvertTableToEntity<T>(dt);
            }
            return null;
        }

        /// <summary>
        /// 执行sql语句返回dataset数据集合
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public override DataSet ExecSqlWithDataSet(string sql, List<SWDbParameter> sqlParameter)
        {
            return sqlHelper.ExecuteDataSet(sql, sqlParameter);
        }

        #endregion
    }
}
