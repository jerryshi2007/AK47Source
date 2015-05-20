using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;
using MCS.Library.Caching;
using MCS.Library.Data;
using MCS.Library.Services;
using MCS.Library.Services.Configuration;
using PermissionCenter.Services;

namespace PermissionCenter.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class PCCacheSoapExtension : CacheSoapExtensionBase<PCServiceMethodCache>
    {
        protected override SimpleRequestSoapMessage CreateMessage(Stream stream, string serviceName)
        {
            SimpleRequestSoapMessage message = null;

            if (serviceName == typeof(PermissionCenterUpdateService).FullName)
            {
                message = UpdatableRequestSoapMessage.CreateMessage(stream);
                message.ServiceName = serviceName;
            }
            else
                return base.CreateMessage(stream, serviceName);

            return message;
        }

        protected override void BeforeExecuteServerMethod(SoapMessage message)
        {
            this.InitConnectionMappings();
        }

        protected override PCServiceMethodCache CreateServiceMethodCache(string instanceName, int maxLength)
        {
            return new PCServiceMethodCache(instanceName, maxLength);
        }

        private void InitConnectionMappings()
        {
            foreach (KeyValuePair<string, string> kp in this.RequestMessage.ConnectionMappings)
            {
                DbConnectionMappingContext.CreateMapping(kp.Key, kp.Value);
            }
        }
    }
}