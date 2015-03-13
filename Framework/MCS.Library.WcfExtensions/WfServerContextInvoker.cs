using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Logging;
using MCS.Library.Passport;
using MCS.Library.Services;
using MCS.Library.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;

namespace MCS.Library.WcfExtensions
{
    internal class WfServerContextInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _InnerInvoker = null;
        private static string MonitorName = "WfServerContextInvoker";

        public WfServerContextInvoker(IOperationInvoker innerInvoker)
        {
            this._InnerInvoker = innerInvoker;
        }

        public object[] AllocateInputs()
        {
            return this._InnerInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            Message messge = OperationContext.Current.RequestContext.RequestMessage;

            object mappingObject = null;

            Dictionary<string, string> connectionMappings = null;

            if (messge.Properties.TryGetValue("ConnectionMappings", out mappingObject) == false)
                connectionMappings = new Dictionary<string, string>();
            else
                connectionMappings = (Dictionary<string, string>)mappingObject;

            InitConnectionMappings(connectionMappings);

            object contextObject = null;

            if (messge.Properties.TryGetValue("Context", out contextObject))
            {
                Dictionary<string, object> context = (Dictionary<string, object>)contextObject;

                if (context.ContainsKey("TenantCode"))
                    TenantContext.Current.TenantCode = (string)context["TenantCode"];

                if (context.ContainsKey("Culture"))
                    ExceptionHelper.DoSilentAction(() => Thread.CurrentThread.CurrentCulture = new CultureInfo((string)context["Culture"]));
            }

            object containerObject = null;

            if (messge.Properties.TryGetValue("TokenContainer", out containerObject))
            {
                GenericTicketTokenContainer container = (GenericTicketTokenContainer)containerObject;

                IPrincipalBuilder principalBuilder = PrincipalSettings.GetConfig().GetPrincipalBuilder(false);

                if (principalBuilder != null)
                {
                    IPrincipal principal = principalBuilder.CreatePrincipal(container, null);

                    PrincipaContextAccessor.SetPrincipal(principal);
                }
            }

            return InvokeWithMonitor(instance, inputs, out outputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return this._InnerInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return this._InnerInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get
            {
                return this._InnerInvoker.IsSynchronous;
            }
        }

        private object InvokeWithMonitor(object instance, object[] inputs, out object[] outputs)
        {
            ServiceMethodConfigurationElement methodElement = ServiceSettings.GetConfig().GetMethodElementByOperationContext();

            object[] result = null;

            if (methodElement != null)
            {
                PerformanceMonitorHelper.DefaultMonitorName = MonitorName;
                MonitorData md = PerformanceMonitorHelper.StartMonitor(MonitorName);

                md.MonitorName = string.Format("{0}.{1}", OperationContext.Current.GetContractName(), OperationContext.Current.GetMethodName());
                md.EnableLogging = methodElement.EnableLogging;
                md.EnablePFCounter = methodElement.EnablePFCounter;

                md.InstanceName = md.MonitorName;
                md.Context["GlobalInstance"] = new WebMethodServerCounters("_Total_");
                md.Context["Instance"] = new WebMethodServerCounters(md.InstanceName);

                if (md.EnableLogging)
                    md.LogWriter.WriteLine("请求{0}的开始时间: {1:yyyy-MM-dd HH:mm:ss.fff}", md.MonitorName, DateTime.Now);

                try
                {
                    PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(md.MonitorName,
                        () =>
                        {
                            result = this.InternalInvoke(instance, inputs);
                        });
                }
                catch (System.Exception ex)
                {
                    md.HasErrors = true;

                    if (md.EnableLogging)
                        ex.WriteToEventLog(md.MonitorName, EventLogEntryType.Error, 8010);

                    throw;
                }
                finally
                {
                    CommitMonitorData();
                }
            }
            else
                result = this.InternalInvoke(instance, inputs);

            outputs = (object[])result[1];

            return result[0];
        }

        private object[] InternalInvoke(object instance, object[] inputs)
        {
            object[] outputs;

            object invokeResult = this._InnerInvoker.Invoke(instance, inputs, out outputs);

            return new object[] { invokeResult, outputs };
        }

        private static void CommitMonitorData()
        {
            if (PerformanceMonitorHelper.ExistsMonitor(MonitorName))
            {
                MonitorData md = PerformanceMonitorHelper.GetMonitor("WfServerContextInvoker");

                md.Stopwatch.Stop();

                if (md.EnableLogging)
                {
                    md.LogWriter.WriteLine("请求{0}的结束时间: {1:yyyy-MM-dd HH:mm:ss.fff}，经过{2:#,##0}毫秒",
                        md.MonitorName, DateTime.Now, md.Stopwatch.ElapsedMilliseconds);

                    CommitLogging(md);
                }

                if (md.EnablePFCounter)
                    SetCountersValues(md, md.HasErrors);

                PerformanceMonitorHelper.RemoveMonitor(MonitorName);
                PerformanceMonitorHelper.DefaultMonitorName = "DefaultMonitor";
            }
        }

        private static void InitConnectionMappings(Dictionary<string, string> connectionMappings)
        {
            foreach (KeyValuePair<string, string> kp in connectionMappings)
            {
                DbConnectionMappingContext.CreateMapping(kp.Key, kp.Value);
            }
        }

        private static void SetCountersValues(MonitorData md, bool hasErrors)
        {
            WebMethodServerCounters global = (WebMethodServerCounters)md.Context["GlobalInstance"];
            WebMethodServerCounters instance = (WebMethodServerCounters)md.Context["Instance"];

            SetOnePFInstanceValues(global, md, hasErrors);
            SetOnePFInstanceValues(instance, md, hasErrors);
        }

        private static void SetOnePFInstanceValues(WebMethodServerCounters instance, MonitorData md, bool hasErrors)
        {
            instance.RequestCount.Increment();

            instance.RequestAverageDurationBase.Increment();
            instance.RequestAverageDuration.IncrementBy(md.Stopwatch.ElapsedMilliseconds / 100);

            if (hasErrors)
                instance.RequestFailCount.Increment();
            else
                instance.RequestSuccessCount.Increment();

            instance.RequestsPerSecond.Increment();
        }

        private static void CommitLogging(MonitorData md)
        {
            StringBuilder strB = ((StringWriter)md.LogWriter).GetStringBuilder();

            if (strB.Length > 0)
            {
                try
                {
                    Logger logger = LoggerFactory.Create("WfServiceMonitor");

                    if (logger != null)
                        logger.Write(strB.ToString(), LogPriority.Normal, 8009, TraceEventType.Information, "服务访问日志");
                }
                catch (System.Exception)
                {
                }
            }
        }
    }
}
