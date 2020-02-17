using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class MemberExpressionProvider
    {
        public static object Analyze(Expression exp)
        {
            if (!exp.ToString().StartsWith("value"))
            {
                MemberExpression me = exp as MemberExpression;
                //右侧为常量字符(或代码中常量表达式，例如：Const.TRUE)
                if (me.Expression != null)
                {
                    return me.Member.Name;
                }
            }
            return ExpressionCompile.GetGetStrCompileResult(exp);
        }
    }
}
