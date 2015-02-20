using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
	   Inherited = false)]
	public sealed class EnumDefaultValueValidatorAttribute : ValidatorAttribute
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new EnumDefaultValueValidator(this.MessageTemplate, this.Tag);
		}
	}
}
