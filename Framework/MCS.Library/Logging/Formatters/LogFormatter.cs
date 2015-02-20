#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogFormatter.cs
// Remark	��	������࣬������־��ʽ�����Ļ���
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ������࣬ʵ��ILogFormatter�ӿ�
    /// </summary>
    /// <remarks>
    /// ����LogFormatter�Ļ��࣬
    /// ����ʱ��Ϊʹ���Ƶ�Formatter֧�ֿ����ã������ڸ���������ʵ�ֲ���ΪLogConfigurationElement����Ĺ��캯��
    /// </remarks>
    public abstract class LogFormatter : ILogFormatter
    {
        private string name = string.Empty;

        /// <summary>
        /// Formatter������
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        ///  ȱʡ���캯��
        /// </summary>
        public LogFormatter()
        {
        }

        /// <summary>
        /// ���캯��������Name����LogFormatter����
        /// </summary>
        /// <param name="formattername">Formatter������</param>
        /// <remarks>
        /// formattername��������Ϊ�գ������׳��쳣
        /// </remarks>
        public LogFormatter(string formattername)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(formattername, "Formatter�����ֲ���Ϊ��");

            this.name = formattername;
        }

        /// <summary>
        /// ���󷽷�����ʽ��LogEntity�����һ���ַ���
        /// </summary>
        /// <param name="log">����ʽ����LogEntity����</param>
        /// <returns>��ʽ���ɵ��ַ���</returns>
        /// <remarks>
        /// �����������ʵ��
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Formatters\TextFormatter.cs" 
        /// lang="cs" region="Format Implementation" title="�ı���ʽ������"></code>
        /// </remarks>
        public abstract string Format(LogEntity log);
    }
}
