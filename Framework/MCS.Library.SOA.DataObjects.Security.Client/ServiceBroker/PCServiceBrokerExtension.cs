using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	/// <summary>
	/// 定制的SoapExtension，用于Client调用
	/// </summary>
	public class PCServiceBrokerExtension : ServiceBrokerExtensionBase<PCServiceBrokerContext>
	{
		protected override PCServiceBrokerContext GetSerivceBrokerContext()
		{
			return PCServiceBrokerContext.Current;
		}
	}
}
