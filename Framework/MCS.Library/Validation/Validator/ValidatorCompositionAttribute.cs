using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// ������ͬһĿ������ϵ�У����֮�����Ϲ�ϵ����ΪAnd �� Or
    /// </summary>
    /// <remarks>
    /// ������ͬһĿ������ϵ�У����֮�����Ϲ�ϵ����ΪAnd �� Or
    /// And ������� ����У�����ȫ�����ϣ�У��ͨ��
    /// Or �������ֻҪ��һ��У�������������У��ͨ��
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
        /// ValidatorCompositionAttribute �Ĺ��캯��
        /// </summary>
        /// <param name="compositionType">Validator����Ϸ�ʽ</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Customer.cs" region="CompositionValidatorUsage" lang="cs" title="�����������У��������"  ></code>
        /// </remarks>
        public ValidatorCompositionAttribute(CompositionType compositionType)
        {
            this.compositionType = compositionType;
        }

        /// <summary>
        /// ��Ϸ�ʽ
        /// </summary>
        public  CompositionType CompositionType
        {
            get { return this.compositionType; }
        }
    }
}
