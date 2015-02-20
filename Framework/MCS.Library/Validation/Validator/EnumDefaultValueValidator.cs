using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 枚举类型是否是缺省值的校验器（如果是，则相当于为空，报出错误）
	/// </summary>
	public class EnumDefaultValueValidator : Validator, IClientValidatable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageTemplate"></param>
		/// <param name="tag"></param>
		public EnumDefaultValueValidator(string messageTemplate, string tag) :
			base(messageTemplate, tag)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectToValidate"></param>
		/// <param name="currentObject"></param>
		/// <param name="key"></param>
		/// <param name="validateResults"></param>
		protected internal override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			bool isValid = true;

			if (objectToValidate != null && objectToValidate.GetType().IsEnum)
			{
				object defaultData = Activator.CreateInstance(objectToValidate.GetType());
				isValid = objectToValidate.Equals(defaultData) == false;
			}

			if (isValid == false)
				RecordValidationResult(validateResults, this.MessageTemplate, currentObject, key);
		}

		/// <summary>
		/// 客户端校验函数名称
		/// </summary>
		public string ClientValidateMethodName
		{
			get { return this.GetType().Name; }
		}

		/// <summary>
		/// 获取客户端校验方法脚本
		/// </summary>
		/// <returns></returns>
		public string GetClientValidateScript()
		{
			return Properties.ScriptResources.EnumDefaultValueValidator;
		}

		/// <summary>
		/// 获取客户端校验附加数据，比如正则表达式，范围值，等等
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> GetClientValidateAdditionalData(object info)
		{
			if (info is PropertyInfo)
			{
				PropertyInfo pi = info as PropertyInfo;
				if (pi != null && pi.PropertyType.IsEnum)
				{
					object defaultValue = Activator.CreateInstance(pi.PropertyType);
					this.Tag = ((int)defaultValue).ToString();
				}
			}
			else if (info is string)
			{
				this.Tag = info as string;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object> { { "tag", this.Tag } };
			return dictionary;
		}
	}
}
