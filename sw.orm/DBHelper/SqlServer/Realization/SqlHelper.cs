using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace sw.orm
{
    internal class SqlHelper : IDBHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string _conStr = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionStrings"></param>
        public SqlHelper(string connectionStrings)
        {
            _conStr = connectionStrings;
        }

        /// <summary>
        /// 执行sql返回DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public override DataTable ExecuteDataTable(string sql, List<SWDbParameter> paramList)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                try
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
                    {
                        //添加参数
                        if (paramList != null && paramList.Count > 0)
                        {
                            adapter.SelectCommand.Parameters.AddRange(GetParameters(paramList));
                        }
                        adapter.Fill(dt);
                        adapter.SelectCommand.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            return dt;
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="sql">所用的sql语句</param>
        /// <param name="param">可变，可以传参也可以不传参数</param>
        /// <returns></returns>
        public override DataSet ExecuteDataSet(string sql, List<SWDbParameter> paramList)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                try
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
                    {
                        //添加参数
                        if (paramList != null && paramList.Count > 0)
                        {
                            adapter.SelectCommand.Parameters.AddRange(GetParameters(paramList));
                        }
                        adapter.Fill(ds);
                        adapter.SelectCommand.Parameters.Clear();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            return ds;
        }

        /// <summary>
        /// 执行增删改，返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override int ExecuteQueryNone(string sql)
        {
            int n = -1;
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    n = cmd.ExecuteNonQuery();
                }
            }
            return n;
        }

        /// <summary>
        /// 执行增删改，返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override int ExecuteQuery(string sql, List<SWDbParameter> param)
        {
            int n = -1;
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (param != null && param.Count > 0)
                    {
                        cmd.Parameters.AddRange(GetParameters(param));
                    }
                    n = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            return n;
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="Sqlstr">sql语句</param>
        /// <returns></returns>
        public override bool ExecTrans(List<string> sqlStr, List<List<SWDbParameter>> paramList)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();//先实例SqlTransaction类，使用这个事务使用的是con 这个连接，使用BeginTransaction这个方法来开始执行这个事务
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = tran;
                try
                {
                    int count = sqlStr.Count;
                    for (int i = 0; i < count; i++)
                    {
                        cmd.CommandText = sqlStr[i];
                        if (paramList[i] != null && paramList[i].Count > 0)
                        {
                            cmd.Parameters.AddRange(GetParameters(paramList[i]));
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    tran.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
                finally
                {
                    cmd.Dispose();
                    tran.Dispose();
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回table
        /// </summary>
        /// <param name="strProcName">存储过程名称</param>
        /// <param name="paraValues">可变的参数数组 数组的个数可以为0，也可以为多个</param>
        /// <returns></returns>
        public override DataTable ExecProcTable(string strProcName, List<SWDbParameter> paramList)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = strProcName;
                    // 设置CommandType的类型
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    conn.Open();
                    if (paramList != null && paramList.Count > 0)
                    {
                        //添加参数
                        cmd.Parameters.AddRange(GetParameters(paramList));
                    }
                    // 取数据
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            return dt;
        }

        /// <summary>
        /// 执行存储过程返回table
        /// </summary>
        /// <param name="strProcName">存储过程名称</param>
        /// <param name="paraValues">可变的参数数组 数组的个数可以为0，也可以为多个</param>
        /// <returns></returns>
        public override DataSet ExecProcDataSet(string strProcName, List<SWDbParameter> paramList)
        {
            DataSet dt = new DataSet();
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = strProcName;
                    // 设置CommandType的类型
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    conn.Open();
                    if (paramList != null && paramList.Count > 0)
                    {
                        //添加参数
                        cmd.Parameters.AddRange(GetParameters(paramList));
                    }
                    // 取数据
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            return dt;
        }

        /// <summary>
        /// 执行存储过程返回table
        /// </summary>
        /// <param name="strProcName">存储过程名称</param>
        /// <param name="paraValues">可变的参数数组 数组的个数可以为0，也可以为多个</param>
        /// <returns></returns>
        public override void ExecProcNone(string strProcName, List<SWDbParameter> paramList)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = strProcName;
                    // 设置CommandType的类型
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = conn;
                    conn.Open();
                    if (paramList != null && paramList.Count > 0)
                    {
                        //添加参数
                        cmd.Parameters.AddRange(GetParameters(paramList));
                    }
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dt"></param>
        public int SqlBulkCopy(DataTable dt, string tableName)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                try
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    conn.Open();
                    if (dt != null && dt.Rows.Count != 0)
                    {
                        bulkCopy.WriteToServer(dt);
                    }
                    return dt.Rows.Count;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 将自定义参数转换为sql server对应参数
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private SqlParameter[] GetParameters(List<SWDbParameter> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }

            SqlParameter[] sqlParameters = new SqlParameter[parameters.Count];
            for (int i = 0, count = parameters.Count; i < count; i++)
            {
                switch (parameters[i].ParameterDbType)
                {
                    case DbType.Int32:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.Int) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.DateTime:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.DateTime) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.String:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.NVarChar) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.Boolean:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.Bit) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.Byte:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.TinyInt) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.Decimal:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.Decimal) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.UInt16:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.SmallInt) { Value = parameters[i].ParameterValue };
                        break;
                    case DbType.UInt64:
                        sqlParameters[i] = new SqlParameter(string.Format("@{0}", parameters[i].ParameterName), SqlDbType.BigInt) { Value = parameters[i].ParameterValue };
                        break;
                    default:
                        throw new Exception(message: string.Format("DbType:{0}转SqlDbType失败", parameters[i].ParameterDbType.ToString()));
                }
            }
            return sqlParameters;
        }
    }
}
