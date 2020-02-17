using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class UnaryExpressionProvider
    {
        public static object Analyze(Expression exp)
        {
            UnaryExpression ue = exp as UnaryExpression;
            var result = ExpressionProvider.AnalyzeWithoutParams(ue.Operand);
            return ExpressionCompile.GetStrResult(result);
        }
    }
}
