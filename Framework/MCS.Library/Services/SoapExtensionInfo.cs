using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Services
{
	/// <summary>
	/// Soap扩展的调用者类型
	/// </summary>
	public enum SoapExtensionCallerType
	{
		/// <summary>
		/// 服务端方法
		/// </summary>
		ServiceMethod,

		/// <summary>
		/// 客户端方法
		/// </summary>
		ClientMethod
	}

	/// <summary>
	/// SOAP扩展信息的调用者信息
	/// </summary>
	public class SoapExtensionCallerInfo
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceName">服务名称</param>
		/// <param name="callerType">调用Extension的类型（客户端还是服务器端）</param>
		public SoapExtensionCallerInfo(string serviceName, SoapExtensionCallerType callerType)
		{
			this.ServiceName = serviceName;
			this.CallerType = callerType;
		}

		/// <summary>
		/// 服务名称
		/// </summary>
		public string ServiceName
		{
			get;
			private set;
		}

		/// <summary>
		/// 调用者的类型
		/// </summary>
		public SoapExtensionCallerType CallerType
		{
			get;
			private set;
		}
	}
}
