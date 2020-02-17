using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    internal class ExpressionBase
    {
        /// <summary>
        /// sql参数
        /// </summary>
        protected List<SWDbParameter> parametersList;

        /// <summary>
        /// sql语句
        /// </summary>
        //protected string sql;

        public ExpressionBase()
        {
            parametersList = new List<SWDbParameter>();
        }
    }
}
