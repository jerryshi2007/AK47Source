using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// ����Ϊ�յ�У����������
    /// </summary>
    /// <remarks>
    /// ����Ϊ�յ�У����������
    /// </remarks>
	[Obsolete("��Validator�Ѿ������ԣ���ObjectCollectionValidatorAttribute����")]
    [AttributeUsage(AttributeTargets.Property
       | AttributeTargets.Field | AttributeTargets.Class,
       AllowMultiple = true,
       Inherited = false)]
    public sealed class NotNullValidatorAttribute : ValidatorAttribute
    {

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="NotNullValidatorAttributeUsage" lang="cs" title="�����Ӷ���ǿ�У��������"  ></code>
        /// </remarks>
        public NotNullValidatorAttribute() 
        {
        }

        /// <summary>
        /// ���ػ���ķ���������NotNullValidator�Ĺ��췽ʽ����һ��NotNullValidatorʵ��
        /// </summary>
        /// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
        /// <returns>NotNullValidatorʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new NotNullValidator();
        }
       
    }
}
