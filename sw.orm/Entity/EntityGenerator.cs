using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace sw.orm
{
    internal class EntityGenerator
    {
        public static EntityInfo GetEntityInfo<T>()
        {
            EntityInfo entityInfo = new EntityInfo();
            //注：typeof 运算符比对象实例上的 GetType 方法要快
            Type type = typeof(T);
            //数据表名(类名)
            string tbname = type.Name;
            //当类别与数据表名称不一致时,获取设置的table名
            var swTableName = type.GetCustomAttributes(typeof(SWTable), true)
                   .Where(it => it is SWTable)
               .Select(it => (SWTable)it)
               .FirstOrDefault();
            if (swTableName != null)
            {
                if (!string.IsNullOrEmpty(swTableName.TableName))
                {
                    tbname = swTableName.TableName;
                }
            }

            entityInfo.DbTableName = tbname;

            entityInfo.Columns = new List<EntityColumnInfo>();
            //获取类各个属性(对应数据库字段)
            foreach (PropertyInfo info in type.GetProperties())
            {
                EntityColumnInfo columnInfo = new EntityColumnInfo();

                bool isPrimaryKey = false;

                //获取列名
                string columnName = info.Name;
                var swColumn = info.GetCustomAttributes(typeof(SWColumn), true)
                    .Where(it => it is SWColumn)
                .Select(it => (SWColumn)it)
                .FirstOrDefault();
                if (swColumn != null)
                {
                    if (!string.IsNullOrEmpty(swColumn.ColumnName))
                    {
                        columnName = swColumn.ColumnName;
                    }
                    isPrimaryKey = swColumn.IsPrimaryKey;
                    columnInfo.DbColumnName = columnName;
                    columnInfo.PropertyInfo = info;
                    columnInfo.ColumnType = info.PropertyType;
                    columnInfo.IsPrimarykey = isPrimaryKey;
                    columnInfo.IsIdentity = swColumn.IsIdentity;

                    entityInfo.Columns.Add(columnInfo);
                }
            }
            return entityInfo;
        }

        /// <summary>
        /// 将数据表转换为实体类对象
        /// where T : new() 是对T的一种类型约束，此约束规定T必须具有无参构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<T> ConvertTableToEntity<T>(DataTable dataTable) where T : new()
        {
            //定义一个list集合
            List<T> list = new List<T>();
            //定义一个临时变量，用来存储属性名
            string tempName = string.Empty;
            foreach (DataRow dr in dataTable.Rows)
            {
                //实例化一个对象
                T t = new T();
                Type model = typeof(T);
                EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
                //遍历T的所有属性
                foreach (EntityColumnInfo pi in entityInfo.Columns)
                {
                    //将属性名赋给临时变量
                    tempName = pi.DbColumnName;
                    //如果表的列名包含属性名
                    if (dataTable.Columns.Contains(tempName))
                    {
                        //如果此属性不可写(有无setter)，则跳过检查下一个属性
                        //if (!pi.CanWrite) continue;
                        //获得表中的字段值
                        object value = dr[tempName];
                        //如果表中非空，则赋给属性值
                        if (value != DBNull.Value)
                        {
                            pi.PropertyInfo.SetValue(t, value, null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 将数据表转换为实体类对象
        /// where T : new() 是对T的一种类型约束，此约束规定T必须具有无参构造函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<T> ConvertTableToEntity2<T>(DataTable dataTable) where T : new()
        {
            //定义一个list集合
            List<T> list = new List<T>();
            //定义一个临时变量，用来存储属性名
            string tempName = string.Empty;
            foreach (DataRow dr in dataTable.Rows)
            {
                //实例化一个对象
                T t = new T();
                Type model = typeof(T);
                EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);
                //遍历T的所有属性
                foreach (EntityColumnInfo pi in entityInfo.Columns)
                {
                    //将属性名赋给临时变量
                    tempName = pi.DbColumnName;
                    //如果表的列名包含属性名
                    if (dataTable.Columns.Contains(tempName))
                    {
                        //如果此属性不可写(有无setter)，则跳过检查下一个属性
                        //if (!pi.CanWrite) continue;
                        //获得表中的字段值
                        object value = dr[tempName];
                        //如果表中非空，则赋给属性值
                        if (value != DBNull.Value)
                        {
                            pi.PropertyInfo.SetValue(t, ChangeType(value, pi.PropertyInfo.PropertyType), null);
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }

        private static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }
            //当mysql中定义类型为char(36)时，默认为guid类型，guid类型数据直接转string或报错
            if (value.GetType() == typeof(Guid))
            {
                value = value.ToString();
            }
            return Convert.ChangeType(value, t);
        }
    }
}
