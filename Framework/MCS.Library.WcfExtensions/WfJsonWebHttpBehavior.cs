using MCS.Library.Core;
using MCS.Library.WcfExtensions.Configuration;
using MCS.Web.Library.Script;
using MCS.Web.Library.Script.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

namespace MCS.Library.WcfExtensions
{
	public class WfJsonWebHttpBehavior : WebHttpBehavior
	{
		public WfJsonWebHttpBehavior()
		{
			JSONSerializerExecute.RegisterConverter(typeof(WfErrorDTOConverter));

            JsonConverterRegisterSettings.GetConfig().GetRegisters().ForEach(r => r.RegisterConverters());
		}

		/// <summary>
		/// 获取Server端处理请求的格式化器
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			IDispatchMessageFormatter formatter = null;

			if (this.IsWebGetOperation(operationDescription))
				formatter = base.GetRequestDispatchFormatter(operationDescription, endpoint);
			else
				if (IsOperationHasParams(operationDescription) == false)
					formatter = base.GetRequestDispatchFormatter(operationDescription, endpoint);
				else
					if (operationDescription.Behaviors.Find<WfJsonFormatterAttribute>() == null)
						formatter = base.GetRequestDispatchFormatter(operationDescription, endpoint);
					else
						formatter = CreateDispatchFormatter(operationDescription, endpoint);

			return formatter;
		}

		/// <summary>
		/// 获取Server端处理响应的格式化器
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			IDispatchMessageFormatter formatter = null;

			if (IsOperationReturnVoid(operationDescription) || operationDescription.Behaviors.Find<WfJsonFormatterAttribute>() == null)
				formatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
			else
			{
				formatter = formatter = CreateDispatchFormatter(operationDescription, endpoint);
			}

			return formatter;
		}

		/// <summary>
		/// 获取Client端处理请求的格式化器
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			IClientMessageFormatter formatter = null;

			if (this.IsWebGetOperation(operationDescription))
			{
				formatter = base.GetRequestClientFormatter(operationDescription, endpoint);
			}
			else
			{
				if (IsOperationHasParams(operationDescription) == false)
				{
					formatter = base.GetRequestClientFormatter(operationDescription, endpoint);
				}
				else
				{
					formatter = new WfJsonClientFormatter(operationDescription, endpoint);
				}
			}

			return formatter;
		}

		/// <summary>
		/// 获取Client端处理响应的格式化器
		/// </summary>
		/// <param name="operationDescription"></param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected override IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			if (operationDescription.Messages.Count == 1 || operationDescription.Messages[1].Body.ReturnValue.Type == typeof(void))
			{
				return base.GetReplyClientFormatter(operationDescription, endpoint);
			}
			else
			{
				return new WfJsonClientFormatter(operationDescription, endpoint);
			}
		}

		protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new WfErrorHandler());
		}

        protected override void AddClientErrorInspector(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new WfClientErrorInspector());

            base.AddClientErrorInspector(endpoint, clientRuntime);
        }

		#region private method
		private static bool IsOperationReturnVoid(OperationDescription operationDescription)
		{
			return operationDescription.Messages.Count == 1 || operationDescription.Messages[1].Body.ReturnValue.Type == typeof(void);
		}

		private static bool IsOperationHasParams(OperationDescription operationDescription)
		{
			return operationDescription.Messages[0].Body.Parts.Count != 0;
		}

		private bool IsWebGetOperation(OperationDescription operation)
		{
			WebGetAttribute webGetAttr = operation.Behaviors.Find<WebGetAttribute>();
			if (webGetAttr != null)
			{
				return true;
			}

			WebInvokeAttribute webInvokeAttr = operation.Behaviors.Find<WebInvokeAttribute>();
			if (webInvokeAttr != null)
			{
				return webInvokeAttr.Method == "HEAD";
			}

			return false;
		}

		private WfJsonDispatchFormatter CreateDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
		{
			bool atlasEnable = endpoint.Behaviors.Find<WfWebScriptBehavior>() == null ? false : true;
			return new WfJsonDispatchFormatter(operationDescription, atlasEnable);
		}
		#endregion
	}
}
