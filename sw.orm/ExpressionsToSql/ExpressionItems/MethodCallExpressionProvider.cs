using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace sw.orm
{
    public class MethodCallExpressionProvider
    {
        public static object Analyze(Expression exp, ref List<SWDbParameter> parameterList)
        {
            MethodCallExpression mce = exp as MethodCallExpression;
            if (mce.Method.Name == "Contains")
            {
                if (mce.Object == null)
                {
                    //return string.Format("{0} in ({1})", ExpressionRouter(mce.Arguments[1]), ExpressionRouter(mce.Arguments[0]));
                }
                else
                {
                    if (mce.Object.NodeType == ExpressionType.MemberAccess)
                    {
                        var _name = ExpressionProvider.Analyze(mce.Object, ref parameterList);
                        var _value = ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList);

                        int count = parameterList.Count(m => m.ParameterName.StartsWith(_name.ToString()));
                        parameterList.Add(new SWDbParameter(string.Format("{0}{1}", _name, count), string.Format("%{0}%", _value), ExpressionCompile.GetStrType(_value)));
                        return string.Format("{0} like @{0}{1}", _name, count);
                    }
                    else if (mce.Object.NodeType == ExpressionType.Call)
                    {
                        //左侧含有函数时，需转换sql函数
                        //w => w.name.Contains("1")
                        //string _name = MethodCallExpressionProvider(mce.Object);
                        //string _value = ExpressionRouter(mce.Arguments[0]);
                        //_value = _value.Trim('\'');
                        ////var index = _value.RetainNumber().ToInt32() - 1;
                        ////listSqlParaModel[index].value = "%{0}%".FormatWith(listSqlParaModel[index].value);
                        //return string.Format("{0} like '%{1}%'", _name, _value);
                    }
                }
            }
            else if (mce.Method.Name == "OrderBy")
            {
                return string.Format("{0} asc", ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList));
            }
            else if (mce.Method.Name == "OrderByDescending")
            {
                return string.Format("{0} desc", ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList));
            }
            else if (mce.Method.Name == "ThenBy")
            {
                return string.Format("{0},{1} asc", ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList), ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList));
            }
            else if (mce.Method.Name == "ThenByDescending")
            {
                return string.Format("{0},{1} desc", ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList), ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList));
            }
            else if (mce.Method.Name == "Like")
            {
                var left = ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList);
                var right = ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList);

                int count = parameterList.Count(m => m.ParameterName.StartsWith(left.ToString()));
                parameterList.Add(new SWDbParameter(string.Format("{0}{1}", left, count), string.Format("%{0}%", right), ExpressionCompile.GetStrType(right)));
                return string.Format("{0} like @{0}{1}", left, count);
            }
            else if (mce.Method.Name == "NotLike")
            {
                var left = ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList);
                var right = ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList);

                int count = parameterList.Count(m => m.ParameterName.StartsWith(left.ToString()));
                parameterList.Add(new SWDbParameter(string.Format("{0}{1}", left, count), string.Format("%{0}%", right), ExpressionCompile.GetStrType(right)));
                return string.Format("{0} not like @{0}{1}", left, count);
            }
            else if (mce.Method.Name == "In")
            {
                var left = ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList);
                var right = ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList);

                //int count = parameterList.Count(m => m.ParameterName.StartsWith(left));
                //parameterList.Add(new SWDbParameter(string.Format("{0}{1}", left, count), right, GetStrType(right)));
                return string.Format("{0} in ({1})", left, right);
            }
            else if (mce.Method.Name == "NotIn")
            {
                var left = ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList);
                var right = ExpressionProvider.Analyze(mce.Arguments[1], ref parameterList);

                //int count = parameterList.Count(m => m.ParameterName.StartsWith(left));
                //parameterList.Add(new SWDbParameter(string.Format("{0}{1}", left, count), right, GetStrType(right)));
                return string.Format("{0} not in ({1})", left, right);
            }
            else if (mce.Method.Name == "Equals")
            {
                var left = ExpressionProvider.Analyze(mce.Object, ref parameterList);
                var right = ExpressionProvider.Analyze(mce.Arguments[0], ref parameterList);

                int count = parameterList.Count(m => m.ParameterName.StartsWith(left.ToString()));
                parameterList.Add(new SWDbParameter(string.Format("{0}{1}", left, count), right, ExpressionCompile.GetStrType(right)));
                return string.Format("{0} = @{0}{1}", left, count);
            }
            else
            {
                //throw new Exception(string.Format("哎哟喂，怎么又不支持格式了鸭,当前方法:{0}", mce.Method.Name));
                //if (mce.Object != null)
                //{
                //    if (mce.Object.NodeType == ExpressionType.MemberAccess)
                //    {
                //        //将方法转换sql函数，例如trim、tolower等
                //    }
                //}
                //暂不支持左侧存在函数的情况，即sql函数操作
                return ExpressionCompile.GetGetStrCompileResult(mce);
            }
            return "";
        }
    }
}
