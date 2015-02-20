using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;
using System.ServiceModel;

namespace MCS.Library.WcfExtensions
{
	public class WfWebScriptBehavior : IEndpointBehavior
	{
		internal const string DEBUG_METADATA_ENDPOINT_SUFFIX = "jsdebug";
		internal const string METADATA_ENDPOINT_SUFFIX = "js";

		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{

		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
		{

		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
		{
			DispatchOperation operation = new DispatchOperation(endpointDispatcher.DispatchRuntime, "*", "*", "*")
			{
				Formatter = new WfWebScriptMetadataFomatter(),
				Invoker = new WfWebScriptMetadataInvoker(endpoint, endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation)
			};

			endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation = operation;
			//AddMetadataEndpoint(endpoint, endpointDispatcher, true);
			//AddMetadataEndpoint(endpoint, endpointDispatcher, false);
		}

		public void Validate(ServiceEndpoint endpoint)
		{

		}

		private void AddMetadataEndpoint(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher, bool debugMode)
		{
			var uri = endpoint.Address.Uri;
			if (uri == null)
			{
				return;
			}
			string pathSuffix = debugMode ? DEBUG_METADATA_ENDPOINT_SUFFIX : METADATA_ENDPOINT_SUFFIX;
			string path = uri.AbsoluteUri + (uri.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? pathSuffix : ("/" + pathSuffix));

			Uri metadataUri = new Uri(path);
			ServiceHostBase host = endpointDispatcher.ChannelDispatcher.Host;
			var channelDispatcher = BuildChannelDispatcher(metadataUri, host);
			var metadataEdpDispatcher = BuildEndpointDispatcher(metadataUri, host, endpoint);
			channelDispatcher.Endpoints.Add(metadataEdpDispatcher);
			channelDispatcher.IncludeExceptionDetailInFaults = true;
			host.ChannelDispatchers.Add(channelDispatcher);
		}

		private ChannelDispatcher BuildChannelDispatcher(Uri listenUri, ServiceHostBase host)
		{
			BindingParameterCollection parameters = new BindingParameterCollection();
			VirtualPathExtension item = host.Extensions.Find<VirtualPathExtension>();
			if (item != null)
			{
				parameters.Add(item);
			}

			IChannelListener<IReplyChannel> listener = null;
			WebHttpBinding binding = new WebHttpBinding();
			if (binding.CanBuildChannelListener<IReplyChannel>(parameters))
			{
				listener = binding.BuildChannelListener<IReplyChannel>(listenUri, parameters);
			}

			ChannelDispatcher channelDispatcher = new ChannelDispatcher(listener)
			{
				MessageVersion = MessageVersion.None
			};

			return channelDispatcher;
		}

		private EndpointDispatcher BuildEndpointDispatcher(Uri uri, ServiceHostBase host, ServiceEndpoint endpoint)
		{
			EndpointAddress address = new EndpointAddress(uri, new AddressHeader[0]);
			var contractDesc = ContractDescription.GetContract(typeof(IMeta));
			EndpointDispatcher endpointDispatcher = new EndpointDispatcher(address, contractDesc.Name, contractDesc.Namespace);
			DispatchOperation operation = new DispatchOperation(endpointDispatcher.DispatchRuntime, "Get", "*", "*")
			{
				Formatter = new WfWebScriptMetadataFomatter(),
				Invoker = new WfWebScriptMetadataInvoker(endpoint, endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation)
			};

			endpointDispatcher.DispatchRuntime.Operations.Add(operation);
			endpointDispatcher.DispatchRuntime.SingletonInstanceContext = new InstanceContext(host, new A());
			endpointDispatcher.DispatchRuntime.InstanceContextProvider = new WfInstanceContextProvider(endpointDispatcher.DispatchRuntime);
			endpointDispatcher.DispatchRuntime.UnhandledDispatchOperation = operation;
			return endpointDispatcher;
		}
	}

	[ServiceContract]
	interface IMeta
	{
		[OperationContract(Action = "*", ReplyAction = "*")]
		Message Get();
	}

	public class A : IMeta
	{
		public Message Get()
		{
			throw new NotImplementedException();
		}
	}

	class WfInstanceContextProvider : IInstanceContextProvider
	{
		private DispatchRuntime _DR { get; set; }
		public WfInstanceContextProvider(DispatchRuntime dr)
		{
			_DR = dr;
		}

		public InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel)
		{
			//if (_DR.SingletonInstanceContext.State != CommunicationState.Opened)
			//{
			//    _DR.SingletonInstanceContext.Open();
			//}
			return _DR.SingletonInstanceContext;
		}

		public void InitializeInstanceContext(InstanceContext instanceContext, Message message, IContextChannel channel)
		{

		}

		public bool IsIdle(InstanceContext instanceContext)
		{
			return false;
		}

		public void NotifyIdle(InstanceContextIdleCallback callback, InstanceContext instanceContext)
		{

		}
	}

}
