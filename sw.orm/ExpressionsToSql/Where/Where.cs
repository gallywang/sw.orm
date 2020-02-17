using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class Where<T>
    {
        /// <summary>
        /// 动态拼接Lambda表达式
        /// </summary>
        private Expression<Func<T, bool>> _expression = null;

        public void Or(Expression<Func<T, bool>> expr)
        {
            if (_expression == null)
            {
                _expression = expr;
            }
            else
            {
                _expression = ExpressionHelper.Or<T>(_expression, expr);
                //var invokedExpr = Expression.Invoke(expr, _expression.Parameters.Cast<Expression>());
                //_expression = Expression.Lambda<Func<T, bool>>
                //(Expression.Or(_expression.Body, invokedExpr), _expression.Parameters);
            }
        }

        public void And(Expression<Func<T, bool>> expr)
        {
            if (_expression == null)
            {
                _expression = expr;
            }
            else
            {
                _expression = ExpressionHelper.And<T>(_expression, expr);
                //var invokedExpr = Expression.Invoke(expr, _expression.Parameters.Cast<Expression>());
                //_expression = Expression.Lambda<Func<T, bool>>
                //(Expression.And(_expression.Body, invokedExpr), _expression.Parameters);
            }
        }

        internal Expression<Func<T, bool>> GetExpression()
        {
            return _expression;
        }
    }
}
