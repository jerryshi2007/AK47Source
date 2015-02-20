using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

using MCS.Library.Core;
using MCS.Library.Logging;

namespace MCS.Library.Services
{
    public class ServiceLog
    {
        private string name = string.Empty;
        private Logger logger;

        private string lastExceptionMessage = string.Empty;
        private string lastMessage = string.Empty;

        private ReaderWriterLock exceptionLock = new ReaderWriterLock();
        private ReaderWriterLock msgLock = new ReaderWriterLock();

        /// <summary>
        /// ���캯��������ServiceLog����
        /// </summary>
        /// <param name="name">servicelog������</param>
        /// <param name="useDefaultLogger">�Ƿ�ʹ�÷�����ȱʡ��Logger</param>
        public ServiceLog(string name, bool useDefaultLogger)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "Name");

            try
            {
                this.name = name;

                if (useDefaultLogger)
                    this.logger = LoggerFactory.Create(ServiceMainSettings.SERVICE_NAME);
                else
                {
                    try
                    {
                        this.logger = LoggerFactory.Create(this.name);
                    }
                    catch (Exception ex)
                    {
                        //���û��Ϊ�÷����߳����ö�����Logger����ȡ������ȱʡ��Logger
                        this.logger = LoggerFactory.Create(ServiceMainSettings.SERVICE_NAME);

                        this.WriteDebugString(ex, EventLogEntryType.Warning, ServiceLogEventID.SERVICEBASE_CREATELOGGER);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteDebugString(ex, EventLogEntryType.Error, ServiceLogEventID.SERVICEBASE_CREATEDEFAULTLOGGER);
            }
        }

        /// <summary>
        /// �����յ�ServiceLog���󣬲��������ļ�����ServiceLog����
        /// </summary>
        /// <param name="name">servicelog������</param>
        public ServiceLog(string name)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(name, "Name");

            this.name = name;
            this.logger = LoggerFactory.Create();
            this.logger.Name = name;
        }

        public List<FormattedTraceListenerBase> Listeners
        {
            get
            {
                return this.logger.Listeners;
            }
        }

