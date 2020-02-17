using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class ExpressionProvider
    {
        public static object Analyze(Expression exp, ref List<SWDbParameter> parameterList)
        {
            var nodeType = exp.NodeType;

            //表示具有二进制运算符的表达式，二进制运算包含：算术运算、逻辑运算
            if (exp is BinaryExpression)
            {
                return BinarExpressionProvider.Analyze(exp, ref parameterList);
            }
            //方法调用：静态方法/实例方法
            else if (exp is MethodCallExpression)
            {
                return MethodCallExpressionProvider.Analyze(exp, ref parameterList);
            }
            //表示访问字段或属性
            else if (exp is MemberExpression)
            {
                return MemberExpressionProvider.Analyze(exp);
            }
            //表示具有常数值的表达式
            else if (exp is ConstantExpression)
            {
                return ConstantExpressionProvider.Analyze(exp);
            }
            //表示具有一元运算符的表达式：顾名思义，只有一个操作数，例如Convert,++,DateTime.Now
            else if (exp is UnaryExpression)
            {
                return UnaryExpressionProvider.Analyze(exp);
            }
            //表示创建一个新数组，并可能初始化该新数组的元素
            else if (exp is NewArrayExpression)
            {
                return NewArrayExpressionProvider.Analyze(exp);
            }

            return string.Empty;
        }

        /// <summary>
        /// 无需解析为带参数sql:例如获取排序字段等操作
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public static object AnalyzeWithoutParams(Expression exp)
        {
            var nodeType = exp.NodeType;

            //表示具有二进制运算符的表达式，二进制运算包含：算术运算、逻辑运算
            if (exp is MemberExpression)
            {
                return MemberExpressionProvider.Analyze(exp);
            }
            //表示具有常数值的表达式
            else if (exp is ConstantExpression)
            {
                return ConstantExpressionProvider.Analyze(exp);
            }
            //表示具有一元运算符的表达式：顾名思义，只有一个操作数，例如Convert,++,DateTime.Now
            else if (exp is UnaryExpression)
            {
                return UnaryExpressionProvider.Analyze(exp);
            }
            //表示对静态方法或实例方法的调用
            else if (exp is MethodCallExpression)
            {
                //
                return ExpressionCompile.GetGetStrCompileResult(exp);
            }

            return string.Empty;
        }
    }
}
