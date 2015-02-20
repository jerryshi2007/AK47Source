using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MCS.Library.Services
{
	/// <summary>
	/// 发送Http请求，然后获得返回结果进行处理的虚基类。
	/// 派生类重载请求和处理响应的文本，将处理过的结果，放在派生类的属性中。
	/// </summary>
	public abstract class HttpWebRequestExecutorBase
	{
		private Encoding _ResponseEncoding = Encoding.UTF8;

		/// <summary>
		/// 响应的文本的编码方式
		/// </summary>
		public Encoding ResponseEncoding
		{
			get
			{
				return this._ResponseEncoding;
			}
			set
			{
				this._ResponseEncoding = value;
			}
		}

		/// <summary>
		/// Http请求对象
		/// </summary>
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

		/// <summary>
		/// 准备请求对象
		/// </summary>
		/// <returns></returns>
		protected abstract HttpWebRequest PrepareWebRequest();

		/// <summary>
		/// 处理响应结果
		/// </summary>
		/// <param name="responseText"></param>
		protected abstract void ProcessResponseText(string responseText);

		/// <summary>
		/// 得到响应的文本
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected virtual string GetResponseText(HttpWebRequest request)
		{
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				string responseText = string.Empty;

				using (StreamReader reader = new StreamReader(response.GetResponseStream(), this.ResponseEncoding))
				{
					responseText = reader.ReadToEnd();
				}

				return responseText;
			}
		}
	}
}
