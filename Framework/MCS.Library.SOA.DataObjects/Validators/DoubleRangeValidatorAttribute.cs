using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
	/// <summary>
	/// doubleֵ��Χ�ж�У����������
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class DoubleRangeValidatorAttribute : ValidatorAttribute
	{
		private double lowerBound;
		private double upperBound;

		/// <summary>
		/// doubleֵ����
		/// </summary>
		public double LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// doubleֵ����
		/// </summary>
		public double UpperBound
		{
			get { return upperBound; }
		}

		/// <summary>
		/// IntegerRangeValidatorAttribute���캯��
		/// </summary>
		/// <param name="lowerBound">doubleֵ����</param>
		/// <param name="upperBound">doubleֵ����</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="���������η�ΧУ��������"  ></code>
		/// </remarks>
		public DoubleRangeValidatorAttribute(double lowerBound, double upperBound)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// ���ػ���ķ���������IntegerRangeValidator�Ĺ��췽ʽ����һ��IntegerRangeValidatorʵ��
		/// </summary>
		/// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
		/// <returns>IntegerRangeValidatorʵ��</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new DoubleRangeValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
		}
	}
}
