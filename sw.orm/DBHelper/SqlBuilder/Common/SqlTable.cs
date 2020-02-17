using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    /// <summary>
    /// 数据表
    /// </summary>
    internal class SqlTable
    {
        public static string GetTableName<T>()
        {
            //获取类
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
            return entityInfo.DbTableName;
        }
    }
}
