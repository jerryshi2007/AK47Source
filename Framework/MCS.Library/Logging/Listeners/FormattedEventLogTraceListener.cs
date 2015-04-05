#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	FormattedEventLogTraceListener.cs
// Remark	：	日志组件缺省实现的侦听器，可格式化的系统事件侦听器（Listerner），包装了EventLogTraceListener
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// 1.1		    ccic\zhangtiejun	20080928		修改构造方法，增加EventSource创建
// 1.2          ccic\zhangtiejun    20081205        修改EventLog的Source属性的来源，优先从LogEntity的Source属性来，
//                                                  如果为空则取EventLogListener配置的source属性
// 1.3          ccic\zhangtiejun    20090108        解决事件日志源等于日志名称时删除事件源注册的异常（此种情况下，不删除），
//                                                  并调整注册关联关系的代码写法（改在TraceData中注册）
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
    /// EventLog侦听器（Listerner）
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase派生类，将日志记入EventLog中
    /// </remarks>
    public sealed class FormattedEventLogTraceListener : FormattedTraceListenerWrapperBase
    {
        private const string DefaultLogName = "";
        private const string DefaultMachineName = ".";
        private const string DefaultSource = "";

        //私有字段，added by ztj on 20081205
        private readonly string logName = DefaultLogName;
        private readonly string source = DefaultSource;

        private string formatterName = string.Empty;//modify by yuanyong 20070603

        /// <summary>
        /// 文本格式化器
        /// </summary>
        /// <remarks>
        /// 该Listener下所配的格式化器
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
        /// 构造函数
        /// </summary>
        /// <param name="element">LogListenerElement对象</param>
        /// <remarks>
        /// 根据配置信息创建FormattedEventLogTraceListener对象
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\\Logging\ListenerTest.cs"
        /// lang="cs" region="EventLogTraceListener Test" tittle="创建Listener对象"></code>
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
        /// 缺省构造函数
        /// </summary>
        /// <remarks>
        /// 用EventLogTraceListener初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener()
            : base(new EventLogTraceListener())
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formater">ILogFormatter实例</param>
        /// <remarks>
        /// 用EventLogTraceListener和Formatter初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener(ILogFormatter formater)
            : base(new EventLogTraceListener(), formater)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventLog">EventLog对象</param>
        /// <remarks>
        /// 用EventLogTraceListener(EventLog)初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog)
            : base(new EventLogTraceListener(eventLog))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="eventLog">EventLog对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 用EventLogTraceListener(EventLog)和Formatter初始化一个实例对象
        /// </remarks>
        public FormattedEventLogTraceListener(EventLog eventLog, ILogFormatter formatter)
            : base(new EventLogTraceListener(eventLog), formatter)
        {
        }

        /// <summary>
        /// 构造函数，用EventLogTraceListener(EventLog)和Formatter初始化一个实例对象
        /// </summary>
        /// <param name="source">EventLog中的事件来源</param>
        /// <param name="logName">EventLog中的日志名称</param>
        /// <param name="machineName">记录事件日志的机器名称</param>
        /// <param name="formatter">ILogFormatter实例</param>
        public FormattedEventLogTraceListener(string source, string logName, string machineName, ILogFormatter formatter)
            : base(new EventLogTraceListener(new EventLog(logName, NormalizeMachineName(machineName), source)), formatter)
        {
        }

        /// <summary>
        /// 是否线程安全，对EventLogTraceListener为true
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 重载方法，写入文件
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="logEventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="eventID">事件的数值标识符</param>
        /// <param name="data">要记录的日志数据</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志"></code>
        /// </remarks>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType logEventType, int eventID, object data)
        {
            if (data is LogEntity)
            {
                LogEntity logData = data as LogEntity;

                //取LogEntity对象的Source属性，如果为空则取来源于配置的缺省值
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
        /// 注册日志来源和日志名称的映射关系
        /// </summary>
        /// <param name="source">来源</param>
        /// <param name="logName">日志名称</param>
        private string RegisterSourceToLogName(string source, string logName)
        {
            string registeredLogName = logName;

            EventSourceCreationData creationData = new EventSourceCreationData(source, logName);

            if (EventLog.SourceExists(source))
            {
                string originalLogName = EventLog.LogNameFromSourceName(source, FormattedEventLogTraceListener.DefaultMachineName);

                //source注册的日志名称和指定的logName不一致，且不等于source自身
                //（事件日志源“System”等于日志名称，不能删除。[System.InvalidOperationException]）
                if (string.Compare(logName, originalLogName, true) != 0 && string.Compare(source, originalLogName, true) != 0)
                {
                    //删除现有的关联重新注册
                    EventLog.DeleteEventSource(source, FormattedEventLogTraceListener.DefaultMachineName);
                    EventLog.CreateEventSource(creationData);
                }
                else
                    registeredLogName = originalLogName;
            }
            else
                //source未在该服务器上注册过
                EventLog.CreateEventSource(creationData);

            return registeredLogName;
        }
    }
}
