#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	FormattedTraceListenerWrapperBase.cs
// Remark	��	������࣬Ҳ��FormattedTraceListenerBase�������࣬��FormattedTraceListenerBase������
//              ��װ��slaveListener�������������е�ϵͳListener����Logging���ȱʡʵ�ֵ�Listener�Ļ��࣬
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
    /// ������࣬Logging�������Listener�Ļ���
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerBase�������࣬�ڲ���װ��TraceListener����
    /// </remarks>
    public abstract class FormattedTraceListenerWrapperBase : FormattedTraceListenerBase
    {
        private TraceListener slaveListener;

		/// <summary>
		/// ȱʡ���캯��
		/// </summary>
		public  FormattedTraceListenerWrapperBase()
		{
		}

		/// <summary>
        /// ���캯��
		/// </summary>
		/// <param name="slaveListener">����װ��TraceListener����</param>
		public FormattedTraceListenerWrapperBase(TraceListener slaveListener)
		{
			this.slaveListener = slaveListener;
		}

		/// <summary>
        /// ���췽��
		/// </summary>
        /// <param name="slaveListener">����װ��TraceListener����</param>
        /// <param name="formater">ILogFormatterʵ��</param>
		public FormattedTraceListenerWrapperBase(TraceListener slaveListener, ILogFormatter formater) 
			: base(formater)
		{
			this.slaveListener = slaveListener;
		}

        /// <summary>
        /// ���ط�����д������
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="logEventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="eventID">�¼�����ֵ��ʶ��</param>
        /// <param name="data">Ҫ��¼����־����</param>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦTraceData����
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, params object[] data)
		{
            this.slaveListener.TraceData(eventCache, source, logEventType, eventID, data);
		}
        
		/// <summary>
        /// ���ط�����д������
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="logEventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="eventID">�¼�����ֵ��ʶ��</param>
        /// <param name="data">Ҫ��¼����־����</param>
        /// <remarks>
        /// ֻ��Formatter���Բ�Ϊ��ʱ���Ÿ�ʽ��
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, object data)
		{
			if (data is LogEntity)
			{
				LogEntity logData = data as LogEntity;
				if (this.Formatter != null)
				{
					this.slaveListener.TraceData(eventCache, source, logEventType, eventID, this.Formatter.Format(logData));
				}
				else
				{
					this.slaveListener.TraceData(eventCache, source, logEventType, eventID, logData);
				}
			}
			else
			{
                this.slaveListener.TraceData(eventCache, source, logEventType, eventID, data);
			}
		}

        /// <summary>
        /// ���ط�����д������
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="logEventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="id">�¼�����ֵ��ʶ��</param>
        /// <param name="message">Ҫ��¼����־����</param>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦTraceEvent����
        /// </remarks>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType logEventType, int id, string message)
		{
            this.slaveListener.TraceEvent(eventCache, source, logEventType, id, message);
		}

        /// <summary>
        /// ���ط�����д������
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="logEventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="id">�¼�����ֵ��ʶ��</param>
        /// <param name="format">��ʽ��</param>
        /// <param name="args">Ҫ��¼����־����</param>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦTraceEvent����
        /// </remarks>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType logEventType, int id, string format, params object[] args)
		{
            this.slaveListener.TraceEvent(eventCache, source, logEventType, id, format, args);
		}

		/// <summary>
        /// ���ط�����д���ַ���
		/// </summary>
		/// <param name="message">��д����ַ���</param>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦWrite����
        /// </remarks>
		public override void Write(string message)
		{
			this.slaveListener.Write(message);
		}

		/// <summary>
        /// ���ط�����д��һ���ַ���
		/// </summary>
        /// <param name="message">��д����ַ���</param>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦWriteLine����
        /// </remarks>
		public override void WriteLine(string message)
		{
			this.slaveListener.WriteLine(message);
		}

        /// <summary>
        /// ˢ��Listener�Ļ�����
        /// </summary>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦFlush����
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void Flush()
        {
            this.slaveListener.Flush();
        }

        /// <summary>
        /// �ر�Listener
        /// </summary>
        /// <remarks>
        /// ���ð�װ��TraceListener��Ķ�ӦClose����
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void Close()
        {
            this.slaveListener.Close();
        }

		internal TraceListener SlaveListener
		{
			get 
            { 
                return this.slaveListener; 
            }
            set
            {
                this.slaveListener = value;
            }
		}

		/// <summary>
		/// �ͷ���Դ
		/// </summary>
		/// <param name="disposing">�Ƿ��ͷŰ�װ����Դ</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.slaveListener.Dispose();
			}
		}
    }
}
