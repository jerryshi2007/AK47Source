﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// decimal类型校验器
	/// </summary>
    internal class DecimalRangeValidator : Validator, IClientValidatable
    {

        private decimal lowerBound;
        private decimal upperBound;

        public DecimalRangeValidator(decimal lowerBound, decimal upperBound)
            : base(string.Empty, string.Empty)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        public DecimalRangeValidator(decimal lowerBound, decimal upperBound, string messageTemplate, string tag)
            : base(messageTemplate, tag)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        protected override void DoValidate(object objectToValidate,
            object currentTarget,
            string key,
            ValidationResults validationResults)
        {
            bool isValid = false;
            decimal cValue;
            decimal.TryParse(objectToValidate.ToString(), out cValue);
            if (cValue.CompareTo(this.lowerBound) >= 0 && cValue.CompareTo(this.upperBound) <= 0)
            {
                isValid = true;
            }
            if (isValid == false)
                RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
        }

        /// <summary>
        /// 客户端校验函数名称
        /// </summary>
        public string ClientValidateMethodName
        {
            get { return "RangeValidator"; }
        }

        /// <summary>
        /// 获取客户端校验方法脚本
        /// </summary>
        /// <returns></returns>
        public string GetClientValidateScript()
        {
            string scriptContent = MCS.Library.SOA.DataObjects.Properties.ScriptResources.RangeValidator;
            return scriptContent;
        }

        /// <summary>
        /// 获取客户端校验附加数据，比如正则表达式，范围值，等等
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetClientValidateAdditionalData(object info)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>
                                                        {
                                                            {"lowerBound", this.lowerBound},
                                                            {"upperBound", this.upperBound}
                                                        };
            return dictionary;
        }
    }
}
