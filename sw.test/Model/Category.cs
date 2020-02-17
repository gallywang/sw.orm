using sw.orm;
using System;
using System.Collections.Generic;
using System.Text;

namespace sw.test
{
    [SWTable("Category")]
    public class Category
    {
        /// <summary>
        /// 数据主键ID
        /// </summary>
        [SWColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        [SWColumn(ColumnName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SWColumn(ColumnName = "Creater")]
        public string Creater { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SWColumn(ColumnName = "CreateTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [SWColumn(ColumnName = "Updater")]
        public string Updater { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SWColumn(ColumnName = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }
    }
}
