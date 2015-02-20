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
	/// <summary>
	/// 单发消息的执行器的基类
	/// </summary>
	public abstract class WeChatSingleSendMessageExecutorBase : WeChatAuthenticatedExecutorBase
	{
		public WeChatSingleSendMessageExecutorBase(string fakeID, WeChatAppMessageType messageType, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			fakeID.CheckStringIsNullOrEmpty("fakeID");

			this.FakeID = fakeID;
			this.MessageType = messageType;
		}

		public string FakeID
		{
			get;
			private set;
		}

		public WeChatAppMessageType MessageType
		{
			get;
			protected set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			this.FakeID.CheckStringIsNullOrEmpty("FakeID");

			string url = "https://mp.weixin.qq.com/cgi-bin/singlesend";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;

			request.Referer = string.Format("https://mp.weixin.qq.com/cgi-bin/singlesendpage?t=message/send&tofakeid={0}&token={1}&lang=zh_CN",
				HttpUtility.UrlEncode(this.FakeID),
				HttpUtility.UrlEncode(this.LoginInfo.Token));

			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
			request.Headers["X-Requested-With"] = "XMLHttpRequest";

			NameValueCollection postParams = new NameValueCollection();

			postParams["type"] = ((int)this.MessageType).ToString();
			postParams["imgcode"] = string.Empty;
			postParams["token"] = this.LoginInfo.Token;
			postParams["lang"] = "zh_CN";
			postParams["random"] = "0.7978688745877286";
			postParams["f"] = "json";
			postParams["t"] = "ajax-response";
			postParams["ajax"] = "1";
			postParams["tofakeid"] = this.FakeID;

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
			WeChatSingleSendMessageRetInfo result = JSONSerializerExecute.Deserialize<WeChatSingleSendMessageRetInfo>(responseText);

			result.CheckResult();
		}

		/// <summary>
		/// 准备Post的参数（基础参数已经准备完毕了）
		/// </summary>
		/// <param name="request"></param>
		/// <param name="postParams"></param>
		protected abstract void PreparePostParams(HttpWebRequest request, NameValueCollection postParams);
	}
}
