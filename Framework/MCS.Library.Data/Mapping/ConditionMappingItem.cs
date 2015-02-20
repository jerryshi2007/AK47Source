using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 条件表达式和对象属性的映射关系
	/// </summary>
	[DebuggerDisplay("PropertyName={propertyName}")]
	public class ConditionMappingItem
	{
		private string propertyName = string.Empty;
		private string subClassTypeDescription = string.Empty;
		private string subClassPropertyName = string.Empty;
		private string dataFieldName = string.Empty;
		private bool isExpression = false;
		private string operation = SqlClauseBuilderBase.EqualTo;
		private string prefix = string.Empty;
		private string postfix = string.Empty;
		private double adjustDays = 0;
		private string template = string.Empty;
		private bool escapeLikeString = false;
		private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;
		private MemberInfo memberInfo = null;

		/// <summary>
		/// 对象的属性名称
		/// </summary>
		public string PropertyName
		{
			get { return this.propertyName; }
			set { this.propertyName = value; }
		}

		/// <summary>
		/// 对应的子对象的类型描述
		/// </summary>
		public string SubClassTypeDescription
		{
			get { return this.subClassTypeDescription; }
			set { this.subClassTypeDescription = value; }
		}

		/// <summary>
		/// 子对象的属性名称
		/// </summary>
		public string SubClassPropertyName
		{
			get { return this.subClassPropertyName; }
			set { this.subClassPropertyName = value; }
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
		/// 对应的成员对象信息
		/// </summary>
		public MemberInfo MemberInfo
		{
			get { return this.memberInfo; }
			internal set { this.memberInfo = value; }
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
}
