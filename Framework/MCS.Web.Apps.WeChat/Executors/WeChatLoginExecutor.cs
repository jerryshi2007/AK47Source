using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatLoginExecutor : WeChatExecutorBase
	{
		public WeChatLoginExecutor()
		{
		}

		public WeChatLoginExecutor(string userName, string password)
		{
			this.UserName = userName;
			this.Password = password;
		}

		public string UserName
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public WeChatLoginInfo LoginInfo
		{
			get;
			private set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			this.UserName.CheckStringIsNullOrEmpty("UserName");
			string password = this.Password;

			if (password == null)
				password = string.Empty;

			string md5Password = Common.GetMd5String(password).ToLower();
			string padata = "username=" + HttpUtility.UrlEncode(this.UserName) + "&pwd=" + HttpUtility.UrlEncode(md5Password) + "&imgcode=&f=json";

			string url = "https://mp.weixin.qq.com/cgi-bin/login?lang=zh_CN";

			CookieContainer cc = new CookieContainer();//接收缓存
			byte[] byteArray = Encoding.UTF8.GetBytes(padata); // 转化
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  //新建一个WebRequest对象用来请求或者响应url

			request.CookieContainer = cc;                                    //保存cookie  
			request.Method = "POST";                                         //请求方式是POST
			request.ContentType = "application/x-www-form-urlencoded";       //请求的内容格式为application/x-www-form-urlencoded
			request.ContentLength = byteArray.Length;
			request.Referer = "https://mp.weixin.qq.com/";

			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
			request.Headers["X-Requested-With"] = "XMLHttpRequest";

			using (Stream newStream = request.GetRequestStream())           //返回用于将数据写入 Internet 资源的 Stream。
			{
				newStream.Write(byteArray, 0, byteArray.Length);			//写入参数
			}

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			WeChatLoginInfo loginInfo = new WeChatLoginInfo();

			loginInfo.Error = responseText;
			WeChatLoginRetInfo retInfo = JSONSerializerExecute.Deserialize<WeChatLoginRetInfo>(responseText);

			retInfo.CheckResult();

			string token = string.Empty;

			if (retInfo.RedirectUrl.Length > 0)
			{
				if (retInfo.RedirectUrl.Contains("ok"))
				{
					token = retInfo.RedirectUrl.Split(new char[] { '&' })[2].Split(new char[] { '=' })[1].ToString();//取得令牌
					loginInfo.LoginCookie = this.Request.CookieContainer;
					loginInfo.CreateDate = DateTime.Now;
					loginInfo.Token = token;

					CookieCollection cookies = this.Request.CookieContainer.GetCookies(new Uri("https://mp.weixin.qq.com/"));

					if (cookies["slave_user"] != null)
						loginInfo.AccountID = cookies["slave_user"].Value;
				}
				else
				{
					throw new ApplicationException(retInfo.ToString());
				}
			}

			WeChatHelper.FillLoginInExtraInfo(loginInfo);

			this.LoginInfo = loginInfo;
		}
	}
}
