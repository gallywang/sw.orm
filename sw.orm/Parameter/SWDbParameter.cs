using System.Data;

namespace sw.orm
{
    public class SWDbParameter
    {
        private string parameterName;      //参数名
        private object parameterValue;     //参数值
        private DbType? parameterDbType;   //参数类型
        private int? parameterSize;        //参数长度

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="parameterDbType"></param>
        /// <param name="parameterSize"></param>
        public SWDbParameter(string parameterName, object parameterValue, DbType? parameterDbType, int? parameterSize)
        {
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
            this.parameterDbType = parameterDbType;
            this.parameterSize = parameterSize;
        }

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="parameterDbType"></param>
        public SWDbParameter(string parameterName, object parameterValue, DbType? parameterDbType)
        {
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
            this.parameterDbType = parameterDbType;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }


        /// <summary>
        /// 参数值
        /// </summary>
        public object ParameterValue
        {
            get { return parameterValue; }
            set { parameterValue = value; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public DbType? ParameterDbType
        {
            get { return parameterDbType; }
            set { parameterDbType = value; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int? ParameterSize
        {
            get { return parameterSize; }
            set { parameterSize = value; }
        }
    }
}
