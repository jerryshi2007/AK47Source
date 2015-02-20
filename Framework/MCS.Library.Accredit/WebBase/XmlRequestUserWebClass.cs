using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Web;
using System.Security.Principal;
using System.Diagnostics;

using MCS.Library.Core;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.OguAdmin.Interfaces;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// 所有用于处理XML Http Request页面的基类（要求带身份）
	/// </summary>
	public class XmlRequestUserWebClass : XmlRequestWebClass
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public XmlRequestUserWebClass()
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

		private void SetUserPrincipal(string strUserName)
		{
			HttpContext context = HttpContext.Current;

			IIdentity id = new GenericIdentity(strUserName);
			string[] strRoles = { "" };
			context.User = new GenericPrincipal(id, strRoles);
		}

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
