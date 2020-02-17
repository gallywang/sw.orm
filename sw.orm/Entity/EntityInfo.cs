using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    internal class EntityInfo
    {
        /// <summary>
        /// 数据表名
        /// </summary>
        public string DbTableName { get; set; }

        //public string TableDescription { get; set; }
        ///public Type Type { get; set; }
        
        
        public List<EntityColumnInfo> Columns { get; set; }
    }
}
