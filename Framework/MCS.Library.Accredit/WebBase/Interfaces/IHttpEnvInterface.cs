using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.WebBase.Interfaces
{
	/// <summary>
	/// 与Http工作环境相关的接口
	/// </summary>
	public interface IHttpEnvInterface
	{
		/// <summary>
		/// 客户端发起请求的IP地址
		/// </summary>
		string UserHostAddress
		{
			get;
		}

		/// <summary>
		/// 客户端发起请求的机器名
		/// </summary>
		string UserHostName
		{
			get;
		}

		/// <summary>
		/// 客户端发起请求的浏览器的UserAgent
		/// </summary>
		string UserAgent
		{
			get;
		}

		/// <summary>
		/// 该页面请求的URL
		/// </summary>
		Uri Url
		{
			get;
		}
	}
}
