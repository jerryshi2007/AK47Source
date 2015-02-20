using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
    /// <summary>
    /// �Զ���У�������Եĳ�����ࡡ
    /// </summary>
    /// <remarks>
    /// ������Ӵ�ϵͳ���Զ������Ի���(Attribute)������
    /// ���ڽ��û������У�����Զ�����Ϣ��Ŀ��Ԫ���������
    /// ��Ҫ����������Ӧ��У����
    /// Ŀ��Ԫ��Ϊ����(Class) ������(Property)���ֶ�(Field)
    /// ����ģ���ṩ��һЩ����У����֮�⣬�û��Զ����У�������Զ���Ҫʵ������������
    /// </remarks>
    public abstract class ValidatorAttribute : BaseValidatorAttribute
    {
        private Validator _validator = null;
       
        /// <summary>
        /// ���У����
        /// </summary>
        public Validator Validator
        {
            get { return this._validator; }
        }

        /// <summary>
        /// ʹ�ø÷�������У����
        /// </summary>
        /// <param name="targetType">Ŀ������</param>
        /// <param name="ownerType">Դ�������ͣ�Ŀǰ�ڳ�����δ�����������Ժ���չʹ��</param>
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
        /// �����Զ����У�������Դ���һ��У����ʵ���ĳ��󷽷�
        /// �ڿ����߱�д�Զ���У������ʱ����Ҫʵ��������󷽷�
        /// </summary>
        /// <param name="targetType">Ŀ������</param>
        /// <returns>�����õļ�����</returns>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\AddressValidatorAttribute.cs" region="HowToImplementedCreateValidator" lang="cs" title="���ʵ��У�����Ĵ���"  ></code>
        /// </remarks>
        protected abstract Validator DoCreateValidator(Type targetType);

    }
}
