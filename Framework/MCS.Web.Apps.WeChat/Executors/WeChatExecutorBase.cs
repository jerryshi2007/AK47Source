using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MCS.Web.Apps.WeChat.Executors
{
	public abstract class WeChatExecutorBase
	{
		public HttpWebRequest Request
		{
			get;
			private set;
		}

		/// <summary>
		/// 执行请求，返回结果
		/// </summary>
		/// <returns></returns>
		public virtual void Execute()
		{
			this.Request = PrepareWebRequest();

			string responseText = GetResponseText(this.Request);

			ProcessResponseText(responseText);
		}

		protected abstract HttpWebRequest PrepareWebRequest();

		protected abstract void ProcessResponseText(string responseText);

		protected virtual string GetResponseText(HttpWebRequest request)
		{
			try
			{
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					string responseText = string.Empty;

					using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
					{
						responseText = reader.ReadToEnd();
					}

					return responseText;
				}
			}
			catch (System.Exception ex)
			{
				string message = string.Format("对地址{0}执行{1}操作错误:{2}",
					request.RequestUri.ToString(),
					request.Method, 
					ex.Message);

				throw new ApplicationException(message, ex);
			}
		}
	}
}
