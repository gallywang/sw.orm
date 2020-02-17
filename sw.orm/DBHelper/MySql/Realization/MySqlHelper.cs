using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace sw.orm
{
    internal class MySqlHelper : IDBHelper
    {
        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        private string _strConn;

        public MySqlHelper(string conn)
        {
            _strConn = conn;
        }

        /// <summary>
        /// 执行sql语句，返回datatable
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public override DataTable ExecuteDataTable(string sql, List<SWDbParameter> paramList)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        //添加参数
                        if (paramList != null && paramList.Count > 0)
                        {
                            adapter.SelectCommand.Parameters.AddRange(GetParameters(paramList));
                        }
                        adapter.Fill(ds, "ds");
                        adapter.SelectCommand.Parameters.Clear();
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 执行sql语句，返回dataset
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public override DataSet ExecuteDataSet(string sql, List<SWDbParameter> paramList)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        //添加参数
                        if (paramList != null && paramList.Count > 0)
                        {
                            adapter.SelectCommand.Parameters.AddRange(GetParameters(paramList));
                        }
                        adapter.Fill(ds, "ds");
                        adapter.SelectCommand.Parameters.Clear();
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
                return ds;
            }
        }

        /// <summary>
        /// 执行无参sql语句
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public override int ExecuteQueryNone(string strSql)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                using (MySqlCommand cmd = new MySqlCommand(strSql, conn))
                {
                    try
                    {
                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行带参数sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public override int ExecuteQuery(string strSql, List<SWDbParameter> cmdParms)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, conn, null, strSql, GetParameters(cmdParms));
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 事务执行多条语句
        /// </summary>
        /// <param name="sqlList"></param>
        /// <returns></returns>
        public override bool ExecTrans(List<string> sqlList, List<List<SWDbParameter>> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < sqlList.Count; n++)
                    {
                        string strsql = sqlList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            PrepareCommand(cmd, conn, tx, strsql, GetParameters(parameters[n]));
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }
                    cmd.ExecuteNonQuery();
                    tx.Commit();
                    return true;
                }
                catch
                {
                    tx.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回datatable
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override DataTable ExecProcTable(string strProcName, List<SWDbParameter> paramList)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(strProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (paramList != null && paramList.Count > 0)
                    {
                        MySqlParameter[] parameters = GetParameters(paramList);
                        foreach (MySqlParameter parameter in parameters)
                        {
                            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                                (parameter.Value == null))
                            {
                                parameter.Value = DBNull.Value;
                            }
                            cmd.Parameters.Add(parameter);
                        }

                    }
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        if(ds != null && ds.Tables.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回dataset
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override DataSet ExecProcDataSet(string strProcName, List<SWDbParameter> paramList)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(strProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (paramList != null && paramList.Count > 0)
                    {
                        MySqlParameter[] parameters = GetParameters(paramList);
                        foreach (MySqlParameter parameter in parameters)
                        {
                            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                                (parameter.Value == null))
                            {
                                parameter.Value = DBNull.Value;
                            }
                            cmd.Parameters.Add(parameter);
                        }

                    }
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            da.Fill(ds, "ds");
                            cmd.Parameters.Clear();
                        }
                        catch (MySql.Data.MySqlClient.MySqlException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程，无返回值
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override void ExecProcNone(string strProcName, List<SWDbParameter> paramList)
        {
            using (MySqlConnection conn = new MySqlConnection(_strConn))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(strProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (paramList != null && paramList.Count > 0)
                    {
                        MySqlParameter[] parameters = GetParameters(paramList);
                        foreach (MySqlParameter parameter in parameters)
                        {
                            if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                                (parameter.Value == null))
                            {
                                parameter.Value = DBNull.Value;
                            }
                            cmd.Parameters.Add(parameter);
                        }

                    }
                    cmd.ExecuteNonQuery();
                    
                }
            }
        }

        /// <summary>
        /// 执行语句前准(解析sql语句)
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, 
            MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null && cmdParms.Length > 0)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                    (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        /// <summary>
        /// 参数类型转换(自定义参数SWDbParameter转换为MySqlParameter)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private MySqlParameter[] GetParameters(List<SWDbParameter> parameters)
        {
            if(parameters == null || parameters.Count == 0)
            {
                return null;
            }

            MySqlParameter[] sqlParameters = new MySqlParameter[parameters.Count];
            for (int i = 0, count = parameters.Count; i < count; i++)
            {
                switch (parameters[i].ParameterDbType)
                {
                    case DbType.Int32:
                        sqlParameters[i] = new MySqlParameter(string.Format("@{0}", parameters[i].ParameterName), MySqlDbType.Int32) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.DateTime:
                        sqlParameters[i] = new MySqlParameter(string.Format("@{0}", parameters[i].ParameterName), MySqlDbType.DateTime) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.String:
                        sqlParameters[i] = new MySqlParameter(string.Format("@{0}", parameters[i].ParameterName), MySqlDbType.String) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.Boolean:
                        sqlParameters[i] = new MySqlParameter(string.Format("@{0}", parameters[i].ParameterName), MySqlDbType.Bit) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.Decimal:
                        sqlParameters[i] = new MySqlParameter(string.Format("@{0}", parameters[i].ParameterName), MySqlDbType.Decimal) { Value = parameters[i].ParameterValue };
                        break;
                    default:
                        throw new Exception(message: string.Format("DbType:{0}转SqlDbType失败", parameters[i].ParameterDbType.ToString()));
                }
            }
            return sqlParameters;
        }

    }
}
