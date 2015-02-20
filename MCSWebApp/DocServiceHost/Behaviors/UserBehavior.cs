using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using MCS.Library.Logging;
using MCS.Library.Services.Log;

namespace MCS.Library.Services.Behaviors
{
	public class UserBehavior : IParameterInspector, IEndpointBehavior
	{
		public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{

		}

		public object BeforeCall(string operationName, object[] inputs)
		{
			int index = OperationContext.Current.IncomingMessageHeaders.FindHeader("username", "");
			if (index >= 0)
			{
				string user = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(index);
				//LogEntity entity = new LogEntity(operationName);
				//entity.Title = operationName;
				//entity.Message = string.Format("用户{0}执行了{1}操作", user, operationName);//operationName;
				
				//2012-12-5
				//LogEntity entity = new DCLogEntityBuilder().BuildEntity(operationName, user, inputs);
				//if (null != entity)
				//{
				//    Logger logger = LoggerFactory.Create("mossLogger");
				//    logger.Listeners.Clear();
				//    logger.Listeners.Add(new MossListLogListener());
				//    logger.Write(entity);
				//}
			}
			return null;
		}


		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			//throw new NotImplementedException();
		}



		public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
			//throw new NotImplementedException();
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			//throw new NotImplementedException();

		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			foreach (DispatchOperation operation in endpointDispatcher.DispatchRuntime.Operations)
			{
				operation.ParameterInspectors.Add(new UserBehavior());
			}
		}

		public void Validate(ServiceEndpoint endpoint)
		{
			//throw new NotImplementedException();
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