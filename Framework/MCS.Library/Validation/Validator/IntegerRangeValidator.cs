using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	internal class IntegerRangeValidator : Validator, IClientValidatable
	{
		private int lowerBound;
		private int upperBound;

		public IntegerRangeValidator(int lowerBound, int upperBound)
			: base(string.Empty, string.Empty)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		public IntegerRangeValidator(int lowerBound, int upperBound, string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		protected internal override void DoValidate(object objectToValidate,
			object currentTarget,
			string key,
			ValidationResults validationResults)
		{
			bool isValid = false;

			if (objectToValidate != null)
			{
				RangeChecker<int> checker = new RangeChecker<int>(this.lowerBound, this.upperBound);
				isValid = checker.IsInRange((int)objectToValidate);
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
			return Properties.ScriptResources.IntegerRangeValidator;
		}

		/// <summary>
		/// 获取客户端校验附加数据，比如正则表达式，范围值，等等
		/// </summary>
		/// <returns></returns>
		public Dictionary<string, object> GetClientValidateAdditionalData(object info)
		{
			return new Dictionary<string, object> {
								{"lowerBound", this.lowerBound},
								{"upperBound", this.upperBound}
							};
		}
	}
}
