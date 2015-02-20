using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Principal;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 用于构造OpenIDIdentity的HttpModule
	/// </summary>
	public class OpenIDAuthenticationModule : IHttpModule
	{
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
		}

		private void context_AuthenticateRequest(object sender, EventArgs e)
		{
			if (OpenIDAuthenticateDirSettings.GetConfig().PageNeedAuthenticate())
				DoAuthentication();
		}

		private void DoAuthentication()
		{
			if (OpenIDIdentity.Current == null)
			{
				OpenIDIdentity identity = OpenIDIdentity.LoadFromUrl();

				if (identity == null)
					identity = OpenIDIdentity.LoadFromCookie();
				else
					identity.SaveToCookie();

				if (identity != null)
					identity.SaveToContextCache();
			}
		}
	}
}
