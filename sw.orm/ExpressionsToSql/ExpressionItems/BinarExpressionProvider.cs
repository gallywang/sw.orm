using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class BinarExpressionProvider
    {
        public static string Analyze(Expression exp, ref List<SWDbParameter> parameterList)
        {
            BinaryExpression be = exp as BinaryExpression;
            Expression left = be.Left;
            Expression right = be.Right;
            ExpressionType type = be.NodeType;
            string sb = "(";
            //先处理左边
            var strLeft = ExpressionProvider.Analyze(left, ref parameterList);
            //操作符号
            string strOperation = ExpressionOperation.GetExpressionOperation(type);
            //sb += ExpressionOperation.GetExpressionOperation(type);
            //再处理右边
            var sbTmp = ExpressionProvider.Analyze(right, ref parameterList);

            //TOTO 优化
            if (sbTmp == null)
            {
                if (OperationSymbol.Equal.Equals(strOperation))
                {
                    sb += string.Format(" {0} IS NULL ", strLeft);
                }
                else
                {
                    sb += string.Format(" {0} IS NOT NULL ", strLeft);
                }
            }
            else
            {
                sb += strLeft;
                sb += strOperation;


                //注意：当左边表达式为方法，例如Trim方法等时，会报错；左边方法不支持处理：即数据库函数
                if (left is BinaryExpression || left is MethodCallExpression)
                //|| right is BinaryExpression || right is MethodCallExpression)
                {
                    sb += sbTmp;
                }
                else
                {
                    int count = parameterList.Count(m => m.ParameterName.StartsWith(strLeft.ToString()));
                    parameterList.Add(new SWDbParameter(string.Format("{0}{1}", strLeft, count), sbTmp, ExpressionCompile.GetStrType(sbTmp)));
                    sb += string.Format("@{0}{1}", strLeft, count);
                }
            }
            return sb += ")";
        }
    }
}
