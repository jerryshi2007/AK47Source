using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 对象校验器属性
    /// </summary>
    /// <remarks>
    /// 对象校验器属性类，附加在对象类型的Property和Field上面。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class ObjectValidatorAttribute : ValidatorAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Customer.cs" region="ObjectValidatorAttributeUsage" lang="cs" title="如何添加对象校验器属性"  ></code>
        /// </remarks>
        public ObjectValidatorAttribute()
        {
        }

        /// <summary>
        /// 重载基类的方法，调用ValidationFactory直接构造目标类型的校验器
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <returns>目标类型的Validator实例</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return ValidationFactory.CreateValidator(targetType, this.Ruleset);
        }
    }
}
