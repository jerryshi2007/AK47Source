using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using MCS.Library.Accredit.WebBase.Interfaces;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// HttpEnvInfo 的摘要说明。
	/// </summary>
	class HttpEnvInfo : IHttpEnvInterface
	{
		/// <summary>
		/// 当前环境中的环境信息数据
		/// </summary>
		/// <param name="request">界面发送过来的请求数据</param>
		public HttpEnvInfo(HttpRequest request)
		{
			_Url = request.Url;
			_UserAgent = request.UserAgent;
			_UserHostAddress = request.UserHostAddress;
			_UserHostName = request.UserHostName;
		}


		#region IHttpEnvInterface 成员

		private Uri _Url = null;
		/// <summary>
		/// 客户端发起请求的IP地址
		/// </summary>
		public Uri Url
		{
			get
			{
				return _Url;
			}
		}

		private string _UserHostName = string.Empty;
		/// <summary>
		/// 客户端发起请求的机器名
		/// </summary>
		public string UserHostName
		{
			get
			{
				return _UserHostName;
			}
		}

		private string _UserAgent = string.Empty;
		/// <summary>
		/// 客户端发起请求的浏览器的UserAgent
		/// </summary>
		public string UserAgent
		{
			get
			{
				return _UserAgent;
			}
		}

		private string _UserHostAddress = string.Empty;
		/// <summary>
		/// 该页面请求的URL
		/// </summary>
		public string UserHostAddress
		{
			get
			{
				return _UserHostAddress;
			}
		}

		#endregion
	}
}
