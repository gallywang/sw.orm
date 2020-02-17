using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace sw.orm
{
    internal abstract class IDBHelper
    {
        public abstract DataTable ExecuteDataTable(string sql, List<SWDbParameter> paramList);

        public abstract DataSet ExecuteDataSet(string sql, List<SWDbParameter> paramList);

        public abstract int ExecuteQueryNone(string sql);

        public abstract int ExecuteQuery(string sql, List<SWDbParameter> paramList);

        public abstract bool ExecTrans(List<string> sqlstr, List<List<SWDbParameter>> paramList);

        public abstract DataTable ExecProcTable(string strProcName, List<SWDbParameter> paramList);

        public abstract DataSet ExecProcDataSet(string strProcName, List<SWDbParameter> paramList);

        public abstract void ExecProcNone(string strProcName, List<SWDbParameter> paramList);
    }
}
