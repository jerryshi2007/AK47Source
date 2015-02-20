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
	/// 修改组信息的虚基类
	/// </summary>
	public abstract class WeChatModifyGroupExecutorBase : WeChatAuthenticatedExecutorBase
	{
		public WeChatModifyGroupExecutorBase(string func, WeChatLoginInfo loginInfo)
			: base(loginInfo)
		{
			func.CheckStringIsNullOrEmpty("func");

			this.FunctionName = func;
		}

		public WeChatModifyGroupRetInfo GroupInfo
		{
			get;
			protected set;
		}

		protected string FunctionName
		{
			get;
			private set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			this.FunctionName.CheckStringIsNullOrEmpty("FunctionName");

			string url = "https://mp.weixin.qq.com/cgi-bin/modifygroup";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			request.CookieContainer = this.LoginInfo.LoginCookie;

			request.Referer = string.Format("https://mp.weixin.qq.com/cgi-bin/contactmanage?t=user/index&pagesize=10&pageidx=0&type=0&token={0}&lang=zh_CN",
				HttpUtility.UrlEncode(this.LoginInfo.Token));

			request.ContentType = "application/x-www-form-urlencoded";
			request.Method = "POST";
			request.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
			request.Headers["X-Requested-With"] = "XMLHttpRequest";

			NameValueCollection postParams = new NameValueCollection();

			postParams["func"] = this.FunctionName;
			postParams["token"] = this.LoginInfo.Token;
			postParams["lang"] = "zh_CN";
			postParams["random"] = "0.7978688745877286";
			postParams["f"] = "json";
			postParams["t"] = "ajax-friend-group";
			postParams["ajax"] = "1";

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
			WeChatModifyGroupRetInfo result = JSONSerializerExecute.Deserialize<WeChatModifyGroupRetInfo>(responseText);

			result.CheckResult();

			this.GroupInfo = result;
		}

		/// <summary>
		/// 准备Post的参数（基础参数已经准备完毕了）
		/// </summary>
		/// <param name="request"></param>
		/// <param name="postParams"></param>
		protected abstract void PreparePostParams(HttpWebRequest request, NameValueCollection postParams);
	}
}
