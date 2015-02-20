using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 用于ServiceBroker使用的SoapExtension的属性定义
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ServiceBrokerExtensionAttribute : SoapExtensionAttribute
	{
		/// <summary>
		/// SoapExtension的类型
		/// </summary>
		public override Type ExtensionType
		{
			get
			{
				return typeof(ServiceBrokerExtension);
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
