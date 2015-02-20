using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Caching
{
	/// <summary>
	/// ����Cookie��Cache������
	/// </summary>
	public class CookieCacheDependency : DependencyBase
	{
		HttpCookie httpCookie = null;

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="cookie">cookie</param>
		public CookieCacheDependency(HttpCookie cookie)
		{
			if (cookie == null)
				throw new ArgumentNullException("cookie");

			HttpContext.Current.Request.Cookies.Add(cookie);
			httpCookie = cookie;
		}

		/// <summary>
		/// �Ƿ����ı�
		/// </summary>
		public override bool HasChanged
		{
			get
			{
				bool result = false;

				if (EnvironmentHelper.Mode == InstanceMode.Web && CacheManager.InScavengeThread == false)
					result = CookieIsInvalid();

				return result;
			}
		}

		/// <summary>
		/// ����޸�����
		/// </summary>
		public override DateTime UtcLastModified
		{
			get
			{
				return base.UtcLastModified;
			}
			set
			{
				if (EnvironmentHelper.Mode == InstanceMode.Web && CacheManager.InScavengeThread == false)
				{
					HttpResponse response = HttpContext.Current.Response;

					response.Cookies.Add(httpCookie);
				}

				base.UtcLastModified = value;
			}
		}

		private bool CookieIsInvalid()
		{
			HttpRequest request = HttpContext.Current.Request;

			HttpCookie cookieReq = request.Cookies[httpCookie.Name];

			return cookieReq == null;
		}
	}
}
