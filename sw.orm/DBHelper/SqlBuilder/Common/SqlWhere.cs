using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    /// <summary>
    /// where条件sql语句
    /// </summary>
    internal class SqlWhere
    {
        /// <summary>
        /// Where条件解析
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string Analysis<T1, T2>(ref List<SWDbParameter> parameters, SearchParameter<T1, T2> searchParameter)
        {
            string strSql = string.Empty;
            if (searchParameter.FilterExp != null)
            {
                ExpressionContext context = new ExpressionContext();
                Expression exp = searchParameter.FilterExp.Body as Expression;
                strSql = string.Format(" WHERE {0}", context.Analyze(exp));
                parameters = context.GetParameters();
            }
            return strSql;
        }

        /// <summary>
        /// Where条件解析
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string Analysis<T1, T2>(ref List<SWDbParameter> parameters, Expression<Func<T1, T2>> expression)
        {
            string strSql = string.Empty;
            if (expression != null)
            {
                ExpressionContext context = new ExpressionContext();
                Expression exp = expression.Body as Expression;
                strSql = string.Format(" WHERE {0}", context.Analyze(exp));
                parameters = context.GetParameters();
            }
            return strSql;
        }
    }
}
