using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// ����У��������
    /// </summary>
    /// <remarks>
    /// ����У���������࣬�����ڶ������͵�Property��Field���档
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class ObjectValidatorAttribute : ValidatorAttribute
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Customer.cs" region="ObjectValidatorAttributeUsage" lang="cs" title="�����Ӷ���У��������"  ></code>
        /// </remarks>
        public ObjectValidatorAttribute()
        {
        }

        /// <summary>
        /// ���ػ���ķ���������ValidationFactoryֱ�ӹ���Ŀ�����͵�У����
        /// </summary>
        /// <param name="targetType">Ŀ������</param>
        /// <returns>Ŀ�����͵�Validatorʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return ValidationFactory.CreateValidator(targetType, this.Ruleset);
        }
    }
}
