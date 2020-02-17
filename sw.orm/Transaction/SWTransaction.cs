using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// 事务处理
    /// </summary>
    public class SWTransaction : IDisposable
    {
        /// <summary>
        /// sql语句集合
        /// </summary>
        private List<string> sqlList = null;

        /// <summary>
        /// sql集合一一对应参数(与sqlList对应)
        /// </summary>
        private List<List<SWDbParameter>> parametersList = null;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string conn = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SWTransaction()
        {
            sqlList = new List<string>();
            parametersList = new List<List<SWDbParameter>>();
            conn = SWClient.CurConnection;
        }

        /// <summary>
        /// 释放空间
        /// </summary>
        public void Dispose()
        {
            if (sqlList != null)
            {
                sqlList.Clear();
                sqlList = null;
            }

            if (parametersList != null)
            {
                parametersList.Clear();
                parametersList = null;
            }
        }

        public void Insert<T>(T tParameter)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = AddSqlBuilder.Insert<T>(tParameter, ref parameters);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(parameters);
            }
        }

        public void Insert<T>(List<T> tParameters)
        {
            if (tParameters != null && tParameters.Count > 0)
            {
                for (int i = 0; i < tParameters.Count; i++)
                {
                    List<SWDbParameter> parameters = new List<SWDbParameter>();
                    string sql = AddSqlBuilder.Insert<T>(tParameters[i], ref parameters);
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sqlList.Add(sql);
                        parametersList.Add(parameters);
                    }
                }
            }
        }

        public void Update<T>(T tParameter)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = UpdateSqlBuilder.Update<T>(tParameter, ref parameters);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(parameters);
            }
        }

        /// <summary>
        /// 更新(以主键为条件更新)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public void Update<T>(List<T> tParameter)
        {
            if (tParameter != null && tParameter.Count > 0)
            {
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
            }
        }

        /// <summary>
        /// 根据条件更新指定列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldEx">更新列</param>
        /// <param name="fiterEx">更新条件</param>
        /// <returns></returns>
        public void Update<T>(Expression<Func<T, T>> fieldEx, Expression<Func<T, bool>> fiterEx)
        {
            if (fieldEx != null)
            {
                List<SWDbParameter> parameters = new List<SWDbParameter>();
                string sql = UpdateSqlBuilder.Update<T>(fieldEx, fiterEx, ref parameters);
                if (!string.IsNullOrEmpty(sql))
                {
                    sqlList.Add(sql);
                    parametersList.Add(parameters);
                }
            }
        }

        /// <summary>
        /// 删除记录:不带参数时，以主键作为删除条件，未设置主键时不可删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        public void Delete<T>(T tParameter)
        {
            string sql = DeleteSqlBuilder.Delete<T>(tParameter);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(null);
            }
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public void Delete<T>(List<string> listId)
        {
            string sql = DeleteSqlBuilder.Delete<T, string>(listId);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(null);
            }
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public void Delete<T>(int id)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = DeleteSqlBuilder.Delete<T, int>(id, ref parameters);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(parameters);
            }
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public void Delete<T>(List<int> listId)
        {
            string sql = DeleteSqlBuilder.Delete<T, int>(listId);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(null);
            }
        }

        /// <summary>
        /// 删除记录:不带参数时，以主键作为删除条件，未设置主键时不可删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        public void Delete<T>(List<T> tParameter)
        {
            if (tParameter != null && tParameter.Count > 0)
            {
                for (int i = 0; i < tParameter.Count; i++)
                {
                    string sql = DeleteSqlBuilder.Delete<T>(tParameter[i]);
                    if (!string.IsNullOrEmpty(sql))
                    {
                        sqlList.Add(sql);
                        parametersList.Add(null);
                    }
                }
            }
        }

        /// <summary>
        /// 根据条件删除，条件为空时删除所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiterEx"></param>
        /// <returns></returns>
        public void Delete<T>(Expression<Func<T, bool>> fiterEx)
        {
            List<SWDbParameter> parameters = new List<SWDbParameter>();
            string sql = DeleteSqlBuilder.Delete<T>(fiterEx, ref parameters);
            if (!string.IsNullOrEmpty(sql))
            {
                sqlList.Add(sql);
                parametersList.Add(parameters);
            }
        }

        public void CommitTran()
        {
            IDBHelper sqlHelper;
            if (SWClient.CurDBType == DBType.SQLServer)
            {
                sqlHelper = new SqlHelper(conn);
            }
            else if (SWClient.CurDBType == DBType.MySql)
            {
                sqlHelper = new MySqlHelper(conn);
            }
            else if (SWClient.CurDBType == DBType.SQLite)
            {
                sqlHelper = new SQLiteHelper(conn);
            }
            else
            {
                throw new Exception(message: "transaction not support.");
            }
            sqlHelper.ExecTrans(sqlList, parametersList);
        }
    }
}
