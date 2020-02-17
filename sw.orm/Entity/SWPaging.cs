using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    public class SWPaging
    {
        private int? _pageIndex = 1;
        private int? _pageSize = 10;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int? PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int? PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
    }
}
