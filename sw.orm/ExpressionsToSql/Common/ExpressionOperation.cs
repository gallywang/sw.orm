using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    internal class ExpressionOperation
    {
        public static string GetExpressionOperation(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return OperationSymbol.AndAlso;
                case ExpressionType.OrElse:
                    return OperationSymbol.OrElse;
                case ExpressionType.Equal:
                    return OperationSymbol.Equal;
                case ExpressionType.GreaterThanOrEqual:
                    return OperationSymbol.GreaterThanOrEqual;
                case ExpressionType.LessThanOrEqual:
                    return OperationSymbol.LessThanOrEqual;
                case ExpressionType.GreaterThan:
                    return OperationSymbol.GreaterThan;
                case ExpressionType.LessThan:
                    return OperationSymbol.LessThan;
                case ExpressionType.NotEqual:
                    return OperationSymbol.NotEqual;
                case ExpressionType.Add:
                    return OperationSymbol.Add;
                case ExpressionType.Subtract:
                    return OperationSymbol.Subtract;
                case ExpressionType.Multiply:
                    return OperationSymbol.Multiply;
                case ExpressionType.Divide:
                    return OperationSymbol.Divide;
                case ExpressionType.Modulo:
                    return OperationSymbol.Modulo;
                default:
                    //TODO loginfo
                    //return OperationSymbol.Failed;
                    throw new Exception(message: string.Format("not support this operation:{0}", expressionType.ToString()));
            }
        }
    }
}
