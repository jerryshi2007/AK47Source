using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace MCS.Library.Validation
{
    /// <summary>
    /// ����ֵ��Χ�ж�У����������
    /// </summary>
    /// <remarks>
    /// ����ֵ��Χ�ж�У��������
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
       Inherited = false)]
    public sealed class IntegerRangeValidatorAttribute : ValidatorAttribute
    {
        private int lowerBound;
        private int upperBound;

        /// <summary>
        /// ����ֵ����
        /// </summary>
        public int LowerBound
        {
            get
            {
                return lowerBound;
            }
        }

        /// <summary>
        /// ����ֵ����
        /// </summary>
        public int UpperBound
        {
            get { return upperBound; }
        }

        /// <summary>
        /// IntegerRangeValidatorAttribute���캯��
        /// </summary>
        /// <param name="lowerBound">����ֵ����</param>
        /// <param name="upperBound">����ֵ����</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="IntegerRangeValidatorAttributeUsage" lang="cs" title="���������η�ΧУ��������"  ></code>
        /// </remarks>
        public IntegerRangeValidatorAttribute(int lowerBound, int upperBound)
        {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }

        /// <summary>
        /// ���ػ���ķ���������IntegerRangeValidator�Ĺ��췽ʽ����һ��IntegerRangeValidatorʵ��
        /// </summary>
        /// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
        /// <returns>IntegerRangeValidatorʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new IntegerRangeValidator(this.lowerBound, this.upperBound, this.MessageTemplate, this.Tag);
        }

    }
}
