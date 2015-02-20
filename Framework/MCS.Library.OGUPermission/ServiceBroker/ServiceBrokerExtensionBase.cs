using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 定制的SoapExtension基类，用于Client调用
	/// </summary>
	public abstract class ServiceBrokerExtensionBase<T> : SoapExtension where T : ServiceBrokerContextBase<T>, new()
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public override object GetInitializer(Type serviceType)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="initializer"></param>
		public override void Initialize(object initializer)
		{
		}

		/// <summary>
		/// 客户端的调用时序是：
		/// BeforeSerilize
		/// AfterSerilize
		/// BeforeDeserialize
		/// AfterDeserialize
		/// </summary>
		/// <param name="message"></param>
		public override void ProcessMessage(SoapMessage message)
		{
			if (message is SoapClientMessage)
			{
				SoapClientMessage clientMessage = (SoapClientMessage)message;

				if (clientMessage.Stage == SoapMessageStage.BeforeSerialize)
				{
					ServiceBrokerSoapHeader header = new ServiceBrokerSoapHeader();

					T broker = GetSerivceBrokerContext();

					header.UseServerCache = broker.UseServerCache;
					header.TimePoint = broker.TimePoint;

					broker.ConnectionMappings.ForEach(mapping =>
						header.ConnectionMappings.Add(new SoapHeaderConnectionMappingItem(mapping.Key, mapping.Value)));

					broker.Context.ForEach(kp => header.Context.Add(new SoapHeaderContextItem(kp.Key, kp.Value)));

					clientMessage.Headers.Add(header);
				}
			}
		}

		/// <summary>
		/// 得到服务代理
		/// </summary>
		/// <returns></returns>
		protected abstract T GetSerivceBrokerContext();
	}
}
