using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class SWClient
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private static DBClient sqlClient;

        /// <summary>
        /// 链接字符串
        /// </summary>
        private static string _conn = null;

        /// <summary>
        /// 数据库类别
        /// </summary>
        private static DBType _dbtype;

        /// <summary>
        /// 初始化Sql
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dBType"></param>
        /// <returns></returns>
        public static DBClient Initialize(string connection, DBType dBType)
        {
            _conn = connection;
            _dbtype = dBType;
            switch (dBType)
            {
                case DBType.SQLServer:
                case DBType.MySql:
                case DBType.SQLite:
                    sqlClient = new SqlClient(_conn, dBType);
                    break;
                case DBType.Mongo:
                    //sqlClient = new MongodbClient(_conn);
                    break;
                default:
                    throw new Exception(message: string.Format("{0} not support.", dBType.ToString()));
            }
            return sqlClient;
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string CurConnection
        {
            get { return _conn; }
        }

        /// <summary>
        /// 获取数据库类别
        /// </summary>
        public static DBType CurDBType
        {
            get { return _dbtype; }
        }
    }
}
