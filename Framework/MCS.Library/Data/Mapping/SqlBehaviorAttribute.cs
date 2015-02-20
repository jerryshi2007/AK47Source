#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	SqlBehaviorAttribute.cs
// Remark	��	SQL����
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���ķ�	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SqlBehaviorAttribute : System.Attribute
    {
        private ClauseBindingFlags bindingFlags = ClauseBindingFlags.All;
        private string defaultExpression = string.Empty;
        private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;

        /// <summary>
        /// �����Ӧ��������ö�����ͣ�����Sqlʱ���Ƿ�ʹ��ö�����͵�ֵ�����ͣ�������ʹ���ַ���
        /// </summary>
        public EnumUsageTypes EnumUsage
        {
            get { return this.enumUsage; }
            set { this.enumUsage = value; }
        }

        /// <summary>
        /// ��Ӧ������Ϊ��ʱ�����ṩ��ȱʡֵ���ʽ
        /// </summary>
        public string DefaultExpression
        {
            get { return this.defaultExpression; }
            set { this.defaultExpression = value; }
        }

        /// <summary>
        /// ��Ӧ������ֵ���������ЩSql�����
        /// </summary>
        public ClauseBindingFlags BindingFlags
        {
            get { return this.bindingFlags; }
            set { this.bindingFlags = value; }
        }

        /// <summary>
        /// ȱʡ���췽��
        /// </summary>
        public SqlBehaviorAttribute()
        {
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="flags">��Ӧ������ֵ���������ЩSql�����</param>
        public SqlBehaviorAttribute(ClauseBindingFlags flags)
        {
            this.bindingFlags = flags;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="defaultExpression">�����Ӧ������ֵΪ�գ�ʹ�õ�ȱʡֵ���ʽ</param>
        public SqlBehaviorAttribute(string defaultExpression)
        {
            this.defaultExpression = defaultExpression;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="flags">��Ӧ������ֵ���������ЩSql�����</param>
        /// <param name="defaultExpression">�����Ӧ������ֵΪ�գ�ʹ�õ�ȱʡֵ���ʽ</param>
        public SqlBehaviorAttribute(ClauseBindingFlags flags, string defaultExpression) : this(flags)
        {
            this.defaultExpression = defaultExpression;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="usage">�����Ӧ��������ö�����ͣ�����Sqlʱ���Ƿ�ʹ��ö�����͵�ֵ�����ͣ�������ʹ���ַ���</param>
        public SqlBehaviorAttribute(EnumUsageTypes usage)
        {
            this.enumUsage = usage;
        }
    }
}
