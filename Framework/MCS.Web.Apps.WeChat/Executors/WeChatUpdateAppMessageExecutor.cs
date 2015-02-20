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
	public class WeChatUpdateAppMessageExecutor : WeChatAuthenticatedExecutorBase
	{
		public WeChatUpdateAppMessageExecutor(WeChatAppMessage message, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			message.NullCheck("message");

			this.Message = message;
		}

		public WeChatAppMessage Message
		{
			get;
			private set;
		}

		public WeChatAppMessage ResponseMessage
		{
			get;
			private set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			this.Message.NullCheck("Message");

			string url = "https://mp.weixin.qq.com/cgi-bin/operate_appmsg";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;
			request.Referer = string.Format("https://mp.weixin.qq.com/cgi-bin/appmsg?t=media/appmsg_edit&action=edit&type=10&isMul=0&isNew=1&token={0}&lang=zh_CN",
				HttpUtility.UrlEncode(this.LoginInfo.Token));

			request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			request.Headers["X-Requested-With"] = "XMLHttpRequest";
			request.Method = "POST";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";

			NameValueCollection postParams = new NameValueCollection();

			postParams["AppMsgId"] = this.Message.AppMessageID > 0 ? this.Message.AppMessageID.ToString() : string.Empty;
			postParams["type"] = ((int)this.Message.MessageType).ToString();
			postParams["count"] = this.Message.Count.ToString();
			postParams["title0"] = this.Message.Title;
			postParams["content0"] = this.Message.Content;
			postParams["digest0"] = this.Message.Digest;
			postParams["author0"] = this.Message.Author;
			postParams["fileid0"] = this.Message.FileID > 0 ? this.Message.FileID.ToString() : string.Empty;
			postParams["sourceurl0"] = this.Message.SourceUrl;
			postParams["show_cover_pic0"] = this.Message.ShowCover ? "1" : "0";
			postParams["vid"] = string.Empty;
			postParams["ajax"] = "1";
			postParams["lang"] = "zh_CN";
			postParams["random"] = "0.963834238394236";
			postParams["f"] = "json";
			postParams["t"] = "ajax-response";
			postParams["sub"] = this.Message.AppMessageID == 0 ? "create" : "update";
			postParams["token"] = this.LoginInfo.Token;

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

			if (this.Message.AppMessageID <= 0)
			{
				this.ResponseMessage = WeChatHelper.GetAppMessages(this.Message.MessageType, this.LoginInfo).FirstOrDefault();

				(this.ResponseMessage != null).FalseThrow("不能找到新保存的素材");
			}
			else
				this.ResponseMessage = this.Message;
		}
	}
}
