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
	/// �������ʽ�Ͷ������Ե�ӳ���ϵ
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
		/// �������������
		/// </summary>
		public string PropertyName
		{
			get { return this.propertyName; }
			set { this.propertyName = value; }
		}

		/// <summary>
		/// ��Ӧ���Ӷ������������
		/// </summary>
		public string SubClassTypeDescription
		{
			get { return this.subClassTypeDescription; }
			set { this.subClassTypeDescription = value; }
		}

		/// <summary>
		/// �Ӷ������������
		/// </summary>
		public string SubClassPropertyName
		{
			get { return this.subClassPropertyName; }
			set { this.subClassPropertyName = value; }
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
		/// ��Ӧ�ĳ�Ա������Ϣ
		/// </summary>
		public MemberInfo MemberInfo
		{
			get { return this.memberInfo; }
			internal set { this.memberInfo = value; }
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
}
