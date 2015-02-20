using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// ����Ϊ�Ӷ���ʱ��Sql������ɹ�ϵ
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class SubClassSqlBehaviorAttribute : SqlBehaviorAttribute
    {
        private string subPropertyName = string.Empty;

        /// <summary>
        /// ȱʡ���췽��
        /// </summary>
        public SubClassSqlBehaviorAttribute()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="flags">��Ӧ������ֵ���������ЩSql�����</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, ClauseBindingFlags flags)
            : base(flags)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="defaultExpression">�����Ӧ������ֵΪ�գ�ʹ�õ�ȱʡֵ���ʽ</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, string defaultExpression)
            : base(defaultExpression)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="flags">��Ӧ������ֵ���������ЩSql�����</param>
        /// <param name="defaultExpression">�����Ӧ������ֵΪ�գ�ʹ�õ�ȱʡֵ���ʽ</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, ClauseBindingFlags flags, string defaultExpression)
            : base(flags, defaultExpression)
        {
            this.subPropertyName = subPropertyName;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="subPropertyName"></param>
        /// <param name="usage">�����Ӧ��������ö�����ͣ�����Sqlʱ���Ƿ�ʹ��ö�����͵�ֵ�����ͣ�������ʹ���ַ���</param>
        public SubClassSqlBehaviorAttribute(string subPropertyName, EnumUsageTypes usage)
            : base(usage)
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
