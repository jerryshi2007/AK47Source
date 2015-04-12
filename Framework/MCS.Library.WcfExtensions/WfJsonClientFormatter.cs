using MCS.Library.Core;
using MCS.Library.Data.Configuration;
using MCS.Library.Passport;
using MCS.Library.WcfExtensions.Configuration;
using MCS.Web.Library.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace MCS.Library.WcfExtensions
{
    /// <summary>
    /// 客户端上行和下行的Json的参数封装
    /// </summary>
    public class WfJsonClientFormatter : IClientMessageFormatter
    {
        private OperationDescription _OperationDesc;
        private Uri _MessageDestinationUri;

        public WfJsonClientFormatter(OperationDescription operation, ServiceEndpoint endpoint)
        {
            this._OperationDesc = operation;

            string endpointAddress = endpoint.Address.Uri.ToString();
            if (!endpointAddress.EndsWith("/"))
            {
                endpointAddress = endpointAddress + "/";
            }

            this._MessageDestinationUri = new Uri(endpointAddress + operation.Name);
        }

        /// <summary>
        /// 反序列化服务调用的返回结果
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object DeserializeReply(System.ServiceModel.Channels.Message message, object[] parameters)
        {
            object bodyFormatProperty;

            if (message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty) == false ||
                (bodyFormatProperty as WebBodyFormatMessageProperty).Format != WebContentFormat.Raw)
            {
                throw new InvalidOperationException("服务行为配置错误，请将WebHttpBinding的ContentTypeMapper属性设置为WfRawWebContentTypeMapper类型");
            }

            try
            {
                string jsonStr = WcfUtils.GetMessageRawContent(message);

                return JSONSerializerExecute.DeserializeObject(jsonStr, _OperationDesc.Messages[1].Body.ReturnValue.Type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("传入的JSON格式错误：" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 序列化服务调用的请求信息
        /// </summary>
        /// <param name="messageVersion"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public System.ServiceModel.Channels.Message SerializeRequest(System.ServiceModel.Channels.MessageVersion messageVersion, object[] parameters)
        {
            Dictionary<string, object> paramNameValuePair = new Dictionary<string, object>();

            for (int i = 0; i < _OperationDesc.Messages[0].Body.Parts.Count; i++)
            {
                string paramName = _OperationDesc.Messages[0].Body.Parts[i].Name;
                object paramVal = parameters[i];

                paramNameValuePair.Add(paramName, paramVal);
            }

            paramNameValuePair["__ConnectionMappings"] = GetConnectionMappings();
            paramNameValuePair["__Context"] = WfClientServiceBrokerContext.Current.Context;

            //if (WfClientServiceBrokerContext.Current.Context.ContainsKey("TenantCode") == false &&
            //    TenantContext.Current.Enabled)
            if (TenantContext.Current.Enabled)
                WfClientServiceBrokerContext.Current.Context["TenantCode"] = TenantContext.Current.TenantCode;

            IGenericTokenPrincipal principal = GetPrincipal(WfClientServiceBrokerContext.Current);

            if (principal != null)
                paramNameValuePair["__TokenContainer"] = principal.GetGenericTicketTokenContainer();

            string paramJson = JSONSerializerExecute.SerializeWithType(paramNameValuePair);
            Message requestMessage = WcfUtils.CreateJsonFormatRequestMessage(messageVersion, _OperationDesc.Messages[0].Action, paramJson);
            requestMessage.Headers.To = _MessageDestinationUri;

            return requestMessage;
        }

        /// <summary>
        /// 从配置文件中初始化连接映射
        /// </summary>
        private static Dictionary<string, string> GetConnectionMappings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (ConnectionMappingElement mappingElement in WfServiceInvokerSettings.GetConfig().ConnectionMappings)
                result[mappingElement.Name] = mappingElement.Destination;

            foreach (KeyValuePair<string, string> kp in WfClientServiceBrokerContext.Current.ConnectionMappings)
                result[kp.Value] = kp.Value;

            return result;
        }

        private static IGenericTokenPrincipal GetPrincipal(WfClientServiceBrokerContext context)
        {
            IGenericTokenPrincipal principal = PrincipaContextAccessor.GetPrincipalInContext<IGenericTokenPrincipal, WfClientServiceBrokerContext>(context);

            if (principal == null)
                principal = PrincipaContextAccessor.GetPrincipal<IGenericTokenPrincipal>();

            return principal;
        }
    }
}
