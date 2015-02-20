using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 日期类型的为空判断的校验器
	/// </summary>
    public class DateTimeEmptyValidator : Validator, IClientValidatable
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="messageTemplate"></param>
		/// <param name="tag"></param>
		public DateTimeEmptyValidator(string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectToValidate"></param>
		/// <param name="currentObject"></param>
		/// <param name="key"></param>
		/// <param name="validationResults"></param>
		protected internal override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validationResults)
		{
			if ((DateTime)objectToValidate == DateTime.MinValue)
				RecordValidationResult(validationResults, this.MessageTemplate, currentObject, key);
		}

        /// <summary>
        /// 客户端校验函数名称
        /// </summary>
        public string ClientValidateMethodName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// 获取客户端校验方法脚本
        /// </summary>
        /// <returns></returns>
        public string GetClientValidateScript()
        {
            return Properties.ScriptResources.DatetimeEmptyValidator;;
        }

        /// <summary>
        /// 获取客户端校验附加数据，比如正则表达式，范围值，等等
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetClientValidateAdditionalData(object info)
        {
            return null;
        }
    }
}
