using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
	/// <summary>
	/// float值范围判断校验器属性类
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
		/// float值下限
		/// </summary>
		public float LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// float值上限
		/// </summary>
		public float UpperBound
		{
			get { return upperBound; }
		}

		/// <summary>
		/// IntegerRangeValidatorAttribute构造函数
		/// </summary>
		/// <param name="lowerBound">float值下限</param>
		/// <param name="upperBound">float值上限</param>
		/// <remarks>
		/// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="如何添加整形范围校验器属性"  ></code>
		/// </remarks>
		public FloatRangeValidatorAttribute(float lowerBound, float upperBound)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// 重载基类的方法，调用IntegerRangeValidator的构造方式创建一个IntegerRangeValidator实例
		/// </summary>
		/// <param name="targetType">目标类型，在本默认实现中未使用到该参数</param>
		/// <returns>IntegerRangeValidator实例</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new FloatRangeValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
		}
	}
}
