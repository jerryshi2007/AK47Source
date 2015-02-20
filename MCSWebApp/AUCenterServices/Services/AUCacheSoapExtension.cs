using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;
using MCS.Library.Caching;
using MCS.Library.Data;
using MCS.Library.Services;

namespace AUCenterServices.Services
{
    public class AUCacheSoapExtension : CacheSoapExtensionBase<AUServiceMethodCache>
    {
        public AUCacheSoapExtension()
        {
        }

        protected override SimpleRequestSoapMessage CreateMessage(Stream stream, string serviceName)
        {
            SimpleRequestSoapMessage message = null;

            if (string.Compare(serviceName, typeof(AUCenterQueryService).FullName, true) == 0)
                message = base.CreateMessage(stream, serviceName);
            else
            {
                message = UpdatableRequestSoapMessage.CreateMessage(stream);
                message.ServiceName = serviceName;
            }

            return message;
        }

        protected override void BeforeExecuteServerMethod(SoapMessage message)
        {
            InitConnectionMappings();
        }

        protected override AUServiceMethodCache CreateServiceMethodCache(string instanceName, int maxLength)
        {
            return new AUServiceMethodCache(instanceName, maxLength);
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