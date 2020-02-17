using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class ConstantExpressionProvider
    {
        public static string Analyze(Expression exp)
        {
            ConstantExpression ce = exp as ConstantExpression;
            if (ce.Value == null)
            {
                return null;
            }
            else if (ce.Value is bool)
            {
                return Convert.ToBoolean(ce.Value) ? "1".ToString() : "0".ToString();
            }
            else if (ce.Value is string || ce.Value is char)
            {
                return string.Format("{0}", ce.Value);
            }
            else if (ce.Value is DateTime)
            {
                return string.Format("{0}", Convert.ToDateTime(ce.Value));
            }
            else
            {
                return string.Format("{0}", ce.Value);
            }
        }
    }
}
