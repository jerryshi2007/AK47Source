using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// �Զ���У�������Եĳ������
    /// </summary>
    public abstract class BaseValidatorAttribute : Attribute
    {
        private string ruleset;
        private string messageTemplate;
        private string tag;

        /// <summary>
        /// У����򼯺�
        /// </summary>
        public string Ruleset
        {
            get
            {
                return this.ruleset != null ? this.ruleset : string.Empty;
            }
            set
            {
                this.ruleset = value;
            }
        }

        /// <summary>
        /// У�������ӵı�ǩ
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
        /// У�����ϸ��ӵ�У��ʧ����Ϣ
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

        /*
        public ValidatorAttribute(string messageTemplate)
        {
            this.messageTemplate = messageTemplate;
        }
        */
    }
}
