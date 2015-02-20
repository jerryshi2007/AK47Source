using MCS.Library.Data;
using MCS.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Ogu
{
    public class WSCacheSoapExtension : CacheSoapExtensionBase<WSServiceMethodCache>
    {
        protected override void BeforeExecuteServerMethod(System.Web.Services.Protocols.SoapMessage message)
        {
            InitConnectionMappings();
        }
        protected override WSServiceMethodCache CreateServiceMethodCache(string instanceName, int maxLength)
        {
            return new WSServiceMethodCache(instanceName, maxLength);
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
