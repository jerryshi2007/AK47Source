using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 校验器抽象基类。
    /// 用户如果要自定义一个校验器，需要实现这个基类。
    /// </summary>
    /// <remarks>
    /// 这个基类定义了一个校验器所需要实现的最基本功能：校验器校验方法和校验结果如何记录。
    /// </remarks>
    public abstract class Validator 
    {
        private string messageTemplate;
		private string tag;
		private string source;

        #region Constructor

        /// <summary>
        /// Validator构造方法
        /// </summary>
        protected Validator() : this(string.Empty, string.Empty)
        { 
        }
        
        /// <summary>
        /// Validator构造方法
        /// </summary>
        /// <param name="messageTemplate">校验未成功所提示的信息</param>
        protected Validator(string messageTemplate)
            : this(messageTemplate, string.Empty)
        {

        }

        /// <summary>
        /// Validator构造方法
        /// </summary>
        /// <param name="messageTemplate">校验未成功所提示的信息</param>
        /// <param name="tag">校验器标签</param>
        protected Validator(string messageTemplate, string tag)
        {
            this.messageTemplate = messageTemplate;
            this.tag = tag;
        }

        #endregion

        /// <summary>
        /// 校验未成功所提示的信息
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
        /// 校验器标签
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
		/// Validator的来源，通常针对于Property或Field上的Validator，指定了属性名称
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
        /// 使用校验器进行校验工作
        /// </summary>
        /// <param name="validateObject">校验对象</param>
        /// <returns>校验结果</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\ValidationFactoryTest.cs" region="UseValidatorToValidate" lang="cs" title="如何使用校验器进行校验" />
        /// </remarks>
        public ValidationResults Validate(object validateObject)
        {
            ValidationResults validationResults = new ValidationResults();

			Validate(validateObject, validationResults);

            return validationResults;
        }

		/// <summary>
		/// 使用校验器进行校验工作，将结果填充到validationResults中
		/// </summary>
		/// <param name="validateObject"></param>
		/// <param name="validationResults"></param>
		public void Validate(object validateObject, ValidationResults validationResults)
		{
			DoValidate(validateObject, validateObject, null, validationResults);
		}

        /// <summary>
        /// 校验器的校验逻辑与规则
        /// </summary>
        /// <param name="objectToValidate">被校验对象</param>
        /// <param name="currentObject">当前对象</param>
        /// <param name="key">当前校验的标识值</param>
        /// <param name="validateResults">校验结果</param>
        protected internal abstract void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults);

        /// <summary>
        /// 将校验结果在给定的结果集合中进行记录
        /// </summary>
        /// <param name="validationResults">校验结果集</param>
        /// <param name="message">校验结果信息</param>
        /// <param name="target">被校验对象</param>
        /// <param name="key">校验结果标识值</param>
        protected void RecordValidationResult(ValidationResults validationResults, string message, object target, string key)
        {
            validationResults.AddResult(new ValidationResult(message, target, key, this.tag, this));
        }

        /// <summary>
        /// 将校验结果在给定的结果集合中进行记录
        /// </summary>
        /// <param name="validationResults">校验结果集</param>
        /// <param name="message">校验结果信息</param>
        /// <param name="target">被校验对象</param>
        /// <param name="key">校验结果标识值</param>
        /// <param name="nestedValidationResults">嵌套的结果列表</param>
        protected void RecordValidationResult(ValidationResults validationResults, string message, object target, string key,
            IEnumerable<ValidationResult> nestedValidationResults)
        {
            validationResults.AddResult(new ValidationResult(message, target, key, this.Tag, this, nestedValidationResults));
        }
        #endregion
    }
}
