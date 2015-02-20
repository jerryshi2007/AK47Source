using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using MCS.Library.Core;
using MCS.Library.Passport;
using System.Web;
using MCS.Library.Caching;

namespace MCS.Library.Principal
{
	/// <summary>
	/// 包含OpenID的Identity
	/// </summary>
	public class OpenIDIdentity : IIdentity
	{
		private string _OpenID = null;

		/// <summary>
		/// 得到上下文中的OpenIDIdentity
		/// </summary>
		public static OpenIDIdentity Current
		{
			get
			{
				object identity = null;

				ObjectContextCache.Instance.TryGetValue("OpenIDContext", out identity);

				return (OpenIDIdentity)identity;
			}
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="openID"></param>
		public OpenIDIdentity(string openID)
		{
			this._OpenID = openID;
		}

		/// <summary>
		/// 从Url加载
		/// </summary>
		public static OpenIDIdentity LoadFromUrl()
		{
			Common.CheckHttpContext();

			OpenIDIdentity identity = null;

			string openID = HttpContext.Current.Request.QueryString["openID"];

			if (openID.IsNotEmpty())
				identity = new OpenIDIdentity(openID);

			return identity;
		}

		/// <summary>
		/// 从Cookie中加载
		/// </summary>
		/// <returns></returns>
		public static OpenIDIdentity LoadFromCookie()
		{
			Common.CheckHttpContext();

			OpenIDIdentity identity = null;

			HttpRequest request = HttpContext.Current.Request;

			HttpCookie cookie = request.Cookies["openID"];

			if (cookie != null)
			{
				string openID = cookie.Value;

				if (openID.IsNotEmpty())
					identity = new OpenIDIdentity(cookie.Value);
			}

			return identity;
		}

		/// <summary>
		/// 认证类型
		/// </summary>
		public string AuthenticationType
		{
			get
			{
				return "OpenID";
			}
		}

		/// <summary>
		/// 是否认证
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return this._OpenID.IsNotEmpty();
			}
		}

		/// <summary>
		/// OpenID
		/// </summary>
		public string OpenID
		{
			get
			{
				return this._OpenID;
			}
		}

		/// <summary>
		/// 用户名，实际上是OpenID
		/// </summary>
		public string Name
		{
			get
			{
				return this._OpenID;
			}
		}

		/// <summary>
		/// 保存到Cookie
		/// </summary>
		public void SaveToCookie()
		{
			Common.CheckHttpContext();

			HttpResponse response = HttpContext.Current.Response;
			HttpRequest request = HttpContext.Current.Request;

			HttpCookie cookie = new HttpCookie("OpenID", this.Name);

			cookie.Expires = DateTime.MinValue;

			response.Cookies.Add(cookie);
		}

		/// <summary>
		/// 保存到上下文缓存中
		/// </summary>
		public void SaveToContextCache()
		{
			ObjectContextCache.Instance.Add("OpenIDContext", this);
		}
	}
}
