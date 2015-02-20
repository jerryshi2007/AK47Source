using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 时间校验器
	/// </summary>
    internal class DateTimeRangeValidator : Validator, IClientValidatable
	{
		private DateTime lowerBound;
		private DateTime upperBound;

		/// <summary>
		/// 下限
		/// </summary>
		public DateTime LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// 上限
		/// </summary>
		public DateTime UpperBound
		{
			get
			{
				return upperBound;
			}
		}

		/// <summary>
		/// 初始化
		/// </summary>
        /// <param name="lowerBound">下限</param>
        /// <param name="upperBound">上限</param>
        /// <param name="messageTemplate">校验未成功所提示的信息</param>
        /// <param name="tag">校验器标签</param>
		public DateTimeRangeValidator(DateTime lowerBound, DateTime upperBound, string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// 校验数据
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
			bool isValid = false;

			if (objectToValidate != null)
			{
				RangeChecker<DateTime> checker = new RangeChecker<DateTime>(this.lowerBound, this.upperBound);
				isValid = checker.IsInRange((DateTime)objectToValidate);
			}

			if (isValid == false)
				RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
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
            return Properties.ScriptResources.DateTimeRangeValidator; 
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
