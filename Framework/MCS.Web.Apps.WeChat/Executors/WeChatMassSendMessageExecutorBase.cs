using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
	public abstract class WeChatMassSendMessageExecutorBase : WeChatAuthenticatedExecutorBase
	{
		/// <summary>
		/// 群发消息基类
		/// </summary>
		/// <param name="groupID">groupID为-1时，表示发送范围为全部</param>
		/// <param name="messageType"></param>
		/// <param name="loginInfo"></param>
		public WeChatMassSendMessageExecutorBase(int groupID, WeChatAppMessageType messageType, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			this.GroupID = groupID;
			this.MessageType = messageType;
		}

		public int GroupID
		{
			get;
			set;
		}

		public WeChatAppMessageType MessageType
		{
			get;
			protected set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			string url = "https://mp.weixin.qq.com/cgi-bin/masssend";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;
			request.Referer = string.Format("https://mp.weixin.qq.com/cgi-bin/masssendpage?t=mass/send&token={0}&lang=zh_CN",
				HttpUtility.UrlEncode(this.LoginInfo.Token));

			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";

			NameValueCollection postParams = new NameValueCollection();

			postParams["type"] = ((int)this.MessageType).ToString();
			postParams["sex"] = "0";
			postParams["groupid"] = this.GroupID.ToString();
			postParams["synctxweibo"] = "0";
			postParams["synctxnews"] = "0";
			postParams["country"] = string.Empty;
			postParams["province"] = string.Empty;
			postParams["city"] = string.Empty;
			postParams["imgcode"] = string.Empty;
			postParams["token"] = this.LoginInfo.Token;
			postParams["lang"] = "zh_CN";
			postParams["random"] = "0.3247161018546267";
			postParams["f"] = "json";
			postParams["t"] = "ajax-response";

			PreparePostParams(request, postParams);

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
			WeChatSendMessageRetInfo result = JSONSerializerExecute.Deserialize<WeChatSendMessageRetInfo>(responseText);

			if (result.ret != 0)
				throw new ApplicationException(result.msg);
		}

		/// <summary>
		/// 准备Post的参数（基础参数已经准备完毕了）
		/// </summary>
		/// <param name="request"></param>
		/// <param name="postParams"></param>
		protected abstract void PreparePostParams(HttpWebRequest request, NameValueCollection postParams);
	}
}
