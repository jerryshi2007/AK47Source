using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// 加在类定义之前，用于表示表名的Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ORTableMappingAttribute : System.Attribute
    {
        private string tableName = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="tblName">表名</param>
        public ORTableMappingAttribute(string tblName)
        {
            this.tableName = tblName;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return this.tableName; }
            set { this.tableName = value; }
        }
    }
}
