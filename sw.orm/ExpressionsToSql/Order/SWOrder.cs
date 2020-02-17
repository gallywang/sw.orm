using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    /// <summary>
    /// 自定义排序(可指定不同字段根据不同排序方式排序)
    /// </summary>
    public class SWOrder<T>
    {
        /// <summary>
        /// 指定排序列(根据实体指定)
        /// </summary>
        public Expression<Func<T, object>> OrderBy { get; set; }

        /// <summary>
        /// 排序方式指定
        /// </summary>
        public AscOrDesc AscOrDesc { get; set; }
    }

    /// <summary>
    /// 自定义排序(根据字段名进行排序)
    /// </summary>
    public class SWOrder
    {
        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string OrderName { get; set; }

        /// <summary>
        /// 排序方式(默认升序排序)
        /// </summary>
        private AscOrDesc ascOrDesc = AscOrDesc.Asc;
        public AscOrDesc AscOrDesc
        {
            get { return ascOrDesc; }
            set { ascOrDesc = value; }
        }
    }
}
