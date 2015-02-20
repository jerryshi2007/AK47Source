using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// ʱ��У����
	/// </summary>
    internal class DateTimeRangeValidator : Validator, IClientValidatable
	{
		private DateTime lowerBound;
		private DateTime upperBound;

		/// <summary>
		/// ����
		/// </summary>
		public DateTime LowerBound
		{
			get
			{
				return lowerBound;
			}
		}

		/// <summary>
		/// ����
		/// </summary>
		public DateTime UpperBound
		{
			get
			{
				return upperBound;
			}
		}

		/// <summary>
		/// ��ʼ��
		/// </summary>
        /// <param name="lowerBound">����</param>
        /// <param name="upperBound">����</param>
        /// <param name="messageTemplate">У��δ�ɹ�����ʾ����Ϣ</param>
        /// <param name="tag">У������ǩ</param>
		public DateTimeRangeValidator(DateTime lowerBound, DateTime upperBound, string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
			this.lowerBound = lowerBound;
			this.upperBound = upperBound;
		}

		/// <summary>
		/// У������
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
            return Properties.ScriptResources.DateTimeRangeValidator; 
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
