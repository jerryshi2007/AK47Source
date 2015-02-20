#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	FormattedTraceListenerBase.cs
// Remark	：	抽象基类，日志组件中所有Listener的基类，在系统TraceListener的基础上封装了格式化器
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MCS.Library.Logging
{
    /// <summary>
    /// 抽象基类，日志组件中所有Listener的基类
    /// </summary>
    /// <remarks>
    /// TraceListener的派生类，扩展Formatter属性
    /// 派生时，为使定制的Listener支持可配置，必须在该派生类中实现参数为LogConfigurationElement对象的构造函数
    /// </remarks>
    public abstract class FormattedTraceListenerBase : TraceListener
    {
        private ILogFormatter formatter;

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public FormattedTraceListenerBase()
        { 
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatter">ILogFormatter对象</param>
        public FormattedTraceListenerBase(ILogFormatter formatter)
            : this()
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// 日志文本格式化器（可选）
        /// </summary>
        /// <remarks>
        /// 将LogEntity格式化成string。该属性可以不设定，此时不进行格式化
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
        /// 重载方法，写入数据
        /// </summary>
        /// <param name="eventCache">包含当前进程 ID、线程 ID 以及堆栈跟踪信息的 TraceEventCache 对象</param>
        /// <param name="source">标识输出时使用的名称，通常为生成跟踪事件的应用程序的名称</param>
        /// <param name="eventType">TraceEventType枚举值，指定引发日志记录的事件类型</param>
        /// <param name="id">事件的数值标识符</param>
        /// <param name="data">要记录的日志数据</param>
        /// <remarks>
        /// <code source="..\Framework\src\DeluxeWorks.Library\Logging\Logger.cs" 
        /// lang="cs" region="Process Log" title="写日志"></code>
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
