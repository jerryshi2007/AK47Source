using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Library.Core
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ServerInfo
	{
		private string serverName = string.Empty;
		private LogOnIdentity identity;
		private int port = 0;
		private AuthenticateType authenticateType = AuthenticateType.Anonymous;

		/// <summary>
		/// 
		/// </summary>
		public LogOnIdentity Identity
		{
			get { return this.identity; }
			set { this.identity = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string ServerName
		{
			get { return this.serverName; }
			set { this.serverName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int Port
		{
			get { return this.port; }
			set { this.port = value; }
		}

		/// <summary>
		/// 认证方式
		/// </summary>
		public AuthenticateType AuthenticateType
		{
			get { return this.authenticateType; }
			set { this.authenticateType = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public ServerInfo()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="identity"></param>
		public ServerInfo(string serverName, LogOnIdentity identity)
		{
			this.serverName = serverName;
			this.identity = identity;
		}
	}
}
