﻿using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.Configuration;
using MCS.Library.Passport;
using MCS.Library.WcfExtensions;
using MCS.Library.WcfExtensions.Configuration;
using MCS.Web.Library.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 服务调用
    /// </summary>
    public class WfServiceInvoker
    {
        private WfServiceOperationDefinition _SvcOperationDef = null;
        private WebHeaderCollection _Headers = null;
        private Dictionary<string, string> _ConnectionMappings = null;
        private WfApplicationRuntimeParameters _InvokeContext = null;

        public WfServiceInvoker(WfServiceOperationDefinition svcOperationDef)
        {
            this._SvcOperationDef = svcOperationDef;

            this.InitConnectionMappings();
            this.InitHeaders();
        }

        public WfServiceInvoker(WfServiceOperationDefinition svcOperationDef, WfApplicationRuntimeParameters invokeContext)
        {
            this._SvcOperationDef = svcOperationDef;
            this._InvokeContext = invokeContext;

            this.InitConnectionMappings();
            this.InitHeaders();
        }

        /// <summary>
        /// Http请求时的Header
        /// </summary>
        public WebHeaderCollection Headers
        {
            get
            {
                if (this._Headers == null)
                    this._Headers = new WebHeaderCollection();

                return this._Headers;
            }
        }

        /// <summary>
        /// 连接串映射
        /// </summary>
        public Dictionary<string, string> ConnectionMappings
        {
            get
            {
                if (this._ConnectionMappings == null)
                    this._ConnectionMappings = new Dictionary<string, string>();

                return this._ConnectionMappings;
            }
        }

        /// <summary>
        /// 调用时的参数上下文
        /// </summary>
        public static WfApplicationRuntimeParameters InvokeContext
        {
            get
            {
                WfApplicationRuntimeParameters result = null;

                if (WfRuntime.ProcessContext.CurrentProcess != null)
                    result = WfRuntime.ProcessContext.CurrentProcess.ApplicationRuntimeParameters;
                else
                {
                    result = (WfApplicationRuntimeParameters)ObjectContextCache.Instance.GetOrAddNewValue("WfServiceInvoker.InvokeContext", (cache, key) =>
                    {
                        WfApplicationRuntimeParameters parameters = new WfApplicationRuntimeParameters();

                        cache.Add(key, parameters);

                        return parameters;
                    });
                }

                return result;
            }
        }

        /// <summary>
        /// 调用服务，默认超时30秒
        /// </summary>
        /// <returns></returns>
        public object Invoke()
        {
            return Invoke(this._SvcOperationDef.Timeout);
        }

        /// <summary>
        /// 调用服务，默认超时30秒
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public object Invoke(WfApplicationRuntimeParameters context)
        {
            return Invoke(this._SvcOperationDef.Timeout, context);
        }

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <param name="timeout">请求超时时间，单位毫秒</param>
        /// <returns></returns>
        public object Invoke(TimeSpan timeout)
        {
            return this.Invoke(timeout, null);
        }

        /// <summary>
        /// 调用服务
        /// </summary>
        /// <param name="timeout">请求超时时间，单位毫秒</param>
        /// <param name="context">上下文参数</param>
        /// <returns></returns>
        public object Invoke(TimeSpan timeout, WfApplicationRuntimeParameters context)
        {
            try
            {
                if (context == null)
                    context = WfServiceInvoker.InvokeContext;

                HttpWebRequest request = GenerateWebRequestObj(timeout, context);

                try
                {
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            object result = null;

                            if (stream != null)
                            {
                                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                                string rtnContent = streamReader.ReadToEnd();

                                result = ParseServiceResultToObject(rtnContent);

                                if (result == null)
                                {
                                    result = ExceptionHelper.DoSilentFunc(() => JSONSerializerExecute.DeserializeObject(rtnContent), rtnContent);

                                    if (result is WfErrorDTO)
                                    {
                                        string errorMessage = ((WfErrorDTO)result).ToString() + Environment.NewLine + request.RequestUri.ToString();

                                        throw new WfServiceInvokeException(((WfErrorDTO)result).ToString());
                                    }
                                }
                            }

                            if (this._SvcOperationDef.RtnXmlStoreParamName.IsNotEmpty())
                                context[this._SvcOperationDef.RtnXmlStoreParamName] = result;

                            return result;
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response == null)
                        throw new WfServiceInvokeException(string.Format("调用服务时发生了异常，{0}，但无响应内容。HTTP状态为{1}", ex.Message, ex.Status), ex);
                    else
                        throw WfServiceInvokeException.FromWebResponse(ex.Response);
                }
            }
            catch (WebException ex)
            {
                throw new WfServiceInvokeException(ex.Message, ex);
            }
        }

        private HttpWebRequest GenerateWebRequestObj(TimeSpan timeout, WfApplicationRuntimeParameters context)
        {
            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(this._SvcOperationDef.AddressDef.Address), "服务地址定义不能为空.");
            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(this._SvcOperationDef.OperationName), "调用方法名称不能为空.");

            HttpWebRequest request;

            switch (this._SvcOperationDef.AddressDef.RequestMethod)
            {
                case WfServiceRequestMethod.Get:
                    request = this.CreateGetRequest(timeout, context);
                    break;
                case WfServiceRequestMethod.Post:
                    request = this.CreatePostRequest(timeout, context);
                    break;
                case WfServiceRequestMethod.Soap:
                    request = this.CreateSoapRequest(timeout);
                    break;
                default:
                    throw new NotImplementedException();
            };

            if (this._SvcOperationDef.AddressDef.Credential != null)
                request.Credentials = (NetworkCredential)this._SvcOperationDef.AddressDef.Credential;

            return request;
        }

        private string GetJsonValue(string json, string key)
        {
            string keyStr = "\"" + key + "\":";
            int startIndex = json.IndexOf(keyStr);

            if (startIndex < 0)
                return string.Empty;

            int endIndex = json.IndexOf("\"", startIndex + keyStr.Length + 1);

            if (endIndex < 0)
                return string.Empty;

            return json.Substring(startIndex + keyStr.Length + 1, endIndex - startIndex - keyStr.Length - 1);
        }

        /// <summary>
        /// 解析Web Service 返回的Xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static object ParseServiceResultToObject(string xml)
        {
            XDocument doc = null;

            if (string.IsNullOrEmpty(xml))
                return null;

            ExceptionHelper.DoSilentAction(() => doc = XDocument.Parse(xml));

            XElement firstElement = (XElement)doc.FirstNode;

            if (firstElement == null)
                return null;

            if (string.IsNullOrEmpty(firstElement.Value))
                return null;

            object result;
            switch (firstElement.Name.LocalName.ToLower())
            {
                case "int":
                    result = int.Parse(firstElement.Value);
                    break;
                case "decimal":
                    result = decimal.Parse(firstElement.Value);
                    break;
                case "double":
                    result = double.Parse(firstElement.Value);
                    break;
                case "datetime":
                    result = DateTime.Parse(firstElement.Value);
                    break;
                case "timespan":
                    result = TimeSpan.Parse(firstElement.Value);
                    break;
                case "boolean":
                    result = bool.Parse(firstElement.Value);
                    break;
                case "string":
                    result = firstElement.Value;
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }

        private HttpWebRequest CreateGetRequest(TimeSpan timeout, WfApplicationRuntimeParameters context)
        {
            string url = string.Concat(FormatUrl(this._SvcOperationDef.AddressDef.Address, true),
                HttpUtility.UrlEncode(this._SvcOperationDef.OperationName));

            string strQuery = this.CreateQueryString(context);

            if (strQuery.IsNotEmpty())
                url = string.Format("{0}?{1}", url, strQuery);

            HttpWebRequest result = (HttpWebRequest)HttpWebRequest.Create(url);

            result.Timeout = (int)timeout.TotalMilliseconds;
            result.Method = "GET";
            result.ContentType = "text/xml";
            result.KeepAlive = false;
            result.ProtocolVersion = HttpVersion.Version10;
            result.Headers.CopyFrom(this.Headers);

            return result;
        }

        private HttpWebRequest CreatePostRequest(TimeSpan timeout, WfApplicationRuntimeParameters context)
        {
            HttpWebRequest result = (HttpWebRequest)HttpWebRequest.Create(FormatUrl(_SvcOperationDef.AddressDef.Address, true)
                + HttpUtility.UrlEncode(_SvcOperationDef.OperationName));

            result.Method = "POST";
            result.Timeout = (int)timeout.TotalMilliseconds;
            result.KeepAlive = false;
            result.ProtocolVersion = HttpVersion.Version10;
            result.Headers.CopyFrom(this.Headers);
            
            string postData = string.Empty;

            switch (_SvcOperationDef.AddressDef.ContentType)
            {
                case WfServiceContentType.Form:
                    result.ContentType = "application/x-www-form-urlencoded";
                    postData = this.CreateQueryString(context);
                    break;
                case WfServiceContentType.Json:
                    result.ContentType = "application/json";
                    postData = this.CreateJsonData(context);
                    break;
                default: break;
            }

            AttachDataToRequest(result, postData);
            return result;
        }

        private HttpWebRequest CreateSoapRequest(TimeSpan timeout)
        {
            string url = FormatUrl(this._SvcOperationDef.AddressDef.Address, false);

            HttpWebRequest result = (HttpWebRequest)HttpWebRequest.Create(url);
            result.Method = "POST";
            result.Timeout = (int)timeout.TotalMilliseconds;
            result.ContentType = "text/xml; charset=utf-8"; //soap1.1
            result.KeepAlive = false;
            result.ProtocolVersion = HttpVersion.Version10;
            result.Headers.CopyFrom(this.Headers);

            result.Headers.Add("SOAPAction", FormatUrl(this._SvcOperationDef.AddressDef.ServiceNS, true) + _SvcOperationDef.OperationName);

            string postXml = CreateSoapEnvelope(_SvcOperationDef.Params);

            AttachDataToRequest(result, postXml);

            return result;
        }

        private static void AttachDataToRequest(HttpWebRequest targetRequest, string strData)
        {
            byte[] postData = Encoding.UTF8.GetBytes(strData);
            targetRequest.ContentLength = postData.Length;

            using (Stream reqWriter = targetRequest.GetRequestStream())
            {
                reqWriter.Write(postData, 0, postData.Length);
            }
        }

        private static string FormatUrl(string url, bool hasSlash)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            if (hasSlash)
            {
                if (url.Last() == '/')
                {
                    return url;
                }

                return url + "/";
            }
            else
            {
                if (url.Last() == '/')
                {
                    return url.Remove(url.Length - 1, 1);
                }

                return url;
            }
        }

        /// <summary>
        /// 构造请求字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string CreateQueryString(WfApplicationRuntimeParameters context)
        {
            StringBuilder result = new StringBuilder();
            foreach (var item in this._SvcOperationDef.Params)
            {
                if (result.Length > 0)
                {
                    result.Append("&");
                }

                result.Append(HttpUtility.UrlEncode(item.Name));
                result.Append("=");

                if (item.Type == WfSvcOperationParameterType.RuntimeParameter)
                {
                    var paraName = item.Value != null ? item.Value.ToString() : "";

                    if (string.IsNullOrEmpty(paraName))
                        paraName = item.Name;		//流程运行时参数名与方法参数名相同

                    string paraVal = context.GetValueRecursively(paraName, string.Empty);

                    result.Append(HttpUtility.UrlEncode(paraVal));
                }
                else
                {
                    if (item.Value != null)
                        result.Append(HttpUtility.UrlEncode(item.Value.ToString()));
                }
            }

            return result.ToString();
        }

        private string CreateJsonData(WfApplicationRuntimeParameters context)
        {
            string result = string.Empty;

            if (_SvcOperationDef.Params.Count > 0)
            {
                Dictionary<string, object> jsonDict = new Dictionary<string, object>();

                Dictionary<string, object> headers = new Dictionary<string, object>();

                foreach (string key in this.Headers.AllKeys)
                    headers[key] = this.Headers[key];

                jsonDict["__Headers"] = headers;
                jsonDict["__ConnectionMappings"] = this.ConnectionMappings;

                foreach (var item in this._SvcOperationDef.Params)
                {
                    if (item.Type == WfSvcOperationParameterType.RuntimeParameter)
                    {
                        var paraName = item.Value != null ? item.Value.ToString() : string.Empty;

                        if (paraName.IsNullOrEmpty())
                            paraName = item.Name;		//流程运行时参数名与方法参数名相同

                        var paramValue = context.GetValueRecursively<object>(paraName, null);

                        if (paramValue == null)
                            throw new ArgumentException("未能在CurrentProcess.ApplicationRuntimeParameters中找到参数" + paraName);

                        jsonDict.Add(item.Name, paramValue);
                    }
                    else
                    {
                        jsonDict.Add(item.Name, item.Value);
                    }
                }

                WfConverterHelper.RegisterConverters();

                result = JSONSerializerExecute.Serialize(jsonDict);
            }

            return result;
        }

        public string CreateSoapEnvelope(WfServiceOperationParameterCollection operationParams)
        {
            if (this._SvcOperationDef == null)
                throw new ArgumentNullException("WfServiceOperationDefinition不能为空！");

            if (this._SvcOperationDef.OperationName.IsNullOrEmpty())
                throw new ArgumentNullException("OperationName不能为空！");

            XNamespace methodNS = this._SvcOperationDef.AddressDef.ServiceNS;
            XElement operationElement = new XElement(methodNS + this._SvcOperationDef.OperationName);

            foreach (WfServiceOperationParameter paramDef in operationParams)
            {
                XElement paraElement = new XElement(paramDef.Name);

                if (paramDef.Type == WfSvcOperationParameterType.RuntimeParameter)
                {
                    Type dataType = paramDef.Value.GetType();		//mark 须从流程上下文取值，目前只是为了方便
                    PropertyInfo[] propsInfo = dataType.GetProperties();

                    //mark 用反射属性？还是用xml serialize？
                    foreach (PropertyInfo item in propsInfo)
                    {
                        object propVal = item.GetValue(paramDef.Value, null);

                        if (propVal == null)
                            continue;

                        paraElement.Add(new XElement(item.Name, propVal));
                    }
                }

                operationElement.Add(paraElement);
            }

            XElement envelopeElement = XElement.Parse(@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body></soap:Body></soap:Envelope>");
            XElement bodyElement = (XElement)envelopeElement.FirstNode;

            bodyElement.Add(operationElement);

            XDeclaration xmlDeclare = new XDeclaration("1.0", "utf-8", "");
            XDocument doc = new XDocument(xmlDeclare, envelopeElement);

            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                writer.Flush();

                //mark
                string trashStr = @" xmlns=""""";

                return writer.ToString().Replace("utf-16", "utf-8").Replace(trashStr, "");
            }
        }

        /// <summary>
        /// 从配置文件中初始化连接映射
        /// </summary>
        private void InitConnectionMappings()
        {
            foreach (ConnectionMappingElement mappingElement in WfServiceInvokerSettings.GetConfig().ConnectionMappings)
            {
                this.ConnectionMappings[mappingElement.Name] = mappingElement.Destination;
            }
        }

        private void InitHeaders()
        {
            this.Headers[TenantExtensions.TenantCodeParamName] = HttpUtility.UrlEncode(TenantContext.Current.TenantCode);
        }
    }
}
