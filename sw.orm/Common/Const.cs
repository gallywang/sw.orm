using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// 常量
    /// </summary>
    internal class Const
    {
        /// <summary>
        /// 字符串1
        /// </summary>
        public const string STR_ONE = "1";

        /// <summary>
        /// 字符串0
        /// </summary>
        public const string STR_ZERO = "1";

        /// <summary>
        /// 默认单页显示数量
        /// </summary>
        public const int DEFAULT_PAGESIZE = 10;

        /// <summary>
        /// 默认页码
        /// </summary>
        public const int DEFAULT_PAGEINDEX = 1;

        /// <summary>
        /// 日期格式
        /// </summary>
        public const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 降序
        /// </summary>
        public const string DESC = "desc";

        /// <summary>
        /// 升序
        /// </summary>
        public const string ASC = "asc";

        /// <summary>
        /// 默认排序字段
        /// Mongodb更新及删除时会以该ID更新/删除
        /// 排序时以该字段进行排序
        /// </summary>
        public const string DEFAULT_FIELD = "ID";

        /// <summary>
        /// 分隔符comma
        /// </summary>
        public const char COMMA = ',';
    }
}
