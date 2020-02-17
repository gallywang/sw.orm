using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    /// <summary>
    /// 参数生成
    /// </summary>
    internal class ParameterGenerate
    {
        /// <summary>
        /// 转sql 拼接字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SWDbParameter Generate(string columnName, object value, Type type)
        {
            if (typeof(int) == type || typeof(Nullable<int>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Int32);
            }
            else if (typeof(Boolean) == type || typeof(Nullable<Boolean>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Boolean);
            }
            else if (typeof(DateTime) == type || typeof(Nullable<DateTime>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.DateTime);
            }
            else if (typeof(byte) == type || typeof(Nullable<byte>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Byte);
            }
            else if (typeof(Byte) == type || typeof(Nullable<Byte>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Byte);
            }
            else if (typeof(short) == type || typeof(Nullable<short>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.UInt16);
            }
            else if (typeof(long) == type || typeof(Nullable<long>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.UInt64);
            }
            else if (typeof(float) == type || typeof(Nullable<float>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Single);
            }
            else if (typeof(decimal) == type || typeof(Nullable<decimal>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Decimal);
            }
            else if (typeof(Decimal) == type || typeof(Nullable<Decimal>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Decimal);
            }
            else if (typeof(Guid) == type || typeof(Nullable<Guid>) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Guid);
            }
            else if (typeof(byte[]) == type)
            {
                return new SWDbParameter(columnName, value, DbType.Binary);
            }
            else
            {
                return new SWDbParameter(columnName, value, DbType.String);
            }
        }
    }
}
