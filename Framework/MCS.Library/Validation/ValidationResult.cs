using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 校验器校验结果
    /// </summary>
    /// <remarks>
    /// 定义校验器校验结果的数据结构
    /// </remarks>
    [Serializable]
    public sealed class ValidationResult
    {
        private readonly string message;
        private readonly string key;
        private readonly string tag;

        [NonSerialized]
        private readonly object target;

        [NonSerialized]
        private readonly Validator validator;
        private readonly IEnumerable<ValidationResult> nestedValidationResults;
        private static readonly IEnumerable<ValidationResult> NoNestedValidationResults = new ValidationResult[0];

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">校验显示信息</param>
        /// <param name="target">校验对象</param>
        /// <param name="key">校验结果标识</param>
        /// <param name="tag">校验结果标记</param>
        /// <param name="validator">校验器</param>
        public ValidationResult(string message, object target, string key, string tag, Validator validator)
            : this(message, target, key, tag, validator, ValidationResult.NoNestedValidationResults)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">校验信息</param>
        /// <param name="target">校验对象</param>
        /// <param name="key">校验结果标识</param>
        /// <param name="tag">校验结果标记</param>
        /// <param name="validator">本次校验的校验器</param>
        /// <param name="nestedValidationResults">嵌套的校验结果（应用场景：或型的校验器，通常在这个结构内记录多个校验失败的信息）</param>
        public ValidationResult(string message, object target, string key, string tag, Validator validator,
            IEnumerable<ValidationResult> nestedValidationResults)
        {
            this.message = message;
            this.key = key;
            this.target = target;
            this.tag = tag;
            this.validator = validator;
            this.nestedValidationResults = nestedValidationResults;
        }

        /// <summary>
        /// 标识
        /// </summary>
        public string Key
        {
            get
            {
                return this.key;
            }
        }

        /// <summary>
        /// 校验信息
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>
        /// 标记
        /// </summary>
        public string Tag
        {
            get
            {
                return tag;
            }
        }

        /// <summary>
        /// 被校验对象
        /// </summary>
        public object Target
        {
            get
            {
                return this.target;
            }
        }

        /// <summary>
        /// 校验器
        /// </summary>
        public Validator Validator
        {
            get
            {
                return this.validator;
            }
        }

        /// <summary>
        /// 嵌套的校验结果
        /// </summary>
        public IEnumerable<ValidationResult> NestedValidationResults
        {
            get
            {
                return this.nestedValidationResults;
            }
        }
    }

    /// <summary>
    /// 校验结果枚举集合
    /// </summary>
    public sealed class ValidationResults : IEnumerable<ValidationResult>
    {
        private List<ValidationResult> validationResults;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ValidationResults()
        {
            this.validationResults = new List<ValidationResult>();
        }

        /// <summary>
        /// 该校验结果集合中存放的校验结果数目
        /// </summary>
        /// <returns></returns>
        public int ResultCount
        {
            get
            {
                return this.validationResults.Count;
            }
        }

        /// <summary>
        /// 向校验结果的枚举集合内添加校验结果对象
        /// </summary>
        /// <param name="result">校验结果对象</param>
        public void AddResult(ValidationResult result)
        {
            this.validationResults.Add(result);
        }

        /// <summary>
        /// 判断校验是否通过
        /// </summary>
        /// <returns>校验结果</returns>
        public bool IsValid()
        {
            return this.validationResults.Count == 0;
        }

        /// <summary>
        /// 将校验结果转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString("\n");
        }

        /// <summary>
        /// 将校验结果转换为字符串，中间以splitChars分割
        /// </summary>
        /// <param name="splitChars"></param>
        /// <returns></returns>
        public string ToString(string splitChars)
        {
            if (splitChars == null)
                splitChars = string.Empty;

            StringBuilder strB = new StringBuilder();

            foreach (ValidationResult result in this)
            {
                if (strB.Length > 0)
                    strB.Append(splitChars);

                strB.Append(result.Message);
            }

            return strB.ToString();
        }

        IEnumerator<ValidationResult> IEnumerable<ValidationResult>.GetEnumerator()
        {
            return this.validationResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.validationResults.GetEnumerator();
        }
    }
}
