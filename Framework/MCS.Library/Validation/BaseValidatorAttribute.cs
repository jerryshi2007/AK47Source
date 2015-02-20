using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 自定义校验器属性的抽象基类
    /// </summary>
    public abstract class BaseValidatorAttribute : Attribute
    {
        private string ruleset;
        private string messageTemplate;
        private string tag;

        /// <summary>
        /// 校验规则集合
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
        /// 校验器附加的标签
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
        /// 校验器上附加的校验失败信息
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
