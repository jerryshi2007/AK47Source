using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 正则表达式校验器属性类，校验被校验对象是否匹配正则表达式
    /// </summary>
    /// <remarks>
    /// 正则表达式校验器属性类，校验被校验对象是否匹配正则表达式
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class RegexValidatorAttribute : ValidatorAttribute
    {
        private string pattern;
        private RegexOptions options;

        /// <summary>
        /// 表达式参数
        /// </summary>
        public string Pattern
        {
            get
            {
                return pattern;
            }
        }
        /// <summary>
        /// 认证选项
        /// </summary>
        public RegexOptions Options
        {
            get { return options; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Address.cs" region="RegexValidatorAttributeUsage" lang="cs" title="如何添加正则表达式校验器属性"  ></code>
        /// </remarks>
        public RegexValidatorAttribute(string pattern)
            : this(pattern, RegexOptions.None)
        {

        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <param name="options">正则表达式选项枚举值</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="RegexValidatorAttributeUsage2" lang="cs" title="如何添加正则表达式校验器属性"  ></code>
        /// </remarks>
        public RegexValidatorAttribute(string pattern, RegexOptions options)
        {
            this.pattern = pattern;
            this.options = options;

        }

        /// <summary>
        /// 重载基类的方法，调用RegexValidator的构造方法直接构造RegexValidator校验器
        /// </summary>
        /// <param name="targetType">目标类型，在本默认实现中未使用到该参数</param>
        /// <returns>RegexValidator实例</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new RegexValidator(this.pattern, this.options, this.MessageTemplate, this.Tag);
        }
    }
}
