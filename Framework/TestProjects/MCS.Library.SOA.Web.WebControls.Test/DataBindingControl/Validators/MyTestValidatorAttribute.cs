using System;
using System.Collections.Generic;
using System.Web;

using MCS.Library.Validation;
namespace MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.Validators
{
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field,
		AllowMultiple = true,
		Inherited = false)]
	public class MyTestValidatorAttribute : ValidatorAttribute, IClientValidatable
	{
		private string _clientValidateMethodName;

		public MyTestValidatorAttribute()
		{
		}

		public MyTestValidatorAttribute(string clientValidateMethodName)
		{
			_clientValidateMethodName = clientValidateMethodName;
		}

		private string GetDefaultClientMethodName()
		{
			return "myTestValidatorDoValidate";
		}

		protected override Validator DoCreateValidator(Type targetType)
		{
			return new MyTestValidator();
		}

		public string ClientValidateMethodName
		{
			get
			{
				if (!string.IsNullOrEmpty(_clientValidateMethodName))
				{
					return _clientValidateMethodName;
				}
				else
				{
					_clientValidateMethodName = GetDefaultClientMethodName();
					return _clientValidateMethodName;
				}
			}
		}

		public string GetClientValidateScript()
		{
			string scriptContent = ScriptResource.MyTestValidator;
			scriptContent = scriptContent.Replace("$$methodname", this._clientValidateMethodName);
			return scriptContent;
		}

		public Dictionary<string, object> GetClientValidateAdditionalData(object info)
		{
			throw new NotImplementedException();
		}
	}
}