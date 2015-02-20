using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.Web.Script.Services;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Net;
using System.Globalization;
using System.ServiceModel;
using System.Web;

namespace MCS.Library.WcfExtensions
{
	/// <summary>
	/// 用于Asp.net Ajax Service Reference时产生Javascript脚本
	/// </summary>
	class WfWebScriptMetadataInvoker : IOperationInvoker
	{
		private ServiceEndpoint Endpoint { get; set; }
		private DispatchOperation DefaultUnhandledDispatchOperation { get; set; }

		/// <summary>
		/// 服务最后更改时间
		/// </summary>
		internal static DateTime ServiceLastModified { get; set; }

		static WfWebScriptMetadataInvoker()
		{
			ServiceLastModified = DateTime.UtcNow;
			ServiceLastModified = new DateTime(ServiceLastModified.Year, ServiceLastModified.Month,
				ServiceLastModified.Day, ServiceLastModified.Hour, ServiceLastModified.Minute,
				ServiceLastModified.Second);
		}

		public WfWebScriptMetadataInvoker(ServiceEndpoint endpoint, DispatchOperation dispatchOperation)
		{
			Endpoint = endpoint;
			this.DefaultUnhandledDispatchOperation = dispatchOperation;
		}

		public object[] AllocateInputs()
		{
			return null;
		}

		public object Invoke(object instance, object[] inputs, out object[] outputs)
		{
			outputs = null;

			var request = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest;
			var endpointUrl = RewriteUri(this.Endpoint.Address.Uri, request.Headers["HOST"]).AbsoluteUri;
			var requestUrl = RewriteUri(OperationContext.Current.IncomingMessageProperties.Via, request.Headers["HOST"]).AbsoluteUri;
			var scriptUrl = endpointUrl + (endpointUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? WfWebScriptBehavior.METADATA_ENDPOINT_SUFFIX
				: "/" + WfWebScriptBehavior.METADATA_ENDPOINT_SUFFIX);
			var scriptDebugUrl = endpointUrl + (endpointUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? WfWebScriptBehavior.DEBUG_METADATA_ENDPOINT_SUFFIX
				: "/" + WfWebScriptBehavior.DEBUG_METADATA_ENDPOINT_SUFFIX);

			if (requestUrl == scriptUrl || requestUrl == scriptDebugUrl)
			{
				Message replyMsg = null;
				if (IsServiceUnChanged())
				{
					replyMsg = CreateNotModifiedMessage();
				}
				else
				{
					string scriptContent = ProxyGenerator.GetClientProxyScript(this.Endpoint.Contract.ContractType,
							endpointUrl, false, this.Endpoint);

					replyMsg = CreateMetadataMessage(scriptContent);
				}
				return replyMsg;
			}

			if (inputs == null)
			{
				inputs = new object[1];
			}
			inputs[0] = OperationContext.Current.RequestContext.RequestMessage; ;

			return this.DefaultUnhandledDispatchOperation.Invoker.Invoke(instance, inputs, out outputs);
		}

		public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
		{
			throw new NotImplementedException();
		}

		public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public bool IsSynchronous
		{
			get { return true; }
		}

		internal static Uri RewriteUri(Uri uri, string host)
		{
			if (!string.IsNullOrEmpty(host) && !string.Equals(uri.Host + (!uri.IsDefaultPort ? (":" + uri.Port.ToString(CultureInfo.InvariantCulture)) : string.Empty), host, StringComparison.OrdinalIgnoreCase))
			{
				Uri uri2 = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}", new object[] { uri.Scheme, host }));
				UriBuilder builder = new UriBuilder(uri)
				{
					Host = uri2.Host,
					Port = uri2.Port
				};
				return builder.Uri;
			}
			return uri;
		}

		private static bool IsServiceUnChanged()
		{
			var requestMessage = OperationContext.Current.RequestContext.RequestMessage;
			HttpRequestMessageProperty property = requestMessage.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;

			if (property == null) return false;


			if (string.IsNullOrEmpty(property.Headers["If-Modified-Since"])) return false;

			DateTime httpHeaderTime;

			var parseResult = DateTime.TryParse(property.Headers["If-Modified-Since"],
				DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out httpHeaderTime);

			if (!parseResult) return false;

			return httpHeaderTime >= ServiceLastModified;
		}

		private static Message CreateMetadataMessage(string msgContent)
		{
			var bodyBytes = Encoding.UTF8.GetBytes(msgContent);

			Message replyMessage = Message.CreateMessage(MessageVersion.None, string.Empty, new WfRawMessageBodyWriter(bodyBytes));
			replyMessage.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

			var lastModifiedStr = ServiceLastModified.ToString("R", DateTimeFormatInfo.InvariantInfo);
			HttpResponseMessageProperty respProp = new HttpResponseMessageProperty();
			respProp.Headers[HttpResponseHeader.ContentType] = "application/x-javascript";
			respProp.Headers.Add("Last-Modified", lastModifiedStr);
			respProp.Headers.Add("Expires", lastModifiedStr);
			if (HttpContext.Current != null)
			{
				HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
			}
			else
			{
				respProp.Headers.Add("Cache-Control", "public");
			}

			replyMessage.Properties.Add(HttpResponseMessageProperty.Name, respProp);
			return replyMessage;
		}

		private static Message CreateNotModifiedMessage()
		{
			HttpResponseMessageProperty responseProp = new HttpResponseMessageProperty();
			var replyMsg = Message.CreateMessage(MessageVersion.None, string.Empty);
			responseProp.StatusCode = HttpStatusCode.NotModified;
			replyMsg.Properties.Add(HttpResponseMessageProperty.Name, responseProp);
			return replyMsg;
		}
	}
}
