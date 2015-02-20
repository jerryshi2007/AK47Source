using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 
	/// </summary>
	public class ValueAccessValidator : Validator
	{
		private ValueAccess valueAccess;
		private Validator innerValidator;
		private string key;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="valueAccess"></param>
		/// <param name="innerValidator"></param>
		public ValueAccessValidator(ValueAccess valueAccess, Validator innerValidator)
			: base(string.Empty, string.Empty)
		{
			this.valueAccess = valueAccess;
			this.innerValidator = innerValidator;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="valueAccess"></param>
		/// <param name="innerValidator"></param>
		/// <param name="key"></param>
		public ValueAccessValidator(ValueAccess valueAccess, Validator innerValidator, string key)
			: base(string.Empty, string.Empty)
		{
			this.valueAccess = valueAccess;
			this.innerValidator = innerValidator;
			this.key = key;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectToValidate"></param>
		/// <param name="currentTarget"></param>
		/// <param name="key"></param>
		/// <param name="validationResults"></param>
		protected internal override void DoValidate(object objectToValidate,
			object currentTarget,
			string key,
			ValidationResults validationResults)
		{
			if (objectToValidate != null)
			{
				if (this.key.IsNotEmpty())
					key = this.key;

				this.innerValidator.DoValidate(this.valueAccess.GetValue(objectToValidate), currentTarget, key, validationResults);
			}
		}
	}
}
