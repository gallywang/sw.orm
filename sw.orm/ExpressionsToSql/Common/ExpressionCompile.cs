using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class ExpressionCompile
    {
        /// <summary>
        /// lambda表达式直接计算
        /// </summary>
        /// <param name="mce"></param>
        /// <returns></returns>
        public static object GetGetStrCompileResult(Expression mce)
        {
            var result = Expression.Lambda(mce).Compile().DynamicInvoke();

            return GetStrResult(result);
        }

        /// <summary>
        /// 结果对象转换为字符串
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static object GetStrResult(object result)
        {
            if (result == null)
            {
                return "NULL";
            }
            else if (result.GetType() == typeof(bool) || result.GetType() == typeof(Boolean))
            {
                return Convert.ToBoolean(result);
            }
            else if (result.GetType() == typeof(string))
            {
                return string.Format("{0}", result);
            }
            else if (result.GetType() == typeof(DateTime))
            {
                return Convert.ToDateTime(result);
            }
            else if (result.GetType() == typeof(char))
            {
                return result;
            }
            else if (result.GetType() == typeof(decimal) || result.GetType() == typeof(Decimal))
            {
                return Convert.ToDecimal(result);
            }
            else if (result.GetType() == typeof(int))
            {
                return result;
            }
            else if (result.GetType() == typeof(int[]))
            {
                var rl = result as int[];
                if (rl.Length <= 0)
                {
                    return "";
                }
                else
                {
                    StringBuilder sbTmp = new StringBuilder();
                    foreach (var r in rl)
                    {
                        sbTmp.AppendFormat("{0},", r.ToString());
                    }
                    return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
                }
            }
            else if (result.GetType() == typeof(List<int>))
            {
                var rl = result as List<int>;
                if (rl.Count <= 0)
                {
                    return "''";
                }
                else
                {
                    StringBuilder sbTmp = new StringBuilder();
                    foreach (var r in rl)
                    {
                        sbTmp.AppendFormat("{0},", r.ToString());
                    }
                    return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
                }
            }
            else if (result.GetType() == typeof(string[]))
            {
                var rl = result as string[];
                if (rl.Length <= 0)
                {
                    return "''";
                }
                else
                {
                    StringBuilder sbTmp = new StringBuilder();
                    foreach (var r in rl)
                    {
                        sbTmp.AppendFormat("'{0}',", r.ToString());
                    }
                    return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
                }
            }
            else if (result.GetType() == typeof(List<string>))
            {
                var rl = result as List<string>;
                if (rl.Count <= 0)
                {
                    return "''";
                }
                else
                {
                    StringBuilder sbTmp = new StringBuilder();
                    foreach (var r in rl)
                    {
                        //TODO
                        sbTmp.AppendFormat("'{0}',", r.ToString());
                    }
                    return sbTmp.ToString().Substring(0, sbTmp.ToString().Length - 1);
                }
            }
            else
            {
                throw new Exception(string.Format("抱歉，暂不支持该格式计算:{0}", result));
            }
        }

        /// <summary>
        /// 根据数据对象获取对应数据类型
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static System.Data.DbType GetStrType(object result)
        {
            if (result == null)
            {
                return System.Data.DbType.String;
            }
            else if (result.GetType() == typeof(bool) || result.GetType() == typeof(Boolean)
                || result.GetType() == typeof(bool?) || result.GetType() == typeof(Boolean?))
            {
                return System.Data.DbType.Boolean;
            }
            else if (result.GetType() == typeof(byte) || result.GetType() == typeof(Byte))
            {
                return System.Data.DbType.Byte;
            }
            else if (result.GetType() == typeof(decimal) || result.GetType() == typeof(Decimal))
            {
                return System.Data.DbType.Decimal;
            }
            else if (result.GetType() == typeof(float))
            {
                return System.Data.DbType.Double;
            }
            else if (result.GetType() == typeof(string))
            {
                return System.Data.DbType.String;
            }
            else if (result.GetType() == typeof(DateTime))
            {
                return System.Data.DbType.DateTime;
            }
            else if (result.GetType() == typeof(char))
            {
                return System.Data.DbType.String;
            }
            else if (result.GetType() == typeof(Int16))
            {
                return System.Data.DbType.Int16;
            }
            else if (result.GetType() == typeof(int) || result.GetType() == typeof(Int32))
            {
                return System.Data.DbType.Int32;
            }
            else if (result.GetType() == typeof(Int32))
            {
                return System.Data.DbType.Int32;
            }
            else if (result.GetType() == typeof(Int64))
            {
                return System.Data.DbType.Int64;
            }
            else if (result.GetType() == typeof(int[]))
            {
                return System.Data.DbType.Int32;
            }
            else if (result.GetType() == typeof(List<int>))
            {
                return System.Data.DbType.Int32;
            }
            else if (result.GetType() == typeof(string[]))
            {
                return System.Data.DbType.String;
            }
            else if (result.GetType() == typeof(List<string>))
            {
                return System.Data.DbType.String;
            }
            else
            {
                throw new Exception(string.Format("抱歉，暂不支持该格式计算:{0}", result));
            }
        }
    }
}
