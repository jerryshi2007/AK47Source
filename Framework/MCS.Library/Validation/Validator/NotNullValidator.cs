using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	[Obsolete("该Validator已经被忽略，由ObjectNullValidator代替")]
    internal class NotNullValidator : Validator, IClientValidatable
    {
        public NotNullValidator() : base(string.Empty)
        { 
        }

        public NotNullValidator(string messageTemplate)
            : base(messageTemplate)
        {
        }

        protected internal override  void DoValidate(object objectToValidate,
                                                    object currentTarget,
                                                    string key,
                                                    ValidationResults validationResults)
        {
            if (objectToValidate == null)
            {
                RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
            }
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
            return Properties.ScriptResources.NotNullValidator;
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
