using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public abstract class DBClient
    {
        #region 插入

        public abstract int Insert<T>(T tParameter);
        public abstract int Insert<T>(List<T> tParameter);

        #endregion

        #region 更新

        public abstract int Update<T>(T tParameter);
        public abstract int Update<T>(List<T> tParameter);
        public abstract int Update<T>(Expression<Func<T, T>> fieldEx, Expression<Func<T, bool>> fiterEx);

        #endregion

        #region 删除

        public abstract int Delete<T>(string id);
        public abstract int Delete<T>(List<string> listId);
        public abstract int Delete<T>(int id);
        public abstract int Delete<T>(List<int> listId);
        public abstract int Delete<T>(T tParameter);
        public abstract int Delete<T>(List<T> tParameter);
        public abstract int Delete<T>(Expression<Func<T, bool>> fiterEx);
        public abstract int Delete<T>(Where<T> where);

        #endregion

        #region 查询

        #region 查询单条记录

        public abstract T GetModel<T>(Expression<Func<T, bool>> expression) where T : new();

        public abstract T GetModel<T>(string id) where T : new();

        public abstract T GetModel<T>(int id) where T : new();

        #endregion

        #region 查询所有记录

        public abstract List<T> GetAll<T>() where T : new();

        public abstract List<T> GetAll<T>(string orderFieldName, AscOrDesc ascOrDesc = AscOrDesc.Asc) where T : new();

        public abstract List<T> GetAll<T>(List<SWOrder> orderList) where T : new();

        public abstract List<T> GetAll<T>(Expression<Func<T, object>> orderBy, AscOrDesc ascOrDesc = AscOrDesc.Asc) where T : new();

        public abstract List<T> GetAll<T>(List<SWOrder<T>> orderList) where T : new();

        #endregion

        #region 判断数据是否存在

        public abstract bool Exists<T>(Expression<Func<T, bool>> expression);

        public abstract bool Exists<T>(Expression<Func<T, T>> expression);

        #endregion

        #region 查询记录总数

        public abstract int GetCount<T>();

        public abstract int GetCount<T>(Expression<Func<T, bool>> expression);

        public abstract int GetCount<T>(Expression<Func<T, T>> expression);

        public abstract int GetCount<T>(Where<T> where) where T : new();

        #endregion

        #region 获取数据列表

        public abstract List<T> GetModelList<T>() where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, bool>> expression) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, T>> expression, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, T>> expression, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Expression<Func<T, bool>> expression, SWPaging paging) where T : new();

        public abstract List<T> GetModelList<T>(Where<T> where) where T : new();

        public abstract List<T> GetModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Where<T> where, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Where<T> where, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelList<T>(Where<T> where, SWPaging paging) where T : new();

        #endregion

        #region 获取数据列表并返回数据总条数(分页查询)

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, SWPaging paging) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Where<T> where, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Where<T> where, out int count, List<SWOrder<T>> orderByList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Where<T> where, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Where<T> where, out int count, List<SWOrder> orderFieldNameList = null, int? pageSize = null, int? pageIndex = null) where T : new();

        public abstract List<T> GetModelListWithCount<T>(Where<T> where, out int count, SWPaging paging) where T : new();

        #endregion

        #region 获取数据列表前n条数据

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder<T>> orderByList = null, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, List<SWOrder> orderFieldNameList = null, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, List<SWOrder<T>> orderByList = null, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, List<SWOrder> orderFieldNameList = null, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Where<T> where, List<SWOrder<T>> orderByList = null, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null) where T : new();

        public abstract List<T> GetTopModelList<T>(Where<T> where, List<SWOrder> orderFieldNameList = null, int? top = null) where T : new();

        #endregion

        #endregion

        #region 其他(存储过程/sql语句)

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strProcName"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public abstract List<T> GetProcDataList<T>(string strProcName, List<SWDbParameter> sqlParameter) where T : new();

        /// <summary>
        /// 执行sql语句返回DataSet数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public abstract DataSet ExecSqlWithDataSet(string sql, List<SWDbParameter> sqlParameter);

        #endregion
    }
}
