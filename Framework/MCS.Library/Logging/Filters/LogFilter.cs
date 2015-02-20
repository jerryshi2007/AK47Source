#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	LogFilter.cs
// Remark	��	������࣬��־��������ʵ��ILogFilter�ӿ�
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
    /// �����࣬ʵ��ILogFilter�ӿ�
    /// </summary>
    /// <remarks>
    /// ����LogFilter�Ļ���
    /// ����ʱ��Ϊʹ���Ƶ�Filter֧�ֿ����ã������ڸ���������ʵ�ֲ���ΪLogConfigurationElement����Ĺ��캯��
    /// </remarks>
    public abstract class LogFilter : ILogFilter
    {
        private string name = string.Empty;

        /// <summary>
        /// Filter����
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public LogFilter()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
		/// <param name="filterName">Filter����</param>
        /// <remarks>
        /// name��������Ϊ�գ������׳��쳣
        /// </remarks>
        public LogFilter(string filterName)
        {
			ExceptionHelper.CheckStringIsNullOrEmpty(filterName, "Filter�����Ʋ���Ϊ��");

			this.name = filterName;
        }

        /// <summary>
        /// ���󷽷���ʵ����־����
        /// </summary>
        /// <param name="log">��־����</param>
        /// <returns>����ֵ��true��ͨ����false����ͨ��</returns>
        public abstract bool IsMatch(LogEntity log);
    }
}
