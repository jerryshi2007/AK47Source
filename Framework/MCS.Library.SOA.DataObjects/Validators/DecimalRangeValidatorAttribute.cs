using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
	/// <summary>
	/// decimal值范围判断校验器属性类
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class DecimalRangeValidatorAttribute : ValidatorAttribute
	{
		private decimal lowerBound;
		private decimal upperBound;

		/// <summary>
		/// decimal值下限
		/// </summary>
		public decimal LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// decimal值上限
		/// </summary>
		public decimal UpperBound
		{
			get { return upperBound; }
		}

		/// <summary>
		/// DecimalRangeValidatorAttribute构造函数
		/// </summary>
		/// <param name="lowerBound">decimal值下限</param>
		/// <param name="upperBound">decimal值上限</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="如何添加整形范围校验器属性"  ></code>
		/// </remarks>
		public DecimalRangeValidatorAttribute(decimal lowerBound, decimal upperBound)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// DecimalRangeValidatorAttribute构造函数
		/// </summary>
		/// <param name="dLowerBound">decimal值下限</param>
		/// <param name="dUpperBound">decimal值上限</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="如何添加整形范围校验器属性"  ></code>
		/// </remarks>
		public DecimalRangeValidatorAttribute(double dLowerBound, double dUpperBound)
		{
			this.lowerBound = Convert.ToDecimal(dLowerBound);
			this.upperBound = Convert.ToDecimal(dUpperBound);
		}

		/// <summary>
		/// DecimalRangeValidatorAttribute构造函数
		/// </summary>
		/// <param name="nLowerBound">decimal值下限</param>
		/// <param name="nUpperBound">decimal值上限</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="如何添加整形范围校验器属性"  ></code>
		/// </remarks>
		public DecimalRangeValidatorAttribute(int nLowerBound, int nUpperBound)
		{
			this.lowerBound = Convert.ToDecimal(nLowerBound);
			this.upperBound = Convert.ToDecimal(nUpperBound);
		}

		/// <summary>
		/// 重载基类的方法，调用IntegerRangeValidator的构造方式创建一个IntegerRangeValidator实例
		/// </summary>
		/// <param name="targetType">目标类型，在本默认实现中未使用到该参数</param>
		/// <returns>IntegerRangeValidator实例</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new DecimalRangeValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
		}
	}
}
