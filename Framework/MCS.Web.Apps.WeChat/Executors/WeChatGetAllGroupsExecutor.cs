using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatGetAllGroupsExecutor : WeChatAuthenticatedExecutorBase
	{
		private WeChatGroupCollection _AllGroups = new WeChatGroupCollection();

		public WeChatGetAllGroupsExecutor(WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
		}

		public WeChatGroupCollection AllGroups
		{
			get
			{
				return this._AllGroups;
			}
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["t"] = "user/index";
			parameters["pagesize"] = "100";
			parameters["pageidx"] = "0";
			parameters["type"] = "0";
			parameters["groupid"] = "0";
			parameters["token"] = this.LoginInfo.Token;
			parameters["lang"] = "zh-CN";

			string url = "https://mp.weixin.qq.com/cgi-bin/contactmanage?" + parameters.ToUrlParameters(true);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;
			request.ContentType = "text/html; charset=UTF-8";
			request.Method = "GET";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			Regex GroupRex = new Regex(@"(?<=""groups"":).*(?=\}\).groups)");
			MatchCollection mcGroup = GroupRex.Matches(responseText);

			WeChatGroupCollection result = new WeChatGroupCollection();

			if (mcGroup.Count > 0)
				result = JSONSerializerExecute.Deserialize<WeChatGroupCollection>(mcGroup[0].Value);
			else
				result = new WeChatGroupCollection();

			this._AllGroups = result;
		}
	}
}
