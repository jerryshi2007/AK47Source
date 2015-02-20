using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.WcfExtensions
{
	/// <summary>
	/// 工作流相关的客户端调用的上下文对象
	/// </summary>
	[Serializable]
	[ActionContextDescription("WfClientServiceBrokerContext")]
	public class WfClientServiceBrokerContext : ActionContextBase<WfClientServiceBrokerContext>
	{
		private Dictionary<string, string> _ConnectionMappings = null;
		private WebHeaderCollection _Headers = null;

		public Dictionary<string, string> ConnectionMappings
		{
			get
			{
				if (this._ConnectionMappings == null)
					this._ConnectionMappings = new Dictionary<string, string>();

				return this._ConnectionMappings;
			}
		}

		/// <summary>
		/// Http请求时的Header
		/// </summary>
		public WebHeaderCollection Headers
		{
			get
			{
				if (this._Headers == null)
					this._Headers = new WebHeaderCollection();

				return this._Headers;
			}
		}
	}
}
