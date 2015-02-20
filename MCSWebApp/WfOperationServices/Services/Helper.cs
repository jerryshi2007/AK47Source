using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;
using MCS.Library.OGUPermission;
using MCS.Library.Core;

namespace WfOperationServices.Services
{
    public static class Helper
    {
        /// <summary>
        /// 将当前Operation中的Header Info填写到ServiceBrokerContext中，为调用机构人员的服务设置上下文
        /// </summary>
        public static void FillContextToOguServiceContext(this OperationContext opContext)
        {
            if (opContext != null)
            {
                if (opContext.IncomingMessageProperties.ContainsKey("Context"))
                {
                    Dictionary<string, object> headers = (Dictionary<string, object>)opContext.IncomingMessageProperties["Context"];

                    foreach (KeyValuePair<string, object> kp in headers)
                        ServiceBrokerContext.Current.Context[kp.Key] = kp.Value != null ? kp.Value.ToString() : string.Empty;

                    object tenantCode = null;

                    if (headers.TryGetValue("TenantCode", out tenantCode))
                        TenantContext.Current.TenantCode = (tenantCode != null) ? tenantCode.ToString() : string.Empty;
                }
            }
        }
    }
}