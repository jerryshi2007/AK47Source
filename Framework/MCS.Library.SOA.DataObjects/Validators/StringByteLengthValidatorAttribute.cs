using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MCS.Library.SOA.DataObjects.Properties;

namespace MCS.Library.Validation
{
	/// <summary>
	/// StringByteLengthValidatorAttributeֵ��Χ�ж�У����������
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class StringByteLengthValidatorAttribute : ValidatorAttribute
	{
		private int lowerBound;
		private int upperBound;

		/// <summary>
		/// intֵ����
		/// </summary>
		public int LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// intֵ����
		/// </summary>
		public int UpperBound
		{
			get { return upperBound; }
		}

		/// <summary>
		/// StringByteLengthValidatorAttribute���캯��
		/// </summary>
		/// <param name="lowerBound">doubleֵ����</param>
		/// <param name="upperBound">doubleֵ����</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="���������η�ΧУ��������"  ></code>
		/// </remarks>
		public StringByteLengthValidatorAttribute(int lowerBound, int upperBound)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// ���ػ���ķ���������StringByteLengthValidatorAttribute�Ĺ��췽ʽ����һ��StringByteLengthValidatorʵ��
		/// </summary>
		/// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
		/// <returns>IntegerRangeValidatorʵ��</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new StringByteLengthValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
		}
	}
}
