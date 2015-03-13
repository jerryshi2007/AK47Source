using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace MCS.Library.Core
{
    /// <summary>
    /// 需要监控和记录日志的性能对象。主要用于计量执行时间
    /// </summary>
    public sealed class MonitorData : IDisposable
    {
        private string instanceName = string.Empty;
        private string monitorName = string.Empty;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly TextWriter logWriter = new StringWriter(new StringBuilder(256));
        private bool enableLogging = true;
        private bool enablePFCounter = true;
        private bool hasErrors = false;
        private Dictionary<string, object> context = null;

        /// <summary>
        /// 
        /// </summary>
        public string InstanceName
        {
            get { return this.instanceName; }
            set { this.instanceName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasErrors
        {
            get { return this.hasErrors; }
            set { this.hasErrors = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MonitorName
        {
            get { return this.monitorName; }
            set { this.monitorName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnablePFCounter
        {
            get { return this.enablePFCounter; }
            set { this.enablePFCounter = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableLogging
        {
            get { return this.enableLogging; }
            set { this.enableLogging = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextWriter LogWriter
        {
            get { return this.logWriter; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Stopwatch Stopwatch
        {
            get { return this.stopwatch; }
        }

        /// <summary>
        /// 上下文对象
        /// </summary>
        public Dictionary<string, object> Context
        {
            get
            {
                if (this.context == null)
                    this.context = new Dictionary<string, object>();

                return this.context;
            }
        }

        /// <summary>
        /// 向Log中输出执行的时间
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="action"></param>
        public void WriteExecutionDuration(string operationName, Action action)
        {
            if (action != null)
            {
                if (this.enableLogging)
                {
                    operationName.CheckStringIsNullOrEmpty("operationName");

                    LogWriter.WriteLine("\t\t{0}开始：{1:yyyy-MM-dd HH:mm:ss.fff}",
                            operationName, DateTime.Now);

                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    try
                    {
                        action();
                    }
                    finally
                    {
                        sw.Stop();
                        LogWriter.WriteLine("\t\t{0}结束：{1:yyyy-MM-dd HH:mm:ss.fff}；经过时间：{2:#,##0}毫秒",
                            operationName, DateTime.Now, sw.ElapsedMilliseconds);
                    }
                }
                else
                {
                    action();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this.logWriter != null)
                this.logWriter.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
