using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// ���󼯺�У����������
    /// </summary>
    /// <remarks>
    /// ���󼯺�У����������
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
        /// ���췽��
        /// </summary>
        /// <param name="targetType">Ŀ������</param>
        public ObjectCollectionValidatorAttribute(Type targetType)
            : this(targetType, string.Empty)
        { }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="targetType">Ŀ������</param>
        /// <param name="targetRuleset">У�������򼯺�</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Customer.cs" region="ObjectCollectionValidatorAttributeUsage" lang="cs" title="�����Ӷ��󼯺�У��������"  ></code>
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
        /// ���ػ���ķ���������ObjectCollectionValidator�Ĺ��췽ʽ����һ��ObjectCollectionValidatorʵ��
        /// </summary>
        /// <param name="targetType">Ŀ�����ͣ����ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
        /// <returns>ObjectCollectionValidatorʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new ObjectCollectionValidator(this.targetType, 
                this.targetRuleset = (string.IsNullOrEmpty(this.targetRuleset)) ? this.Ruleset : this.targetRuleset);   
          
        }
    }
}
