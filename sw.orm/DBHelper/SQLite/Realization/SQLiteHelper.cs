using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace sw.orm
{
    internal class SQLiteHelper : IDBHelper
    {
        private string connectionString = string.Empty;
        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="dbPath">SQLite数据库文件路径</param> 
        public SQLiteHelper(string dbPath)
        {
            this.connectionString = "Data Source=" + dbPath;
        }

        /// <summary> 
        /// 执行一个查询语句，返回一个包含查询结果的DataTable 
        /// </summary> 
        /// <param name="sql">要执行的查询语句</param> 
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public override DataTable ExecuteDataTable(string sql, List<SWDbParameter> paramList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (paramList != null && paramList.Count > 0)
                    {
                        command.Parameters.AddRange(GetParameters(paramList));
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable data = new DataTable();
                    adapter.Fill(data);
                    return data;
                }
            }
        }

        public override DataSet ExecuteDataSet(string sql, List<SWDbParameter> paramList)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (paramList != null && paramList.Count > 0)
                    {
                        command.Parameters.AddRange(GetParameters(paramList));
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataSet data = new DataSet();
                    adapter.Fill(data);
                    return data;
                }
            }
        }

        public override int ExecuteQueryNone(string sql)
        {
            int affectedRows = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        affectedRows = command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            return affectedRows;
        }

        /// <summary> 
        /// 对SQLite数据库执行增删改操作，返回受影响的行数。 
        /// </summary> 
        /// <param name="sql">要执行的增删改的SQL语句</param> 
        /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param> 
        /// <returns></returns> 
        public override int ExecuteQuery(string sql, List<SWDbParameter> parameters)
        {
            int affectedRows = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;
                        if (parameters != null && parameters.Count > 0)
                        {
                            command.Parameters.AddRange(GetParameters(parameters));
                        }
                        affectedRows = command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            return affectedRows;
        }

        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override bool ExecTrans(List<string> sqlstr, List<List<SWDbParameter>> parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                DbTransaction tran = conn.BeginTransaction();//先实例SqlTransaction类，使用这个事务使用的是con 这个连接，使用BeginTransaction这个方法来开始执行这个事务
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                try
                {
                    int count = sqlstr.Count;
                    for (int i = 0; i < count; i++)
                    {
                        cmd.CommandText = sqlstr[i];
                        if (parameters[i] != null && parameters[i].Count > 0)
                        {
                            cmd.Parameters.AddRange(GetParameters(parameters[i]));
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
                finally
                {
                    cmd.Dispose();
                    tran.Dispose();
                }
            }
        }


        public override DataTable ExecProcTable(string strProcName, List<SWDbParameter> paramList)
        {
            throw new Exception(message: "sqlite not support exec proc.");
        }

        public override DataSet ExecProcDataSet(string strProcName, List<SWDbParameter> paramList)
        {
            throw new Exception(message: "sqlite not support exec proc.");
        }

        public override void ExecProcNone(string strProcName, List<SWDbParameter> paramList)
        {
            throw new Exception(message: "sqlite not support exec proc.");
        }

        private SQLiteParameter[] GetParameters(List<SWDbParameter> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }

            SQLiteParameter[] sqlParameters = new SQLiteParameter[parameters.Count];
            for (int i = 0, count = parameters.Count; i < count; i++)
            {
                sqlParameters[i] = new SQLiteParameter(string.Format("@{0}", parameters[i].ParameterName), parameters[i].ParameterDbType) { Value = parameters[i].ParameterValue };
            }
            return sqlParameters;
        }
    }
}
