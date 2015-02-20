using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	internal class StringLengthValidator : Validator, IClientValidatable
	{
		private int lowerBound;
		private int upperBound;

		public StringLengthValidator(int upperBound)
			: this(0, upperBound)
		{
		}

		public StringLengthValidator(int lowerBound, int upperBound)
			: this(lowerBound, upperBound, string.Empty, string.Empty)
		{
		}

		public StringLengthValidator(int lowerBound, int upperBound, string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		protected override internal void DoValidate(object objectToValidate,
			object currentTarget,
			string key,
			ValidationResults validationResults)
		{
			bool isValid = false;

			if (objectToValidate != null)
			{
				RangeChecker<int> checker = new RangeChecker<int>(this.lowerBound, this.upperBound);
				isValid = checker.IsInRange(objectToValidate.ToString().Length);
			}
			else
				isValid = (this.lowerBound <= 0);

			if (isValid == false)
				RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
		}

		/// <summary>
		/// �ͻ���У�麯������
		/// </summary>
		public string ClientValidateMethodName
		{
			get { return this.GetType().Name; }
		}

		/// <summary>
		/// ��ȡ�ͻ���У�鷽���ű�
		/// </summary>
		/// <returns></returns>
		public string GetClientValidateScript()
		{
			return Properties.ScriptResources.StringLengthValidator;
		}

		/// <summary>
		/// ��ȡ�ͻ���У�鸽�����ݣ�����������ʽ����Χֵ���ȵ�
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
