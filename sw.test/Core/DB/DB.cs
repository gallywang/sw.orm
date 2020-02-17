using sw.orm;
using System;
using System.Collections.Generic;
using System.Text;

namespace sw.test
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    internal class DB
    {
        internal static DBClient client = SWClient.Initialize("server=122.51.176.153;userid=wanggang;password=1992Sxy1113!@;database=sw_account;port=3306;Charset=utf8;Allow Zero Datetime=True; Pooling=false; Max Pool Size=100000;Pooling=false;", DBType.MySql);
    }
}
