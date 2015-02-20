using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 字符串长度构造器属性类，校验字符串长度是否在指定的范围内
    /// </summary>
    /// <remarks>
    /// 字符串长度构造器属性类，校验字符串长度是否在指定的范围内
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class StringLengthValidatorAttribute : ValidatorAttribute
    {
        private string clientValidateMethodName;
        private int lowerBound;
        private int upperBound;

        /// <summary>
        /// 区间的最小值
        /// </summary>
        public int LowerBound
        {
            get { return lowerBound; }
        }

        /// <summary>
        /// 区间最大值
        /// </summary>
        public int UpperBound
        {
            get { return upperBound; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="upperBound">字符串长度上限</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="StringLengthValidatorAttributeDemo" lang="cs" title="如何添加字符串长度校验器属性"  ></code>
        /// </remarks>
        public StringLengthValidatorAttribute(int upperBound)
            : this(0, upperBound)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lowerBound">字符串长度下限</param>
        /// <param name="upperBound">字符串长度上限</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="StringLengthValidatorAttributeDemo2" lang="cs" title="如何添加字符串长度校验器属性"  ></code>
        /// </remarks>
        public StringLengthValidatorAttribute(int lowerBound, int upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="methodName">客户端校验方法名称</param>
        /// <param name="lowerBound">下限</param>
        /// <param name="upperBound">上限</param>
        public StringLengthValidatorAttribute(string methodName, int lowerBound, int upperBound)
            : this(lowerBound, upperBound)
        {
            this.clientValidateMethodName = methodName;
        }

        /// <summary>
        /// 重载基类的方法，调用StringLengthValidator构造方法创建StringLengthValidator
        /// </summary>
        /// <param name="targetType">目标类型，在本默认实现中未使用到该参数</param>
        /// <returns>字符串长度校验器实例</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new StringLengthValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
        }
    }
}
