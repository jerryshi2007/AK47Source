using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// У����������ࡣ
    /// �û����Ҫ�Զ���һ��У��������Ҫʵ��������ࡣ
    /// </summary>
    /// <remarks>
    /// ������ඨ����һ��У��������Ҫʵ�ֵ���������ܣ�У����У�鷽����У������μ�¼��
    /// </remarks>
    public abstract class Validator 
    {
        private string messageTemplate;
		private string tag;
		private string source;

        #region Constructor

        /// <summary>
        /// Validator���췽��
        /// </summary>
        protected Validator() : this(string.Empty, string.Empty)
        { 
        }
        
        /// <summary>
        /// Validator���췽��
        /// </summary>
        /// <param name="messageTemplate">У��δ�ɹ�����ʾ����Ϣ</param>
        protected Validator(string messageTemplate)
            : this(messageTemplate, string.Empty)
        {

        }

        /// <summary>
        /// Validator���췽��
        /// </summary>
        /// <param name="messageTemplate">У��δ�ɹ�����ʾ����Ϣ</param>
        /// <param name="tag">У������ǩ</param>
        protected Validator(string messageTemplate, string tag)
        {
            this.messageTemplate = messageTemplate;
            this.tag = tag;
        }

        #endregion

        /// <summary>
        /// У��δ�ɹ�����ʾ����Ϣ
        /// </summary>
        public string MessageTemplate
        {
            get
            {
                return this.messageTemplate;
            }
            set 
            { 
                this.messageTemplate = value; 
            }
        }

        /// <summary>
        /// У������ǩ
        /// </summary>
        public string Tag
        {
            get
            {
                return this.tag;
            }
            set 
            { 
                this.tag = value; 
            }
        }

		/// <summary>
		/// Validator����Դ��ͨ�������Property��Field�ϵ�Validator��ָ������������
		/// </summary>
		internal string Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

        #region
        /// <summary>
        /// ʹ��У��������У�鹤��
        /// </summary>
        /// <param name="validateObject">У�����</param>
        /// <returns>У����</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\ValidationFactoryTest.cs" region="UseValidatorToValidate" lang="cs" title="���ʹ��У��������У��" />
        /// </remarks>
        public ValidationResults Validate(object validateObject)
        {
            ValidationResults validationResults = new ValidationResults();

			Validate(validateObject, validationResults);

            return validationResults;
        }

		/// <summary>
		/// ʹ��У��������У�鹤�����������䵽validationResults��
		/// </summary>
		/// <param name="validateObject"></param>
		/// <param name="validationResults"></param>
		public void Validate(object validateObject, ValidationResults validationResults)
		{
			DoValidate(validateObject, validateObject, null, validationResults);
		}

        /// <summary>
        /// У������У���߼������
        /// </summary>
        /// <param name="objectToValidate">��У�����</param>
        /// <param name="currentObject">��ǰ����</param>
        /// <param name="key">��ǰУ��ı�ʶֵ</param>
        /// <param name="validateResults">У����</param>
        protected internal abstract void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults);

        /// <summary>
        /// ��У�����ڸ����Ľ�������н��м�¼
        /// </summary>
        /// <param name="validationResults">У������</param>
        /// <param name="message">У������Ϣ</param>
        /// <param name="target">��У�����</param>
        /// <param name="key">У������ʶֵ</param>
        protected void RecordValidationResult(ValidationResults validationResults, string message, object target, string key)
        {
            validationResults.AddResult(new ValidationResult(message, target, key, this.tag, this));
        }

        /// <summary>
        /// ��У�����ڸ����Ľ�������н��м�¼
        /// </summary>
        /// <param name="validationResults">У������</param>
        /// <param name="message">У������Ϣ</param>
        /// <param name="target">��У�����</param>
        /// <param name="key">У������ʶֵ</param>
        /// <param name="nestedValidationResults">Ƕ�׵Ľ���б�</param>
        protected void RecordValidationResult(ValidationResults validationResults, string message, object target, string key,
            IEnumerable<ValidationResult> nestedValidationResults)
        {
            validationResults.AddResult(new ValidationResult(message, target, key, this.Tag, this, nestedValidationResults));
        }
        #endregion
    }
}
