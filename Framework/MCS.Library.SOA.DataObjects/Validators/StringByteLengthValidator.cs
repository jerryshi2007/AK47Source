using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.SOA.DataObjects.Properties;
using MCS.Library.Validation;

namespace MCS.Library.Validation
{
	internal class StringByteLengthValidator : Validator, IClientValidatable
	{
		private int lowerBound;
		private int upperBound;

		public StringByteLengthValidator(int upperBound)
			: this(0, upperBound)
		{
		}

		public StringByteLengthValidator(int lowerBound, int upperBound)
			: this(lowerBound, upperBound, string.Empty, string.Empty)
		{
		}

		public StringByteLengthValidator(int lowerBound, int upperBound, string messageTemplate, string tag)
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

			if (objectToValidate != null)
			{
				RangeChecker<int> checker = new RangeChecker<int>(this.lowerBound, this.upperBound);
				isValid = checker.IsInRange(Encoding.Default.GetByteCount(objectToValidate.ToString()));
			}

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
			return ScriptResources.StringByteLengthValidator;
		}

		/// <summary>
		/// ��ȡ�ͻ���У�鸽�����ݣ�����������ʽ����Χֵ���ȵ�
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