        /// <summary>
        /// ������쳣��Ϣ
        /// </summary>
        public string LastExceptionMessage
        {
            get
            {
                this.exceptionLock.AcquireReaderLock(1000);

                try
                {
                    return this.lastExceptionMessage;
                }
                finally
                {
                    this.exceptionLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// �������Ϣ
        /// </summary>
        public string LastMessage
        {
            get
            {
                this.msgLock.AcquireReaderLock(1000);

                try
                {
                    return this.lastMessage;
                }
                finally
                {
                    this.msgLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// ����ı���ؼ���Listener
        /// </summary>
        /// <param name="textBox"></param>
        public void AddTextBoxTraceListener(TextBox textBox)
        {
            ExceptionHelper.FalseThrow(this.logger != null, "Logger����Ϊ��");

            FormattedTextWriterTraceListener listener = new FormattedTextWriterTraceListener(new TextBoxWriter(textBox));

            listener.Formatter = new TextLogFormatter("textLogFormatter"); //LoggingSection.GetConfig().LogFormatterElements["TextLogFormatter"]

            this.logger.Listeners.Add(listener);
        }

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="log">��д����־��¼����</param>
        public void Write(LogEntity log)
        {
            ExceptionHelper.FalseThrow(log != null, "LogEntity����Ϊ��");

            try
            {
                UpdateLastMsg(log.Message);

                if (string.IsNullOrEmpty(log.Source))
                    log.Source = this.name;
                
                this.logger.Write(log);
            }
            catch (Exception ex)
            {
                this.WriteDebugString(ex, EventLogEntryType.Warning, ServiceLogEventID.SERVICEBASE_WRITELOG);
            }
        }

        /// <summary>
        /// д��Ϣ
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        public void Write(string message)
        {
            LogEntity log = new LogEntity(message);

            UpdateLastMsg(message);

            this.Write(log);
        }

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="priority">��־���ȼ�</param>
        /// <param name="eventId">��־�¼�</param>
        /// <param name="logEventType">��־�¼�����</param>
        /// <param name="title">��־����</param>
        public void Write(string message, LogPriority priority, int eventId,
                                TraceEventType logEventType, string title)
        {
            try
            {
                UpdateLastMsg(message);

                LogEntity log = new LogEntity(title, message, eventId, priority, logEventType, this.name, string.Empty, null);
                
                this.logger.Write(log);
            }
            catch (LogException ex)
            {
                this.WriteDebugString(ex, EventLogEntryType.Warning, ServiceLogEventID.SERVICEBASE_WRITELOG);
            }
        }

        /// <summary>
        /// д��־
        /// </summary>
        /// <param name="message">��־��Ϣ</param>
        /// <param name="eventId">��־�¼�</param>
        /// <param name="title">��־����</param>
        public void Write(string title, string message, int eventId)
        {
            LogEntity log = new LogEntity(title, message, eventId);

            UpdateLastMsg(message);

            this.Write(log);
        }
        
        /// <summary>
        /// д�쳣��־
        /// </summary>
        /// <param name="ex"></param>
        public void Write(Exception ex)
        {
            this.Write(ex, 0);
        }

        /// <summary>
        /// д�쳣��־��ָ���¼�ID
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="eventId"></param>
        public void Write(Exception ex, int eventId)
        {
            this.Write(ex.Message, ex, eventId);
        }

        /// <summary>
        /// д�쳣��־��ָ����־������¼�ID
        /// </summary>
        /// <param name="title">ָ������־����</param>
        /// <param name="ex"></param>
        /// <param name="eventId"></param>
        public void Write(string title, Exception ex, int eventId)
        {
            this.Write(title, ex, TraceEventType.Error, eventId);
        }

        public void Write(string title, Exception ex, TraceEventType eventType, int eventId)
        {
            LogEntity log = new LogEntity(ex);
            log.EventID = eventId;
            log.Title = title;
            log.LogEventType = eventType;
            log.ExtendedProperties.Add("exceptionType", ex.GetType());

            UpdateLastExceptionMessage(ex);

            this.Write(log);
        }

        private void WriteDebugString(string strMessage, EventLogEntryType eventType, int eventId)
        {
            try
            {
                if (ServiceMainSettings.GetConfig().OutputDebugString)
                    Trace.WriteLine(strMessage);

                EventLog.WriteEntry(ServiceMainSettings.SERVICE_NAME, strMessage, eventType, eventId);
            }
            catch (Exception)
            {
            }
        }

        private void WriteDebugString(Exception ex, EventLogEntryType eventType, int eventId)
        {
            string strMsg = ex.Message;

            if (string.IsNullOrEmpty(ex.StackTrace) == false)
                strMsg += "\n" + ex.StackTrace;

            this.WriteDebugString(strMsg, eventType, eventId);
        }

        private void UpdateLastExceptionMessage(Exception ex)
        {
            this.exceptionLock.AcquireWriterLock(1000);

            try
            {
                this.lastExceptionMessage = string.Format("{1}[{0:yyyy-MM-dd HH:mm:ss}] \n �����ջΪ��{2}", DateTime.Now, ex.Message, ex.StackTrace);
            }
            finally
            {
                this.exceptionLock.ReleaseWriterLock();
            }
        }

        private void UpdateLastMsg(string message)
        {
            this.msgLock.AcquireWriterLock(1000);

            try
            {
                this.lastMessage = string.Format("{1}[{0:yyyy-MM-dd HH:mm:ss}]", DateTime.Now, message);
            }
            finally
            {
                this.msgLock.ReleaseWriterLock();
            }
        }
    }

    public class ServiceLogEventID
    {
        public const int SERVICEMAIN_STARTUPINFO = 10;
        public const int SERVICEBASE_CREATELOGGER = 11;
        public const int SERVICEMAIN_ADDTHREAD = 12;
        public const int SERVICEMAIN_ONSTART = 13;
        public const int SERVICEMAIN_ONSTOP = 14;
        public const int SERVICEMAIN_CREATECONTROL = 15;
        public const int SERVICEBASE_THREADEXECUTE = 16;
        public const int SERVICEBASE_CREATETHREADPARAM = 17;
        public const int SERVICEBASE_THREADABORTED = 18;
        public const int SERVICEBASE_CREATEDEFAULTLOGGER = 19;
        public const int SERVICEBASE_WRITELOG = 20;
        public const int SERVICEMAIN_MAIN = 999;
    }

    public class ServiceStopException : Exception
    {
        public ServiceStopException(string msg)
            : base(msg)
        {
        }
        
        public static void WaitAndThrow(WaitHandle handle)
        {
            if (handle.WaitOne(0, false))
                throw new ServiceStopException("Stop EventHandle Triggered");
        }
    }
}
