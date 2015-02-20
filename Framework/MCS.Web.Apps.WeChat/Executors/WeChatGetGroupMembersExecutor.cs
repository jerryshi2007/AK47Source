using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MCS.Library.Core;
using MCS.Web.Apps.WeChat.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.Apps.WeChat.Executors
{
	public class WeChatGetGroupMembersExecutor : WeChatAuthenticatedExecutorBase
	{
		private WeChatFriendCollection _Friends = new WeChatFriendCollection();
		private int _PageSize = 10;

		public WeChatGetGroupMembersExecutor(int groupID, int pageIndex, int pageSize, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			this.GroupID = groupID;
			this.PageIndex = pageIndex;
			this._PageSize = pageSize;
		}

		/// <summary>
		/// 所属组的ID，如果小于0，则表示所有的用户
		/// </summary>
		public int GroupID
		{
			get;
			private set;
		}

		public int PageSize
		{
			get
			{
				return this._PageSize;
			}
			set
			{
				this._PageSize = value;
			}
		}

		public int PageIndex
		{
			get;
			private set;
		}

		public WeChatFriendCollection Friends
		{
			get
			{
				return this._Friends;
			}
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			NameValueCollection parameters = new NameValueCollection();

			parameters["t"] = "user/index";
			parameters["pagesize"] = this.PageSize.ToString();
			parameters["pageidx"] = this.PageIndex.ToString();
			parameters["type"] = "0";

			if (this.GroupID >= 0)
				parameters["groupid"] = this.GroupID.ToString();

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
			Regex GroupRex = new Regex(@"(?<=""contacts"":).*(?=\}\).contacts)");
			MatchCollection mcGroup = GroupRex.Matches(responseText);

			WeChatFriendCollection result = new WeChatFriendCollection();

			if (mcGroup.Count > 0)
				result = JSONSerializerExecute.Deserialize<WeChatFriendCollection>(mcGroup[0].Value);
			else
				result = new WeChatFriendCollection();

			this._Friends = result;
		}
	}
}
