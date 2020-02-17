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
        /// <summary>
        /// 数据库处理类
        /// </summary>
        private IDBHelper sqlHelper;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _coon;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strConn"></param>
        /// <param name="dBType"></param>
        public SqlClient(string strConn, DBType dBType)
        {
            _coon = strConn;
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
            get { return _coon; }
        }

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
        /// 更新(以主键为条件更新)
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

        #endregion

        #region 查询

        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override T GetModel<T>(Expression<Func<T, bool>> expression)
        {
            List<T> ts = GetModelList<T, bool>(expression, "", null, 1);
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
                if(dataList != null && dataList.Count > 0)
                {
                    return dataList[0];
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<T> GetAll<T>(Expression<Func<T, object>> orderBy, AscOrDesc ascOrDesc = AscOrDesc.Asc)
        {
            return GetModelList<T, bool>(null, orderBy, ascOrDesc, null);
        }

        public override List<T> GetAll<T>(string orderFieldName, AscOrDesc ascOrDesc = AscOrDesc.Asc)
        {
            return GetModelList<T, bool>(null, orderFieldName, ascOrDesc, null);
        }

        public override List<T> GetAll<T>()
        {
            return GetModelList<T, bool>(null, "", null, null, null);
        }

        public override int GetCount<T>()
        {
            return GetCount<T, T>(null);
        }

        /// <summary>
        /// 查询数据记录(条件为空获取所有)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int GetCount<T>(Expression<Func<T, bool>> expression)
        {
            return GetCount<T, bool>(expression);
        }

        /// <summary>
        /// 查询数据记录(条件为空获取所有)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int GetCount<T>(Expression<Func<T, T>> expression)
        {
            return GetCount<T, T>(expression);
        }

        /// <summary>
        /// 查询数据记录(条件为空获取所有)
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

        public override List<T> GetModelList<T>()
        {
            return GetModelList<T, bool>(null, "", null, null, null);
        }

        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression)
        {
            return GetModelList<T, bool>(expression, "", null, null, null);
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelList<T, bool>(expression, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelList<T, bool>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelList<T, T>(expression, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelList<T, T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelListWithCount<T, bool>(expression, out count, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelListWithCount<T, bool>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelListWithCount<T, T>(expression, out count, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            return GetModelListWithCount<T, T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            return GetModelList<T, bool>(expression, orderBy, ascOrDesc, top);
        }

        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            return GetModelList<T, bool>(expression, orderFieldName, ascOrDesc, top);
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            return GetModelList<T, T>(expression, orderBy, ascOrDesc, top);
        }

        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            return GetModelList<T, T>(expression, orderFieldName, ascOrDesc, top);
        }

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

        /// <summary>
        /// 含分页
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private List<T1> GetModelList<T1, T2>(Expression<Func<T1, T2>> expression, Expression<Func<T1, object>> orderBy = null, AscOrDesc? ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T1 : new()
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = string.Empty;
            //通用查询参数
            SearchParameter<T1, T2> searchParameter = new SearchParameter<T1, T2>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
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

        private List<T1> GetModelList<T1, T2>(Expression<Func<T1, T2>> expression, string orderFieldName = null, AscOrDesc? ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T1 : new()
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();

            string sql = string.Empty;
            //通用查询参数
            SearchParameter<T1, T2> searchParameter = new SearchParameter<T1, T2>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                PageSize = pageSize,
                PageIndex = pageIndex
            };
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
        /// 含top查询
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private List<T1> GetModelList<T1, T2>(Expression<Func<T1, T2>> expression, Expression<Func<T1, object>> orderBy = null, AscOrDesc? ascOrDesc = AscOrDesc.Asc, int? top = null) where T1 : new()
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();

            string sql = string.Empty;
            //通用查询参数
            SearchParameter<T1, T2> searchParameter = new SearchParameter<T1, T2>
            {
                FilterExp = expression,
                OrderByExp = orderBy,
                AscOrDesc = ascOrDesc,
                Top = top
            };
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
                throw new Exception(message: "not support modellist by top.");
            }

            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            return DataConvertList<T1>(sql, parameters);
        }

        private List<T1> GetModelList<T1, T2>(Expression<Func<T1, T2>> expression, string orderFieldName = null, AscOrDesc? ascOrDesc = AscOrDesc.Asc, int? top = null) where T1 : new()
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();

            string sql = string.Empty;
            //通用查询参数
            SearchParameter<T1, T2> searchParameter = new SearchParameter<T1, T2>
            {
                FilterExp = expression,
                OrderStr = orderFieldName,
                AscOrDesc = ascOrDesc,
                Top = top
            };
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
                throw new Exception(message: "not support modellist by top.");
            }

            if (string.IsNullOrEmpty(sql))
            {
                return null;
            }

            return DataConvertList<T1>(sql, parameters);
        }
        
        /// <summary>
        /// 分页查询数据并返回总数
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private List<T1> GetModelListWithCount<T1, T2>(Expression<Func<T1, T2>> expression, out int count, Expression<Func<T1, object>> orderBy = null, AscOrDesc? ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T1 : new()
        {
            count = -1;
            List<SWDbParameter> parameters = new List<SWDbParameter>();

            string sql = string.Empty;
            if (SWClient.CurDBType == DBType.SQLServer || SWClient.CurDBType == DBType.MySql)
            {
                SearchParameter<T1, T2> searchParameter = new SearchParameter<T1, T2>
                {
                    FilterExp = expression,
                    OrderByExp = orderBy,
                    AscOrDesc = ascOrDesc,
                    PageSize = pageSize,
                    PageIndex = pageIndex
                };
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
            else if (SWClient.CurDBType == DBType.SQLite)
            {
                count = GetCount<T1, T2>(expression);
                return GetModelList<T1, T2>(expression, orderBy, ascOrDesc, pageSize, pageIndex);
            }
            else
            {
                throw new Exception(message: "not support modellist by page with count.");
            }
        }

        private List<T1> GetModelListWithCount<T1, T2>(Expression<Func<T1, T2>> expression, out int count, string orderFieldName = null, AscOrDesc? ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T1 : new()
        {
            count = -1;
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = string.Empty;
            if (SWClient.CurDBType == DBType.SQLServer || SWClient.CurDBType == DBType.MySql)
            {
                SearchParameter<T1, T2> searchParameter = new SearchParameter<T1, T2>
                {
                    FilterExp = expression,
                    OrderStr = orderFieldName,
                    AscOrDesc = ascOrDesc,
                    PageSize = pageSize,
                    PageIndex = pageIndex
                };
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
            else if (SWClient.CurDBType == DBType.SQLite)
            {
                count = GetCount<T1, T2>(expression);
                return GetModelList<T1, T2>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
            }
            else
            {
                throw new Exception(message: "not support modellist by page with count.");
            }
        }

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
                throw new Exception(message: "not support modellist by page.");
            }
        }

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

        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, SWPaging paging)
        {
            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, SWPaging paging)
        {
            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        public override int GetCount<T>(Where<T> where)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetCount<T>(expression);
        }

        public override List<T> GetModelList<T>(Where<T> where)
        {
            if (where == null || where.GetExpression() == null)
            {
                return GetModelList<T>();
            }
            return GetModelList<T>(where.GetExpression());
        }

        public override List<T> GetModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderBy, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetTopModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderBy, ascOrDesc, top);
        }

        public override List<T> GetModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelList<T>(Where<T> where, SWPaging paging)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }

            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, SWPaging paging)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }

            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }

        public override List<T> GetTopModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderFieldName, ascOrDesc, top);
        }

        #endregion

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
    }
}
