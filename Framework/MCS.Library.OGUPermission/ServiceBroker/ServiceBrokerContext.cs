using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// 调用服务的上下文
    /// </summary>
    [Serializable]
    [ActionContextDescription("ServiceBrokerContext")]
    public class ServiceBrokerContext : ServiceBrokerContextBase<ServiceBrokerContext>
    {
        private ListObjectMask listObjectCondition = ListObjectMask.Common;

        //private static object AddNewContext(ContextCacheQueueBase<object, object> cache, object key)
        //{
        //    ServiceBrokerContext context = new ServiceBrokerContext();

        //    OguPermissionSettings config = OguPermissionSettings.GetConfig();

        //    context.Timeout = config.Timeout;
        //    context.UseLocalCache = config.UseLocalCache;
        //    context.UseServerCache = config.UseServerCache;

        //    foreach (OguConnectionMappingElement cm in config.ConnectionMappings)
        //        context.ConnectionMappings[cm.Name] = cm.Destination;

        //    cache.Add(key, context);

        //    return context;
        //}

        /// <summary>
        /// 查询删除对象的掩码
        /// </summary>
        public ListObjectMask ListObjectCondition
        {
            get { return this.listObjectCondition; }
            set { this.listObjectCondition = value; }
        }

        /// <summary>
        /// 初始化Broker的属性
        /// </summary>
        protected override void InitProperties()
        {
            OguPermissionSettings settings = OguPermissionSettings.GetConfig();

            this.Timeout = settings.Timeout;
            this.UseLocalCache = settings.UseLocalCache;
            this.UseServerCache = settings.UseServerCache;

            foreach (OguConnectionMappingElement cm in settings.ConnectionMappings)
                this.ConnectionMappings[cm.Name] = cm.Destination;

            if (TenantContext.Current.TenantCode.IsNotEmpty())
                this.Context["TenantCode"] = TenantContext.Current.TenantCode;
        }
    }
}
