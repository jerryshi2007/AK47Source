using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Core;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// BaseWebClass 的摘要说明。
	/// </summary>
	public class WebUserBaseClass : WebBaseClass
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public WebUserBaseClass()
		{
		}


		private ILogOnUserInfo _LogOnUserInfo = null;
		/// <summary>
		/// 用户登录信息数据。（如果在配置文件中设置了调试用户，则返回该调试用户）
		/// </summary>
		public ILogOnUserInfo LogOnUserInfo
		{
			get
			{
				if (_LogOnUserInfo == null)
					_LogOnUserInfo = GetLogOnUserInfoObject();

				return _LogOnUserInfo;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual ILogOnUserInfo GetLogOnUserInfoObject()
		{
			//			if (Session["logonUserInfo"] == null)
			//			{
			//				if (Context.User.Identity.Name.Length == 0)
			//				{
			//					Session["logonUserInfo"] = GlobalInfo.UserLogOnInfo;
			////					if (Session["logonUserInfo"] == null)
			////					{
			////						if (Response.Cookies[FormsAuthentication.FormsCookieName] != null)
			////						{
			////							FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Response.Cookies[FormsAuthentication.FormsCookieName].Value);
			////							HttpContext context = HttpContext.Current;
			////			
			////							IIdentity id = new GenericIdentity(ticket.Name);
			////							string[] strRoles = {""};
			////							context.User = new GenericPrincipal(id, strRoles);
			////
			////							Session["logonUserInfo"] = new LogOnUserInfo(HttpContext.Current);
			////						}
			////					}
			//					string strMsg = "对不起，系统没有获取到您的登录信息数据！请重新登录系统！";
			//					HG.HGSupport.Common.CtrlException.TrueThrow(Session["logonUserInfo"] == null, strMsg);
			//				}
			//				else
			//					Session["logonUserInfo"] = new LogOnUserInfo(HttpContext.Current);
			//			}
			//
			//			return (ILogOnUserInfo)Session["logonUserInfo"];
			ILogOnUserInfo ilou = null;
			if (Context.User.Identity.Name.Length == 0)
			{
				ilou = GlobalInfo.UserLogOnInfo;
				string strMsg = "对不起，系统没有获取到您的登录信息数据！请重新登录系统！";
				ExceptionHelper.TrueThrow(ilou == null, strMsg);
			}
			else
				ilou = new LogOnUserInfo(HttpContext.Current);

			return ilou;
		}

		//		private void SetUserPrincipal(string strUserName)
		//		{
		//			HttpContext context = HttpContext.Current;
		//			
		//			IIdentity id = new GenericIdentity(strUserName);
		//			string[] strRoles = {""};
		//			context.User = new GenericPrincipal(id, strRoles);
		//		}

		/// <summary>
		/// 重载页面加载，调用了Page类的OnLoad(e)事件
		/// </summary>
		/// <param name="e">事件参数</param>
		protected override void OnLoad(EventArgs e)
		{
			_LogOnUserInfo = GetLogOnUserInfoObject();

			GlobalInfo.InitLogOnUser(_LogOnUserInfo);

			base.OnLoad(e);
		}

	}
}
