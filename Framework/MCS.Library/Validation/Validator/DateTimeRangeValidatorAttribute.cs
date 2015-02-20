using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 时间校验属性
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
		/// 下限
		/// </summary>
		public DateTime LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// 上限
		/// </summary>
		public DateTime UpperBound
		{
			get
			{
				return upperBound;
			}
		}

       

		/// <summary>
		/// 初始化
		/// </summary>
        /// <param name="lowerBound">下限</param>
        /// <param name="upperBound">上限</param>
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
