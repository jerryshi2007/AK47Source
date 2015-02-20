using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
	/// <summary>
	/// floatֵ��Χ�ж�У����������
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class FloatRangeValidatorAttribute : ValidatorAttribute
	{
		private float lowerBound;
		private float upperBound;

		/// <summary>
		/// floatֵ����
		/// </summary>
		public float LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// floatֵ����
		/// </summary>
		public float UpperBound
		{
			get { return upperBound; }
		}

		/// <summary>
		/// IntegerRangeValidatorAttribute���캯��
		/// </summary>
		/// <param name="lowerBound">floatֵ����</param>
		/// <param name="upperBound">floatֵ����</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="���������η�ΧУ��������"  ></code>
		/// </remarks>
		public FloatRangeValidatorAttribute(float lowerBound, float upperBound)
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
			return new FloatRangeValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
		}
	}
}
