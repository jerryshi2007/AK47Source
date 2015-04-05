using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Web.Library.Script;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace MCS.Library.WcfExtensions
{
    /// <summary>
    /// 服务端反序列化客户端的请求以及序列化返回结果
    /// </summary>
    public class WfJsonDispatchFormatter : IDispatchMessageFormatter
    {
        private readonly OperationDescription _OperationDesc;
        private readonly bool _AtlasEnabled;

        public WfJsonDispatchFormatter(OperationDescription operationDesc, bool atlasEnabled)
        {
            this._OperationDesc = operationDesc;
            this._AtlasEnabled = atlasEnabled;
        }

        public void DeserializeRequest(System.ServiceModel.Channels.Message message, object[] parameters)
        {
            object bodyFormatProperty;
            if (!message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out bodyFormatProperty) ||
                (bodyFormatProperty as WebBodyFormatMessageProperty).Format != WebContentFormat.Raw)
            {
                throw new InvalidOperationException("服务行为配置错误，请将WebHttpBinding的ContentTypeMapper属性设置为WfRawWebContentTypeMapper类型");
            }

            try
            {
                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DeserializeRequest", () =>
                    {
                        string jsonStr = WcfUtils.GetMessageRawContent(message);

                        Dictionary<string, object> paramsInfo = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(jsonStr);

                        Dictionary<string, object> headers = PopHeaderInfo(paramsInfo);
                        PushHeaderInfoToMessageProperties(message, headers);

                        Dictionary<string, string> connectionMappings = PopConnectionMappings(paramsInfo);
                        PushConnectionMappingsToMessageProperties(message, connectionMappings);

                        Dictionary<string, object> context = PopContextInfo(paramsInfo);
                        PushContextToMessageProperties(message, context);

                        GenericTicketTokenContainer container = PopGenericTicketTokenContainer(paramsInfo);
                        PushGenericTicketTokenContainer(message, container);

                        for (int i = 0; i < _OperationDesc.Messages[0].Body.Parts.Count; i++)
                        {
                            string paramName = _OperationDesc.Messages[0].Body.Parts[i].Name;
                            Type targetType = this._OperationDesc.Messages[0].Body.Parts[i].Type;

                            object val = paramsInfo[paramName];

                            try
                            {
                                parameters[i] = JSONSerializerExecute.DeserializeObject(val, targetType);
                            }
                            catch (System.Exception ex)
                            {
                                string errorMessage = string.Format("反序列化参数{0}错误，类型为{1}：{2}",
                                    paramName,
                                    targetType.ToString(),
                                    ex.Message);

                                throw new InvalidDataException(errorMessage, ex);
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("传入的JSON格式错误：" + ex.Message, ex);
            }
        }

        public System.ServiceModel.Channels.Message SerializeReply(System.ServiceModel.Channels.MessageVersion messageVersion, object[] parameters, object result)
        {
            Message returnMessage = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("SerializeReply", () =>
                {
                    //返回值总是Raw格式的
                    string jsonResult = string.Empty;

                    if (result is string)
                    {
                        if (this._AtlasEnabled)
                        {
                            //asp.net ajax 返回值格式
                            Dictionary<string, object> returnDict = new Dictionary<string, object>();
                            returnDict.Add("d", result);
                            jsonResult = JSONSerializerExecute.Serialize(returnDict);
                        }
                        else
                        {
                            jsonResult = result.ToString();
                        }
                    }
                    else
                    {
                        jsonResult = JSONSerializerExecute.SerializeWithType(result);
                    }

                    returnMessage = WcfUtils.CreateJsonFormatReplyMessage(messageVersion, this._OperationDesc.Messages[1].Action, jsonResult);
                });

            return returnMessage;
        }

        /// <summary>
        /// 从参数字典中分拣出__Headers信息，然后移除它
        /// </summary>
        /// <param name="paramsInfo"></param>
        /// <returns></returns>
        private Dictionary<string, object> PopHeaderInfo(Dictionary<string, object> paramsInfo)
        {
            object headerObject = null;

            if (paramsInfo.TryGetValue("__Headers", out headerObject) == false)
                headerObject = new Dictionary<string, object>();

            return (Dictionary<string, object>)headerObject;
        }

        /// <summary>
        /// 从参数字典中分拣出Context信息，然后移除它
        /// </summary>
        /// <param name="paramsInfo"></param>
        /// <returns></returns>
        private Dictionary<string, object> PopContextInfo(Dictionary<string, object> paramsInfo)
        {
            object contextObject = null;

            if (paramsInfo.TryGetValue("__Context", out contextObject) == false)
                contextObject = new Dictionary<string, object>();

            return (Dictionary<string, object>)contextObject;
        }

        private Dictionary<string, string> PopConnectionMappings(Dictionary<string, object> paramsInfo)
        {
            object mappingObject = null;

            if (paramsInfo.TryGetValue("__ConnectionMappings", out mappingObject) == false)
                mappingObject = new Dictionary<string, object>();

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (KeyValuePair<string, object> kp in (Dictionary<string, object>)mappingObject)
                result[kp.Key] = (string)kp.Value;

            return result;
        }

        private GenericTicketTokenContainer PopGenericTicketTokenContainer(Dictionary<string, object> paramsInfo)
        {
            GenericTicketTokenContainer container = null;

            object containerObject = null;

            if (paramsInfo.TryGetValue("__TokenContainer", out containerObject))
                container = containerObject as GenericTicketTokenContainer;

            return container;
        }

        private static void PushHeaderInfoToMessageProperties(System.ServiceModel.Channels.Message message, Dictionary<string, object> headers)
        {
            message.Properties["Headers"] = headers;
        }

        private static void PushConnectionMappingsToMessageProperties(System.ServiceModel.Channels.Message message, Dictionary<string, string> connectionMappings)
        {
            message.Properties["ConnectionMappings"] = connectionMappings;
        }

        private static void PushContextToMessageProperties(System.ServiceModel.Channels.Message message, Dictionary<string, object> context)
        {
            message.Properties["Context"] = context;
        }

        private static void PushGenericTicketTokenContainer(System.ServiceModel.Channels.Message message, GenericTicketTokenContainer container)
        {
            if (container != null)
                message.Properties["TokenContainer"] = container;
        }
    }
}
