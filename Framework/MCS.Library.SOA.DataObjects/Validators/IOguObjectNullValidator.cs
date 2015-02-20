using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.Validation
{
    internal class IOguObjectNullValidator : Validator, IClientValidatable
	{

        public IOguObjectNullValidator(string messageTemplate, string tag)
            : base(messageTemplate, tag)
        {

        }

        protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
            if (objectToValidate == null || OguBase.IsNullOrEmpty((IOguObject)objectToValidate))
            {
                RecordValidationResult(validateResults, this.MessageTemplate, currentObject, key);
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
            string scriptContent = MCS.Library.SOA.DataObjects.Properties.ScriptResources.IOguObjectNullValidator;
            return scriptContent;
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
