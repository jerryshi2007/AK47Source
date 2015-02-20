using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using MCS.Library.Data.Mapping;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 条件对象的映射属性
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
		/// 操作符，缺省为“=”
		/// </summary>
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}

		/// <summary>
		/// 是否是表达式
		/// </summary>
		public bool IsExpression
		{
			get { return this.isExpression; }
			set { this.isExpression = value; }
		}

		/// <summary>
		/// 数据字段的类型
		/// </summary>
		public string DataFieldName
		{
			get { return this.dataFieldName; }
			set { this.dataFieldName = value; }
		}

		/// <summary>
		/// 枚举类型的使用方法（值/还是描述）
		/// </summary>
		public EnumUsageTypes EnumUsage
		{
			get { return this.enumUsage; }
			set { this.enumUsage = value; }
		}

		/// <summary>
		/// 生成Value时的前缀
		/// </summary>
		public string Prefix
		{
			get { return this.prefix; }
			set { this.prefix = value; }
		}

		/// <summary>
		/// 生成Value时的后缀
		/// </summary>
		public string Postfix
		{
			get { return this.postfix; }
			set { this.postfix = value; }
		}

		/// <summary>
		/// 如果是日期型，需要调整天数。
		/// </summary>
		public double AdjustDays
		{
			get { return this.adjustDays; }
			set { this.adjustDays = value; }
		}

		/// <summary>
		/// 生成的SQL子句的表达式模板。默认是${DataField}$ ${Operation}$ ${Data}$
		/// </summary>
		public string Template
		{
			get { return this.template; }
			set { this.template = value; }
		}

		/// <summary>
		/// 是否按照LIKE子句转义字符串中的LIKE保留字
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
		/// 源对象的属性名称
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
