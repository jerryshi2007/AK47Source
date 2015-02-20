using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// ����Ϊ�Ӷ���ʱ��ORMaping��ϵ
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class SubClassORFieldMappingAttribute : ORFieldMappingAttribute
    {
        private string subPropertyName = string.Empty;

        /// <summary>
        /// ���췽��
        /// </summary>
        protected SubClassORFieldMappingAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="fieldName"></param>
        public SubClassORFieldMappingAttribute(string subPropertyName, string fieldName)
            : base(fieldName)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="fieldName"></param>
        /// <param name="nullable"></param>
        public SubClassORFieldMappingAttribute(string subPropertyName, string fieldName, bool nullable)
            : base(fieldName, nullable)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// Դ�������������
        /// </summary>
        public string SubPropertyName
        {
            get
            {
                return this.subPropertyName;
            }
            set
            {
                this.subPropertyName = value;
            }
        }
    }
}
