using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 对象不为空的校验器属性类
    /// </summary>
    /// <remarks>
    /// 对象不为空的校验器属性类
    /// </remarks>
	[Obsolete("该Validator已经被忽略，由ObjectCollectionValidatorAttribute代替")]
    [AttributeUsage(AttributeTargets.Property
       | AttributeTargets.Field | AttributeTargets.Class,
       AllowMultiple = true,
       Inherited = false)]
    public sealed class NotNullValidatorAttribute : ValidatorAttribute
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="NotNullValidatorAttributeUsage" lang="cs" title="如何添加对象非空校验器属性"  ></code>
        /// </remarks>
        public NotNullValidatorAttribute() 
        {
        }

        /// <summary>
        /// 重载基类的方法，调用NotNullValidator的构造方式创建一个NotNullValidator实例
        /// </summary>
        /// <param name="targetType">目标类型，在本默认实现中未使用到该参数</param>
        /// <returns>NotNullValidator实例</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new NotNullValidator();
        }
       
    }
}
