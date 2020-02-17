using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    public class NewArrayExpressionProvider
    {
        public static object Analyze(Expression exp)
        {
            var expression = (NewArrayExpression)exp;
            object value = Expression.Lambda(expression).Compile().DynamicInvoke();
            if(value is IEnumerable<string>[])
            {
                string[] result = (value as IEnumerable<string>[])[0].ToArray();
                return ExpressionCompile.GetStrResult(result);
            }
            else if (value is IEnumerable<int>[])
            {
                int[] result = (value as IEnumerable<int>[])[0].ToArray();
                return ExpressionCompile.GetStrResult(result);
            }
            throw new Exception(message: "exception occur while analyze NewArrayExpression, result type :+ " + value.GetType().ToString());
        }
    }
}
