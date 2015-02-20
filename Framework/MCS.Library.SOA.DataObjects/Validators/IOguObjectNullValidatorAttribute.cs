using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// IOguObject对象为空的校验属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public class IOguObjectNullValidatorAttribute : ValidatorAttribute
	{
		protected override Validator DoCreateValidator(Type targetType)
		{
            return new IOguObjectNullValidator(this.MessageTemplate, this.Tag);
		}
	}
}
