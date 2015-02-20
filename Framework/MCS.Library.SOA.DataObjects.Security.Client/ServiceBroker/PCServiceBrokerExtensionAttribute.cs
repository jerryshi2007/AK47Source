using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.SOA.DataObjects.Security.Client
{
	/// <summary>
	/// 用于ServiceBroker使用的SoapExtension的属性定义
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PCServiceBrokerExtensionAttribute : SoapExtensionAttribute
	{
		/// <summary>
		/// SoapExtension的类型
		/// </summary>
		public override Type ExtensionType
		{
			get
			{
				return typeof(PCServiceBrokerExtension);
			}
		}

		/// <summary>
		/// 优先级
		/// </summary>
		public override int Priority
		{
			get;
			set;
		}
	}
}
