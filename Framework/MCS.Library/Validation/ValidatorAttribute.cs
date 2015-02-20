using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 自定义校验器属性的抽象基类　
    /// </summary>
    /// <remarks>
    /// 这个类间接从系统的自定义属性基类(Attribute)下派生
    /// 用于将用户定义的校验器自定义信息与目标元素相关联　
    /// 主要用来生成相应的校验器
    /// 目标元素为：类(Class) 、属性(Property)、字段(Field)
    /// 除本模块提供的一些基本校验器之外，用户自定义的校验器属性都需要实现这个抽象基类
    /// </remarks>
    public abstract class ValidatorAttribute : BaseValidatorAttribute
    {
        private Validator _validator = null;
       
        /// <summary>
        /// 获得校验器
        /// </summary>
        public Validator Validator
        {
            get { return this._validator; }
        }

        /// <summary>
        /// 使用该方法创建校验器
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="ownerType">源对象类型，目前在程序当中未做处理，留待以后扩展使用</param>
        /// <returns></returns>
        public Validator CreateValidator(Type targetType, Type ownerType)
        {
            if (this._validator == null)
            {
                this._validator = DoCreateValidator(targetType);
                this._validator.MessageTemplate = this.MessageTemplate;
                this._validator.Tag = this.Tag;
            }

            return this._validator;
        }

        /// <summary>
        /// 根据自定义的校验器属性创建一个校验器实例的抽象方法
        /// 在开发者编写自定义校验器的时候需要实现这个抽象方法
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <returns>创建好的检验器</returns>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\AddressValidatorAttribute.cs" region="HowToImplementedCreateValidator" lang="cs" title="如何实现校验器的创建"  ></code>
        /// </remarks>
        protected abstract Validator DoCreateValidator(Type targetType);

    }
}
