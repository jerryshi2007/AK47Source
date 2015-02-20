using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
	/// <summary>
	/// 修改粉丝所属的组
	/// </summary>
	public class WeChatChangeFriendsGroupExecutor : WeChatAuthenticatedExecutorBase
	{
		public WeChatChangeFriendsGroupExecutor(int groupID, string[] fakeIDs, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			fakeIDs.NullCheck("fakeIDs");

			this.GroupID = groupID;
			this.FakeIDs = fakeIDs;
		}

		public int GroupID
		{
			get;
			private set;
		}

		public string[] FakeIDs
		{
			get;
			private set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			string url = "https://mp.weixin.qq.com/cgi-bin/modifycontacts";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;

			request.Referer = string.Format("https://mp.weixin.qq.com/cgi-bin/contactmanage?t=user/index&pagesize=10&pageidx=0&type=0&token={0}&lang=zh_CN",
				HttpUtility.UrlEncode(this.LoginInfo.Token));

			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
			request.Headers["X-Requested-With"] = "XMLHttpRequest";

			NameValueCollection postParams = new NameValueCollection();

			postParams["contacttype"] = this.GroupID.ToString();
			postParams["tofakeidlist"] = string.Join("|", this.FakeIDs);
			postParams["token"] = this.LoginInfo.Token;
			postParams["lang"] = "zh_CN";
			postParams["random"] = "0.691624744434424";
			postParams["f"] = "json";
			postParams["t"] = "ajax-putinto-group";
			postParams["ajax"] = "1";
			postParams["action"] = "modifycontacts";

			string postData = postParams.ToUrlParameters(true);

			byte[] data = Encoding.UTF8.GetBytes(postData);

			request.ContentLength = data.Length;

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			WeChatChangeFriendsRetInfo result = JSONSerializerExecute.Deserialize<WeChatChangeFriendsRetInfo>(responseText);

			if (result.ret != "0")
				throw new ApplicationException(string.Format("将粉丝切换到组{0}失败", this.GroupID));
		}
	}
}
