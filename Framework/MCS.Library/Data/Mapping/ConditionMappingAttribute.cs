using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using MCS.Library.Data.Mapping;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// ���������ӳ������
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ConditionMappingAttribute : System.Attribute
	{
		private string dataFieldName = string.Empty;
		private bool isExpression = false;
		private string operation = "=";
		private string prefix = string.Empty;
		private string postfix = string.Empty;
		private double adjustDays = 0;
		private string template = string.Empty;
		private bool escapeLikeString = false;
		private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;

		/// <summary>
		/// 
		/// </summary>
		public ConditionMappingAttribute()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		public ConditionMappingAttribute(string fieldName)
		{
			this.dataFieldName = fieldName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="op"></param>
		public ConditionMappingAttribute(string fieldName, string op)
		{
			this.dataFieldName = fieldName;
			this.operation = op;
		}

		/// <summary>
		/// ��������ȱʡΪ��=��
		/// </summary>
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}

		/// <summary>
		/// �Ƿ��Ǳ��ʽ
		/// </summary>
		public bool IsExpression
		{
			get { return this.isExpression; }
			set { this.isExpression = value; }
		}

		/// <summary>
		/// �����ֶε�����
		/// </summary>
		public string DataFieldName
		{
			get { return this.dataFieldName; }
			set { this.dataFieldName = value; }
		}

		/// <summary>
		/// ö�����͵�ʹ�÷�����ֵ/����������
		/// </summary>
		public EnumUsageTypes EnumUsage
		{
			get { return this.enumUsage; }
			set { this.enumUsage = value; }
		}

		/// <summary>
		/// ����Valueʱ��ǰ׺
		/// </summary>
		public string Prefix
		{
			get { return this.prefix; }
			set { this.prefix = value; }
		}

		/// <summary>
		/// ����Valueʱ�ĺ�׺
		/// </summary>
		public string Postfix
		{
			get { return this.postfix; }
			set { this.postfix = value; }
		}

		/// <summary>
		/// ����������ͣ���Ҫ����������
		/// </summary>
		public double AdjustDays
		{
			get { return this.adjustDays; }
			set { this.adjustDays = value; }
		}

		/// <summary>
		/// ���ɵ�SQL�Ӿ�ı��ʽģ�塣Ĭ����${DataField}$ ${Operation}$ ${Data}$
		/// </summary>
		public string Template
		{
			get { return this.template; }
			set { this.template = value; }
		}

		/// <summary>
		/// �Ƿ���LIKE�Ӿ�ת���ַ����е�LIKE������
		/// </summary>
		public bool EscapeLikeString
		{
			get { return this.escapeLikeString; }
			set { this.escapeLikeString = value; }
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public sealed class SubConditionMappingAttribute : ConditionMappingAttribute
	{
		private string subPropertyName = string.Empty;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subPropertyName"></param>
		/// <param name="fieldName"></param>
		public SubConditionMappingAttribute(string subPropertyName, string fieldName)
			: base(fieldName)
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
