using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	///
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true)]
	public sealed class ObjectNullValidatorAttribute : ValidatorAttribute
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new ObjectNullValidator(this.MessageTemplate, this.Tag);
		}
    }
}
