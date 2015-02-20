using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
    /// <summary>
    /// 对象集合的校验器，会依次调用对象集合中的对象进行校验
    /// </summary>
    internal class ObjectCollectionValidator : Validator
    {
        private Type targetType;
        private string targetRuleset;
        private Validator targetTypeValidator;

        /// <summary>
        /// ObjectCollectionValidator的构造函数
        /// </summary>
        /// <param name="targetType">目标类型</param>
        /// <param name="targetRuleset">所校验类型所属的校验规则集</param>
        public ObjectCollectionValidator(Type targetType, string targetRuleset)
        {
            this.targetType = targetType;
            this.targetRuleset = targetRuleset;
            this.targetTypeValidator = ValidationFactory.CreateValidator(targetType, targetRuleset);
        }

        protected internal override void DoValidate(
            object objectToValidate,
            object currentObject,
            string key,
            ValidationResults validateResults)
        {
            if (objectToValidate != null)
            {
                IEnumerable enumerableObjects = (IEnumerable)objectToValidate;

                if (enumerableObjects != null)
                {
                    foreach (object enumerableObject in enumerableObjects)
                    {
                        if (this.targetType.IsAssignableFrom(enumerableObject.GetType()))
                        {
                            this.targetTypeValidator.DoValidate(enumerableObject, enumerableObject,
                                key, validateResults);
                        }
                    }
                }
            }
        }
    }
}
