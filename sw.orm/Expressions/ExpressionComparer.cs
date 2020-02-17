using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// 该类为旧版本Expression处理类，不可处理sql语句中含特殊字符串情况
    /// 若需要防sql注入处理，需采用ExpressionsToSql/ExpressionContext处理方式
    /// </summary>
    //internal class ExpressionComparer
    //{
    //    public static string ExpressionRouter(Expression exp)
    //    {
    //        var nodeType = exp.NodeType;
    //        if (exp is BinaryExpression)    //表示具有二进制运算符的表达式
    //        {
    //            return BinarExpressionProvider(exp);
    //        }
    //        else if (exp is ConstantExpression) //表示具有常数值的表达式
    //        {
    //            return ConstantExpressionProvider(exp);
    //        }
    //        else if (exp is LambdaExpression)   //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
    //        {
    //            return LambdaExpressionProvider(exp);
    //        }
    //        else if (exp is MemberExpression)   //表示访问字段或属性
    //        {
    //            return MemberExpressionProvider(exp);
    //        }
    //        else if (exp is MethodCallExpression)   //表示对静态方法或实例方法的调用
    //        {
    //            return MethodCallExpressionProvider(exp);
    //        }
    //        else if (exp is NewArrayExpression) //表示创建一个新数组，并可能初始化该新数组的元素
    //        {
    //            return NewArrayExpressionProvider(exp);
    //        }
    //        else if (exp is ParameterExpression)    //表示一个命名的参数表达式。
    //        {
    //            return ParameterExpressionProvider(exp);
    //        }
    //        else if (exp is UnaryExpression)    //表示具有一元运算符的表达式
    //        {
    //            return UnaryExpressionProvider(exp);
    //        }
    //        else if (exp is MemberInitExpression) //表示调用构造函数并初始化新对象的一个或多个成员
    //        {
    //            return MemberInitExpressionProvider(exp);
    //        }
    //        return null;
    //    }

    //    private static string BinarExpressionProvider(Expression exp)
    //    {
    //        BinaryExpression be = exp as BinaryExpression;
    //        Expression left = be.Left;
    //        Expression right = be.Right;
    //        ExpressionType type = be.NodeType;
    //        string sb = "(";
    //        //先处理左边
    //        string strLeft = ExpressionRouter(left);
    //        if ("1".Equals(strLeft) || "0".Equals(strLeft))
    //        {
    //            sb += " 1=1 ";
    //        }
    //        else
    //        {
    //            sb += strLeft;
    //        }
    //        sb += GetExpressionOperation(type);
    //        //再处理右边
    //        string sbTmp = ExpressionRouter(right);
    //        if (sbTmp == "null")
    //        {
    //            if (sb.EndsWith(" = "))
    //                sb = sb.Substring(0, sb.Length - 2) + " is null";
    //            else if (sb.EndsWith(" <> "))
    //                sb = sb.Substring(0, sb.Length - 3) + "is not null";
    //        }
    //        else
    //            sb += sbTmp;
    //        return sb += ")";
    //    }

    //    private static string ConstantExpressionProvider(Expression exp)
    //    {
    //        ConstantExpression ce = exp as ConstantExpression;
    //        if (ce.Value == null)
    //        {
    //            return "null";
    //        }
    //        else if (ce.Value is bool)
    //        {
    //            //TODO
    //            //return GetValueType(ce.Value).ToString();
    //            return Convert.ToBoolean(ce.Value) ? "1".ToString() : "0".ToString();
    //        }
    //        else if (ce.Value is string || ce.Value is char)
    //        {
    //            //TODO
    //            //return GetValueType(ce.Value).ToString();
    //            return string.Format("'{0}'", ce.Value);
    //        }
    //        else if (ce.Value is DateTime)
    //        {
    //            return string.Format("'{0}'", Convert.ToDateTime(ce.Value).ToString(Const.DATE_FORMAT));
    //        }
    //        else
    //        {
    //            return string.Format("{0}", ce.Value);
    //        }
    //    }

    //    private static string LambdaExpressionProvider(Expression exp)
    //    {
    //        LambdaExpression le = exp as LambdaExpression;
    //        return ExpressionRouter(le.Body);
    //    }

    //    private static string MemberExpressionProvider(Expression exp)
    //    {
    //        //m => m.ParentID.Equals(Const.ALL_SELECT_ID),包含Const
    //        if (!exp.ToString().StartsWith("value") && !exp.ToString().StartsWith("Const"))
    //        {
    //            MemberExpression me = exp as MemberExpression;
    //            if (me.Member.Name == "Now")
    //            {
    //                //TODO
    //            }
    //            return me.Member.Name;
    //        }
    //        else
    //        {
    //            var result = Expression.Lambda(exp).Compile().DynamicInvoke();
    //            return GetStrCompileResult(result);
    //        }
    //    }

    //    private static string MethodCallExpressionProvider(Expression exp)
    //    {
    //        MethodCallExpression mce = exp as MethodCallExpression;
    //        if (mce.Method.Name == "Contains")
    //        {
    //            if (mce.Object == null)
    //            {
    //                return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[1]), ExpressionRouter(mce.Arguments[0]));
    //            }
    //            else
    //            {
    //                if (mce.Object.NodeType == ExpressionType.MemberAccess)
    //                {
    //                    //w => w.name.Contains("1")
    //                    string _name = ExpressionRouter(mce.Object);
    //                    string _value = ExpressionRouter(mce.Arguments[0]);
    //                    _value = _value.Trim('\'');
    //                    //var index = _value.RetainNumber().ToInt32() - 1;
    //                    //listSqlParaModel[index].value = "%{0}%".FormatWith(listSqlParaModel[index].value);
    //                    return string.Format("{0} like '%{1}%'", _name, _value);
    //                }
    //                else if (mce.Object.NodeType == ExpressionType.Call)
    //                {
    //                    //左侧含有函数时，需转换sql函数
    //                    //w => w.name.Contains("1")
    //                    //string _name = MethodCallExpressionProvider(mce.Object);
    //                    //string _value = ExpressionRouter(mce.Arguments[0]);
    //                    //_value = _value.Trim('\'');
    //                    ////var index = _value.RetainNumber().ToInt32() - 1;
    //                    ////listSqlParaModel[index].value = "%{0}%".FormatWith(listSqlParaModel[index].value);
    //                    //return string.Format("{0} like '%{1}%'", _name, _value);
    //                }
    //            }
    //        }
    //        else if (mce.Method.Name == "OrderBy")
    //        {
    //            return string.Format("{0} asc", ExpressionRouter(mce.Arguments[1]));
    //        }
    //        else if (mce.Method.Name == "OrderByDescending")
    //        {
    //            return string.Format("{0} desc", ExpressionRouter(mce.Arguments[1]));
    //        }
    //        else if (mce.Method.Name == "ThenBy")
    //        {
    //            return string.Format("{0},{1} asc", MethodCallExpressionProvider(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
    //        }
    //        else if (mce.Method.Name == "ThenByDescending")
    //        {
    //            return string.Format("{0},{1} desc", MethodCallExpressionProvider(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
    //        }
    //        else if (mce.Method.Name == "Like")
    //        {
    //            return string.Format("({0} like '%{1}%')", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]).Replace("'", ""));
    //        }
    //        else if (mce.Method.Name == "NotLike")
    //        {
    //            return string.Format("({0} not like '%{1}%')", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]).Replace("'", ""));
    //        }
    //        else if (mce.Method.Name == "In")
    //        {
    //            return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
    //        }
    //        else if (mce.Method.Name == "NotIn")
    //        {
    //            return string.Format("{0} not in ({1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
    //        }
    //        else if (mce.Method.Name == "Equals")
    //        {
    //            return string.Format("{0} = {1}", ExpressionRouter(mce.Object), ExpressionRouter(mce.Arguments[0]));
    //        }
    //        else
    //        {
    //            //if (mce.Object != null)
    //            //{
    //            //    if (mce.Object.NodeType == ExpressionType.MemberAccess)
    //            //    {
    //            //        //将方法转换sql函数，例如trim、tolower等
    //            //    }
    //            //}
    //            //暂不支持左侧存在函数的情况，即sql函数操作
    //            var result = Expression.Lambda(mce).Compile().DynamicInvoke();
    //            return GetStrCompileResult(result);
    //        }
    //        return "";
    //    }

    //    private static string NewArrayExpressionProvider(Expression exp)
    //    {
    //        NewArrayExpression ae = exp as NewArrayExpression;
    //        StringBuilder sbTmp = new StringBuilder();
    //        foreach (Expression ex in ae.Expressions)
    //        {
    //            sbTmp.Append(ExpressionRouter(ex));
    //            sbTmp.Append(",");
    //        }
    //        return sbTmp.ToString(0, sbTmp.Length - 1);
    //    }

    //    private static string ParameterExpressionProvider(Expression exp)
    //    {
    //        ParameterExpression pe = exp as ParameterExpression;
    //        return pe.Type.Name;
    //    }

    //    private static string UnaryExpressionProvider(Expression exp)
    //    {
    //        UnaryExpression ue = exp as UnaryExpression;
    //        var result = ExpressionRouter(ue.Operand);
    //        ExpressionType type = exp.NodeType;
    //        if (type == ExpressionType.Not)
    //        {
    //            if (result.Contains(" in "))
    //            {
    //                result = result.Replace(" in ", " not in ");
    //            }
    //            if (result.Contains(" like "))
    //            {
    //                result = result.Replace(" like ", " not like ");
    //            }
    //        }
    //        return result;
    //    }

    //    private static string MemberInitExpressionProvider(Expression exp)
    //    {
    //        MemberInitExpression mie = exp as MemberInitExpression;
    //        string result = string.Empty;
    //        List<string> member = new List<string>();
    //        foreach (MemberAssignment item in mie.Bindings)
    //        {
    //            string update = item.Member.Name + "=" + ExpressionRouter(item.Expression);
    //            member.Add(update);
    //        }
    //        result = string.Join(",", member);
    //        return result;
    //    }

    //    /// <summary>
    //    /// 判断表达式类型
    //    /// </summary>
    //    /// <param name="func">lambda表达式</param>
    //    /// <returns></returns>
    //    private static string GetExpressionOperation(ExpressionType expressionType)
    //    {
    //        switch (expressionType)
    //        {
    //            case ExpressionType.AndAlso:
    //            case ExpressionType.And:
    //                return OperationSymbol.AndAlso;
    //            case ExpressionType.OrElse:
    //                return OperationSymbol.OrElse;
    //            case ExpressionType.Equal:
    //                return OperationSymbol.Equal;
    //            case ExpressionType.GreaterThanOrEqual:
    //                return OperationSymbol.GreaterThanOrEqual;
    //            case ExpressionType.LessThanOrEqual:
    //                return OperationSymbol.LessThanOrEqual;
    //            case ExpressionType.GreaterThan:
    //                return OperationSymbol.GreaterThan;
    //            case ExpressionType.LessThan:
    //                return OperationSymbol.LessThan;
    //            case ExpressionType.NotEqual:
    //                return OperationSymbol.NotEqual;
    //            case ExpressionType.Add:
    //                return OperationSymbol.Add;
    //            case ExpressionType.Subtract:
    //                return OperationSymbol.Subtract;
    //            case ExpressionType.Multiply:
    //                return OperationSymbol.Multiply;
    //            case ExpressionType.Divide:
    //                return OperationSymbol.Divide;
    //            case ExpressionType.Modulo:
    //                return OperationSymbol.Modulo;
    //            default:
    //                //TODO loginfo
    //                return OperationSymbol.Failed;
    //        }
    //    }

    //    /// <summary>
    //    /// 将计算的结果转换为字符串
    //    /// </summary>
    //    /// <param name="result"></param>
    //    /// <returns></returns>
    //    private static string GetStrCompileResult(object result)
    //    {
    //        if (result == null)
    //        {
    //            return "NULL";
    //        }
    //        //else if (result is ValueType)
    //        //{
    //        //    //TODO
    //        //    throw new Exception(string.Format("哎哟喂，{0}:怎么又不支持格式了鸭:{1}", result, "ValueType"));
    //        //}
    //        else if (result is string)
    //        {
    //            return string.Format("'{0}'", result);
    //        }
    //        else if (result is DateTime)
    //        {
    //            return string.Format("'{0}'", Convert.ToDateTime(result).ToString("yyyy-MM-dd HH:mm:ss"));
    //        }
    //        else if (result is char)
    //        {
    //            throw new Exception(string.Format("哎哟喂，{0}:怎么又不支持格式了鸭:{1}", result, "char"));
    //        }
    //        else if (result is int[])
    //        {
    //            var rl = result as int[];
    //            if (rl.Length <= 0)
    //            {
    //                return "''";
    //            }
    //            else
    //            {
    //                StringBuilder sbTmp = new StringBuilder();
    //                foreach (var r in rl)
    //                {
    //                    sbTmp.AppendFormat("{0},", r.ToString());
    //                }
    //                return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
    //            }
    //        }
    //        else if (result is List<int>)
    //        {
    //            var rl = result as List<int>;
    //            if (rl.Count <= 0)
    //            {
    //                return "''";
    //            }
    //            else
    //            {
    //                StringBuilder sbTmp = new StringBuilder();
    //                foreach (var r in rl)
    //                {
    //                    sbTmp.AppendFormat("{0},", r.ToString());
    //                }
    //                return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
    //            }
    //        }
    //        else if (result is string[])
    //        {
    //            var rl = result as string[];
    //            if (rl.Length <= 0)
    //            {
    //                return "''";
    //            }
    //            else
    //            {
    //                StringBuilder sbTmp = new StringBuilder();
    //                foreach (var r in rl)
    //                {
    //                    sbTmp.AppendFormat("'{0}',", r.ToString());
    //                }
    //                return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
    //            }
    //        }
    //        else if (result is List<string>)
    //        {
    //            var rl = result as List<string>;
    //            if (rl.Count <= 0)
    //            {
    //                return "''";
    //            }
    //            else
    //            {
    //                StringBuilder sbTmp = new StringBuilder();
    //                foreach (var r in rl)
    //                {
    //                    //TODO
    //                    sbTmp.AppendFormat("'{0}',", r.ToString());
    //                }
    //                return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
    //            }
    //        }
    //        else
    //        {
    //            throw new Exception("哎哟喂，怎么又不支持格式了鸭.");
    //        }
    //    }
    //}
}
