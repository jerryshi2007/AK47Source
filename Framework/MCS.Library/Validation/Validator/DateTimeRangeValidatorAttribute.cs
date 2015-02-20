using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
	/// <summary>
	/// ʱ��У������
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
   | AttributeTargets.Field,
   AllowMultiple = true,
  Inherited = false)]
	public sealed class DateTimeRangeValidatorAttribute : ValidatorAttribute
	{

		private DateTime lowerBound;
		private DateTime upperBound;

		/// <summary>
		/// ����
		/// </summary>
		public DateTime LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		public DateTime UpperBound
		{
			get
			{
				return upperBound;
			}
		}

       

		/// <summary>
		/// ��ʼ��
		/// </summary>
        /// <param name="lowerBound">����</param>
        /// <param name="upperBound">����</param>
		public DateTimeRangeValidatorAttribute(string lowerBound, string upperBound)
		{
			this.lowerBound = Convert.ToDateTime(lowerBound);
			this.upperBound = Convert.ToDateTime(upperBound);
		}

	    /// <summary>
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new DateTimeRangeValidator(lowerBound, upperBound, this.MessageTemplate, this.Tag);
		}
    }
}
