using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MCS.Library.Validation
{
    internal class RegexValidator : Validator, IClientValidatable
    {
        private string pattern;
        private RegexOptions options;
     
      
        public RegexValidator(string pattern)
            : this(pattern, RegexOptions.None)
        { }

     
        public RegexValidator(string pattern, RegexOptions options)
            : this(pattern, options, string.Empty)
        { }

        public RegexValidator(string pattern, string messageTemplate)
            : this(pattern, RegexOptions.None, messageTemplate)
        { }

      
        public RegexValidator(string pattern, RegexOptions options, string messageTemplate)
           : this(pattern, options, messageTemplate, string.Empty)
        { }

       
        public RegexValidator(string pattern, RegexOptions options, string messageTemplate, string tag)
            : base(messageTemplate, tag)
        {
            this.pattern = pattern;
            this.options = options;
        }

        protected internal override void DoValidate(object objectToValidate,
                                                   object currentTarget,
                                                   string key,
                                                   ValidationResults validationResults)
        {
            bool matchFlag = false;


            if (objectToValidate != null && !string.IsNullOrEmpty(this.pattern))
            {
                Regex regex = new Regex(this.pattern, this.options);
                matchFlag = regex.IsMatch(objectToValidate.ToString());
            }

            if (objectToValidate == null || matchFlag == false)
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
            return Properties.ScriptResources.RegexValidator;
        }

        /// <summary>
        /// 获取客户端校验附加数据，比如正则表达式，范围值，等等
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetClientValidateAdditionalData(object info)
        {
            bool isNumber = false;
            PropertyInfo pi = info as PropertyInfo;
            if (pi != null)
            {
                var pType = pi.PropertyType;
                if (pType == typeof(System.Decimal) || pType == typeof(System.Int32) ||
                    pType == typeof(System.Double) || pType == typeof(System.Single))
                {
                    isNumber = true;
                }
            }
            Dictionary<string, object> dicData = new Dictionary<string, object>
                                                     {
                                                         {"isNumber", isNumber},
                                                         {"pattern", this.pattern}
                                                     };

            return dicData;
        }
    }
}
