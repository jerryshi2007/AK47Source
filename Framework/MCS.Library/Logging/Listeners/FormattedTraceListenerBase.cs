#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	FormattedTraceListenerBase.cs
// Remark	��	������࣬��־���������Listener�Ļ��࣬��ϵͳTraceListener�Ļ����Ϸ�װ�˸�ʽ����
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MCS.Library.Logging
{
    /// <summary>
    /// ������࣬��־���������Listener�Ļ���
    /// </summary>
    /// <remarks>
    /// TraceListener�������࣬��չFormatter����
    /// ����ʱ��Ϊʹ���Ƶ�Listener֧�ֿ����ã������ڸ���������ʵ�ֲ���ΪLogConfigurationElement����Ĺ��캯��
    /// </remarks>
    public abstract class FormattedTraceListenerBase : TraceListener
    {
        private ILogFormatter formatter;

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public FormattedTraceListenerBase()
        { 
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="formatter">ILogFormatter����</param>
        public FormattedTraceListenerBase(ILogFormatter formatter)
            : this()
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// ��־�ı���ʽ��������ѡ��
        /// </summary>
        /// <remarks>
        /// ��LogEntity��ʽ����string�������Կ��Բ��趨����ʱ�����и�ʽ��
        /// </remarks>
        public virtual ILogFormatter Formatter
        {
            get 
            {
                return this.formatter;
            }
            set
            {
                this.formatter = value;
            }
        }

        //public override bool IsThreadSafe
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}

        /// <summary>
        /// ���ط�����д������
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="eventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="id">�¼�����ֵ��ʶ��</param>
        /// <param name="data">Ҫ��¼����־����</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, 
                                       TraceEventType eventType, int id, object data)
        {
            if ((this.Filter == null) || this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                string text1 = string.Empty;
                if (data != null)
                {
                    text1 = data.ToString();

                    this.WriteLine(text1);
                }
            }
        }
    }
}
