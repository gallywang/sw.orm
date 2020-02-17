using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// 错误编码
    /// </summary>
    public class SWErrorCode
    {
        /// <summary>
        /// sql 语句错误
        /// </summary>
        public const int SqlError = -1;

        /// <summary>
        /// Model参数为空
        /// </summary>
        public const int ParamsEmpty = -2;

        /// <summary>
        /// 执行失败
        /// </summary>
        public const int ExecFailed = -3;
    }
}
