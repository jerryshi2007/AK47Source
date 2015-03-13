using MCS.Library.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Services.Configuration
{
    /// <summary>
    /// ServiceMethodConfigurationElement配置相关的扩展方法
    /// </summary>
    public static class ServiceMethodConfigurationElementExtensions
    {
        /// <summary>
        /// 根据WCF的OperationContext找到对应的方法配置元素
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static ServiceMethodConfigurationElement GetMethodElementByOperationContext(this ServiceSettings settings)
        {
            ServiceMethodConfigurationElement result = null;

            if (settings != null && OperationContext.Current != null)
            {
                string contractName = OperationContext.Current.GetContractName();

                ServiceConfigurationElement serviceElement = settings.Services[contractName];

                if (serviceElement != null)
                {
                    string method = OperationContext.Current.GetMethodName();

                    result = serviceElement.Methods[method];

                    if (result == null)
                        result = settings.MethodDefaultSettings;
                }
            }

            return result;
        }
    }
}
