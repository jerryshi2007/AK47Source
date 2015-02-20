using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Reflection;

namespace MCS.Library.Validation
{
    /// <summary>
    /// ������ʽУ���������࣬У�鱻У������Ƿ�ƥ��������ʽ
    /// </summary>
    /// <remarks>
    /// ������ʽУ���������࣬У�鱻У������Ƿ�ƥ��������ʽ
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class RegexValidatorAttribute : ValidatorAttribute
    {
        private string pattern;
        private RegexOptions options;

        /// <summary>
        /// ���ʽ����
        /// </summary>
        public string Pattern
        {
            get
            {
                return pattern;
            }
        }
        /// <summary>
        /// ��֤ѡ��
        /// </summary>
        public RegexOptions Options
        {
            get { return options; }
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="pattern">������ʽ</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Address.cs" region="RegexValidatorAttributeUsage" lang="cs" title="������������ʽУ��������"  ></code>
        /// </remarks>
        public RegexValidatorAttribute(string pattern)
            : this(pattern, RegexOptions.None)
        {

        }

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="pattern">������ʽ</param>
        /// <param name="options">������ʽѡ��ö��ֵ</param>
        /// <remarks>
        /// <code  source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\HelperClass\Student.cs" region="RegexValidatorAttributeUsage2" lang="cs" title="������������ʽУ��������"  ></code>
        /// </remarks>
        public RegexValidatorAttribute(string pattern, RegexOptions options)
        {
            this.pattern = pattern;
            this.options = options;

        }

        /// <summary>
        /// ���ػ���ķ���������RegexValidator�Ĺ��췽��ֱ�ӹ���RegexValidatorУ����
        /// </summary>
        /// <param name="targetType">Ŀ�����ͣ��ڱ�Ĭ��ʵ����δʹ�õ��ò���</param>
        /// <returns>RegexValidatorʵ��</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new RegexValidator(this.pattern, this.options, this.MessageTemplate, this.Tag);
        }
    }
}
