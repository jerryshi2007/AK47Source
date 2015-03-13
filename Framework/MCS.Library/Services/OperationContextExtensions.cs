using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Services
{
    /// <summary>
    /// OperationContext的扩展方法
    /// </summary>
    public static class OperationContextExtensions
    {
        /// <summary>
        /// 将当前Operation中的Header Info填写到ServiceBrokerContext中，为调用机构人员的服务设置上下文
        /// </summary>
        public static string GetContractName(this OperationContext opContext)
        {
            string result = string.Empty;

            if (opContext != null)
                result = opContext.EndpointDispatcher.ContractName;

            return result;
        }

        /// <summary>
        /// 得到方法的名称
        /// </summary>
        /// <param name="opContext"></param>
        /// <returns></returns>
        public static string GetMethodName(this OperationContext opContext)
        {
            string result = string.Empty;

            if (opContext != null)
            {
                string path = OperationContext.Current.IncomingMessageHeaders.To.PathAndQuery;
                result = Path.GetFileName(path);
            }

            return result;
        }
    }
}
