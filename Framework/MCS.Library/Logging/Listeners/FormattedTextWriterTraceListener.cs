#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	FormattedTextWriterTraceListener.cs
// Remark	：	日志组件缺省实现的侦听器，可格式化的文本编写侦听器（Listerner），包装了TextWriterTraceListener
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\zhangtiejun    20070430		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MCS.Library.Logging
{
    /// <summary>
    /// 可格式化的文本编写侦听器（Listerner）
    /// </summary>
    /// <remarks>
    /// FormattedTraceListenerWrapperBase的派生类，包装TextWriterTraceListener类
    /// </remarks>
    public class FormattedTextWriterTraceListener : FormattedTraceListenerWrapperBase
    {
        private string fileName = string.Empty;

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public FormattedTextWriterTraceListener()
            : base(new TextWriterTraceListener())
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formatter">ILogFormatter实例</param>
        public FormattedTextWriterTraceListener(ILogFormatter formatter)
            : this()
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">日志文件名</param>
        /// <remarks>
        /// 将日志记录写入filename指定的文件中
        /// </remarks>
        public FormattedTextWriterTraceListener(string filename)
        {
            this.fileName = RootFileNameAndEnsureTargetFolderExists(filename);
            FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            TextWriterTraceListener textlistener = new TextWriterTraceListener(fs, base.Name);

            base.SlaveListener = textlistener;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filename">日志文件名</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 将日志记录，由formatter格式化后写入filename指定的文件中
        /// </remarks>
        public FormattedTextWriterTraceListener(string filename, ILogFormatter formatter)
            : this(filename)
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">Stream对象</param>
        /// <remarks>
        /// 将日志记录，写入stream流中
        /// </remarks>
        public FormattedTextWriterTraceListener(Stream stream)
        {
            TextWriterTraceListener textlistener = new TextWriterTraceListener(stream, base.Name);

            base.SlaveListener = textlistener;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream">Stream对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 将日志记录，由formatter格式化后写入stream流中
        /// </remarks>
        public FormattedTextWriterTraceListener(Stream stream, ILogFormatter formatter)
            : this(stream)
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">TextWriter对象</param>
        /// <remarks>
        /// 将日志记录，写入writer中
        /// </remarks>
        public FormattedTextWriterTraceListener(TextWriter writer)
            : base(new TextWriterTraceListener(writer))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="writer">TextWriter对象</param>
        /// <param name="formatter">ILogFormatter实例</param>
        /// <remarks>
        /// 将日志记录，由formatter格式化后写入writer中
        /// </remarks>
        public FormattedTextWriterTraceListener(TextWriter writer, ILogFormatter formatter)
            : this(writer)
        {
            base.Formatter = formatter;
        }

        /// <summary>
        /// 是否线程安全，对TextWriterTraceListener为true
        /// </summary>
        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

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
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is LogEntity)
            {
				LogEntity logData = data as LogEntity;
                if (this.Formatter != null)
                {
					base.Write(this.Formatter.Format(logData));
                }
                else
                {
					base.TraceData(eventCache, source, eventType, id, logData);
                }

            }
            else
            {
                base.TraceData(eventCache, source, eventType, id, data);
            }
        }

        /// <summary>
        ///支持的属性
        /// </summary>
        /// <returns>属性数组</returns>
        protected override string[] GetSupportedAttributes()
        {
            return new string[1] { "formatter" };
        }

        /// <summary>
        /// 文件路径处理
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>标准的完整文件路径</returns>
        protected static string RootFileNameAndEnsureTargetFolderExists(string fileName)
        {
            string rootedFileName = fileName;
            if (false == Path.IsPathRooted(rootedFileName))
            {
                rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedFileName);
            }

            string directory = Path.GetDirectoryName(rootedFileName);
            if (false == string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return rootedFileName;
        }
    }

    #region Obsoleted Implementation of FormattedTextWriterTraceListener Class
    //public class FormattedTextWriterTraceListener : TextWriterTraceListener
   // {
   //     private ILogFormatter formatter;
 
   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     public FormattedTextWriterTraceListener()
   //         : base()
   //     {
            
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(ILogFormatter formatter)
   //         : this()
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(Stream stream, ILogFormatter formatter)
   //         : this(stream)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     public FormattedTextWriterTraceListener(Stream stream)
   //         : base(stream)
   //     {
          
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer, ILogFormatter formatter)
   //         : this(writer)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer)
   //         : base(writer)
   //     {
           
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(string fileName, ILogFormatter formatter)
   //         : this(fileName)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     public FormattedTextWriterTraceListener(string fileName)
   //         : base(RootFileNameAndEnsureTargetFolderExists(fileName))
   //     {
          
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     /// <param name="name">The name.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(Stream stream, string name, ILogFormatter formatter)
   //         : this(stream, name)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="stream">The stream to write to.</param>
   //     /// <param name="name">The name.</param>
   //     public FormattedTextWriterTraceListener(Stream stream, string name)
   //         : base(stream, name)
   //     {
           
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     /// <param name="name">The name.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer, string name, ILogFormatter formatter)
   //         : this(writer, name)
   //     {
   //         this.Formatter = formatter;
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="writer">The writer to write to.</param>
   //     /// <param name="name">The name.</param>
   //     public FormattedTextWriterTraceListener(TextWriter writer, string name)
   //         : base(writer, name)
   //     {
           
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     /// <param name="name">The name.</param>
   //     /// <param name="formatter">The formatter to format the messages.</param>
   //     public FormattedTextWriterTraceListener(string fileName, string name, ILogFormatter formatter)
   //         : this(fileName, name)
   //     {
   //     }

   //     /// <summary>
   //     /// 构造函数
   //     /// </summary>
   //     /// <param name="fileName">The file name to write to.</param>
   //     /// <param name="name">The name.</param>
   //     public FormattedTextWriterTraceListener(string fileName, string name)
   //         : base(RootFileNameAndEnsureTargetFolderExists(fileName), name)
   //     {
   //         this.Formatter = formatter;
           
   //     }

   //     /// <summary>
   //     /// 写数据
   //     /// </summary>
   //     /// <remarks>
   //     /// Formatting is only performed if the object to trace is a <see cref="LogEntry"/> and the formatter is set.
   //     /// </remarks>
   //     /// <param name="eventCache">The context information.</param>
   //     /// <param name="source">The trace source.</param>
   //     /// <param name="eventType">The severity.</param>
   //     /// <param name="id">The event id.</param>
   //     /// <param name="data">The object to trace.</param>
   //     public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
   //     {
   //         if (data is LogEntity)
   //         {
   //             if (this.Formatter != null)
   //             {
   //                 base.Write(this.Formatter.Format(data as LogEntity));
   //             }
   //             else
   //             {
   //                 base.TraceData(eventCache, source, eventType, id, data);
   //             }
                
   //         }
   //         else
   //         {
   //             base.TraceData(eventCache, source, eventType, id, data);
   //         }
   //     }

   //     /// <summary>
   //     /// 文本格式化器
   //     /// </summary>
   //     public virtual ILogFormatter Formatter
   //     {
   //         get
   //         {
   //             return this.formatter;
   //         }

   //         set
   //         {
   //             this.formatter = value;
   //         }
   //     }

   //     /// <summary>
   //     ///支持的属性
   //     /// </summary>
   //     /// <returns></returns>
   //     protected override string[] GetSupportedAttributes()
   //     {
   //         return new string[1] { "formatter" };
   //     }
  
   //     private static string RootFileNameAndEnsureTargetFolderExists(string fileName)
   //     {
   //         string rootedFileName = fileName;
   //         if (!Path.IsPathRooted(rootedFileName))
   //         {
   //             rootedFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedFileName);
   //         }

   //         string directory = Path.GetDirectoryName(rootedFileName);
   //         if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
   //         {
   //             Directory.CreateDirectory(directory);
   //         }

   //         return rootedFileName;
   //     }

    // }
    #endregion
}
