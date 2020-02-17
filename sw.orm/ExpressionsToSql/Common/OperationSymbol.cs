using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// sql操作符号
    /// </summary>
    internal class OperationSymbol
    {
        /// <summary>
        /// 或
        /// </summary>
        public const string OrElse = " OR ";

        /// <summary>
        /// 对两个Boolean表达式执行逻辑或
        /// </summary>
        public const string Or = " | ";

        /// <summary>
        /// 并且
        /// </summary>
        public const string AndAlso = " AND ";

        /// <summary>
        /// 对两个Boolean表达式执行逻辑与
        /// </summary>
        public const string And = " & ";

        /// <summary>
        /// 大于
        /// </summary>
        public const string GreaterThan = " > ";

        /// <summary>
        /// 大于等于
        /// </summary>
        public const string GreaterThanOrEqual = " >= ";

        /// <summary>
        /// 小于
        /// </summary>
        public const string LessThan = " < ";

        /// <summary>
        /// 小于等于
        /// </summary>
        public const string LessThanOrEqual = " <= ";

        /// <summary>
        /// 不等于
        /// </summary>
        public const string NotEqual = " <> ";

        /// <summary>
        /// 加
        /// </summary>
        public const string Add = " + ";

        /// <summary>
        /// 减
        /// </summary>
        public const string Subtract = " - ";

        /// <summary>
        /// 乘
        /// </summary>
        public const string Multiply = " * ";

        /// <summary>
        /// 除
        /// </summary>
        public const string Divide = " / ";

        /// <summary>
        /// 取模
        /// </summary>
        public const string Modulo = " % ";

        /// <summary>
        /// 相等
        /// </summary>
        public const string Equal = " = ";

        /// <summary>
        /// 失败
        /// </summary>
        public const string Failed = "failed";
    }
}
