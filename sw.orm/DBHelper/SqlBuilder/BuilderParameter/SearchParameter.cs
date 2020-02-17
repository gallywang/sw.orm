using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    /// <summary>
    /// 查询参数
    /// </summary>
    internal class SearchParameter<T1, T2>
    {
        /// <summary>
        /// 筛选条件
        /// </summary>
        public Expression<Func<T1, T2>> FilterExp { get; set; }

        /// <summary>
        /// 判断字段(指定实体类字段)
        /// </summary>
        public Expression<Func<T1, object>> OrderByExp { get; set; }

        /// <summary>
        /// 指定字段名字符串进行排序
        /// </summary>
        public string OrderStr { get; set; }

        /// <summary>
        /// 根据多列进行排序
        /// </summary>
        public List<SWOrder> OrderStrs { get; set; }

        /// <summary>
        /// 根据多列进行排序(指定实体类字段)
        /// </summary>
        public List<SWOrder<T1>> OrderByExps { get; set; }

        /// <summary>
        /// 排序方式(默认升序排序)
        /// </summary>
        private AscOrDesc? ascOrDesc =  orm.AscOrDesc.Asc;
        public AscOrDesc? AscOrDesc
        {
            get { return ascOrDesc; }
            set { ascOrDesc = value; }
        }

        /// <summary>
        /// 分页(每页显示数量)
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// 分页(当前页码)
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// 取前几条
        /// </summary>
        public int? Top { get; set; }
    }
}
