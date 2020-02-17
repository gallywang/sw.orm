using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class SqlOrder
    {
        /// <summary>
        /// Order条件解析
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="searchParameter"></param>
        /// <returns></returns>
        public static string Analysis<T1, T2>(ref List<SWDbParameter> parameters, SearchParameter<T1, T2> searchParameter)
        {
            //排序字段
            string strOrder = string.Empty;
            //根据实体指定字段
            if (searchParameter.OrderByExp != null)
            {
                Expression exp = searchParameter.OrderByExp.Body as Expression;
                var strOrderTemp = ExpressionProvider.AnalyzeWithoutParams(exp);
                if (strOrderTemp != null)
                {
                    strOrder = strOrderTemp.ToString() + ",";
                }
            }

            //根据字段名称
            if (!string.IsNullOrEmpty(searchParameter.OrderStr))
            {
                strOrder += searchParameter.OrderStr;
            }

            //拼接排序字段
            if (!string.IsNullOrEmpty(strOrder))
            {
                strOrder = string.Format(" ORDER BY {0} {1}", strOrder.TrimEnd(','), searchParameter.AscOrDesc.ToString());
            }

            //根据多个字段排序(实体指定字段)
            if (searchParameter.OrderByExps != null && searchParameter.OrderByExps.Count > 0)
            {
                string orderExpMulti = string.Empty;
                for (int i = 0; i < searchParameter.OrderByExps.Count; i++)
                {
                    Expression exp = searchParameter.OrderByExps[i].OrderBy.Body as Expression;
                    var strOrderTemp = ExpressionProvider.AnalyzeWithoutParams(exp);
                    if (strOrderTemp != null)
                    {
                        strOrder = strOrderTemp.ToString() + ",";
                        orderExpMulti += string.Format("{0} {1},", strOrderTemp.ToString(), searchParameter.OrderByExps[i].AscOrDesc.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(orderExpMulti))
                {
                    //拼接排序sql
                    if (string.IsNullOrEmpty(strOrder))
                    {
                        strOrder = string.Format(" ORDER BY {0}", orderExpMulti.TrimEnd(','));
                    }
                    else
                    {
                        strOrder += string.Format(",{0}", orderExpMulti.TrimEnd(','));
                    }
                }
            }

            //根据多个字段排序(字段名)
            if (searchParameter.OrderStrs != null && searchParameter.OrderStrs.Count > 0)
            {
                string orderStrMulti = string.Empty;
                for (int i = 0; i < searchParameter.OrderStrs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(searchParameter.OrderStrs[i].OrderName))
                    {
                        orderStrMulti += string.Format("{0} {1},", searchParameter.OrderStrs[i].OrderName, searchParameter.OrderStrs[i].AscOrDesc.ToString());
                    }
                }

                if (!string.IsNullOrEmpty(orderStrMulti))
                {
                    //拼接排序sql
                    if (string.IsNullOrEmpty(strOrder))
                    {
                        strOrder = string.Format(" ORDER BY {0}", orderStrMulti.TrimEnd(','));
                    }
                    else
                    {
                        strOrder += string.Format(",{0}", orderStrMulti.TrimEnd(','));
                    }
                }
            }
            return strOrder;
        }

        /// <summary>
        /// 获取排序字段的名称(字符串)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static string GetOrderByName<T>(Expression<Func<T, object>> orderBy)
        {
            //排序字段
            string strOrder = string.Empty;
            if (orderBy != null)
            {
                Expression exp = orderBy.Body as Expression;
                var strOrderTemp = ExpressionProvider.AnalyzeWithoutParams(exp);
                if (strOrderTemp != null)
                {
                    strOrder = strOrderTemp.ToString();
                }
            }
            return strOrder;
        }
    }
}
