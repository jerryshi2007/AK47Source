using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using MCS.Library.Caching;
using MCS.Library.Data;
using MCS.Library.Services;
using MCS.Library.Services.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CacheSoapExtensionBase<T> : SoapExtension, IDisposable where T : ServiceMethodCache
    {
        private Stream _OldStream;
        private Stream _NewStream;

        private bool _IsNewStreamCreator = false;

        private SoapExtensionCallerInfo _Initializer = null;
        private string _InstanceName;

        private Stopwatch _ExecutionWatch = null;

        private SimpleRequestSoapMessage _RequestMessage = null;

        /// <summary>
        /// 请求的消息
        /// </summary>
        public SimpleRequestSoapMessage RequestMessage
        {
            get
            {
                return this._RequestMessage;
            }
        }

        private WebMethodServerCounters _GlobalInstance = null;
        private WebMethodServerCounters _ActionInstance = null;

        /// <summary>
        /// 参数中的serviceType在Server端一般是WebService类的派生类，客户端的一般是代理类。
        /// 通过这个类型来判断到底是Server端还是Client端使用这个Extension
        /// </summary>
        /// <param name="serviceType">使用这个Extension的类</param>
        /// <returns></returns>
        public override object GetInitializer(Type serviceType)
        {
            SoapExtensionCallerType callerType = SoapExtensionCallerType.ClientMethod;

            if (serviceType.IsSubclassOf(typeof(WebService)))
                callerType = SoapExtensionCallerType.ServiceMethod;

            return new SoapExtensionCallerInfo(serviceType.FullName, callerType);
        }

        /// <summary>
        ///  System.Web.Services.Protocols.LogicalMethodInfo，它表示应用 SOAP 扩展的 XML Web services 方法的特定函数原型。
        /// </summary>
        /// <param name="methodInfo">应用于 XML Web services 方法的 System.Web.Services.Protocols.SoapExtensionAttribute。</param>
        /// <param name="attribute"></param>
        /// <returns>System.Object，SOAP 扩展将对其进行初始化以用于缓存。</returns>
        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initializer"></param>
        public override void Initialize(object initializer)
        {
            this._Initializer = (SoapExtensionCallerInfo)initializer;
        }

        /// <summary>
        /// 处理消息。
        /// BeforeDeserialize和AfterDeserialize是发生在服务内代码执行之前。
        /// BeforeSerialize和AfterSerialize是发生在服务内代码执行之后。
        /// 整个执行时序是：ChainStream，BeforeDeserialize，AfterDeserialize，BeforeSerialize，AfterSerialize
        /// </summary>
        /// <param name="message"></param>
        public override void ProcessMessage(SoapMessage message)
        {
            //仅处理Server端的缓存
            if (message is SoapServerMessage)
            {
                switch (message.Stage)
                {
                    case SoapMessageStage.BeforeDeserialize:
                        InitAllParamsAndCounters(message);
                        SetBeforeExecuteMethodCounters(this._RequestMessage);

                        FetchDataFromCache(message);

                        BeforeExecuteServerMethod(message);
                        break;
                    case SoapMessageStage.AfterDeserialize:
                        break;
                    case SoapMessageStage.BeforeSerialize:
                        break;
                    case SoapMessageStage.AfterSerialize:
                        WriteOutputStream(message);
                        SetAfterExecuteMethodCounters(this._RequestMessage, message.Exception == null);
                        break;
                    default:
                        throw new Exception("Error Message Stage");
                }
            }
        }

        /// <summary>
        /// 当在派生类中被重写时，允许 SOAP 扩展访问包含 SOAP 请求或响应的内存缓冲区。
        /// </summary>
        /// <param name="stream">包含 SOAP 请求或响应的内存缓冲区。</param>
        /// <returns>System.IO.Stream，它表示此 SOAP 扩展可以修改的新内存缓冲区。</returns>
        public override Stream ChainStream(Stream stream)
        {
            this._OldStream = stream;

            if (this._Initializer.CallerType == SoapExtensionCallerType.ServiceMethod)
            {
                this._NewStream = new MemoryStream();
                this._IsNewStreamCreator = true;
            }
            else
            {
                this._NewStream = stream;
                this._IsNewStreamCreator = false;
            }

            return this._NewStream;
        }

        /// <summary>
        /// 从Cache中获取数据
        /// </summary>
        /// <param name="message"></param>
        protected virtual void FetchDataFromCache(SoapMessage message)
        {
            ServiceMethodConfigurationElement methodSettings = GetMethodSettings(this._RequestMessage);

            if (this._RequestMessage.UseServerCache && methodSettings.CacheEnabled)
            {
                string dataInCache = GetDataInCache(this._RequestMessage);

                if (dataInCache != null)
                {
                    SetAfterExecuteMethodCounters(this._RequestMessage, true);

                    HttpContext.Current.Response.ContentType = "text/xml";
                    HttpContext.Current.Response.Write(dataInCache);
                    HttpContext.Current.Response.End();
                }
            }
        }

        private void SetBeforeExecuteMethodCounters(SimpleRequestSoapMessage message)
        {
            this._GlobalInstance.RequestCount.Increment();
            this._ActionInstance.RequestCount.Increment();
        }

        private void SetAfterExecuteMethodCounters(SimpleRequestSoapMessage message, bool success)
        {
            this._ExecutionWatch.Stop();

            SetAfterExecuteMethodCounterValues(this._GlobalInstance, message, success);
            SetAfterExecuteMethodCounterValues(this._ActionInstance, message, success);
        }

        private void SetAfterExecuteMethodCounterValues(WebMethodServerCounters instance, SimpleRequestSoapMessage message, bool success)
        {
            instance.RequestAverageDurationBase.Increment();
            instance.RequestAverageDuration.IncrementBy(this._ExecutionWatch.ElapsedMilliseconds / 100);

            if (success)
                instance.RequestSuccessCount.Increment();
            else
                instance.RequestFailCount.Increment();

            instance.RequestsPerSecond.Increment();
        }

        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        protected virtual SimpleRequestSoapMessage CreateMessage(Stream stream, string serviceName)
        {
            SimpleRequestSoapMessage message = SimpleRequestSoapMessage.CreateMessage(stream);

            message.ServiceName = serviceName;

            return message;
        }

        /// <summary>
        /// 执行Server端方法之前
        /// </summary>
        protected virtual void BeforeExecuteServerMethod(SoapMessage message)
        {
        }

        /// <summary>
        /// 得到服务方法缓存的管理器的实例，该实例的类型通常用于刷新缓存
        /// </summary>
        /// <returns></returns>
        protected virtual ServiceMethodCacheManager GetMethodCacheManager()
        {
            return ServiceMethodCacheManager.Instance;
        }

        /// <summary>
        /// 创建方法的缓存队列
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        protected abstract T CreateServiceMethodCache(string instanceName, int maxLength);

        /// <summary>
        /// 全部初始化参数
        /// </summary>
        private void InitAllParamsAndCounters(SoapMessage message)
        {
            this.InitStreamAndMessage(message);
            this.InitParameters(this._RequestMessage);
            this.InitializeCounters(this._InstanceName);
        }

        private void InitStreamAndMessage(SoapMessage message)
        {
            this._NewStream.Position = 0;
            this._OldStream.CopyTo(this._NewStream);
            this._NewStream.Position = 0;

            this._RequestMessage = this.CreateMessage(this._NewStream, this._Initializer.ServiceName);

            SimpleRequestSoapMessage.Current = this._RequestMessage;

            if (SimpleRequestSoapMessage.Current.Context.ContainsKey("TenantCode"))
                TenantContext.Current.TenantCode = SimpleRequestSoapMessage.Current.Context.GetValue("TenantCode", string.Empty);

            this._NewStream.Position = 0;
        }

        private void WriteOutputStream(SoapMessage message)
        {
            this._NewStream.Position = 0;
            this._NewStream.CopyTo(this._OldStream);
            this._NewStream.Position = 0;

            ServiceMethodConfigurationElement methodSettings = GetMethodSettings(this._RequestMessage);

            if (message.Exception == null && this._RequestMessage.UseServerCache && methodSettings.CacheEnabled)
            {
                ServiceMethodCache methodCache = this.GetMethodCacheManager().GetOrAddNewValue(this._RequestMessage.MethodCacheKey, (cache, key) =>
                {
                    T result = this.CreateServiceMethodCache(this._RequestMessage.MethodCacheKey, methodSettings.QueueLength);

                    cache.Add(key, result);

                    return result;
                });

                using (StreamReader reader = new StreamReader(this._NewStream))
                {
                    string dataInCache = reader.ReadToEnd();

                    methodCache.Add(this._RequestMessage.Document.OuterXml, dataInCache, CreateDependency(methodSettings));
                }
            }
        }

        private void InitializeCounters(string instanceName)
        {
            if (this._GlobalInstance == null)
                this._GlobalInstance = new WebMethodServerCounters("_Total_");

            if (this._ActionInstance == null)
                this._ActionInstance = new WebMethodServerCounters(instanceName);
        }

        private void InitParameters(SimpleRequestSoapMessage message)
        {
            if (this._InstanceName == null)
            {
                this._InstanceName = message.Action + " of " + this._Initializer;
                this._InstanceName = this._InstanceName.Replace('/', '-');
                this._InstanceName = this._InstanceName.Replace('\\', '-');
            }

            if (this._ExecutionWatch == null)
            {
                this._ExecutionWatch = new Stopwatch();
                this._ExecutionWatch.Start();
            }
        }

        private static DependencyBase CreateDependency(ServiceMethodConfigurationElement methodSettings)
        {
            SlidingTimeDependency slidingTimeDependency = new SlidingTimeDependency(methodSettings.CacheSlidingTime);

            return new MixedDependency(new UdpNotifierCacheDependency(), slidingTimeDependency, new MemoryMappedFileNotifierCacheDependency());
        }

        private string GetDataInCache(SimpleRequestSoapMessage requestMessage)
        {
            string result = null;

            ServiceMethodCache methodCache = null;

            if (this.GetMethodCacheManager().TryGetValue(requestMessage.MethodCacheKey, out methodCache))
            {
                methodCache.TryGetValue(requestMessage.Document.OuterXml, out result);
            }

            return result;
        }

        private static ServiceMethodConfigurationElement GetMethodSettings(SimpleRequestSoapMessage requestMessage)
        {
            ServiceSettings settings = ServiceSettings.GetConfig();

            ServiceMethodConfigurationElement result = null;

            ServiceConfigurationElement serviceElement = settings.Services[requestMessage.ServiceName];

            if (serviceElement != null)
                result = serviceElement.Methods[requestMessage.Action];

            return result ?? settings.MethodDefaultSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._IsNewStreamCreator && this._NewStream != null)
                    this._NewStream.Dispose();
            }
        }
    }
}