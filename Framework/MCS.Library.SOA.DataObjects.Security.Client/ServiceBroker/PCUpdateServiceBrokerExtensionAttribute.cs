using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	/// <summary>
	/// 更新服务的服务扩展
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	sealed class PCUpdateServiceBrokerExtensionAttribute : SoapExtensionAttribute
	{
		private int priority;

		public override Type ExtensionType
		{
			get { return typeof(PCUpdateServiceBrokerExtension); }
		}

		public override int Priority
		{
			get
			{
				return priority;
			}
			set
			{
				this.priority = value;
			}
		}
	}
}
