using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
    /// <summary>
    /// �ַ������ȹ����������࣬У���ַ��������Ƿ���ָ���ķ�Χ��
    /// </summary>
    /// <remarks>
    /// �ַ������ȹ����������࣬У���ַ��������Ƿ���ָ���ķ�Χ��
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class StringLengthValidatorAttribute : ValidatorAttribute
    {
        private string clientValidateMethodName;
        private int lowerBound;
        private int upperBound;

        /// <summary>
        /// �������Сֵ
        /// </summary>
        public int LowerBound
        {
            get { return lowerBound; }
        }

        /// <summary>
        /// �������ֵ
        /// </summary>
        public int UpperBound
        {
            get { return upperBound; }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="upperBound">�ַ�����������</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="StringLengthValidatorAttributeDemo" lang="cs" title="�������ַ�������У��������"  ></code>
        /// </remarks>
        public StringLengthValidatorAttribute(int upperBound)
            : this(0, upperBound)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="lowerBound">�ַ�����������</param>
        /// <param name="upperBound">�ַ�����������</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="StringLengthValidatorAttributeDemo2" lang="cs" title="�������ַ�������У��������"  ></code>
        /// </remarks>
        public StringLengthValidatorAttribute(int lowerBound, int upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="methodName">�ͻ���У�鷽������</param>
        /// <param name="lowerBound">����</param>
        /// <param name="upperBound">����</param>
        public StringLengthValidatorAttribute(string methodName, int lowerBound, int upperBound)
            : this(lowerBound, upperBound)
        {
            this.clientValidateMethodName = methodName;
        }

        /// <summary>
        /// ���ػ���ķ���������StringLengthValidator���췽������StringLengthValidator
        /// </summary>
        /// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
        /// <returns>�ַ�������У����ʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new StringLengthValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
        }
    }
}
