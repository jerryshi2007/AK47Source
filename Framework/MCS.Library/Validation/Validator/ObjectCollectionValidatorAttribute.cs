using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 对象集合校验器属性类
    /// </summary>
    /// <remarks>
    /// 对象集合校验器属性类
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class ObjectCollectionValidatorAttribute : ValidatorAttribute
    {
        private string targetRuleset;
        private Type targetType;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="targetType">目标类型</param>
        public ObjectCollectionValidatorAttribute(Type targetType)
            : this(targetType, string.Empty)
        { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="targetRuleset">校验器规则集合</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Customer.cs" region="ObjectCollectionValidatorAttributeUsage" lang="cs" title="如何添加对象集合校验器属性"  ></code>
        /// </remarks>
        public ObjectCollectionValidatorAttribute(Type targetType, string targetRuleset)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            this.targetType = targetType;
            this.targetRuleset = targetRuleset;
        }

        /// <summary>
        /// 重载基类的方法，调用ObjectCollectionValidator的构造方式创建一个ObjectCollectionValidator实例
        /// </summary>
        /// <param name="targetType">目标类型，，在本默认实现中未使用到该参数</param>
        /// <returns>ObjectCollectionValidator实例</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new ObjectCollectionValidator(this.targetType, 
                this.targetRuleset = (string.IsNullOrEmpty(this.targetRuleset)) ? this.Ruleset : this.targetRuleset);   
          
        }
    }
}
