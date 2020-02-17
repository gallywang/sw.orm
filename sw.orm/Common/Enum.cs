using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        SQLServer = 1,
        MySql = 2,
        SQLite = 3,
        Mongo = 4
    }

    /// <summary>
    /// 操作节点类型
    /// </summary>
    internal enum EnumNodeType
    {
        [Description("二元运算符")]
        BinaryOperator = 1,
        [Description("一元运算符")]
        UndryOperator = 2,
        [Description("常量表达式")]
        Constant = 3,
        [Description("成员（变量）")]
        MemberAccess = 4,
        [Description("函数")]
        Call = 5,
        [Description("未知")]
        Unknown = -99,
        [Description("不支持")]
        NotSupported = -98
    }

    /// <summary>
    /// 排序方式
    /// </summary>
    public enum AscOrDesc
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,

        /// <summary>
        /// 降序
        /// </summary>
        Desc
    }
}
