using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 字符串为空校验构造器属性类，校验字符串是否为空
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public sealed class StringEmptyValidatorAttribute : ValidatorAttribute
	{
		/// <summary>
		/// 创建校验器
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new StringEmptyValidator(this.MessageTemplate, this.Tag); 
		}
	}
}
