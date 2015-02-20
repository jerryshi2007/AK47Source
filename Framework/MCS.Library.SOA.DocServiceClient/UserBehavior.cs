using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Web;
using MCS.Library.Principal;

namespace MCS.Library.Services.Test
{
	public class UserBehavior : IEndpointBehavior, IClientMessageInspector
	{

		public UserBehavior()
		{
		}

		public UserBehavior(string documentLibraryName, string documentServerName)
		{
			this._DocumentLibraryName = documentLibraryName;
			this._DocumentServerName = documentServerName;
		}

		private string _DocumentLibraryName = "DocumentCenter";
		private string _DocumentServerName = "documentServer";

		/// <summary>
		/// 文档库名称
		/// </summary>
		public string DocumentLibraryName
		{
			get { return this._DocumentLibraryName; }
			set { this._DocumentLibraryName = value; }
		}

		/// <summary>
		/// 服务器配置项的名称
		/// </summary>
		public string DocumentServerName
		{
			get { return this._DocumentServerName; }
			set { this._DocumentServerName = value; }
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			//throw new NotImplementedException();
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new UserBehavior(this._DocumentLibraryName, this._DocumentServerName));
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			//throw new NotImplementedException();
		}

		public void Validate(ServiceEndpoint endpoint)
		{
			//throw new NotImplementedException();
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			//throw new NotImplementedException();
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			//throw new NotImplementedException();
			//OperationContext.Current = new OperationContext(channel);
#if !DEBUG
            if (null != DeluxeIdentity.CurrentUser)
            {
                MessageHeader header = MessageHeader.CreateHeader("username", "", DeluxeIdentity.CurrentUser.LogOnName);
                request.Headers.Add(header);
            }
#else
			MessageHeader header = MessageHeader.CreateHeader("username", "", "administrator");
			request.Headers.Add(header);
#endif

			MessageHeader documentLibraryName = MessageHeader.CreateHeader("libraryName", "", this.DocumentLibraryName);
			MessageHeader documentServerName = MessageHeader.CreateHeader("documentServer", "", this.DocumentServerName);

			request.Headers.Add(documentLibraryName);
			request.Headers.Add(documentServerName);
			//OperationContext.Current.OutgoingMessageHeaders.Add(header);
			return null;

		}
	}

	public class UserBehaviorElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get { return typeof(UserBehavior); }
		}

		protected override object CreateBehavior()
		{
			return new UserBehavior();
		}
	}
}
