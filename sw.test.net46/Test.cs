using sw.orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.test.net46
{
    [SWTable("Test")]
    public class Test
    {
        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "ID", IsPrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "Name", IsPrimaryKey = true)]
        public string Name { get; set; }

        /// <summary>
        /// 这是一段测试
        /// </summary>
        [SWColumn(ColumnName = "TestContent")]
        public string TestContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "Number")]
        public byte? Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "Sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "IsDel")]
        public bool IsDel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "Creater")]
        public string Creater { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SWColumn(ColumnName = "CreateTime")]
        public DateTime? CreateTime { get; set; }

    }
}
