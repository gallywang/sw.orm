using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    /// <summary>
    /// 数据表属性说明
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SWTable : Attribute
    {
        private SWTable() { }
        public string TableName { get; set; }
        public string TableDescription { get; set; }
        public SWTable(string tableName)
        {
            this.TableName = tableName;
        }
        public SWTable(string tableName, string tableDescription)
        {
            this.TableName = tableName;
            this.TableDescription = tableDescription;
        }
    }

    /// <summary>
    /// 数据表字段及说明
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class SWColumn : Attribute
    {
        /// <summary>
        /// 对应数据表列名
        /// </summary>
        private string _ColumnName;
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        /// <summary>
        /// 是否为主键
        /// </summary>
        private bool _IsPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }

        /// <summary>
        /// 是否为自增键/Sqlserver中timestamp类型
        /// </summary>
        private bool _IsIdentity;
        public bool IsIdentity
        {
            get { return _IsIdentity; }
            set { _IsIdentity = value; }
        }
    }
}
