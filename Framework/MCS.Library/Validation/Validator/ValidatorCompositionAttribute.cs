using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 附加在同一目标对象上的校验器之间的组合关系，分为And 和 Or
    /// </summary>
    /// <remarks>
    /// 附加在同一目标对象上的校验器之间的组合关系，分为And 和 Or
    /// And 的情况： 几个校验规则全部符合，校验通过
    /// Or 的情况：只要有一个校验规则符合情况，校验通过
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Class,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class ValidatorCompositionAttribute : BaseValidatorAttribute
    {
        private CompositionType compositionType;

        /// <summary>
        /// ValidatorCompositionAttribute 的构造函数
        /// </summary>
        /// <param name="compositionType">Validator的组合方式</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Customer.cs" region="CompositionValidatorUsage" lang="cs" title="如何添加组合型校验器属性"  ></code>
        /// </remarks>
        public ValidatorCompositionAttribute(CompositionType compositionType)
        {
            this.compositionType = compositionType;
        }

        /// <summary>
        /// 组合方式
        /// </summary>
        public  CompositionType CompositionType
        {
            get { return this.compositionType; }
        }
    }
}
