using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MCS.Library.SOA.DataObjects.Properties;

namespace MCS.Library.Validation
{
	/// <summary>
	/// StringByteLengthValidatorAttribute值范围判断校验器属性类
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
		/// int值下限
		/// </summary>
		public int LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// int值上限
		/// </summary>
		public int UpperBound
		{
			get { return upperBound; }
		}

		/// <summary>
		/// StringByteLengthValidatorAttribute构造函数
		/// </summary>
		/// <param name="lowerBound">double值下限</param>
		/// <param name="upperBound">double值上限</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="如何添加整形范围校验器属性"  ></code>
		/// </remarks>
		public StringByteLengthValidatorAttribute(int lowerBound, int upperBound)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// 重载基类的方法，调用StringByteLengthValidatorAttribute的构造方式创建一个StringByteLengthValidator实例
		/// </summary>
		/// <param name="targetType">目标类型，在本默认实现中未使用到该参数</param>
		/// <returns>IntegerRangeValidator实例</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new StringByteLengthValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
		}
	}
}
