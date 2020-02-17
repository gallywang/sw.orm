using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    internal class ExpressionContext : ExpressionBase
    {
        public object Analyze(Expression exp)
        {
            return ExpressionProvider.Analyze(exp, ref this.parametersList);   
        }

        public List<SWDbParameter> GetParameters()
        {
            return this.parametersList;
        }

        public static object AnalyzeWithoutParams(Expression exp)
        {
            //return ExpressionComparer.ExpressionRouter(exp);
            return ExpressionProvider.AnalyzeWithoutParams(exp);
        }
    }
}
