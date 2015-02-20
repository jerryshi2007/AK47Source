using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 定制的SoapExtension，用于Client调用
	/// </summary>
	public class ServiceBrokerExtension : ServiceBrokerExtensionBase<ServiceBrokerContext>
	{
		/// <summary>
		/// 得到代理扩展的实例
		/// </summary>
		/// <returns></returns>
		protected override ServiceBrokerContext GetSerivceBrokerContext()
		{
			return ServiceBrokerContext.Current;
		}
	}
}
