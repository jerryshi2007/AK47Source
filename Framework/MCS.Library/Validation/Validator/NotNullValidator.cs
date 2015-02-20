using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	[Obsolete("��Validator�Ѿ������ԣ���ObjectNullValidator����")]
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
            return Properties.ScriptResources.NotNullValidator;
        }

        /// <summary>
        /// ��ȡ�ͻ���У�鸽�����ݣ�����������ʽ����Χֵ���ȵ�
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetClientValidateAdditionalData(object info)
        {
            return null;
        }
    }
}
