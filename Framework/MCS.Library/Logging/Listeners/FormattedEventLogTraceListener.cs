#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	FormattedEventLogTraceListener.cs
// Remark	��	��־���ȱʡʵ�ֵ����������ɸ�ʽ����ϵͳ�¼���������Listerner������װ��EventLogTraceListener
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		����
// 1.1		    ccic\zhangtiejun	20080928		�޸Ĺ��췽��������EventSource����
// 1.2          ccic\zhangtiejun    20081205        �޸�EventLog��Source���Ե���Դ�����ȴ�LogEntity��Source��������
//                                                  ���Ϊ����ȡEventLogListener���õ�source����
// 1.3          ccic\zhangtiejun    20090108        ����¼���־Դ������־����ʱɾ���¼�Դע����쳣����������£���ɾ������
//                                                  ������ע�������ϵ�Ĵ���д��������TraceData��ע�ᣩ
// -------------------------------------------------
#endregion

using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace MCS.Library.Logging
{
    /// <summary>
    /// EventLog��������Listerner��
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase�����࣬����־����EventLog��
    /// </remarks>
    public sealed class FormattedEventLogTraceListener : FormattedTraceListenerWrapperBase
    {
        private const string DefaultLogName = "";
        private const string DefaultMachineName = ".";
        private const string DefaultSource = "";

        //˽���ֶΣ�added by ztj on 20081205
        private readonly string logName = DefaultLogName;
        private readonly string source = DefaultSource;

        private string formatterName = string.Empty;//modify by yuanyong 20070603

        /// <summary>
        /// �ı���ʽ����
        /// </summary>
        /// <remarks>
        /// ��Listener������ĸ�ʽ����
        /// </remarks>
        public override ILogFormatter Formatter
        {
            get
            {
                ILogFormatter formatter = base.Formatter;

                if (false == string.IsNullOrEmpty(this.formatterName))// != string.Empty)
                {
                    LoggerFormatterConfigurationElement formatterElement = LoggingSection.GetConfig().Formatters[this.formatterName];

                    formatter = LogFormatterFactory.GetFormatter(formatterElement);
                }

                return formatter;
            }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="element">LogListenerElement����</param>
        /// <remarks>
        /// ����������Ϣ����FormattedEventLogTraceListener����
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\ListenerTest.cs"
        /// lang="cs" region="EventLogTraceListener Test" tittle="����Listener����"></code>
        /// </remarks>
        public FormattedEventLogTraceListener(LoggerListenerConfigurationElement element)
        {
            this.formatterName = element.LogFormatterName;
            this.Name = element.Name;

            this.logName = element.LogName;

            if (this.logName.IsNullOrEmpty())
                this.logName = FormattedEventLogTraceListener.DefaultLogName;

            this.source = element.Source;

            if (this.source.IsNullOrEmpty())
                this.source = string.IsNullOrEmpty(this.logName) ? FormattedEventLogTraceListener.DefaultSource : this.logName;

            this.SlaveListener = new EventLogTraceListener();
        }

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        /// <remarks>
        /// ��EventLogTraceListener��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener()
            : base(new EventLogTraceListener())
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="formater">ILogFormatterʵ��</param>
        /// <remarks>
        /// ��EventLogTraceListener��Formatter��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener(ILogFormatter formater)
            : base(new EventLogTraceListener(), formater)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="eventLog">EventLog����</param>
        /// <remarks>
        /// ��EventLogTraceListener(EventLog)��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog)
            : base(new EventLogTraceListener(eventLog))
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="eventLog">EventLog����</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        /// <remarks>
        /// ��EventLogTraceListener(EventLog)��Formatter��ʼ��һ��ʵ������
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog, ILogFormatter formatter)
            : base(new EventLogTraceListener(eventLog), formatter)
        {
        }

        /// <summary>
        /// ���캯������EventLogTraceListener(EventLog)��Formatter��ʼ��һ��ʵ������
        /// </summary>
        /// <param name="source">EventLog�е��¼���Դ</param>
        /// <param name="logName">EventLog�е���־����</param>
        /// <param name="machineName">��¼�¼���־�Ļ�������</param>
        /// <param name="formatter">ILogFormatterʵ��</param>
        public FormattedEventLogTraceListener(string source, string logName, string machineName, ILogFormatter formatter)
            : base(new EventLogTraceListener(new EventLog(logName, NormalizeMachineName(machineName), source)), formatter)
        {
        }

        /// <summary>
        /// �Ƿ��̰߳�ȫ����EventLogTraceListenerΪtrue
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// ���ط�����д���ļ�
        /// </summary>
        /// <param name="eventCache">������ǰ���� ID���߳� ID �Լ���ջ������Ϣ�� TraceEventCache ����</param>
        /// <param name="source">��ʶ���ʱʹ�õ����ƣ�ͨ��Ϊ���ɸ����¼���Ӧ�ó��������</param>
        /// <param name="logEventType">TraceEventTypeö��ֵ��ָ��������־��¼���¼�����</param>
        /// <param name="eventID">�¼�����ֵ��ʶ��</param>
        /// <param name="data">Ҫ��¼����־����</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="д��־"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, object data)
        {
            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;

                //ȡLogEntity�����Source���ԣ����Ϊ����ȡ��Դ�����õ�ȱʡֵ
                string sourceName = string.IsNullOrEmpty(logData.Source) ? this.source : logData.Source;

                if (this.WriteTraceData(eventCache, this.logName, sourceName, logEventType, eventID, data) == false)
                {
                    string registerLogName = this.RegisterSourceToLogName(sourceName, this.logName);

                    this.WriteTraceData(eventCache, registerLogName, sourceName, logEventType, eventID, data);
                }
            }
            else
                base.TraceData(eventCache, source, logEventType, eventID, data);
        }

        private bool WriteTraceData(TraceEventCache eventCache, string logName, string sourceName, TraceEventType logEventType, int eventID, object data)
        {
            try
            {
                EventLog eventlog = new EventLog(logName, FormattedEventLogTraceListener.DefaultMachineName, sourceName);

                (this.SlaveListener as EventLogTraceListener).EventLog = eventlog;

                base.TraceData(eventCache, source, logEventType, eventID, data);

                return true;
            }
            catch (SecurityException ex)
            {
                if (ex.HResult == -2146233078)
                    return false;

                throw;
            }
        }

        private static string NormalizeMachineName(string machineName)
        {
            return string.IsNullOrEmpty(machineName) ? FormattedEventLogTraceListener.DefaultMachineName : machineName;
        }

        /// <summary>
        /// ע����־��Դ����־���Ƶ�ӳ���ϵ
        /// </summary>
        /// <param name="source">��Դ</param>
        /// <param name="logName">��־����</param>
        private string RegisterSourceToLogName(string source, string logName)
        {
            string registeredLogName = logName;

            EventSourceCreationData creationData = new EventSourceCreationData(source, logName);

            if (EventLog.SourceExists(source))
            {
                string originalLogName = EventLog.LogNameFromSourceName(source, FormattedEventLogTraceListener.DefaultMachineName);

                //sourceע�����־���ƺ�ָ����logName��һ�£��Ҳ�����source����
                //���¼���־Դ��System��������־���ƣ�����ɾ����[System.InvalidOperationException]��
                if (string.Compare(logName, originalLogName, true) != 0 && string.Compare(source, originalLogName, true) != 0)
                {
                    //ɾ�����еĹ�������ע��
                    EventLog.DeleteEventSource(source, FormattedEventLogTraceListener.DefaultMachineName);
                    EventLog.CreateEventSource(creationData);
                }
                else
                    registeredLogName = originalLogName;
            }
            else
                //sourceδ�ڸ÷�������ע���
                EventLog.CreateEventSource(creationData);

            return registeredLogName;
        }
    }
}
