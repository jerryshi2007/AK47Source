using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// �����ඨ��֮ǰ�����ڱ�ʾ������Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ORTableMappingAttribute : System.Attribute
    {
        private string tableName = string.Empty;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="tblName">����</param>
        public ORTableMappingAttribute(string tblName)
        {
            this.tableName = tblName;
        }

        /// <summary>
        /// ����
        /// </summary>
        public string TableName
        {
            get { return this.tableName; }
            set { this.tableName = value; }
        }
    }
}
