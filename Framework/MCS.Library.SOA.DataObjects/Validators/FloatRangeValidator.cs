using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// float����У����
	/// </summary>
    internal class FloatRangeValidator : Validator, IClientValidatable
	{
		private float lowerBound;
		private float upperBound;

		public FloatRangeValidator(float lowerBound, float upperBound)
			: base(string.Empty, string.Empty)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		public FloatRangeValidator(float lowerBound, float upperBound, string messageTemplate, string tag)
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
			float cValue;
			if (float.TryParse(objectToValidate.ToString(), out cValue) &&
				cValue.CompareTo(this.lowerBound) >= 0 && cValue.CompareTo(this.upperBound) <= 0)
			{
				isValid = true;
			}
			if (isValid == false)
				RecordValidationResult(validationResults, this.MessageTemplate, currentTarget, key);
		}

        /// <summary>
        /// �ͻ���У�麯������
        /// </summary>
        public string ClientValidateMethodName
        {
            get { return "RangeValidator"; }
        }

        /// <summary>
        /// ��ȡ�ͻ���У�鷽���ű�
        /// </summary>
        /// <returns></returns>
        public string GetClientValidateScript()
        {
            string scriptContent = MCS.Library.SOA.DataObjects.Properties.ScriptResources.RangeValidator;
            return scriptContent;
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
