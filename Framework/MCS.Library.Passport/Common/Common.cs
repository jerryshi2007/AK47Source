#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	Common.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Xml;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using System.Web.Configuration;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
	internal class Common
	{
		public const string CultureCategory = "Passport";

		public const string C_SESSION_KEY_NAME = "DeluxeWorksPassport_SignInInfo";
		public const string C_USER_SETTINGS_KEY_NAME = "User_Settings_Key_Name";
		public const string C_USER_DELEGATION_KEY_NAME = "User_Delegation_Key_Name";

		public const string C_LOGOFF_CALLBACK_VIRTUAL_PATH = "MCSAuthenticateLogOff.axd";

		/// <summary>
		/// 检查Http的上下文是否存在
		/// </summary>
		public static void CheckHttpContext()
		{
			ExceptionHelper.FalseThrow(HttpContext.Current != null,
				"无法取得HttpContext。代码必须运行在Web请求的处理过程中");
		}

		/// <summary>
		/// 加密字符串，返回加密后的Base64的字符串
		/// </summary>
		/// <param name="strData">原始字符串</param>
		/// <returns>加密后的Base64的字符串</returns>
		public static string EncryptString(string strData)
		{
			return Convert.ToBase64String(PassportEncryptionSettings.GetConfig().StringEncryption.EncryptString(strData));
		}

		/// <summary>
		/// 解密字符串，将加密后的Base64的字符串解密为原始字符串
		/// </summary>
		/// <param name="strEncText">加密后的Base64</param>
		/// <returns>解密后的原始字符串</returns>
		public static string DecryptString(string strEncText)
		{
			byte[] data = Convert.FromBase64String(strEncText);

			return PassportEncryptionSettings.GetConfig().StringEncryption.DecryptString(data);
		}
		/// <summary>
		/// 生成SignInInfo的Xml格式数据
		/// </summary>
		/// <param name="userInfo">用户登录信息</param>
		/// <param name="bDontSaveUserID">是否保存用户名</param>
		/// <param name="bAutoSignIn">是否自动登录</param>
		/// <returns>SignInfo的xml格式数据</returns>
		public static XmlDocument GenerateSignInInfo(ISignInUserInfo userInfo, bool bDontSaveUserID, bool bAutoSignIn)
		{
			string userID = ImpersonateSettings.GetConfig().Impersonation[userInfo.UserID];

			HttpContext context = HttpContext.Current;

			HttpRequest request = context.Request;

			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<SignInInfo/>");

			XmlHelper.AppendNode(xmlDoc.DocumentElement, "SSID", Guid.NewGuid().ToString());
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "UID", userID);
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "OUID", userInfo.OriginalUserID);
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "DO", userInfo.Domain);

			object windowsIntegrated;

			if (true == userInfo.Properties.TryGetValue("WindowsIntegrated", out windowsIntegrated))
			{
				XmlHelper.AppendNode(xmlDoc.DocumentElement, "WI", true);
			}

			XmlHelper.AppendNode(xmlDoc.DocumentElement, "DSUID", bDontSaveUserID);
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "ASI", bAutoSignIn);
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "STime", DateTimeStandardFormat(DateTime.Now));
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "AS", request.Url.Host + ":" + request.Url.Port);

			if (userInfo.Properties.Count > 0)
			{
				XmlNode nodeProps = XmlHelper.AppendNode(xmlDoc.DocumentElement, Resource.SignInInfoExtraProperties);

				foreach (KeyValuePair<string, object> kp in userInfo.Properties)
				{
					XmlNode nodeProp = XmlHelper.AppendNode(nodeProps, "add");

					XmlHelper.AppendAttr(nodeProp, "key", kp.Key);
					XmlHelper.AppendAttr(nodeProp, "value", kp.Value.ToString());
				}
			}

			DateTime dtExpireTime = DateTime.MaxValue;

			PassportSignInSettings settings = PassportSignInSettings.GetConfig();

			if (settings.DefaultTimeout >= TimeSpan.Zero)
				dtExpireTime = DateTime.Now.Add(settings.DefaultTimeout);
			else
				if (settings.DefaultTimeout < TimeSpan.FromSeconds(-1))
					dtExpireTime = DateTime.MinValue;
				else
					if (settings.DefaultTimeout == TimeSpan.FromSeconds(-1))
						dtExpireTime = DateTime.MaxValue;

			XmlHelper.AppendNode(xmlDoc.DocumentElement, "STimeout", DateTimeStandardFormat(dtExpireTime));

			return xmlDoc;
		}
		/// <summary>
		/// 生成Ticket的字符串
		/// </summary>
		/// <param name="signInInfo">登录信息</param>
		/// <param name="strIP">客户端ip</param>
		/// <returns>Ticket的字符串</returns>
		public static string GenerateTicketString(ISignInInfo signInInfo, string strIP)
		{
			HttpContext context = HttpContext.Current;

			HttpRequest request = context.Request;

			XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<Ticket/>");

			XmlDocument xmlSignInInfo = signInInfo.SaveToXml();

			XmlNode SignInNode = XmlHelper.AppendNode(xmlDoc.DocumentElement, "SignInInfo");
			SignInNode.InnerXml = xmlSignInInfo.DocumentElement.InnerXml;

			string strTimeout = request.QueryString["to"];
			int nTimeout = -1;

			if (strTimeout != null)
			{
				try
				{
					nTimeout = int.Parse(strTimeout);
				}
				catch (System.Exception)
				{
				}
			}
			else
				nTimeout = (int)(PassportSignInSettings.GetConfig().DefaultTimeout.TotalSeconds);

			string strAppID = request.QueryString["aid"];

			XmlHelper.AppendNode(xmlDoc.DocumentElement, "AppSSID", Guid.NewGuid().ToString());
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "AppID", strAppID);
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "AppSTime", DateTimeStandardFormat(DateTime.Now));
			XmlHelper.AppendNode(xmlDoc.DocumentElement, "IP", strIP);

			DateTime dtExpireTime = DateTime.MaxValue;

			if (nTimeout >= 0)
				dtExpireTime = DateTime.Now.AddSeconds(nTimeout);
			else
				if (nTimeout < -1)
					dtExpireTime = DateTime.MinValue;
				else
					if (nTimeout == -1)
						dtExpireTime = DateTime.MaxValue;

			XmlHelper.AppendNode(xmlDoc.DocumentElement, "AppSTimeout", DateTimeStandardFormat(dtExpireTime));

			return xmlDoc.OuterXml;
		}

		/// <summary>
		/// 加密Ticket
		/// </summary>
		/// <param name="ticket">ticket</param>
		/// <returns>加密后的Ticket并且使用Base64编码</returns>
		public static string EncryptTicket(ITicket ticket)
		{
			ITicketEncryption et = PassportEncryptionSettings.GetConfig().TicketEncryption;

			//byte[] data = et.EncryptTicket(ticket, PassportClientSettings.GetConfig().RsaKeyValue); /del by yuanyong 20090416
			//原来是PassportClientSettings，不正确的。加密ticket是PassportService的事情。虽然Client和Service都配置了密钥，但是应该使用Service方的
			byte[] data = et.EncryptTicket(ticket, PassportSignInSettings.GetConfig().RsaKeyValue);

			return Convert.ToBase64String(data);
		}

		/// <summary>
		/// 将加密之后的ticket对应字符转换回来
		/// </summary>
		/// <param name="strTicketEncoded">待解密的字符串原始数据</param>
		/// <returns>解密之后的ticket对象</returns>
		/// <remarks>
		/// 解密的源数据要求采用相同加密格式要求，配置于PassportEncryptionSettings中
		/// </remarks>
		public static ITicket DecryptTicket(string strTicketEncoded)
		{
			ITicketEncryption et = PassportEncryptionSettings.GetConfig().TicketEncryption;

			byte[] data = Convert.FromBase64String(strTicketEncoded);

			return et.DecryptTicket(data, PassportClientSettings.GetConfig().RsaKeyValue);
		}

		/// <summary>
		/// 计算缓存Principal所使用的Session Key
		/// </summary>
		/// <returns>Session Key值</returns>
		public static string GetPrincipalSessionKey()
		{
			HttpRequest request = HttpContext.Current.Request;

			HttpCookie cookie = request.Cookies[C_SESSION_KEY_NAME];

			string result;

			if (cookie != null)
				result = cookie.Value;
			else
			{
				result = UuidHelper.NewUuidString() + "-UserPrincipal";

				cookie = new HttpCookie(C_SESSION_KEY_NAME);
				cookie.Value = result;
				cookie.Expires = DateTime.MinValue;

				HttpContext.Current.Response.Cookies.Add(cookie);
			}

			return result;
		}
		/// <summary>
		/// 获取Session超时时间
		/// </summary>
		/// <returns>SessionTimeOut</returns>
		public static TimeSpan GetSessionTimeOut()
		{
			TimeSpan timeout = TimeSpan.FromMinutes(20);

			SessionStateSection section = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");

			if (section != null)
				timeout = section.Timeout;

			return timeout;
		}

		public static string DateTimeStandardFormat(DateTime dt)
		{
			string result = dt.ToString("yyyy-MM-dd HH:mm:ss");

			result = result.Replace(".", ":");

			return result;
		}
	}
}
